using Microsoft.AspNetCore.Mvc;
using TaskAssignmentSystem.Models.Task;
using TaskAssignmentSystem.Models.Workspaces;
using TaskAssignmentSystem.Services.Implementations;
using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Controllers
{
    public class WorkspacesController : Controller
    {
        private readonly IWorkspaceService _workspaces;
        private readonly IAuthService _auth;
        private readonly ITeamService _teamService;
        private readonly ITaskService _taskService;

        public WorkspacesController(ITaskService taskService,IWorkspaceService workspaces, IAuthService auth, ITeamService teamService)
        {
            _workspaces = workspaces;
            _auth = auth;
            _teamService = teamService;
            _taskService = taskService;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            IEnumerable<Workspace> data = Enumerable.Empty<Workspace>();

            if (role == "Teacher" && userId.HasValue)
            {
                data = _workspaces.GetActive().Where(ws => ws.CreatedByUserId == userId.Value);
            }
            else if (role == "Admin")
            {
                data = _workspaces.GetActive();
            }
            else if (role == "Student" && userId.HasValue)
            {
                data = _workspaces.GetByStudent(userId.Value);
            }

            // Compute progress for each workspace
            var workspaceProgress = data.ToDictionary(
                w => w.Id,
                w =>
                {
                    var tasks = _taskService.GetTasksByWorkspace(w.Id); // get all tasks for this workspace
                    if (!tasks.Any()) return 0;

                    var allProgress = tasks
                        .SelectMany(t => t.ProgressUpdates)
                        .DefaultIfEmpty()
                        .Average(p => p?.ProgressPercent ?? 0);

                    return (int)allProgress;
                }
            );

            ViewBag.WorkspaceProgress = workspaceProgress;

            return View(data);
        }



        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Teacher")
            {
                TempData["Error"] = "Only Teachers can create workspaces.";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string name, int year, bool isTeamBased)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null || role != "Teacher")
            {
                TempData["Error"] = "Please login as Teacher to create workspaces.";
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Workspace name is required.";
                return View();
            }

            var ws = _workspaces.Create(name, year, userId.Value, isTeamBased);
            TempData["Success"] = $"Workspace '{ws.Name}' created. Join Code: {ws.JoinCode}";
            return RedirectToAction("Details", new { id = ws.Id });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ws = _workspaces.GetById(id);
            if (ws == null) return NotFound();
            return View(ws);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string name, int year, bool isTeamBased)
        {
            var ws = _workspaces.GetById(id);
            if (ws == null) return NotFound();

            ws.Name = name;
            ws.Year = year;
            ws.IsTeamBased = isTeamBased;
            _workspaces.Update(ws);

            TempData["Success"] = "Workspace updated successfully.";
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkInactive(int id)
        {
            var ok = _workspaces.Archive(id);
            TempData[ok ? "Success" : "Error"] = ok ? "Workspace marked inactive." : "Error archiving workspace.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Join()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Student")
            {
                TempData["Error"] = "Only Students can join a workspace.";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(string code)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null || role != "Student")
            {
                TempData["Error"] = "Only logged-in Students can join.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                TempData["Error"] = "Join code is required.";
                return View();
            }

            var ok = _workspaces.JoinByCode(code.Trim(), userId.Value);
            if (!ok)
            {
                TempData["Error"] = "Invalid or inactive join code.";
                return View();
            }

            TempData["Success"] = "Joined workspace successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var ws = _workspaces.GetById(id);
            if (ws == null) return NotFound();

            // if team-based ? redirect to Teams/ForWorkspace
            if (ws.IsTeamBased)
            {
                return RedirectToAction("ForWorkspace", "Teams", new { id });
            }

            // else (class-based) ? load tasks & show details normally
            var tasks = _taskService.GetTasksByWorkspace(id);
            ViewBag.Tasks = tasks;

            return View(ws);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignTask(int workspaceId, string title, string description)
        {
            var ws = _workspaces.GetById(workspaceId);
            if (ws == null) return NotFound();

            _taskService.AssignTask(workspaceId, title, description);

            TempData["Success"] = "Task assigned successfully.";
            return RedirectToAction("Details", new { id = workspaceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProgress(int taskId, int progress)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            _taskService.UpdateProgress(taskId, userId.Value, progress);

            TempData["Success"] = "Progress updated successfully.";

            // Redirect using task’s workspace
            var task = _taskService.GetTaskById(taskId);
            return RedirectToAction("Details", "Workspaces", new { id = task?.WorkspaceId });
        }


    }
}

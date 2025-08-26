using Microsoft.AspNetCore.Mvc;
using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Controllers
{
    public class WorkspacesController : Controller
    {
        private readonly IWorkspaceService _workspaces;
        private readonly IAuthService _auth;

        public WorkspacesController(IWorkspaceService workspaces, IAuthService auth)
        {
            _workspaces = workspaces;
            _auth = auth;
        }

        public IActionResult Index()
        {
            var data = _workspaces.GetActive();
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
            return View(ws);
        }
    }
}

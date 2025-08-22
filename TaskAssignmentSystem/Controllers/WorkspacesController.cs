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

        // GET: /Workspaces (show ACTIVE only to everyone)
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "Admin" || role == "Teacher")
            {
                return RedirectToAction("Workspaces", "Admin");
            }
            var data = _workspaces.GetActive(); // Students see active only
            return View(data);
        }

        // GET: /Workspaces/Create  (Teacher/Admin only)
        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Teacher" && role != "Admin")
            {
                TempData["Error"] = "Only Teachers or Admins can create workspaces.";
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: /Workspaces/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string name)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null || (role != "Teacher" && role != "Admin"))
            {
                TempData["Error"] = "Please login as Teacher/Admin.";
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Workspace name is required.";
                return View();
            }

            var ws = _workspaces.Create(name, userId.Value);
            TempData["Success"] = $"Workspace '{ws.Name}' created. Join Code: {ws.JoinCode}";
            return RedirectToAction("Details", new { id = ws.Id });
        }

        // GET: /Workspaces/Join  (Students only)
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

        // POST: /Workspaces/Join
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

        // GET: /Workspaces/Details/5
        [HttpGet]
        public IActionResult Details(int id)
        {
            var ws = _workspaces.GetById(id);
            if (ws == null) return NotFound();
            return View(ws);
        }
    }
}

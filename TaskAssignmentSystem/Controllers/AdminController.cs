using Microsoft.AspNetCore.Mvc;
using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IWorkspaceService _workspaces;

        public AdminController(IAuthService auth, IWorkspaceService workspaces)
        {
            _auth = auth;
            _workspaces = workspaces;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Index", "Home");
            }

            var users = _auth.GetAll();
            var activeWs = _workspaces.GetActive();
            var inactiveWs = _workspaces.GetInactive();

            var model = new
            {
                TotalUsers = users.Count,
                TotalTeachers = users.Count(u => u.Role.ToString() == "Teacher"),
                TotalStudents = users.Count(u => u.Role.ToString() == "Student"),
                ActiveWorkspaces = activeWs.Count,
                InactiveWorkspaces = inactiveWs.Count
            };

            return View(model);
        }

        // GET: /Admin/Workspaces
        public IActionResult Workspaces()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Index", "Home");
            }
            var active = _workspaces.GetActive();
            var inactive = _workspaces.GetInactive();
            var model = new { Active = active, Inactive = inactive };
            return View(model);
        }

        // POST: /Admin/Archive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Archive(int id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Index", "Home");
            }
            var ok = _workspaces.Archive(id);
            TempData[ok ? "Success" : "Error"] = ok ? "Workspace archived." : "Unable to archive.";
            return RedirectToAction("Workspaces");
        }

        // POST: /Admin/Restore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restore(int id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Index", "Home");
            }
            var ok = _workspaces.Restore(id);
            TempData[ok ? "Success" : "Error"] = ok ? "Workspace restored." : "Unable to restore.";
            return RedirectToAction("Workspaces");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using TaskAssignmentSystem.Models.Users;
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
                InactiveWorkspaces = inactiveWs.Count,
                PendingUsers = _auth.GetPending().Count
            };

            return View(model);
        }

        // GET: /Admin/Users (manage approvals)
        public IActionResult Users()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Index", "Home");
            }
            var pending = _auth.GetPending();
            var all = _auth.GetAll();
            var model = new { Pending = pending, All = all };
            return View(model);
        }

        // POST: /Admin/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Index", "Home");
            }
            var ok = _auth.Approve(id);
            TempData[ok ? "Success" : "Error"] = ok ? "User approved." : "Unable to approve.";
            return RedirectToAction("Users");
        }

        // POST: /Admin/SetRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetRole(int id, Role role)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Index", "Home");
            }
            var ok = _auth.SetRole(id, role);
            TempData[ok ? "Success" : "Error"] = ok ? "Role updated." : "Unable to update role.";
            return RedirectToAction("Users");
        }

        // Existing workspace overview endpoints remain read-only for Admin
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

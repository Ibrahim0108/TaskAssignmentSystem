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

        // POST: /Admin/Deactivate/5 (Workspace owner)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deactivate(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to deactivate a workspace.";
                return RedirectToAction("Workspaces");
            }

            var ws = _workspaces.GetById(id);
            if (ws == null || ws.CreatedByUserId != userId.Value)
            {
                TempData["Error"] = "You do not have permission to deactivate this workspace.";
                return RedirectToAction("Workspaces");
            }

            var ok = _workspaces.Archive(id);
            TempData[ok ? "Success" : "Error"] = ok ? "Workspace deactivated." : "Unable to deactivate.";
            return RedirectToAction("Workspaces");
        }

        // GET: /Admin/PendingUsers
        public IActionResult PendingUsers()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Index", "Home");
            }

            var pendingUsers = _auth.GetPendingUsers();
            return View(pendingUsers);
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

            var ok = _auth.ApproveUser(id);
            TempData[ok ? "Success" : "Error"] = ok ? "User approved." : "Unable to approve user.";
            return RedirectToAction("PendingUsers");
        }

        // POST: /Admin/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Index", "Home");
            }

            var ok = _auth.RejectUser(id);
            TempData[ok ? "Success" : "Error"] = ok ? "User rejected." : "Unable to reject user.";
            return RedirectToAction("PendingUsers");
        }

        // Optional Index redirect to Dashboard
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }
    }
}

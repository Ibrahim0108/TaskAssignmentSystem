using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TaskAssignmentSystem.Data;
using TaskAssignmentSystem.Models.Users;

namespace TaskAssignmentSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Only Admin is the top guy now
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public IActionResult Dashboard()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Login", "Auth");
            }

            var totalUsers = _context.Users.Count();
            var totalTeachers = _context.Users.Count(u => u.Role == Role.Teacher);
            var totalStudents = _context.Users.Count(u => u.Role == Role.Student);
            var pendingUsers = _context.Users.Count(u => !u.IsApproved);

            // if you don’t have workspaces yet, keep these 0 or adjust later
            var activeWorkspaces = 0;
            var inactiveWorkspaces = 0;

            var dashboardData = new
            {
                TotalUsers = totalUsers,
                TotalTeachers = totalTeachers,
                TotalStudents = totalStudents,
                PendingUsers = pendingUsers,
                ActiveWorkspaces = activeWorkspaces,
                InactiveWorkspaces = inactiveWorkspaces
            };

            return View(dashboardData);
        }


        // ✅ Show all users
        public IActionResult Users()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Dashboard");
            }

            var pending = _context.Users.Where(u => !u.IsApproved).ToList();
            var all = _context.Users
    .Where(u => u.Role == Role.Admin || u.Role == Role.Teacher)
    .ToList();


            var model = new
            {
                Pending = pending,
                All = all
            };

            return View(model);
        }


        // ✅ Approve pending users
        public IActionResult Pending()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Dashboard");
            }

            var pendingUsers = _context.Users.Where(u => !u.IsApproved).ToList();
            return View(pendingUsers);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Dashboard");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.IsApproved = true;
                _context.SaveChanges();
                TempData["Success"] = $"{user.Username} approved.";
            }

            return RedirectToAction("Pending");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Dashboard");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                TempData["Success"] = $"{user.Username} rejected and deleted.";
            }

            return RedirectToAction("Pending");
        }

        [HttpPost]
        public IActionResult Restore([FromForm] int id)
        {
            var workspace = _context.Workspaces.Find(id);
            if (workspace != null)
            {
                workspace.IsActive = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Workspaces");
        }


        // ✅ Change role of user
        [HttpPost]
        public IActionResult SetRole(int id, string role)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Dashboard");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                if (Enum.TryParse(role, out Role parsedRole))
                {
                    user.Role = parsedRole;
                    _context.SaveChanges();
                    TempData["Success"] = $"Role updated for {user.Username}.";
                }
                else
                {
                    TempData["Error"] = "Invalid role.";
                }
            }

            return RedirectToAction("Users");
        }

        // ✅ Delete a user
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Dashboard");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                TempData["Success"] = $"{user.Username} deleted.";
            }

            return RedirectToAction("Users");
        }

        public IActionResult Workspaces()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Admin only.";
                return RedirectToAction("Dashboard");
            }

            var active = _context.Workspaces.Where(w => w.IsActive).ToList();
            var inactive = _context.Workspaces.Where(w => !w.IsActive).ToList();


            var model = new
            {
                Active = active,
                Inactive = inactive
            };

            return View(model);
        }

    }
}

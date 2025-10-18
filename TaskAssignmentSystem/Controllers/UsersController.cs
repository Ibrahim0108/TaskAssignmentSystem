using Microsoft.AspNetCore.Mvc;
using TaskAssignmentSystem.Models.Users;
using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IWorkspaceService _workspaces;

        public UsersController(IAuthService auth, IWorkspaceService workspaces)
        {
            _auth = auth;
            _workspaces = workspaces;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string username, string password, string fullName, Role role, string email, string department, int? year, string section)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Username and Password are required.";
                return View();
            }

            if (role == Role.Student && year == null)
            {
                TempData["Error"] = "Year is required for students.";
                return View();
            }

            var user = _auth.Register(username, password, fullName ?? username, role, email, department, year, section);

            TempData["Success"] = $"User '{user.Username}' registered as {user.Role}. Please wait for admin approval before logging in.";
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var user = _auth.Login(username, password);
            if (user == null)
            {
                TempData["Error"] = "Invalid credentials.";
                return View();
            }

            if (!user.IsApproved)
            {
                TempData["Error"] = "Your account is pending admin approval.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());

            TempData["Success"] = $"Welcome, {user.FullName}!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var users = _auth.GetAll();
            return View(users);
        }



        [HttpGet]
        public IActionResult ProfilePage()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _auth.GetById(userId.Value);
            return View(user); // Pass user to the page
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(User updatedUser)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _auth.GetById(userId.Value);
            if (user == null) return NotFound();

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.Department = updatedUser.Department;
            user.Year = updatedUser.Year;
            user.Section = updatedUser.Section;
            // Role stays immutable

            _auth.Update(user);

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("ProfilePage");
        }

        [HttpGet]
        public IActionResult NotificationsPage()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var notifications = _auth.GetNotifications(userId.Value);
            return View(notifications);
        }

        [HttpGet]
        public IActionResult GetNotificationById(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var notification = _auth.GetNotificationById(id);

            if (notification == null || notification.UserId != userId.Value)
                return NotFound();

            return Json(new
            {
                id = notification.Id,
                message = notification.Message,
                isRead = notification.IsRead,
                createdAt = notification.CreatedAt.ToString("f") // formatted date
            });
        }


        [HttpPost]
        public IActionResult MarkNotificationRead(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var notif = _auth.GetNotificationById(id);
            if (notif == null || notif.UserId != userId.Value)
                return NotFound();

            var success = _auth.MarkNotificationAsRead(id);
            return Json(new { success });
        }

        [HttpPost]
        public IActionResult MarkAllNotificationsRead()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var notifications = _auth.GetNotifications(userId.Value);
            foreach (var n in notifications.Where(n => !n.IsRead))
                _auth.MarkNotificationAsRead(n.Id);

            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult CreateGlobalAnnouncement(string message, int year)
        {
            var teacherId = HttpContext.Session.GetInt32("UserId");
            if (teacherId == null) return Unauthorized();

            var teacher = _auth.GetById(teacherId.Value);
            if (teacher == null) return Unauthorized();

            // Get all students in the same department & year
            var students = _auth.GetAll()
                .Where(u => u.Role == Role.Student
                         && u.Department == teacher.Department
                         && u.Year == year)
                .ToList();

            // Insert notifications for each student
            foreach (var s in students)
            {
                _auth.AddNotification(new Notification
                {
                    UserId = s.Id,
                    Message = message,
                    CreatedAt = DateTime.Now
                });
            }

            TempData["Success"] = "Announcement sent successfully!";
            return Json(new { success = true });
        }

    }
}
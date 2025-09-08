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


    }
}
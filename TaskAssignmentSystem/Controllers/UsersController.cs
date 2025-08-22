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

        // GET: /Users/Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: /Users/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string username, string password, string fullName, Role role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Username and Password are required.";
                return View();
            }

            var user = _auth.Register(username, password, fullName ?? username, role);

            TempData["Success"] = $"User '{user.Username}' registered as {user.Role}. Please wait for admin approval before logging in.";
            return RedirectToAction("Login");
        }

        // GET: /Users/Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: /Users/Login
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

            // Check approval status
            if (!user.IsApproved)
            {
                TempData["Error"] = "Your account is pending admin approval.";
                return View();
            }

            // store user in session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());

            TempData["Success"] = $"Welcome, {user.FullName}!";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Users/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out.";
            return RedirectToAction("Login");
        }

        // GET: /Users/Index (list all users - admin/teacher overview)
        [HttpGet]
        public IActionResult Index()
        {
            var users = _auth.GetAll();
            return View(users);
        }
    }
}

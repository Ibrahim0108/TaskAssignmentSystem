using Microsoft.AspNetCore.Mvc;

namespace TaskAssignmentSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.CurrentUserName = HttpContext.Session.GetString("UserName");
            ViewBag.CurrentUserRole = HttpContext.Session.GetString("UserRole");
            return View();
        }

        public IActionResult Privacy() => View();
    }
}

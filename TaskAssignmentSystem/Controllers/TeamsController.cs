using Microsoft.AspNetCore.Mvc;
using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Controllers
{
    public class TeamsController : Controller
    {
        private readonly ITeamService _teams;
        private readonly IWorkspaceService _workspaces;

        public TeamsController(ITeamService teams, IWorkspaceService workspaces)
        {
            _teams = teams;
            _workspaces = workspaces;
        }

        // GET: /Teams/ForWorkspace/5
        [HttpGet]
        public IActionResult ForWorkspace(int id)
        {
            var ws = _workspaces.GetById(id);
            if (ws == null) return NotFound();
            var list = _teams.GetByWorkspace(id);
            ViewBag.Workspace = ws;
            return View(list);
        }

        // GET: /Teams/Create?workspaceId=5
        [HttpGet]
        public IActionResult Create(int workspaceId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Teacher" && role != "Admin")
            {
                TempData["Error"] = "Only Teachers/Admins can create teams.";
                return RedirectToAction("ForWorkspace", new { id = workspaceId });
            }
            ViewBag.WorkspaceId = workspaceId;
            return View();
        }

        // POST: /Teams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int workspaceId, string name, int leaderUserId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Teacher" && role != "Admin")
            {
                TempData["Error"] = "Only Teachers/Admins can create teams.";
                return RedirectToAction("ForWorkspace", new { id = workspaceId });
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Team name is required.";
                return View();
            }
            var t = _teams.CreateTeam(workspaceId, name.Trim(), leaderUserId);
            TempData["Success"] = $"Team '{t.Name}' created. Join Code: {t.JoinCode}";
            return RedirectToAction("ForWorkspace", new { id = workspaceId });
        }

        // GET: /Teams/Join
        [HttpGet]
        public IActionResult Join()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Student" && role != "Teacher")
            {
                TempData["Error"] = "Only logged-in users can join teams.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Teams/Join
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(string code)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login first.";
                return RedirectToAction("Login", "Users");
            }
            if (string.IsNullOrWhiteSpace(code))
            {
                TempData["Error"] = "Join code is required.";
                return View();
            }
            var ok = _teams.JoinTeamByCode(code.Trim(), userId.Value);
            if (!ok)
            {
                TempData["Error"] = "Invalid join code.";
                return View();
            }
            TempData["Success"] = "Joined team successfully.";
            var team = _teams.GetByJoinCode(code.Trim());
            return RedirectToAction("Details", new { id = team!.Id });
        }

        // GET: /Teams/Details/5
        [HttpGet]
        public IActionResult Details(int id)
        {
            var t = _teams.GetById(id);
            if (t == null) return NotFound();
            return View(t);
        }

        // POST: /Teams/AddUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUpdate(int teamId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login.";
                return RedirectToAction("Login", "Users");
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Update content is required.";
                return RedirectToAction("Details", new { id = teamId });
            }
            try
            {
                _teams.AddUpdate(teamId, userId.Value, content.Trim());
                TempData["Success"] = "Update added.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", new { id = teamId });
        }

        // POST: /Teams/LeaderReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LeaderReview(int teamId, int updateId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login.";
                return RedirectToAction("Login", "Users");
            }
            var ok = _teams.LeaderReviewUpdate(teamId, updateId, userId.Value);
            TempData[ok ? "Success" : "Error"] = ok ? "Marked as reviewed." : "Only leader can review.";
            return RedirectToAction("Details", new { id = teamId });
        }

        // POST: /Teams/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(int teamId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login.";
                return RedirectToAction("Login", "Users");
            }
            var ok = _teams.SubmitTeam(teamId, userId.Value);
            TempData[ok ? "Success" : "Error"] = ok ? "Team submitted to teacher." : "Only leader can submit.";
            return RedirectToAction("Details", new { id = teamId });
        }
    }
}
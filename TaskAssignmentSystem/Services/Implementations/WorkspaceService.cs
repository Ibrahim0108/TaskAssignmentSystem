using TaskAssignmentSystem.Data;
using TaskAssignmentSystem.Models.Workspaces;
using TaskAssignmentSystem.Models.Users;
using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Services.Implementations
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly ApplicationDbContext _db;
        private static readonly string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public WorkspaceService(ApplicationDbContext db)
        {
            _db = db;
        }

        public (string? Department, int? Year) GetWorkspaceDetails(int workspaceId)
        {
            var ws = _db.Workspaces.FirstOrDefault(w => w.Id == workspaceId);
            if (ws == null) return (null, null);

            // find the teacher who created the workspace and read their department
            var teacher = _db.Users.FirstOrDefault(u => u.Id == ws.CreatedByUserId);
            return (teacher?.Department, ws.Year);
        }

        public Workspace Create(string name, int year, int createdByUserId, bool isTeamBased = false)
        {
            var ws = new Workspace
            {
                Name = name,
                JoinCode = GenerateJoinCode(6),
                CreatedByUserId = createdByUserId,
                IsActive = true,
                Year = year,
                IsTeamBased = isTeamBased,
            };

            _db.Workspaces.Add(ws);
            _db.SaveChanges();
            return ws;
        }

        public List<Workspace> GetAll() => _db.Workspaces.ToList();
        public List<Workspace> GetActive() => _db.Workspaces.Where(w => w.IsActive).ToList();
        public List<Workspace> GetInactive() => _db.Workspaces.Where(w => !w.IsActive).ToList();
        public Workspace? GetById(int id) => _db.Workspaces.FirstOrDefault(w => w.Id == id);
        public Workspace? GetByJoinCode(string code) =>
            _db.Workspaces.FirstOrDefault(w => w.JoinCode == code && w.IsActive);

        public bool JoinByCode(string code, int userId)
        {
            var ws = GetByJoinCode(code);
            if (ws == null) return false;

            if (!ws.MemberUserIds.Contains(userId))
                ws.MemberUserIds.Add(userId);

            _db.Workspaces.Update(ws);
            _db.SaveChanges();
            return true;
        }

        public bool Archive(int id)
        {
            var ws = GetById(id);
            if (ws == null) return false;
            ws.IsActive = false;
            _db.SaveChanges();
            return true;
        }

        public bool Restore(int id)
        {
            var ws = GetById(id);
            if (ws == null) return false;
            ws.IsActive = true;
            _db.SaveChanges();
            return true;
        }

        private string GenerateJoinCode(int len)
        {
            var rand = new Random();
            return new string(Enumerable.Range(0, len).Select(_ => Alphabet[rand.Next(Alphabet.Length)]).ToArray());
        }

        public bool Update(Workspace ws)
        {
            var existing = _db.Workspaces.FirstOrDefault(w => w.Id == ws.Id);
            if (existing == null) return false;

            existing.Name = ws.Name;
            // add other fields you want updatable

            _db.Workspaces.Update(existing);
            _db.SaveChanges();
            return true; // ✅ return value
        }


    }
}

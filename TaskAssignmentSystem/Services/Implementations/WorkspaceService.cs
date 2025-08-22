using TaskAssignmentSystem.Models.Workspaces;
using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Services.Implementations
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly List<Workspace> _workspaces = new();
        private int _nextId = 1;
        private static readonly string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public Workspace Create(string name, int createdByUserId)
        {
            var ws = new Workspace
            {
                Id = _nextId++,
                Name = name,
                JoinCode = GenerateJoinCode(6),
                CreatedByUserId = createdByUserId,
                IsActive = true
            };
            _workspaces.Add(ws);
            return ws;
        }

        public List<Workspace> GetAll() => _workspaces;
        public List<Workspace> GetActive() => _workspaces.Where(w => w.IsActive).ToList();
        public List<Workspace> GetInactive() => _workspaces.Where(w => !w.IsActive).ToList();
        public Workspace? GetById(int id) => _workspaces.FirstOrDefault(w => w.Id == id);
        public Workspace? GetByJoinCode(string code) => _workspaces.FirstOrDefault(w => w.JoinCode.Equals(code, StringComparison.OrdinalIgnoreCase));
        public bool JoinByCode(string code, int userId)
        {
            var ws = GetByJoinCode(code);
            if (ws == null || !ws.IsActive) return false;
            if (!ws.MemberUserIds.Contains(userId))
                ws.MemberUserIds.Add(userId);
            return true;
        }
        public bool Archive(int id)
        {
            var ws = GetById(id);
            if (ws == null) return false;
            ws.IsActive = false;
            return true;
        }
        public bool Restore(int id)
        {
            var ws = GetById(id);
            if (ws == null) return false;
            ws.IsActive = true;
            return true;
        }

        private string GenerateJoinCode(int len)
        {
            var rand = new Random();
            return new string(Enumerable.Range(0, len).Select(_ => Alphabet[rand.Next(Alphabet.Length)]).ToArray());
        }
    }
}

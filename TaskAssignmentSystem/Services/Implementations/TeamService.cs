using TaskAssignmentSystem.Models.Teams;
using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Services.Implementations
{
    public class TeamService : ITeamService
    {
        private readonly List<Team> _teams = new();
        private readonly List<TeamProgressUpdate> _updates = new();
        private int _nextTeamId = 1;
        private int _nextUpdateId = 1;
        private static readonly string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public Team CreateTeam(int workspaceId, string name, int leaderUserId)
        {
            var team = new Team
            {
                Id = _nextTeamId++,
                WorkspaceId = workspaceId,
                Name = name,
                JoinCode = GenerateJoinCode(6),
                LeaderUserId = leaderUserId,
            };
            if (!team.MemberUserIds.Contains(leaderUserId))
                team.MemberUserIds.Add(leaderUserId);
            _teams.Add(team);
            return team;
        }

        public Team? GetById(int id) => _teams.FirstOrDefault(t => t.Id == id);

        public List<Team> GetByWorkspace(int workspaceId) => _teams.Where(t => t.WorkspaceId == workspaceId).ToList();

        public Team? GetByJoinCode(string code) => _teams.FirstOrDefault(t => t.JoinCode.Equals(code, StringComparison.OrdinalIgnoreCase));

        public bool JoinTeamByCode(string code, int userId)
        {
            var team = GetByJoinCode(code);
            if (team == null) return false;
            if (!team.MemberUserIds.Contains(userId))
                team.MemberUserIds.Add(userId);
            return true;
        }

        public TeamProgressUpdate AddUpdate(int teamId, int userId, string content)
        {
            var team = GetById(teamId) ?? throw new InvalidOperationException("Team not found");
            if (!team.MemberUserIds.Contains(userId)) throw new InvalidOperationException("Only members can add updates");
            var upd = new TeamProgressUpdate
            {
                Id = _nextUpdateId++,
                TeamId = teamId,
                UserId = userId,
                Content = content
            };
            team.Updates.Add(upd);
            _updates.Add(upd);
            return upd;
        }

        public bool LeaderReviewUpdate(int teamId, int updateId, int leaderUserId)
        {
            var team = GetById(teamId);
            if (team == null || team.LeaderUserId != leaderUserId) return false;
            var upd = team.Updates.FirstOrDefault(u => u.Id == updateId);
            if (upd == null) return false;
            upd.ReviewedByLeader = true;
            return true;
        }

        public bool SubmitTeam(int teamId, int leaderUserId)
        {
            var team = GetById(teamId);
            if (team == null || team.LeaderUserId != leaderUserId) return false;
            team.IsSubmitted = true;
            team.SubmittedAt = DateTime.UtcNow;
            return true;
        }

        private string GenerateJoinCode(int len)
        {
            var rand = new Random();
            return new string(Enumerable.Range(0, len).Select(_ => Alphabet[rand.Next(Alphabet.Length)]).ToArray());
        }
    }
}
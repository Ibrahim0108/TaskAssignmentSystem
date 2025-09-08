using Microsoft.EntityFrameworkCore;
using TaskAssignmentSystem.Data;
using TaskAssignmentSystem.Models.Teams;
using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Services.Implementations
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _db;
        private static readonly string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private readonly Random _rand = new();

        public TeamService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Team CreateTeam(int workspaceId, string name, int leaderUserId, string leaderType)
        {
            var team = new Team
            {
                WorkspaceId = workspaceId,
                Name = name,
                JoinCode = GenerateJoinCode(6),
                LeaderUserId = leaderUserId,
                LeaderType = leaderType,
                IsSubmitted = false
            };

            _db.Teams.Add(team);
            _db.SaveChanges();

            // Add leader as member automatically
            var leaderMember = new TeamMember { TeamId = team.Id, UserId = leaderUserId };
            _db.TeamMembers.Add(leaderMember);
            _db.SaveChanges();

            return team;
        }

        public Team? GetById(int id)
        {
            return _db.Teams
                .Include(t => t.Updates)
                .Include(t => t.TeamMembers)
                .FirstOrDefault(t => t.Id == id);
        }

        public List<Team> GetByWorkspace(int workspaceId)
        {
            return _db.Teams
                .Include(t => t.Updates)
                .Include(t => t.TeamMembers)
                .Where(t => t.WorkspaceId == workspaceId)
                .ToList();
        }

        public Team? GetByJoinCode(string code)
        {
            return _db.Teams
                .Include(t => t.TeamMembers)
                .FirstOrDefault(t => t.JoinCode == code);
        }

        public bool JoinTeamByCode(string code, int userId)
        {
            var team = GetByJoinCode(code);
            if (team == null) return false;

            if (!_db.TeamMembers.Any(m => m.TeamId == team.Id && m.UserId == userId))
            {
                _db.TeamMembers.Add(new TeamMember { TeamId = team.Id, UserId = userId });
                _db.SaveChanges();
            }

            return true;
        }

        public TeamProgressUpdate AddUpdate(int teamId, int userId, string content, SubtaskStatus status, int? assignedToUserId = null)
        {
            var team = GetById(teamId) ?? throw new InvalidOperationException("Team not found");

            if (!_db.TeamMembers.Any(m => m.TeamId == teamId && m.UserId == userId))
                throw new InvalidOperationException("Only members can add updates");

            var upd = new TeamProgressUpdate
            {
                TeamId = teamId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                Status = status,
                AssignedToUserId = assignedToUserId
            };

            _db.TeamProgressUpdates.Add(upd);
            _db.SaveChanges();
            return upd;
        }


        public TeamProgressUpdate UpdateExistingUpdate(int updateId, int userId, string content, SubtaskStatus status)
        {
            var upd = _db.TeamProgressUpdates.FirstOrDefault(u => u.Id == updateId);
            if (upd == null) throw new InvalidOperationException("Update not found");

            // Only the assigned user can replace their assigned task (security)
            if (!upd.AssignedToUserId.HasValue || upd.AssignedToUserId.Value != userId)
                throw new InvalidOperationException("You are not allowed to update this task.");

            // Replace content/status, set the UserId to the member (so it shows in Updates)
            upd.Content = content;
            upd.Status = status;
            upd.UserId = userId;

            // Reset leader review when a member changes the task
            upd.ReviewedByLeader = false;

            _db.SaveChanges();
            return upd;
        }


        public bool LeaderReviewUpdate(int teamId, int updateId, int leaderUserId)
        {
            var team = GetById(teamId);
            if (team == null || team.LeaderUserId != leaderUserId) return false;

            var upd = _db.TeamProgressUpdates.FirstOrDefault(u => u.Id == updateId && u.TeamId == teamId);
            if (upd == null) return false;

            upd.ReviewedByLeader = true;
            _db.SaveChanges();
            return true;
        }

        public bool SubmitTeam(int teamId, int leaderUserId)
        {
            var team = GetById(teamId);
            if (team == null || team.LeaderUserId != leaderUserId) return false;

            team.IsSubmitted = true;
            team.SubmittedAt = DateTime.UtcNow;
            _db.SaveChanges();
            return true;
        }

        public bool Update(Team team)
        {
            var existing = _db.Teams.Find(team.Id);
            if (existing == null) return false;

            existing.Name = team.Name;
            existing.LeaderUserId = team.LeaderUserId;

            _db.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var t = GetById(id);
            if (t == null) return false;

            _db.TeamMembers.RemoveRange(_db.TeamMembers.Where(m => m.TeamId == id));
            _db.TeamProgressUpdates.RemoveRange(_db.TeamProgressUpdates.Where(u => u.TeamId == id));
            _db.Teams.Remove(t);

            _db.SaveChanges();
            return true;
        }

        public bool RemoveMember(int teamId, int userId)
        {
            var member = _db.TeamMembers.FirstOrDefault(m => m.TeamId == teamId && m.UserId == userId);
            if (member == null) return false;

            _db.TeamMembers.Remove(member);
            _db.SaveChanges();
            return true;
        }

        private string GenerateJoinCode(int len)
        {
            return new string(Enumerable.Range(0, len)
                .Select(_ => Alphabet[_rand.Next(Alphabet.Length)]).ToArray());
        }
    }
}

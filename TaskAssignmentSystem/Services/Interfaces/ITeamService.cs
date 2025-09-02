using TaskAssignmentSystem.Models.Teams;

namespace TaskAssignmentSystem.Services.Interfaces
{
    public interface ITeamService
    {
        Team CreateTeam(int workspaceId, string name, int leaderUserId, string leaderType);
        Team? GetById(int id);
        List<Team> GetByWorkspace(int workspaceId);
        Team? GetByJoinCode(string code);
        bool JoinTeamByCode(string code, int userId);
        TeamProgressUpdate AddUpdate(int teamId, int userId, string content, SubtaskStatus status, int? assignedToUserId = null);
        bool LeaderReviewUpdate(int teamId, int updateId, int leaderUserId);
        bool SubmitTeam(int teamId, int leaderUserId);
        bool Update(Team team);
        bool Delete(int id);
        bool RemoveMember(int teamId, int userId);

    }
}
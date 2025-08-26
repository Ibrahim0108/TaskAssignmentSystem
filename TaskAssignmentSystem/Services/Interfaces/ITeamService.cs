using TaskAssignmentSystem.Models.Teams;

namespace TaskAssignmentSystem.Services.Interfaces
{
    public interface ITeamService
    {
        Team CreateTeam(int workspaceId, string name, int leaderUserId);
        Team? GetById(int id);
        List<Team> GetByWorkspace(int workspaceId);
        Team? GetByJoinCode(string code);
        bool JoinTeamByCode(string code, int userId);
        TeamProgressUpdate AddUpdate(int teamId, int userId, string content);
        bool LeaderReviewUpdate(int teamId, int updateId, int leaderUserId);
        bool SubmitTeam(int teamId, int leaderUserId);
        bool Update(Team team);
        bool Delete(int id);
        bool RemoveMember(int teamId, int userId);

    }
}
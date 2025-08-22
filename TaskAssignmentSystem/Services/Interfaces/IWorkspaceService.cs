using TaskAssignmentSystem.Models.Workspaces;

namespace TaskAssignmentSystem.Services.Interfaces
{
    public interface IWorkspaceService
    {
        Workspace Create(string name, int createdByUserId);
        List<Workspace> GetAll();
        List<Workspace> GetActive();
        List<Workspace> GetInactive();
        Workspace? GetById(int id);
        Workspace? GetByJoinCode(string code);
        bool JoinByCode(string code, int userId);
        bool Archive(int id);
        bool Restore(int id);
    }
}

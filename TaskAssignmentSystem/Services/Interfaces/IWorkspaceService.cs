using TaskAssignmentSystem.Models.Workspaces;

namespace TaskAssignmentSystem.Services.Interfaces
{
    public interface IWorkspaceService
    {
        Workspace Create(string name, int createdByUserId);
        List<Workspace> GetAll();               // includes active + inactive
        List<Workspace> GetActive();
        List<Workspace> GetInactive();
        Workspace? GetById(int id);
        Workspace? GetByJoinCode(string code);
        bool JoinByCode(string code, int userId);      // only students should succeed
        bool Archive(int id);                          // set IsActive = false
        bool Restore(int id);                          // set IsActive = true
    }
}

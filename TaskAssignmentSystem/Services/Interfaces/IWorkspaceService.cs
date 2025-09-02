using TaskAssignmentSystem.Models.Workspaces;

public interface IWorkspaceService
{
    Workspace Create(string name, int year, int createdByUserId, bool isTeamBased = false);
    List<Workspace> GetAll();
    List<Workspace> GetActive();
    List<Workspace> GetInactive();
    Workspace? GetById(int id);
    Workspace? GetByJoinCode(string code);
    bool JoinByCode(string code, int userId);
    bool Archive(int id);
    bool Restore(int id);
    // add near the other method signatures
    (string? Department, int? Year) GetWorkspaceDetails(int workspaceId);

    bool Update(Workspace workspace);

    IEnumerable<Workspace> GetByStudent(int studentId);

}

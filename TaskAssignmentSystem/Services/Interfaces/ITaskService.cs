using TaskAssignmentSystem.Models.Task;

namespace TaskAssignmentSystem.Services.Interfaces
{
    public interface ITaskService
    {
        WorkspaceTask AssignTask(int workspaceId, string title, string description);
        void UpdateProgress(int taskId, int studentId, int progress);
        WorkspaceTask? GetTaskById(int id);
        IEnumerable<WorkspaceTask> GetTasksByWorkspace(int workspaceId);

    }
}

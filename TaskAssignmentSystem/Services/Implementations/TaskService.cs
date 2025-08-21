using TaskAssignmentSystem.Services.Interfaces;

namespace TaskAssignmentSystem.Services.Implementations
{
    public class TaskService : ITaskService
    {
        public string GetTaskById(int id)
        {
            // 🔹 Placeholder logic
            return $"Task {id} details (demo)";
        }
    }
}

using TaskAssignmentSystem.Models.Workspaces;



namespace TaskAssignmentSystem.Models.Task
{
    public class WorkspaceTask
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // navigation
        public Workspace Workspace { get; set; } = null!;
        public List<TaskProgress> ProgressUpdates { get; set; } = new();
    }
}

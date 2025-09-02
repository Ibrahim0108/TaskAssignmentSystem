namespace TaskAssignmentSystem.Models.Task
{
    public class TaskProgress
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int StudentId { get; set; }
        public int ProgressPercent { get; set; } = 0; // 0–100
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public WorkspaceTask Task { get; set; } = null!;
    }
}

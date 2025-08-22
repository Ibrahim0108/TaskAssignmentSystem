namespace TaskAssignmentSystem.Models.Teams
{
    public class TeamProgressUpdate
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool ReviewedByLeader { get; set; }
    }
}
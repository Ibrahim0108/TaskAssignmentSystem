namespace TaskAssignmentSystem.Models.Teams
{
    public class Team
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public string Name { get; set; }
        public string JoinCode { get; set; }
        public int LeaderUserId { get; set; }
        public bool IsSubmitted { get; set; }
        public DateTime? SubmittedAt { get; set; }

        public List<int> MemberUserIds { get; set; } = new();
        public List<TeamProgressUpdate> Updates { get; set; } = new();
    }
}
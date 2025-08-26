namespace TaskAssignmentSystem.Models.Workspaces
{
    public class Workspace
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string JoinCode { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
        public bool IsActive { get; set; } = true;

        // NEW: whether workspace is team-based
        public bool IsTeamBased { get; set; } = false;
        public int? Year { get; set; }

        // store members as string in DB (comma-separated IDs)
        public List<int> MemberUserIds { get; set; } = new();
    }
}

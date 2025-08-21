namespace TaskAssignmentSystem.Models.Workspaces
{
    public class Workspace
    {
        public int Id { get; set; }
        public string Name { get; set; }               // e.g., "Computer Networks - Class A"
        public string JoinCode { get; set; }           // auto-generated code students use
        public int CreatedByUserId { get; set; }         // Teacher/Admin who created it
        public bool IsActive { get; set; } = true; 

        // For Phase 1 we’ll keep a simple list of member user IDs.
        // Later we can expand to role-in-workspace, teams, etc.
        public List<int> MemberUserIds { get; set; } = new();
    }
}

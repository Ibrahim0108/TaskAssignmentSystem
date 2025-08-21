namespace TaskAssignmentSystem.Models.Config
{
    public class AppSettings
    {
        public string JwtSecret { get; set; }
        public int TokenExpiryMinutes { get; set; }
    }
}

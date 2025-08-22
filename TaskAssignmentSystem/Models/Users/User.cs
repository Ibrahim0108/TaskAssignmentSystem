using System;
using System.ComponentModel.DataAnnotations;

namespace TaskAssignmentSystem.Models.Users
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(64)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }   // store hash, not plain text

        [Required, MaxLength(128)]
        public string FullName { get; set; }

        [Required]
        public Role Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Add this property for pending approvals
        public bool IsApproved { get; set; } = false;
    }
}

namespace TaskAssignmentSystem.Models.Users
{
    public enum Role
    {
        Admin = 1,
        Teacher = 2,
        Student = 3
    }
}

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

        [Required, EmailAddress]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Add this property for pending approvals
        public bool IsApproved { get; set; } = false;

        public string? Department { get; set; }  // Teacher department / Student stream
        public int? Year { get; set; }           // Only used for students
        public string? Section { get; set; }    // Optional for students
    }
}



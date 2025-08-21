using System.ComponentModel.DataAnnotations;
using TaskAssignmentSystem.Models.Users;

namespace TaskAssignmentSystem.Models.Auth
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}

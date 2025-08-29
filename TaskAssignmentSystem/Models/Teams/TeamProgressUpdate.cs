using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskAssignmentSystem.Models.Teams
{
    public class TeamProgressUpdate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TeamId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public SubtaskStatus Status { get; set; } = SubtaskStatus.NotStarted;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool ReviewedByLeader { get; set; } = false;

        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }
    }


    public enum SubtaskStatus
    {
        NotStarted = 0,
        InProgress = 50,
        Completed = 100,
        Blocked = 0
    }
}

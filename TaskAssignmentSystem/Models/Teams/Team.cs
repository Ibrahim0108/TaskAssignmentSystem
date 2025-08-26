using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskAssignmentSystem.Models.Teams
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WorkspaceId { get; set; }

        [Required, MaxLength(128)]
        public string Name { get; set; }

        [Required, MaxLength(16)]
        public string JoinCode { get; set; }

        [Required]
        public int LeaderUserId { get; set; }

        public bool IsSubmitted { get; set; } = false;
        public DateTime? SubmittedAt { get; set; }

        // ? Matches Workspace style
        public List<int> MemberUserIds { get; set; } = new();

        public virtual List<TeamProgressUpdate> Updates { get; set; } = new();
    }

    // If you decide to keep TeamMember, that’s fine too,
    // but it won’t be used in service/views anymore.
    public class TeamMember
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TeamId { get; set; }
        [Required]
        public int UserId { get; set; }

        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }
    }
}

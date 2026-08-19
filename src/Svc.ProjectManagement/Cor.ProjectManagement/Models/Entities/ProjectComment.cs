// Models/Entities/ProjectComment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.ProjectManagement.Models.Entities
{
    public class ProjectComment : BaseEntity
    {
        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public Guid ProjectId { get; set; }

        public Guid? TaskId { get; set; }
        public Guid? MilestoneId { get; set; }
        public Guid? IssueId { get; set; }
        public Guid? ParentCommentId { get; set; }

        [Required]
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;

        public DateTime? EditedAt { get; set; }
        public Guid? EditedById { get; set; }
        public string EditedByName { get; set; } = string.Empty;

        public bool IsPinned { get; set; }
        public bool IsResolved { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Metadata { get; set; }

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
        public virtual ProjectTask? Task { get; set; }
        public virtual ProjectMilestone? Milestone { get; set; }
        public virtual ProjectIssue? Issue { get; set; }
        public virtual ProjectComment? ParentComment { get; set; }
        public virtual ICollection<ProjectComment> Replies { get; set; } = new List<ProjectComment>();
    }
}
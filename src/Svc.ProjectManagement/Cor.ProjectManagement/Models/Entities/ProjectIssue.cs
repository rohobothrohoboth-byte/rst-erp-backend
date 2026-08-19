// Models/Entities/ProjectIssue.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.ProjectManagement.Models.Entities
{
    public enum IssueType
    {
        Technical = 1,
        Resource = 2,
        Schedule = 3,
        Budget = 4,
        Quality = 5,
        Scope = 6,
        Communication = 7,
        Stakeholder = 8,
        Vendor = 9,
        Other = 10
    }

    public enum IssuePriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum IssueStatus
    {
        Open = 1,
        InProgress = 2,
        UnderReview = 3,
        Resolved = 4,
        Closed = 5,
        Rejected = 6
    }

    public class ProjectIssue : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public IssueType Type { get; set; }

        [Required]
        public IssuePriority Priority { get; set; }

        [Required]
        public IssueStatus Status { get; set; } = IssueStatus.Open;

        public Guid? ReportedById { get; set; }
        public string ReportedByName { get; set; } = string.Empty;
        public DateTime ReportedAt { get; set; }

        public Guid? AssignedToId { get; set; }
        public string AssignedToName { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public Guid? ResolvedById { get; set; }
        public string ResolvedByName { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;

        public string? RelatedTaskId { get; set; }
        public string RelatedMilestoneId { get; set; } = string.Empty;

        public string RootCause { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;

        [Column(TypeName = "jsonb")]
        public string? CustomFields { get; set; }

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
        public virtual ICollection<ProjectComment> Comments { get; set; } = new List<ProjectComment>();
    }
}
// Models/Entities/ProjectChange.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.ProjectManagement.Models.Entities
{
    public enum ChangeType
    {
        Scope = 1,
        Schedule = 2,
        Budget = 3,
        Resource = 4,
        Quality = 5,
        Risk = 6,
        Other = 7
    }

    public enum ChangePriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Urgent = 4
    }

    public enum ChangeStatus
    {
        Submitted = 1,
        UnderReview = 2,
        Approved = 3,
        Rejected = 4,
        Implemented = 5,
        Closed = 6
    }

    public class ProjectChange : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public ChangeType Type { get; set; }

        [Required]
        public ChangePriority Priority { get; set; }

        [Required]
        public ChangeStatus Status { get; set; } = ChangeStatus.Submitted;

        public string CurrentState { get; set; } = string.Empty;
        public string ProposedState { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
        public string ImpactAnalysis { get; set; } = string.Empty;

        public decimal CostImpact { get; set; }
        public int ScheduleImpact { get; set; } // Days

        public Guid? RequestedById { get; set; }
        public string RequestedByName { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }

        public Guid? ReviewedById { get; set; }
        public string ReviewedByName { get; set; } = string.Empty;
        public DateTime? ReviewedAt { get; set; }
        public string ReviewNotes { get; set; } = string.Empty;

        public Guid? ApprovedById { get; set; }
        public string ApprovedByName { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string ApprovalNotes { get; set; } = string.Empty;

        public DateTime? ImplementedAt { get; set; }
        public Guid? ImplementedById { get; set; }
        public string ImplementedByName { get; set; } = string.Empty;

        [Column(TypeName = "jsonb")]
        public string? CustomFields { get; set; }

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
    }
}
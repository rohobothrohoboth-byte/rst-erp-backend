// Models/DTOs/ProjectChangeDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectChangeDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public ChangeType Type { get; set; }
        public ChangePriority Priority { get; set; }
        public ChangeStatus Status { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        public string ProposedState { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
        public string ImpactAnalysis { get; set; } = string.Empty;
        public decimal CostImpact { get; set; }
        public int ScheduleImpact { get; set; }
        public string RequestedByName { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public string ReviewedByName { get; set; } = string.Empty;
        public DateTime? ReviewedAt { get; set; }
        public string ReviewNotes { get; set; } = string.Empty;
        public string ApprovedByName { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string ApprovalNotes { get; set; } = string.Empty;
        public DateTime? ImplementedAt { get; set; }
        public string ImplementedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class ProjectChangeCreateDto
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

        public string CurrentState { get; set; } = string.Empty;
        public string ProposedState { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
        public string ImpactAnalysis { get; set; } = string.Empty;

        public decimal CostImpact { get; set; }
        public int ScheduleImpact { get; set; }

        public string? RequestedByName { get; set; }

        public string? CreatedBy { get; set; }
    }

    public class ProjectChangeUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ChangeType? Type { get; set; }
        public ChangePriority? Priority { get; set; }
        public ChangeStatus? Status { get; set; }
        public string? CurrentState { get; set; }
        public string? ProposedState { get; set; }
        public string? Justification { get; set; }
        public string? ImpactAnalysis { get; set; }
        public decimal? CostImpact { get; set; }
        public int? ScheduleImpact { get; set; }
        public string? ReviewNotes { get; set; }
        public string? ApprovalNotes { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ChangeApprovalDto
    {
        public Guid ChangeId { get; set; }
        public bool IsApproved { get; set; }
        public string? Notes { get; set; }
        public string? ApprovedBy { get; set; }
    }
     public class ChangeSummaryDto
        {
            public Guid ProjectId { get; set; }
            public int TotalChanges { get; set; }
            public int ApprovedChanges { get; set; }
            public int RejectedChanges { get; set; }
            public int ImplementedChanges { get; set; }
            public int PendingChanges { get; set; }
            public decimal TotalCostImpact { get; set; }
            public int TotalScheduleImpact { get; set; }
            public Dictionary<string, int> ChangesByType { get; set; } = new();
            public Dictionary<string, int> ChangesByStatus { get; set; } = new();
        }
}
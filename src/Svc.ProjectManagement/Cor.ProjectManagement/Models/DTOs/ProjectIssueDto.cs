// Models/DTOs/ProjectIssueDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectIssueDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public IssueType Type { get; set; }
        public IssuePriority Priority { get; set; }
        public IssueStatus Status { get; set; }
        public string ReportedByName { get; set; } = string.Empty;
        public DateTime ReportedAt { get; set; }
        public string AssignedToName { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string ResolvedByName { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;
        public string RootCause { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public int CommentCount { get; set; }
        public List<ProjectCommentDto> Comments { get; set; } = new List<ProjectCommentDto>();
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != IssueStatus.Resolved && Status != IssueStatus.Closed;
    }

    public class ProjectIssueCreateDto
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

        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }

        public DateTime? DueDate { get; set; }

        public string? ReportedByName { get; set; }

        public string? CreatedBy { get; set; }
    }

    public class ProjectIssueUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public IssueType? Type { get; set; }
        public IssuePriority? Priority { get; set; }
        public IssueStatus? Status { get; set; }
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Resolution { get; set; }
        public string? RootCause { get; set; }
        public string? Impact { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public class IssueSummaryDto
        {
            public Guid ProjectId { get; set; }
            public int TotalIssues { get; set; }
            public int OpenIssues { get; set; }
            public int ResolvedIssues { get; set; }
            public Dictionary<string, int> IssuesByPriority { get; set; } = new();
            public Dictionary<string, int> IssuesByStatus { get; set; } = new();
            public Dictionary<string, int> IssuesByType { get; set; } = new();
            public double AverageResolutionTime { get; set; }
        }
}
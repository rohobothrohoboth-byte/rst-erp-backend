// Models/DTOs/ProjectRiskDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectRiskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public RiskImpact Impact { get; set; }
        public RiskProbability Probability { get; set; }
        public RiskSeverity Severity { get; set; }
        public int RiskScore { get; set; }
        public string MitigationStrategy { get; set; } = string.Empty;
        public string ContingencyPlan { get; set; } = string.Empty;
        public RiskStatus Status { get; set; }
        public string IdentifiedByName { get; set; } = string.Empty;
        public DateTime IdentifiedAt { get; set; }
        public string AssignedToName { get; set; } = string.Empty;
        public DateTime? ReviewDate { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string ResolvedByName { get; set; } = string.Empty;
        public string ResolutionNotes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string RiskLevel => $"{Impact} - {Probability}";
    }

    public class ProjectRiskCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public RiskImpact Impact { get; set; }

        [Required]
        public RiskProbability Probability { get; set; }

        public string MitigationStrategy { get; set; } = string.Empty;
        public string ContingencyPlan { get; set; } = string.Empty;

        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }

        public string? IdentifiedByName { get; set; }

        public string? CreatedBy { get; set; }
    }

    public class ProjectRiskUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public RiskImpact? Impact { get; set; }
        public RiskProbability? Probability { get; set; }
        public string? MitigationStrategy { get; set; }
        public string? ContingencyPlan { get; set; }
        public RiskStatus? Status { get; set; }
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? ResolutionNotes { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class RiskMatrixDto
    {
        public RiskImpact Impact { get; set; }
        public RiskProbability Probability { get; set; }
        public int RiskScore { get; set; }
        public string Severity { get; set; } = string.Empty;
        public string RecommendedAction { get; set; } = string.Empty;
    }
     public class RiskHeatmapDto
        {
            public Guid ProjectId { get; set; }
            public Dictionary<string, Dictionary<string, int>> HeatmapData { get; set; } = new();
            public int TotalRisks { get; set; }
            public int HighRiskCount { get; set; }
        }

        public class RiskSummaryDto
        {
            public Guid ProjectId { get; set; }
            public int TotalRisks { get; set; }
            public int OpenRisks { get; set; }
            public int ResolvedRisks { get; set; }
            public int AcceptedRisks { get; set; }
            public int CriticalRisks { get; set; }
            public Dictionary<string, int> RisksByStatus { get; set; } = new();
            public Dictionary<string, int> RisksBySeverity { get; set; } = new();
            public double AverageRiskScore { get; set; }
        }
}
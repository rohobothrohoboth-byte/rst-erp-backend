// Models/Entities/ProjectRisk.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.ProjectManagement.Models.Entities
{
    public enum RiskImpact
    {
        VeryLow = 1,
        Low = 2,
        Medium = 3,
        High = 4,
        Critical = 5
    }

    public enum RiskProbability
    {
        Rare = 1,
        Unlikely = 2,
        Possible = 3,
        Likely = 4,
        AlmostCertain = 5
    }

    public enum RiskSeverity
    {
        VeryLow = 1,
        Low = 2,
        Medium = 3,
        High = 4,
        Critical = 5
    }

    public enum RiskStatus
    {
        Identified = 1,
        Analyzing = 2,
        Mitigating = 3,
        Monitored = 4,
        Resolved = 5,
        Accepted = 6,
        Closed = 7
    }

    public class ProjectRisk : BaseEntity
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

        public RiskSeverity Severity { get; set; }

        public int RiskScore { get; set; }

        public string MitigationStrategy { get; set; } = string.Empty;
        public string ContingencyPlan { get; set; } = string.Empty;

        [Required]
        public RiskStatus Status { get; set; } = RiskStatus.Identified;

        public Guid? IdentifiedById { get; set; }
        public string IdentifiedByName { get; set; } = string.Empty;
        public DateTime IdentifiedAt { get; set; }

        public Guid? AssignedToId { get; set; }
        public string AssignedToName { get; set; } = string.Empty;

        public DateTime? ReviewDate { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public Guid? ResolvedById { get; set; }
        public string ResolvedByName { get; set; } = string.Empty;
        public string ResolutionNotes { get; set; } = string.Empty;

        public string? RelatedTaskId { get; set; }
        public string RelatedMilestoneId { get; set; } = string.Empty;

        [Column(TypeName = "jsonb")]
        public string? CustomFields { get; set; }

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
    }
}
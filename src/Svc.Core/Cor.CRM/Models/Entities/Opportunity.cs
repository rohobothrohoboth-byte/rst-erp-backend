using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum OpportunityStage
{
    Discovery = 1,
    Qualification = 2,
    Proposal = 3,
    Negotiation = 4,
    ClosedWon = 5,
    ClosedLost = 6
}

public enum WinProbability
{
    VeryLow = 10,
    Low = 30,
    Medium = 50,
    High = 70,
    VeryHigh = 90
}

public class Opportunity : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public OpportunityStage Stage { get; set; } = OpportunityStage.Discovery;
    public WinProbability WinProbability { get; set; } = WinProbability.Medium;

    public DateTime? ExpectedCloseDate { get; set; }
    public DateTime? ActualCloseDate { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }

    [Column(TypeName = "jsonb")]
    public string? ProductsJson { get; set; }

    [MaxLength(500)]
    public string? Competitors { get; set; }

    [MaxLength(500)]
    public string? DecisionMakers { get; set; }

    [MaxLength(500)]
    public string? KeyChallenges { get; set; }

    [MaxLength(500)]
    public string? UniqueValueProposition { get; set; }

    public bool IsActive { get; set; } = true;
    public int ProbabilityScore { get; set; } = 0;
    public int Priority { get; set; } = 0;

    // Revenue forecasting
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ForecastedRevenue { get; set; }

    public DateTime? LastActivityDate { get; set; }
    public int ActivityCount { get; set; } = 0;

    // Navigation Properties
    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("LeadId")]
    public virtual Lead? Lead { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
     public virtual ICollection<OpportunityProduct> OpportunityProducts { get; set; } = new List<OpportunityProduct>();
}
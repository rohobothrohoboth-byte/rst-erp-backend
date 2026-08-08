using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum CampaignStatus
{
    Draft = 1,
    Active = 2,
    Paused = 3,
    Completed = 4,
    Cancelled = 5,
    Archived = 6,
    Scheduled=7
}

public enum CampaignType
{
    Email = 1,
    SocialMedia = 2,
    Advertisement = 3,
    Event = 4,
    DirectMail = 5,
    Telemarketing = 6,
    ContentMarketing = 7,
    Other = 8
}

public class Campaign : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public CampaignType Type { get; set; }
    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Budget { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ActualCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ExpectedRevenue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ActualRevenue { get; set; }

    [MaxLength(500)]
    public string? TargetAudience { get; set; }

    [MaxLength(200)]
    public string? TargetIndustry { get; set; }

    [MaxLength(100)]
    public string? TargetLocation { get; set; }

    public int TargetCount { get; set; } = 0;
    public int ReachCount { get; set; } = 0;
    public int EngagementCount { get; set; } = 0;
    public int ConversionCount { get; set; } = 0;

    [Column(TypeName = "decimal(5,2)")]
    public decimal? ConversionRate { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? EngagementRate { get; set; }

    [MaxLength(50)]
    public string? Channel { get; set; }

    [Column(TypeName = "jsonb")]
    public string? MetricsJson { get; set; }

    [Column(TypeName = "jsonb")]
    public string? ContentJson { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
}
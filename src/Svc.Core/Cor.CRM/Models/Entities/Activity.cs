using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum ActivityType
{
    Call = 1,
    Email = 2,
    Meeting = 3,
    Task = 4,
    Note = 5,
    FollowUp = 6,
    Demo = 7,
    Presentation = 8,
    SiteVisit = 9,
    QuoteSent = 10,
    ContractSigned = 11,
    Other = 12
}

public enum ActivityStatus
{
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Postponed = 5,
    Overdue = 6
}

public class Activity : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ActivityType Type { get; set; }
    public ActivityStatus Status { get; set; } = ActivityStatus.Scheduled;

    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public int DurationMinutes { get; set; } = 0;

    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? CampaignId { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    public bool IsAllDay { get; set; } = false;
    public int? ReminderMinutes { get; set; }

    [MaxLength(500)]
    public string? Outcome { get; set; }

    public DateTime? CompletedAt { get; set; }

    [Column(TypeName = "jsonb")]
    public string? MetadataJson { get; set; }

    // Navigation Properties
    [ForeignKey("LeadId")]
    public virtual Lead? Lead { get; set; }

    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("OpportunityId")]
    public virtual Opportunity? Opportunity { get; set; }

    [ForeignKey("CampaignId")]
    public virtual Campaign? Campaign { get; set; }
}
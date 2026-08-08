// Cor.CRM/Models/Entities/SMSCampaign.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum SMSCampaignStatus
{
    Draft = 1,
    Scheduled = 2,
    Sending = 3,
    Sent = 4,
    Failed = 5,
    Paused = 6,
    Cancelled = 7
}

public class SMSCampaign : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(1600)]
    public string Message { get; set; } = string.Empty;

    public SMSCampaignStatus Status { get; set; } = SMSCampaignStatus.Draft;

    public DateTime? ScheduledDate { get; set; }
    public DateTime? SentDate { get; set; }

    public int RecipientCount { get; set; } = 0;
    public int SentCount { get; set; } = 0;
    public int DeliveredCount { get; set; } = 0;
    public int FailedCount { get; set; } = 0;

    public int CharacterCount { get; set; } = 0;
    public int MessageParts { get; set; } = 1;

    [MaxLength(500)]
    public string? RecipientListJson { get; set; }

    [MaxLength(50)]
    public string? FromNumber { get; set; }

    [Column(TypeName = "jsonb")]
    public string? AnalyticsJson { get; set; }

    public Guid? CampaignId { get; set; }

    [ForeignKey("CampaignId")]
    public virtual Campaign? Campaign { get; set; }
}
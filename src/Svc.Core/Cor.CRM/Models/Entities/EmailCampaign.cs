// Cor.CRM/Models/Entities/EmailCampaign.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum EmailCampaignStatus
{
    Draft = 1,
    Scheduled = 2,
    Sending = 3,
    Sent = 4,
    Failed = 5,
    Paused = 6,
    Cancelled = 7
}

public class EmailCampaign : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? HtmlContent { get; set; }

    public EmailCampaignStatus Status { get; set; } = EmailCampaignStatus.Draft;

    public DateTime? ScheduledDate { get; set; }
    public DateTime? SentDate { get; set; }

    public int RecipientCount { get; set; } = 0;
    public int SentCount { get; set; } = 0;
    public int DeliveredCount { get; set; } = 0;
    public int OpenCount { get; set; } = 0;
    public int ClickCount { get; set; } = 0;
    public int BounceCount { get; set; } = 0;
    public int UnsubscribeCount { get; set; } = 0;

    [Column(TypeName = "decimal(5,2)")]
    public decimal? OpenRate { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? ClickRate { get; set; }

    [MaxLength(500)]
    public string? RecipientListJson { get; set; }

    public Guid? TemplateId { get; set; }

    [ForeignKey("TemplateId")]
    public virtual EmailTemplate? Template { get; set; }

    [Column(TypeName = "jsonb")]
    public string? AnalyticsJson { get; set; }

    public Guid? CampaignId { get; set; }

    [ForeignKey("CampaignId")]
    public virtual Campaign? Campaign { get; set; }
}
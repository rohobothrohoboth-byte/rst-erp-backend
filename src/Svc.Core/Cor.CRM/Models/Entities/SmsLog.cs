// SmsLog.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum SmsStatus
{
    Sent = 1,
    Delivered = 2,
    Failed = 3,
    Pending = 4
}

public class SmsLog : BaseEntity
{
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? CampaignId { get; set; }

    [Required]
    [MaxLength(20)]
    public string FromNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ToNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(1600)]
    public string Message { get; set; } = string.Empty;

    public SmsStatus Status { get; set; } = SmsStatus.Pending;

    public DateTime? SentDate { get; set; }
    public DateTime? DeliveredDate { get; set; }

    [MaxLength(500)]
    public string? ErrorMessage { get; set; }

    [MaxLength(50)]
    public string? MessageId { get; set; }

    public int? CharacterCount { get; set; }
    public int? MessageParts { get; set; }

    // Navigation Properties
    [ForeignKey("LeadId")]
    public virtual Lead? Lead { get; set; }

    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("CampaignId")]
    public virtual Campaign? Campaign { get; set; }
}
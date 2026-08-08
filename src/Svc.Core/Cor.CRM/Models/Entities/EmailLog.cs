// EmailLog.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum EmailStatus
{
    Draft = 1,
    Sent = 2,
    Delivered = 3,
    Opened = 4,
    Clicked = 5,
    Bounced = 6,
    Failed = 7
}

public class EmailLog : BaseEntity
{
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? CampaignId { get; set; }
    public Guid? TemplateId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FromEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string ToEmail { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Cc { get; set; }

    [MaxLength(255)]
    public string? Bcc { get; set; }

    [Required]
    [MaxLength(500)]
    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;
    public string? HtmlBody { get; set; }

    public EmailStatus Status { get; set; } = EmailStatus.Draft;

    public DateTime? SentDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public DateTime? OpenedDate { get; set; }
    public DateTime? ClickedDate { get; set; }

    public int OpenCount { get; set; } = 0;
    public int ClickCount { get; set; } = 0;

    [MaxLength(500)]
    public string? ErrorMessage { get; set; }

    [MaxLength(255)]
    public string? MessageId { get; set; }

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    [MaxLength(255)]
    public string? UserAgent { get; set; }

    // Navigation Properties
    [ForeignKey("LeadId")]
    public virtual Lead? Lead { get; set; }

    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("CampaignId")]
    public virtual Campaign? Campaign { get; set; }

    [ForeignKey("TemplateId")]
    public virtual EmailTemplate? Template { get; set; }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public class Note : BaseEntity
{
    [Required]
    public string Content { get; set; } = string.Empty;

    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? CampaignId { get; set; }

    public bool IsPinned { get; set; } = false;
    public bool IsPrivate { get; set; } = false;

    [MaxLength(500)]
    public string? Category { get; set; }

    [MaxLength(500)]
    public string? Tags { get; set; }

    public DateTime? PinnedAt { get; set; }

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
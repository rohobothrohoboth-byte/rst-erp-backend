// Quote.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum QuoteStatus
{
    Draft = 1,
    Sent = 2,
    Viewed = 3,
    Negotiating = 4,
    Accepted = 5,
    Rejected = 6,
    Expired = 7,
    Converted = 8
}

public class Quote : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string QuoteNumber { get; set; } = string.Empty;

    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ShippingCost { get; set; }

    public DateTime? ValidUntil { get; set; }
    public DateTime? SentDate { get; set; }
    public DateTime? AcceptedDate { get; set; }

    public QuoteStatus Status { get; set; } = QuoteStatus.Draft;

    [MaxLength(500)]
    public string? TermsAndConditions { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public int ViewCount { get; set; } = 0;
    public DateTime? LastViewedAt { get; set; }

    public string? CustomFieldsJson { get; set; }

    // Navigation Properties
    [ForeignKey("LeadId")]
    public virtual Lead? Lead { get; set; }

    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("OpportunityId")]
    public virtual Opportunity? Opportunity { get; set; }

    public virtual ICollection<QuoteLine> QuoteLines { get; set; } = new List<QuoteLine>();
}
// Invoice.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum InvoiceStatus
{
    Draft = 1,
    Sent = 2,
    Viewed = 3,
    Paid = 4,
    Partial = 5,
    Overdue = 6,
    Cancelled = 7,
    Refunded = 8
}

public enum InvoiceType
{
    Sales = 1,
    Purchase = 2,
    Credit = 3,
    Debit = 4
}

public class Invoice : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;
     public Guid? OrderId { get; set; }
    public virtual SalesOrder? Order { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? QuoteId { get; set; }

    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AmountPaid { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? BalanceDue { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public InvoiceType Type { get; set; } = InvoiceType.Sales;

    [MaxLength(500)]
    public string? Terms { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(500)]
    public string? BillingAddress { get; set; }

    [MaxLength(500)]
    public string? ShippingAddress { get; set; }

    public int? PaymentTermDays { get; set; }

    [MaxLength(50)]
    public string? Currency { get; set; }

    public decimal? ExchangeRate { get; set; }

    public string? CustomFieldsJson { get; set; }

    // Navigation Properties
    [ForeignKey("LeadId")]
    public virtual Lead? Lead { get; set; }

    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("OpportunityId")]
    public virtual Opportunity? Opportunity { get; set; }

    [ForeignKey("QuoteId")]
    public virtual Quote? Quote { get; set; }

    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
// Cor.CRM/Models/Entities/SalesOrder.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum OrderStatus
{
    Draft = 1,
    Pending = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Completed = 6,
    Cancelled = 7,
    Refunded = 8
}

public class SalesOrder : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    // Customer
    public Guid CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    // Optional: Link to Opportunity
    public Guid? OpportunityId { get; set; }
    [ForeignKey(nameof(OpportunityId))]
    public virtual Opportunity? Opportunity { get; set; }

    // Optional: Link to Quote
    public Guid? QuoteId { get; set; }
    [ForeignKey(nameof(QuoteId))]
    public virtual Quote? Quote { get; set; }

    // Optional: Link to Invoice (one-to-one)
    public Guid? InvoiceId { get; set; }
    [ForeignKey(nameof(InvoiceId))]
    public virtual Invoice? Invoice { get; set; }

    // Dates
    public DateTime OrderDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? CancelledDate { get; set; }

    // Financial
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AmountPaid { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? BalanceDue { get; set; }

    // Addresses
    [MaxLength(500)]
    public string? ShippingAddress { get; set; }

    [MaxLength(500)]
    public string? BillingAddress { get; set; }

    // Terms
    [MaxLength(500)]
    public string? Terms { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(10)]
    public string? Currency { get; set; } = "USD";

    // Status
    public OrderStatus Status { get; set; } = OrderStatus.Draft;

    // Tracking
    [MaxLength(100)]
    public string? TrackingNumber { get; set; }

    [MaxLength(100)]
    public string? Carrier { get; set; }

    // Navigation Properties
    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
}
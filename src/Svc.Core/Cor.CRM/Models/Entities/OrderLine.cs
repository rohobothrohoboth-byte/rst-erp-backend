// Cor.CRM/Models/Entities/OrderLine.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public class OrderLine : BaseEntity
{
    public Guid OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public virtual SalesOrder? Order { get; set; }

    // Optional: Link to Product
    public Guid? ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public virtual Product? Product { get; set; }

    // Optional: Link to QuoteLine (to track source)
    public Guid? QuoteLineId { get; set; }
    [ForeignKey(nameof(QuoteLineId))]
    public virtual QuoteLine? QuoteLine { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    public int SortOrder { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
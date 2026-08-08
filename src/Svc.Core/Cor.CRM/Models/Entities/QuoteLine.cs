// QuoteLine.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public class QuoteLine : BaseEntity
{
    public Guid QuoteId { get; set; }
    public Guid? ProductId { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TaxRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    public int? SortOrder { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation Properties
    [ForeignKey("QuoteId")]
    public virtual Quote Quote { get; set; } = null!;

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}
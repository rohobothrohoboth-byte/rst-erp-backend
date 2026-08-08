using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Procurement.Models.Entities;

public class InvoiceLineItem : BaseEntity
{
    [Required]
    public Guid InvoiceId { get; set; }

    [Required]
    public Guid PurchaseOrderItemId { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TaxAmount { get; set; }

    // Navigation
    [ForeignKey(nameof(InvoiceId))]
    public virtual Invoice? Invoice { get; set; }
}
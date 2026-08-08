using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Procurement.Models.Entities;

public class GoodsReceiptItem : BaseEntity
{
    [Required]
    public Guid GoodsReceiptNoteId { get; set; }

    [Required]
    public Guid PurchaseOrderItemId { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public int QuantityReceived { get; set; }

    [Required]
    public int QuantityAccepted { get; set; }

    [Required]
    public int QuantityRejected { get; set; }

    [MaxLength(50)]
    public string? Condition { get; set; } // Good, Damaged, Partial

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    // Navigation
    [ForeignKey(nameof(GoodsReceiptNoteId))]
    public virtual GoodsReceiptNote? GoodsReceiptNote { get; set; }
}
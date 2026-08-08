using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Procurement.Models.Entities;

public class GoodsReceiptNote : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string GrnNumber { get; set; } = string.Empty;

    [Required]
    public Guid PurchaseOrderId { get; set; }

    [MaxLength(50)]
    public string? PurchaseOrderNumber { get; set; }

    [MaxLength(100)]
    public string? DeliveryNoteNumber { get; set; }

    [Required]
    public DateTime ReceivedDate { get; set; }

    [Required]
    public Guid WarehouseId { get; set; }

    [MaxLength(200)]
    public string? WarehouseName { get; set; }

    [Required]
    [MaxLength(100)]
    public string ReceivedBy { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? InspectedBy { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Draft"; // Draft, Completed, Cancelled

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalReceived { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAccepted { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalRejected { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime? CompletedDate { get; set; }

    // Navigation
    [ForeignKey(nameof(PurchaseOrderId))]
    public virtual PurchaseOrder? PurchaseOrder { get; set; }

    public virtual ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>();
}
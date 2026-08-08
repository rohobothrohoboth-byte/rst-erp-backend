using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Procurement.Models.Entities;

public class InspectionItem : BaseEntity
{
    [Required]
    public Guid InspectionId { get; set; }

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

    [MaxLength(20)]
    public string? Condition { get; set; } // Good, Damaged, Partial

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? UnitPrice { get; set; }

    [MaxLength(100)]
    public string? InspectedBy { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Passed, Failed

    // Navigation
    [ForeignKey(nameof(InspectionId))]
    public virtual Inspection? Inspection { get; set; }
}
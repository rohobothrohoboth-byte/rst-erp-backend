// Models/Entities/PurchaseOrder.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class PurchaseOrder : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string PurchaseOrderNumber { get; set; } = string.Empty;

    [Required]
    public DateTime OrderDate { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    [Required]
    public Guid VendorId { get; set; }

    [MaxLength(200)]
    public string? VendorName { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Draft";

    [MaxLength(20)]
    public string? Currency { get; set; } = "USD";

    public DateTime? ReceivedDate { get; set; }

    public Guid? ReceivedBy { get; set; }

    // ❌ REMOVE: public Guid PeriodId { get; set; }
    // ❌ REMOVE: public virtual FinancialPeriod? FinancialPeriod { get; set; }
    // ✅ PeriodId and Period are inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }

    public virtual ICollection<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();
}
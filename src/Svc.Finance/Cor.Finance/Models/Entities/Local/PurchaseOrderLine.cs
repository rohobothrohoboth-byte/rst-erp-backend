// Models/Entities/PurchaseOrderLine.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class PurchaseOrderLine : BaseEntity
{
    [Required]
    public Guid PurchaseOrderId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TaxRate { get; set; }

    // ❌ REMOVED: public bool IsDeleted { get; set; } = false; (inherited from BaseEntity)

    // ✅ PeriodId is inherited from BaseEntity
    // ❌ REMOVED duplicate: public Guid PeriodId { get; set; }
    // ❌ REMOVED duplicate: public virtual FinancialPeriod? FinancialPeriod { get; set; }

    // Navigation
    [ForeignKey(nameof(PurchaseOrderId))]
    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
}
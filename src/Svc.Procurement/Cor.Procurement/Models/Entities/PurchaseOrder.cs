// Models/Entities/PurchaseOrder.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Procurement.Models.Entities.Local;

namespace Cor.Procurement.Models.Entities;

public class PurchaseOrder : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string PurchaseOrderNumber { get; set; } = string.Empty;

    [Required]
    public DateTime OrderDate { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    public Guid? VendorId { get; set; }

    [MaxLength(200)]
    public string? VendorName { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Draft";

    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    public DateTime? ReceivedDate { get; set; }

    [MaxLength(100)]
    public string? ReceivedBy { get; set; }

    public Guid? RequisitionId { get; set; }

    [MaxLength(50)]
    public string? RequisitionNumber { get; set; }

    [MaxLength(200)]
    public string? PaymentTerms { get; set; }

    [MaxLength(500)]
    public string? ShippingAddress { get; set; }

    public DateTime? SentDate { get; set; }

    public Guid? SentBy { get; set; }

    public DateTime? ConfirmedDate { get; set; }

    public Guid? ConfirmedBy { get; set; }

    // ✅ Add missing properties
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public DateTime? CancelledDate { get; set; }

    public new bool IsDeleted { get; set; }

    public new DateTime DateAdd { get; set; }

    public new DateTime? DateMod { get; set; }

    public new string? RowVersion { get; set; }

    public new Guid? PeriodId { get; set; }

    // ============================================================
    // ✅ NAVIGATION PROPERTIES
    // ============================================================

    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }

    [ForeignKey(nameof(RequisitionId))]
    public virtual Requisition? Requisition { get; set; }

    [ForeignKey(nameof(PeriodId))]
    public new virtual FinancialPeriod? Period { get; set; }

    public virtual ICollection<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();
}
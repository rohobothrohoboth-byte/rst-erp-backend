// Models/Entities/PurchaseOrderLine.cs
using System;

namespace Cor.Procurement.Models.Entities;

public class PurchaseOrderLine : BaseEntity
{
    public Guid PurchaseOrderId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? UnitOfMeasure { get; set; }
    public Guid? RequisitionLineId { get; set; }
    public new bool IsDeleted { get; set; }
    public new DateTime DateAdd { get; set; }
    public new DateTime? DateMod { get; set; }
    public new string? RowVersion { get; set; }
    public new Guid? PeriodId { get; set; }

    // Navigation
    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
}
// Models/Entities/RequisitionLine.cs
using System;

namespace Cor.Procurement.Models.Entities;

public class RequisitionLine : BaseEntity
{
    public Guid RequisitionId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string? UnitOfMeasure { get; set; }
    public string? Notes { get; set; }
    public new bool IsDeleted { get; set; }
    public new DateTime DateAdd { get; set; }
    public new DateTime? DateMod { get; set; }
    public new string? RowVersion { get; set; }

    // Navigation
    public virtual Requisition Requisition { get; set; } = null!;
}
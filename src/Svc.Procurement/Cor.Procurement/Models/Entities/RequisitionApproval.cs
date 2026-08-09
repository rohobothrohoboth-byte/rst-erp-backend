// Models/Entities/RequisitionApproval.cs
using System;

namespace Cor.Procurement.Models.Entities;

public class RequisitionApproval : BaseEntity
{
    public Guid RequisitionId { get; set; }
    public Guid ApproverId { get; set; }
    public string? ApproverName { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string? Comments { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int ApprovalLevel { get; set; }
    public new bool IsDeleted { get; set; }
    public new DateTime DateAdd { get; set; }
    public new DateTime? DateMod { get; set; }

    // Navigation
    public virtual Requisition Requisition { get; set; } = null!;
}
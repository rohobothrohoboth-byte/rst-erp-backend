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
    public bool IsDeleted { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }

    // Navigation
    public virtual Requisition Requisition { get; set; } = null!;
}
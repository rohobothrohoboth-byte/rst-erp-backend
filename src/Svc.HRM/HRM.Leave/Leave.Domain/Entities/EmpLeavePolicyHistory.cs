// Leave.Domain/Entities/EmpLeavePolicyHistory.cs
using System;

namespace Leave.Domain.Entities;

public class EmpLeavePolicyHistory  // Remove : BaseEntity
{
    public Guid Id { get; set; }
    public Guid OriginalId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public Guid? LeavePolicyId { get; set; }
    public decimal AssignedEntitlement { get; set; }
    public decimal UsedEntitlement { get; set; }
    public decimal CarryForward { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public string? AssignmentReason { get; set; }
    public string? Reason { get; set; }
    public DateTime? DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime ArchivedDate { get; set; }
    public string ArchiveReason { get; set; } = string.Empty;
    public Guid? ProcessedBy { get; set; }
    public int? ProcessedYear { get; set; }
}
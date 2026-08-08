// Leave.Domain/Entities/EmpLeavePolicy.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace Leave.Domain.Entities;

public class EmpLeavePolicy : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public Guid? LeavePolicyId { get; set; }  // CHANGE: Make this nullable (Guid? instead of Guid)
    public decimal AssignedEntitlement { get; set; }
    public decimal UsedEntitlement { get; set; }
    public decimal RemainingEntitlement => AssignedEntitlement - UsedEntitlement;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string AssignmentReason { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
 public decimal CarryForward { get; set; } = 0;  // Add this
    // Navigation properties
    [ForeignKey("LeaveTypeId")]
    public virtual LeaveType? LeaveType { get; set; }  // Make nullable

    [ForeignKey("LeavePolicyId")]
    public virtual LeavePolicy? LeavePolicy { get; set; }  // Make nullable
}
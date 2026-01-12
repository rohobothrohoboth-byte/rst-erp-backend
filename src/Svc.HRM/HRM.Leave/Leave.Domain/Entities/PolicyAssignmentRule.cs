namespace Leave.Domain.Entities;

public class PolicyAssignmentRule : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int Priority { get; set; } // lower = higher priority
    public bool IsActive { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public Guid LeaveTypeId { get; set; } // LeaveType

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
    public LeavePolicy LeavePolicy { get; set; } = null!;
}
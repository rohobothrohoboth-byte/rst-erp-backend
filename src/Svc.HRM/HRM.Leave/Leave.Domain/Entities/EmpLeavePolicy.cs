namespace Leave.Domain.Entities;

public class EmpLeavePolicy : BaseEntity
{
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public double AssignedEntitlement { get; set; } 
    public string Reason { get; set; } = default!; // enum.EmpLeavePolReason(0/1)
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public Guid LeavePolicyId { get; set; } // LeavePolicy

    //******************************************//

    public bool IsActive(DateTime date) => date >= EffectiveFrom && (EffectiveTo == null || date <= EffectiveTo);

    public LeaveType LeaveType { get; set; } = null!;
    public LeavePolicy LeavePolicy { get; set; } = null!;
}
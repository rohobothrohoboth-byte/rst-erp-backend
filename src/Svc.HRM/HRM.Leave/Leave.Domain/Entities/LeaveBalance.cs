namespace Leave.Domain.Entities;

public class LeaveBalance : BaseEntity
{
    public double Balance { get; set; }
    public DateTime AsOf { get; set; } = DateTime.UtcNow;
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public Guid LeavePolicyId { get; set; } // LeavePolicy

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
    public LeavePolicy LeavePolicy { get; set; } = null!;
}
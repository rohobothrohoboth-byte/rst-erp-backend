namespace Leave.Domain.Entities;

public class LeaveBalance : BaseEntity
{
    public double Balance { get; set; }
    public DateTime AsOf { get; set; } = DateTime.UtcNow;
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public Guid? LeaveLedgerId { get; set; } // LeaveLedger

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
    public LeaveLedger LeaveLedger { get; set; } = null!;
}
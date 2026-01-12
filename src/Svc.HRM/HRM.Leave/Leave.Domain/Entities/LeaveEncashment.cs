namespace Leave.Domain.Entities;

public class LeaveEncashment : BaseEntity
{
    public double DaysEncashed { get; set; }
    public double RatePerDay { get; set; }
    public double TotalAmount { get; set; }
    public string Status { get; set; } = default!; // enum.Status (0/1)
    public int CurrentAppStep { get; set; } = 0;
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public Guid? LeavePolicyId { get; set; } // LeavePolicy

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
    public LeavePolicy LeavePolicy { get; set; } = null!;
}
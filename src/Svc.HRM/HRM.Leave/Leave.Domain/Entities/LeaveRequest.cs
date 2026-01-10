namespace Leave.Domain.Entities;

public class LeaveRequest : BaseEntity
{
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public double DaysRequested { get; set; } = default!;
    public bool IsHalfDay { get; set; } = false;
    public string Status { get; set; } = default!; // enum.Status (0/1)
    public DateTime? DateApproved { get; set; } = default!;
    public string Comments { get; set; } = default!;
    public int CurrentAppStep { get; set; } = 0;
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid? ApprovedById { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
}
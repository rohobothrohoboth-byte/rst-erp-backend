namespace Leave.Domain.Entities;

public class LeaveRequest : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Guid? ApprovedById { get; set; }
    public Guid LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public double DaysRequested { get; set; } = default!;
    public bool IsHalfDay { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.LeaveRequestStatus (0/1)
    public DateTime? DateApproved { get; set; } = default!;
    public string Comments { get; set; } = default!;

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
}
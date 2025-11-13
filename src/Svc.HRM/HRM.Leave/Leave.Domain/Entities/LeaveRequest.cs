namespace Leave.Domain.Entities;

public class LeaveRequest : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public double DaysRequested { get; set; } = default!;
    public bool IsHalfDayStart { get; set; } = default!;
    public bool IsHalfDayEnd { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.LeaveRequestStatus (0/1)
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; } = default!;
    public string? Comments { get; set; } = default!;
    public Guid? ApproverId { get; set; } // optional: approver (Employee)
    public Guid LeaveTypeId { get; set; }

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
}
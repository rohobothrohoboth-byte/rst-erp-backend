namespace Leave.Domain.DTOs;

public class ApprovedLeaveRangeDto
{
    public Guid RequestId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = default!;
    public string LeaveCategory { get; set; } = default!; // Paid, Unpaid, Special
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DaysRequested { get; set; }
    public bool IsHalfDay { get; set; }
    public string Status { get; set; } = default!;
}

public class UnpaidLeaveDaysDto
{
    public Guid EmployeeId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public double UnpaidDays { get; set; }
    public List<ApprovedLeaveRangeDto> Items { get; set; } = [];
}

namespace Svc.HRM.Attendance.Models.DTOs;

public class LeaveRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string EmployeeCode { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DaysRequested { get; set; }
    public double DaysApproved { get; set; }
    public string Reason { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? RejectedReason { get; set; }
    public bool IsPaid { get; set; }
}

public class LeaveRequestCreateDto
{
    public Guid EmployeeId { get; set; }
    public string LeaveType { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = default!;
    public bool IsPaid { get; set; } = true;
}

public class LeaveApproveDto
{
    public Guid Id { get; set; }
    public double DaysApproved { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Notes { get; set; }
}

public class LeaveBalanceDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
    public double TotalDays { get; set; }
    public double UsedDays { get; set; }
    public double BalanceDays { get; set; }
    public int Year { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
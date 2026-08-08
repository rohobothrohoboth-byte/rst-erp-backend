namespace Svc.HRM.Attendance.Models.DTOs;

public class OvertimeDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string EmployeeCode { get; set; } = default!;
    public DateTime Date { get; set; }
    public double HoursRequested { get; set; }
    public double HoursApproved { get; set; }
    public string Reason { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? RejectedReason { get; set; }
}

public class OvertimeRequestDto
{
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public double HoursRequested { get; set; }
    public string Reason { get; set; } = default!;
}

public class OvertimeApproveDto
{
    public Guid Id { get; set; }
    public double HoursApproved { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Notes { get; set; }
}
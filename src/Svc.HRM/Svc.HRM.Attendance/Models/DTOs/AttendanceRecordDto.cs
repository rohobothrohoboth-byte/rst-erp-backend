namespace Svc.HRM.Attendance.Models.DTOs;

public class AttendanceRecordDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string EmployeeCode { get; set; } = default!;
    public DateTime Date { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public string Status { get; set; } = default!;
    public double HoursWorked { get; set; }
    public double OvertimeHours { get; set; }
    public bool IsLate { get; set; }
    public int LateMinutes { get; set; }
    public bool IsEarlyDeparture { get; set; }
    public int EarlyDepartureMinutes { get; set; }
    public string? ShiftName { get; set; }
    public string? Notes { get; set; }
}

public class ClockInDto
{
    public Guid EmployeeId { get; set; }
    public DateTime? CheckIn { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

public class ClockOutDto
{
    public Guid EmployeeId { get; set; }
    public DateTime? CheckOut { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

public class UpdateAttendanceDto
{
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

public class AttendanceSummaryDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int LeaveDays { get; set; }
    public int HolidayDays { get; set; }
    public int WeekendDays { get; set; }
    public double TotalHoursWorked { get; set; }
    public double TotalOvertimeHours { get; set; }
    public double AverageHoursPerDay { get; set; }
    public double AttendanceRate { get; set; }
}
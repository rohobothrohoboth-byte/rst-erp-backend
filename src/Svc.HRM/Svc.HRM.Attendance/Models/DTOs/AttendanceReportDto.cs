namespace Svc.HRM.Attendance.Models.DTOs;

public class AttendanceReportDto
{
    public DateTime ReportDate { get; set; }
    public string ReportType { get; set; } = default!; // Daily, Weekly, Monthly
    public int TotalEmployees { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public int LeaveCount { get; set; }
    public int HolidayCount { get; set; }
    public double AttendanceRate { get; set; }
    public List<AttendanceRecordDto> Records { get; set; } = new();
    public List<DepartmentAttendanceSummary> DepartmentSummaries { get; set; } = new();
}

public class DepartmentAttendanceSummary
{
    public string DepartmentName { get; set; } = default!;
    public int TotalEmployees { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double AttendanceRate { get; set; }
}
using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Enums;
namespace Svc.HRM.Attendance.Services;

public interface IAttendanceService
{
    // Clock in/out
    Task<AttendanceRecordDto> ClockInAsync(Guid employeeId, ClockInDto dto, CancellationToken ct = default);
    Task<AttendanceRecordDto> ClockOutAsync(Guid employeeId, ClockOutDto dto, CancellationToken ct = default);

    // Get attendance
    Task<AttendanceRecordDto> GetAttendanceByIdAsync(Guid id, CancellationToken ct = default);
    Task<AttendanceRecordDto> GetTodayAttendanceAsync(Guid employeeId, CancellationToken ct = default);
    Task<List<AttendanceRecordDto>> GetAttendanceByPeriodAsync(Guid employeeId, DateTime start, DateTime end, CancellationToken ct = default);
    Task<List<AttendanceRecordDto>> GetAttendanceByDateAsync(DateTime date, CancellationToken ct = default);
    Task<AttendanceSummaryDto> GetAttendanceSummaryAsync(Guid employeeId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default);

    // Update
    Task<AttendanceRecordDto> UpdateAttendanceAsync(Guid id, UpdateAttendanceDto dto, CancellationToken ct = default);

    // Reports
    Task<AttendanceReportDto> GetDailyReportAsync(DateTime date, CancellationToken ct = default);
    Task<AttendanceReportDto> GetMonthlyReportAsync(int year, int month, CancellationToken ct = default);
    Task<List<AttendanceRecordDto>> GetLateEmployeesAsync(DateTime date, int? thresholdMinutes = 15, CancellationToken ct = default);
    Task<List<AttendanceRecordDto>> GetAbsentEmployeesAsync(DateTime date, CancellationToken ct = default);

    // Batch processing
    Task ProcessDailyAttendanceAsync(DateTime date, CancellationToken ct = default);

    // Admin
    Task MarkAttendanceAsync(Guid employeeId, DateTime date, string status, string? notes = null, CancellationToken ct = default);
}
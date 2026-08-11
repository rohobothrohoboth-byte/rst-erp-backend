using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Helpers;
using Common;
namespace Svc.HRM.Attendance.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/attendance")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(IAttendanceService attendanceService, ILogger<AttendanceController> logger)
    {
        _attendanceService = attendanceService;
        _logger = logger;
    }

    /// <summary>
    /// Clock in
    /// </summary>
    [PerAuth("hr.attend.checkin.do")]
    [HttpPost("clock-in")]
    public async Task<IActionResult> ClockIn([FromBody] ClockInDto dto, CancellationToken ct)
    {
        var result = await _attendanceService.ClockInAsync(dto.EmployeeId, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Clock out
    /// </summary>
    [PerAuth("hr.attend.checkin.do")]
    [HttpPost("clock-out")]
    public async Task<IActionResult> ClockOut([FromBody] ClockOutDto dto, CancellationToken ct)
    {
        var result = await _attendanceService.ClockOutAsync(dto.EmployeeId, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get today's attendance for employee
    /// </summary>
    [PerAuth("hr.attend.list.view")]
    [HttpGet("today/{employeeId}")]
    public async Task<IActionResult> GetToday(Guid employeeId, CancellationToken ct)
    {
        var result = await _attendanceService.GetTodayAttendanceAsync(employeeId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get attendance by period
    /// </summary>
    [PerAuth("hr.attend.list.view")]
    [HttpGet("employee/{employeeId}/period")]
    public async Task<IActionResult> GetByPeriod(Guid employeeId, [FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken ct)
    {
        var result = await _attendanceService.GetAttendanceByPeriodAsync(employeeId, start, end, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get attendance summary
    /// </summary>
    [PerAuth("hr.attend.report.view")]
    [HttpGet("employee/{employeeId}/summary")]
    public async Task<IActionResult> GetSummary(Guid employeeId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var result = await _attendanceService.GetAttendanceSummaryAsync(employeeId, from, to, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get daily report
    /// </summary>
    [PerAuth("hr.attend.report.view")]
    [HttpGet("report/daily")]
    public async Task<IActionResult> GetDailyReport([FromQuery] DateTime date, CancellationToken ct)
    {
        var result = await _attendanceService.GetDailyReportAsync(date, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get monthly report
    /// </summary>
    [PerAuth("hr.attend.report.view")]
    [HttpGet("report/monthly")]
    public async Task<IActionResult> GetMonthlyReport([FromQuery] int year, [FromQuery] int month, CancellationToken ct)
    {
        var result = await _attendanceService.GetMonthlyReportAsync(year, month, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get late employees
    /// </summary>
    [PerAuth("hr.attend.list.view")]
    [HttpGet("late/{date}")]
    public async Task<IActionResult> GetLateEmployees(DateTime date, [FromQuery] int? thresholdMinutes, CancellationToken ct)
    {
        var result = await _attendanceService.GetLateEmployeesAsync(date, thresholdMinutes, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get absent employees
    /// </summary>
    [PerAuth("hr.attend.list.view")]
    [HttpGet("absent/{date}")]
    public async Task<IActionResult> GetAbsentEmployees(DateTime date, CancellationToken ct)
    {
        var result = await _attendanceService.GetAbsentEmployeesAsync(date, ct);
        return Ok(result);
    }

    /// <summary>
    /// Update attendance record
    /// </summary>
    [PerAuth("hr.attend.checkin.correction")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttendanceDto dto, CancellationToken ct)
    {
        var result = await _attendanceService.UpdateAttendanceAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Mark attendance manually (Admin)
    /// </summary>
    [PerAuth("hr.attend.checkin.manual")]
    [HttpPost("admin/mark")]
    public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceRequest request, CancellationToken ct)
    {
        await _attendanceService.MarkAttendanceAsync(request.EmployeeId, request.Date, request.Status, request.Notes, ct);
        return Ok(new { message = "Attendance marked successfully" });
    }

    /// <summary>
    /// Process daily attendance
    /// </summary>
    [PerAuth("hr.attend.manage")]
    [HttpPost("admin/process-daily")]
    public async Task<IActionResult> ProcessDaily([FromQuery] DateTime? date, CancellationToken ct)
    {
        var targetDate = date ?? DateTime.UtcNow.Date;
        await _attendanceService.ProcessDailyAttendanceAsync(targetDate, ct);
        return Ok(new { message = $"Daily attendance processed for {targetDate:yyyy-MM-dd}" });
    }
}

public class MarkAttendanceRequest
{
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = default!;
    public string? Notes { get; set; }
}
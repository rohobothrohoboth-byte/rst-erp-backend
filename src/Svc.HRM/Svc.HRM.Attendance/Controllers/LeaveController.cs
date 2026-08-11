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
[Route("api/v{version:apiVersion}/leave")]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;
    private readonly ILogger<LeaveController> _logger;

    public LeaveController(ILeaveService leaveService, ILogger<LeaveController> logger)
    {
        _leaveService = leaveService;
        _logger = logger;
    }

    /// <summary>
    /// Get all leave requests (with optional filtering)
    /// </summary>
    [PerAuth("leave.approve.view")]
    [HttpGet("requests")]
    public async Task<IActionResult> GetAllRequests([FromQuery] string? status = null, CancellationToken ct = default)
    {
        if (status?.ToLower() == "pending")
        {
            var pending = await _leaveService.GetPendingLeaveRequestsAsync(ct);
            return Ok(pending);
        }

        // Return all leave requests (you might want to add pagination)
        // For now, return pending as default
        var allPending = await _leaveService.GetPendingLeaveRequestsAsync(ct);
        return Ok(allPending);
    }

    /// <summary>
    /// Get leave requests by employee ID
    /// </summary>
    [PerAuth("my.leave.view")]
    [HttpGet("requests/employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, CancellationToken ct)
    {
        var requests = await _leaveService.GetEmployeeLeaveRequestsAsync(employeeId, ct);
        return Ok(requests);
    }

    /// <summary>
    /// Get leave request by ID
    /// </summary>
    [PerAuth("my.leave.view")]
    [HttpGet("requests/{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var request = await _leaveService.GetLeaveRequestAsync(id, ct);
        return Ok(request);
    }

    /// <summary>
    /// Create a leave request
    /// </summary>
    [PerAuth("my.leave.add")]
    [HttpPost("requests")]
    public async Task<IActionResult> Create([FromBody] LeaveRequestCreateDto dto, CancellationToken ct)
    {
        var result = await _leaveService.CreateLeaveRequestAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update a leave request
    /// </summary>
    [PerAuth("my.leave.mod")]
    [HttpPut("requests/{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] LeaveRequestCreateDto dto, CancellationToken ct)
    {
        var result = await _leaveService.UpdateLeaveRequestAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Approve a leave request
    /// </summary>
    [PerAuth("leave.approve.process")]
    [HttpPost("requests/{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] LeaveApproveDto dto, CancellationToken ct)
    {
        var result = await _leaveService.ApproveLeaveRequestAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Reject a leave request
    /// </summary>
    [PerAuth("leave.approve.process")]
    [HttpPost("requests/{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectLeaveRequest request, CancellationToken ct)
    {
        var result = await _leaveService.RejectLeaveRequestAsync(id, request.Reason, request.RejectedBy, ct);
        return Ok(result);
    }

    /// <summary>
    /// Delete a leave request
    /// </summary>
    [PerAuth("my.leave.del")]
    [HttpDelete("requests/{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _leaveService.DeleteLeaveRequestAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Get leave balances for an employee
    /// </summary>
    [PerAuth("leave.balance.view")]
    [HttpGet("balances/employee/{employeeId}")]
    public async Task<IActionResult> GetBalances(Guid employeeId, [FromQuery] int year, CancellationToken ct)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        var balances = await _leaveService.GetEmployeeLeaveBalancesAsync(employeeId, year, ct);
        return Ok(balances);
    }

    /// <summary>
    /// Get leave balance by type
    /// </summary>
    [PerAuth("leave.balance.view")]
    [HttpGet("balances/employee/{employeeId}/type/{leaveType}")]
    public async Task<IActionResult> GetBalanceByType(Guid employeeId, string leaveType, [FromQuery] int year, CancellationToken ct)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        var balance = await _leaveService.GetLeaveBalanceAsync(employeeId, leaveType, year, ct);
        return Ok(balance);
    }

    /// <summary>
    /// Initialize leave balance for an employee
    /// </summary>
    [PerAuth("hr.leave.manage")]
    [HttpPost("balances/employee/{employeeId}/initialize")]
    public async Task<IActionResult> InitializeBalance(Guid employeeId, [FromQuery] int year, CancellationToken ct)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        await _leaveService.InitializeLeaveBalanceAsync(employeeId, year, ct);
        return Ok(new { message = $"Leave balance initialized for employee {employeeId} for year {year}" });
    }

    /// <summary>
    /// Get leave calendar
    /// </summary>
    [PerAuth("my.leave.view")]
    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendar([FromQuery] int year, [FromQuery] int? month, CancellationToken ct)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        var calendar = await _leaveService.GetLeaveCalendarAsync(year, month, ct);
        return Ok(calendar);
    }
}

public class RejectLeaveRequest
{
    public string Reason { get; set; } = default!;
    public string? RejectedBy { get; set; }
}
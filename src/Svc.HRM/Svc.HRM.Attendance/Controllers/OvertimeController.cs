using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Helpers;

namespace Svc.HRM.Attendance.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/overtime")]
public class OvertimeController : ControllerBase
{
    private readonly IOvertimeService _overtimeService;
    private readonly ILogger<OvertimeController> _logger;

    public OvertimeController(IOvertimeService overtimeService, ILogger<OvertimeController> logger)
    {
        _overtimeService = overtimeService;
        _logger = logger;
    }

    /// <summary>
    /// Get all overtime requests (with optional filtering)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status = null, CancellationToken ct = default)
    {
        if (status?.ToLower() == "pending")
        {
            var pending = await _overtimeService.GetPendingOvertimeRequestsAsync(ct);
            return Ok(pending);
        }

        // Return all pending as default
        var allPending = await _overtimeService.GetPendingOvertimeRequestsAsync(ct);
        return Ok(allPending);
    }

    /// <summary>
    /// Get overtime requests by employee ID
    /// </summary>
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, CancellationToken ct)
    {
        var requests = await _overtimeService.GetEmployeeOvertimeRequestsAsync(employeeId, ct);
        return Ok(requests);
    }

    /// <summary>
    /// Get overtime request by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var request = await _overtimeService.GetOvertimeRequestAsync(id, ct);
        return Ok(request);
    }

    /// <summary>
    /// Create an overtime request
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OvertimeRequestDto dto, CancellationToken ct)
    {
        var result = await _overtimeService.CreateOvertimeRequestAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an overtime request
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] OvertimeRequestDto dto, CancellationToken ct)
    {
        var result = await _overtimeService.UpdateOvertimeRequestAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Approve an overtime request
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] OvertimeApproveDto dto, CancellationToken ct)
    {
        var result = await _overtimeService.ApproveOvertimeRequestAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Reject an overtime request
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectOvertimeRequest request, CancellationToken ct)
    {
        var result = await _overtimeService.RejectOvertimeRequestAsync(id, request.Reason, request.RejectedBy, ct);
        return Ok(result);
    }

    /// <summary>
    /// Delete an overtime request
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _overtimeService.DeleteOvertimeRequestAsync(id, ct);
        return NoContent();
    }
}

public class RejectOvertimeRequest
{
    public string Reason { get; set; } = default!;
    public string? RejectedBy { get; set; }
}
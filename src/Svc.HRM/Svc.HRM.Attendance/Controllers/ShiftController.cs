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
[Route("api/v{version:apiVersion}/shifts")]
public class ShiftController : ControllerBase
{
    private readonly IShiftService _shiftService;
    private readonly ILogger<ShiftController> _logger;

    public ShiftController(IShiftService shiftService, ILogger<ShiftController> logger)
    {
        _shiftService = shiftService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var shifts = await _shiftService.GetAllShiftsAsync(ct);
        return Ok(shifts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var shift = await _shiftService.GetShiftAsync(id, ct);
        return Ok(shift);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ShiftCreateDto dto, CancellationToken ct)
    {
        var result = await _shiftService.CreateShiftAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ShiftCreateDto dto, CancellationToken ct)
    {
        var result = await _shiftService.UpdateShiftAsync(id, dto, ct);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _shiftService.DeleteShiftAsync(id, ct);
        return NoContent();
    }

    // Shift Assignments
    [HttpGet("assignments")]
    public async Task<IActionResult> GetAssignments(CancellationToken ct)
    {
        var assignments = await _shiftService.GetActiveShiftAssignmentsAsync(ct);
        return Ok(assignments);
    }

    [HttpGet("assignments/employee/{employeeId}")]
    public async Task<IActionResult> GetEmployeeAssignments(Guid employeeId, CancellationToken ct)
    {
        var assignments = await _shiftService.GetEmployeeShiftAssignmentsAsync(employeeId, ct);
        return Ok(assignments);
    }

    [HttpPost("assignments")]
    public async Task<IActionResult> AssignShift([FromBody] ShiftAssignmentCreateDto dto, CancellationToken ct)
    {
        var result = await _shiftService.AssignShiftAsync(dto, ct);
        return Ok(result);
    }

    [HttpPut("assignments/{id}")]
    public async Task<IActionResult> UpdateAssignment(Guid id, [FromBody] ShiftAssignmentCreateDto dto, CancellationToken ct)
    {
        var result = await _shiftService.UpdateShiftAssignmentAsync(id, dto, ct);
        return Ok(result);
    }

    [HttpDelete("assignments/{id}")]
    public async Task<IActionResult> UnassignShift(Guid id, CancellationToken ct)
    {
        await _shiftService.UnassignShiftAsync(id, ct);
        return NoContent();
    }

    [HttpGet("employee/{employeeId}/current")]
    public async Task<IActionResult> GetCurrentShift(Guid employeeId, CancellationToken ct)
    {
        var shift = await _shiftService.GetEmployeeCurrentShiftAsync(employeeId, ct);
        return Ok(shift);
    }
}
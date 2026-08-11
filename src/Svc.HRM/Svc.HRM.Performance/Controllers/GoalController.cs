using Asp.Versioning;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Performance.Models.DTOs;
using Svc.HRM.Performance.Models.Entities;
using Svc.HRM.Performance.Services;

namespace Svc.HRM.Performance.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/Goal")]
public class GoalController : ControllerBase
{
    private readonly IPerformanceService _service;

    public GoalController(IPerformanceService service)
    {
        _service = service;
    }

    [PerAuth("hr.emp.performance.view")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _service.GetGoalsAsync(ct);
        return Ok(ApiResponse<List<Goal>>.Ok(items, "Goals retrieved successfully."));
    }

    [PerAuth("hr.emp.performance.view")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await _service.GetGoalAsync(id, ct);
        if (item is null)
            return NotFound(ApiResponse<Goal>.Error("Goal not found.", statusCode: 404));
        return Ok(ApiResponse<Goal>.Ok(item, "Goal retrieved successfully."));
    }

    [PerAuth("hr.emp.performance.view")]
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, CancellationToken ct)
    {
        var items = await _service.GetGoalsByEmployeeAsync(employeeId, ct);
        return Ok(ApiResponse<List<Goal>>.Ok(items, "Goals retrieved successfully."));
    }

    [PerAuth("hr.emp.performance.add")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GoalCreateDto dto, CancellationToken ct)
    {
        var item = await _service.CreateGoalAsync(dto, ct);
        return Ok(ApiResponse<Goal>.Ok(item, "Goal created successfully."));
    }

    [PerAuth("hr.emp.performance.mod")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] GoalCreateDto dto, CancellationToken ct)
    {
        var item = await _service.UpdateGoalAsync(id, dto, ct);
        if (item is null)
            return NotFound(ApiResponse<Goal>.Error("Goal not found.", statusCode: 404));
        return Ok(ApiResponse<Goal>.Ok(item, "Goal updated successfully."));
    }

    [PerAuth("hr.emp.performance.del")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteGoalAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<bool>.Error("Goal not found.", statusCode: 404));
        return Ok(ApiResponse<bool>.Ok(true, "Goal deleted successfully."));
    }
}

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Performance.Models.DTOs;
using Svc.HRM.Performance.Services;

namespace Svc.HRM.Performance.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/goals")]
public class GoalController(IPerformanceService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? employeeId, CancellationToken ct) =>
        Ok(await svc.GetGoalsAsync(employeeId, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await svc.GetGoalAsync(id, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GoalCreateDto dto, CancellationToken ct)
    {
        var item = await svc.CreateGoalAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] GoalUpdateDto dto, CancellationToken ct) =>
        Ok(await svc.UpdateGoalAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await svc.DeleteGoalAsync(id, ct);
        return NoContent();
    }
}

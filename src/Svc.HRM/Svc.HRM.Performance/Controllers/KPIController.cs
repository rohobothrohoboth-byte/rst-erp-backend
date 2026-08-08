using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Performance.Models.DTOs;
using Svc.HRM.Performance.Services;

namespace Svc.HRM.Performance.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/kpis")]
public class KPIController(IPerformanceService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? employeeId, CancellationToken ct) =>
        Ok(await svc.GetKPIsAsync(employeeId, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await svc.GetKPIAsync(id, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] KPICreateDto dto, CancellationToken ct)
    {
        var item = await svc.CreateKPIAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] KPIUpdateDto dto, CancellationToken ct) =>
        Ok(await svc.UpdateKPIAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await svc.DeleteKPIAsync(id, ct);
        return NoContent();
    }
}

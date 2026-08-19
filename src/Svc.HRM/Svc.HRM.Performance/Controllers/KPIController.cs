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
[Route("api/v{version:apiVersion}/KPI")]
public class KPIController : ControllerBase
{
    private readonly IPerformanceService _service;

    public KPIController(IPerformanceService service)
    {
        _service = service;
    }

    [PerAuth("hr.emp.performance.view")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _service.GetKpisAsync(ct);
        return Ok(ApiResponse<List<Kpi>>.Ok(items, "KPIs retrieved successfully."));
    }

    [PerAuth("hr.emp.performance.view")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await _service.GetKpiAsync(id, ct);
        if (item is null)
            return NotFound(ApiResponse<Kpi>.Error("KPI not found.", statusCode: 404));
        return Ok(ApiResponse<Kpi>.Ok(item, "KPI retrieved successfully."));
    }

    [PerAuth("hr.emp.performance.add")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] KpiCreateDto dto, CancellationToken ct)
    {
        var item = await _service.CreateKpiAsync(dto, ct);
        return Ok(ApiResponse<Kpi>.Ok(item, "KPI created successfully."));
    }

    [PerAuth("hr.emp.performance.mod")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] KpiCreateDto dto, CancellationToken ct)
    {
        var item = await _service.UpdateKpiAsync(id, dto, ct);
        if (item is null)
            return NotFound(ApiResponse<Kpi>.Error("KPI not found.", statusCode: 404));
        return Ok(ApiResponse<Kpi>.Ok(item, "KPI updated successfully."));
    }

    [PerAuth("hr.emp.performance.del")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteKpiAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<bool>.Error("KPI not found.", statusCode: 404));
        return Ok(ApiResponse<bool>.Ok(true, "KPI deleted successfully."));
    }
}

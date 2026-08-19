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
[Route("api/v{version:apiVersion}/Review")]
public class PerformanceReviewController : ControllerBase
{
    private readonly IPerformanceService _service;

    public PerformanceReviewController(IPerformanceService service)
    {
        _service = service;
    }

    [PerAuth("hr.emp.performance.view")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _service.GetReviewsAsync(ct);
        return Ok(ApiResponse<List<PerformanceReview>>.Ok(items, "Reviews retrieved successfully."));
    }

    [PerAuth("hr.emp.performance.view")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await _service.GetReviewAsync(id, ct);
        if (item is null)
            return NotFound(ApiResponse<PerformanceReview>.Error("Review not found.", statusCode: 404));
        return Ok(ApiResponse<PerformanceReview>.Ok(item, "Review retrieved successfully."));
    }

    [PerAuth("hr.emp.performance.view")]
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, CancellationToken ct)
    {
        var items = await _service.GetReviewsByEmployeeAsync(employeeId, ct);
        return Ok(ApiResponse<List<PerformanceReview>>.Ok(items, "Reviews retrieved successfully."));
    }

    [PerAuth("hr.emp.performance.add")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PerformanceReviewCreateDto dto, CancellationToken ct)
    {
        var item = await _service.CreateReviewAsync(dto, ct);
        return Ok(ApiResponse<PerformanceReview>.Ok(item, "Review created successfully."));
    }

    [PerAuth("hr.emp.performance.mod")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PerformanceReviewCreateDto dto, CancellationToken ct)
    {
        var item = await _service.UpdateReviewAsync(id, dto, ct);
        if (item is null)
            return NotFound(ApiResponse<PerformanceReview>.Error("Review not found.", statusCode: 404));
        return Ok(ApiResponse<PerformanceReview>.Ok(item, "Review updated successfully."));
    }

    [PerAuth("hr.emp.performance.del")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteReviewAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<bool>.Error("Review not found.", statusCode: 404));
        return Ok(ApiResponse<bool>.Ok(true, "Review deleted successfully."));
    }

    [PerAuth("hr.emp.performance.submit")]
    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
    {
        var item = await _service.ChangeReviewStatusAsync(id, "Submitted", ct);
        if (item is null)
            return NotFound(ApiResponse<PerformanceReview>.Error("Review not found.", statusCode: 404));
        return Ok(ApiResponse<PerformanceReview>.Ok(item, "Review submitted successfully."));
    }

    [PerAuth("hr.emp.performance.approve")]
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
    {
        var item = await _service.ChangeReviewStatusAsync(id, "Approved", ct);
        if (item is null)
            return NotFound(ApiResponse<PerformanceReview>.Error("Review not found.", statusCode: 404));
        return Ok(ApiResponse<PerformanceReview>.Ok(item, "Review approved successfully."));
    }
}

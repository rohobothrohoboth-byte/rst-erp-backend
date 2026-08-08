using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Performance.Models.DTOs;
using Svc.HRM.Performance.Services;

namespace Svc.HRM.Performance.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reviews")]
public class PerformanceReviewController(IPerformanceService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? employeeId, CancellationToken ct) =>
        Ok(await svc.GetReviewsAsync(employeeId, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await svc.GetReviewAsync(id, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PerformanceReviewCreateDto dto, CancellationToken ct)
    {
        var item = await svc.CreateReviewAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PerformanceReviewUpdateDto dto, CancellationToken ct) =>
        Ok(await svc.UpdateReviewAsync(id, dto, ct));

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct) =>
        Ok(await svc.SubmitReviewAsync(id, ct));

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ReviewDecisionDto dto, CancellationToken ct) =>
        Ok(await svc.ApproveReviewAsync(id, dto, ct));

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ReviewDecisionDto dto, CancellationToken ct) =>
        Ok(await svc.RejectReviewAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await svc.DeleteReviewAsync(id, ct);
        return NoContent();
    }

    [HttpGet("~/api/v{version:apiVersion}/feedback")]
    public async Task<IActionResult> GetFeedback([FromQuery] Guid? employeeId, [FromQuery] Guid? reviewId, CancellationToken ct) =>
        Ok(await svc.GetFeedbackAsync(employeeId, reviewId, ct));

    [HttpPost("~/api/v{version:apiVersion}/feedback")]
    public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateDto dto, CancellationToken ct) =>
        Ok(await svc.CreateFeedbackAsync(dto, ct));

    [HttpDelete("~/api/v{version:apiVersion}/feedback/{id:guid}")]
    public async Task<IActionResult> DeleteFeedback(Guid id, CancellationToken ct)
    {
        await svc.DeleteFeedbackAsync(id, ct);
        return NoContent();
    }

    [HttpGet("~/api/v{version:apiVersion}/templates")]
    public async Task<IActionResult> GetTemplates(CancellationToken ct) =>
        Ok(await svc.GetTemplatesAsync(ct));

    [HttpPost("~/api/v{version:apiVersion}/templates")]
    public async Task<IActionResult> CreateTemplate([FromBody] ReviewTemplateCreateDto dto, CancellationToken ct) =>
        Ok(await svc.CreateTemplateAsync(dto, ct));
}

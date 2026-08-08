using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Training.Models.DTOs;
using Svc.HRM.Training.Services;

namespace Svc.HRM.Training.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/enrollments")]
public class EnrollmentController(ITrainingService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? programId, [FromQuery] Guid? employeeId, [FromQuery] Guid? sessionId, CancellationToken ct) =>
        Ok(await svc.GetEnrollmentsAsync(programId, employeeId, sessionId, ct));

    [HttpPost]
    public async Task<IActionResult> Enroll([FromBody] TrainingEnrollmentCreateDto dto, CancellationToken ct)
    {
        try
        {
            var item = await svc.EnrollAsync(dto, ct);
            return Ok(item);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] TrainingEnrollmentStatusDto dto, CancellationToken ct)
    {
        try
        {
            return Ok(await svc.UpdateEnrollmentStatusAsync(id, dto, ct));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await svc.DeleteEnrollmentAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

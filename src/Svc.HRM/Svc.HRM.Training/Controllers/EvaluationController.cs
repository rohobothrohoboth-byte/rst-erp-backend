using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Training.Models.DTOs;
using Svc.HRM.Training.Services;

namespace Svc.HRM.Training.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/evaluations")]
public class EvaluationController(ITrainingService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? programId, [FromQuery] Guid? employeeId, CancellationToken ct) =>
        Ok(await svc.GetEvaluationsAsync(programId, employeeId, ct));

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] TrainingEvaluationCreateDto dto, CancellationToken ct)
    {
        try
        {
            return Ok(await svc.SubmitEvaluationAsync(dto, ct));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

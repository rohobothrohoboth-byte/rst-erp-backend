using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Training.Models.DTOs;
using Svc.HRM.Training.Services;

namespace Svc.HRM.Training.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sessions")]
public class SessionController(ITrainingService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? courseId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct) =>
        Ok(await svc.GetSessionsAsync(courseId, from, to, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await svc.GetSessionAsync(id, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TrainingSessionCreateDto dto, CancellationToken ct)
    {
        try
        {
            var item = await svc.CreateSessionAsync(dto, ct);
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TrainingSessionUpdateDto dto, CancellationToken ct)
    {
        try
        {
            return Ok(await svc.UpdateSessionAsync(id, dto, ct));
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await svc.DeleteSessionAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

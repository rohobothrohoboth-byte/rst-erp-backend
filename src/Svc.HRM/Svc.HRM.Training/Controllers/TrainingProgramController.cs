using Asp.Versioning;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Training.Models.DTOs;
using Svc.HRM.Training.Models.Entities;
using Svc.HRM.Training.Services;

namespace Svc.HRM.Training.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/Program")]
public class TrainingProgramController : ControllerBase
{
    private readonly ITrainingService _service;

    public TrainingProgramController(ITrainingService service)
    {
        _service = service;
    }

    [PerAuth("hr.training.list.view")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var programs = await _service.GetProgramsAsync(ct);
        return Ok(ApiResponse<List<TrainingProgram>>.Ok(programs, "Training programs retrieved."));
    }

    [PerAuth("hr.training.list.view")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var program = await _service.GetProgramAsync(id, ct);
        if (program is null)
            return NotFound(ApiResponse<TrainingProgram>.Error("Training program not found.", statusCode: 404));

        return Ok(ApiResponse<TrainingProgram>.Ok(program, "Training program retrieved."));
    }

    [PerAuth("hr.training.list.add")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TrainingProgramCreateDto dto, CancellationToken ct)
    {
        var program = await _service.CreateProgramAsync(dto, ct);
        return Ok(ApiResponse<TrainingProgram>.Ok(program, "Training program created."));
    }

    [PerAuth("hr.training.list.mod")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TrainingProgramCreateDto dto, CancellationToken ct)
    {
        var program = await _service.UpdateProgramAsync(id, dto, ct);
        if (program is null)
            return NotFound(ApiResponse<TrainingProgram>.Error("Training program not found.", statusCode: 404));

        return Ok(ApiResponse<TrainingProgram>.Ok(program, "Training program updated."));
    }

    [PerAuth("hr.training.list.del")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteProgramAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<bool>.Error("Training program not found.", statusCode: 404));

        return Ok(ApiResponse<bool>.Ok(true, "Training program deleted."));
    }
}

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
[Route("api/v{version:apiVersion}/Course")]
public class TrainingCourseController : ControllerBase
{
    private readonly ITrainingService _service;

    public TrainingCourseController(ITrainingService service)
    {
        _service = service;
    }

    [PerAuth("hr.training.list.view")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var courses = await _service.GetCoursesAsync(ct);
        return Ok(ApiResponse<List<TrainingCourse>>.Ok(courses, "Training courses retrieved."));
    }

    [PerAuth("hr.training.list.view")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var course = await _service.GetCourseAsync(id, ct);
        if (course is null)
            return NotFound(ApiResponse<TrainingCourse>.Error("Training course not found.", statusCode: 404));

        return Ok(ApiResponse<TrainingCourse>.Ok(course, "Training course retrieved."));
    }

    [PerAuth("hr.training.list.view")]
    [HttpGet("program/{programId}")]
    public async Task<IActionResult> GetByProgram(Guid programId, CancellationToken ct)
    {
        var courses = await _service.GetCoursesByProgramAsync(programId, ct);
        return Ok(ApiResponse<List<TrainingCourse>>.Ok(courses, "Training courses retrieved."));
    }

    [PerAuth("hr.training.list.add")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TrainingCourseCreateDto dto, CancellationToken ct)
    {
        var course = await _service.CreateCourseAsync(dto, ct);
        return Ok(ApiResponse<TrainingCourse>.Ok(course, "Training course created."));
    }

    [PerAuth("hr.training.list.mod")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TrainingCourseCreateDto dto, CancellationToken ct)
    {
        var course = await _service.UpdateCourseAsync(id, dto, ct);
        if (course is null)
            return NotFound(ApiResponse<TrainingCourse>.Error("Training course not found.", statusCode: 404));

        return Ok(ApiResponse<TrainingCourse>.Ok(course, "Training course updated."));
    }

    [PerAuth("hr.training.list.del")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteCourseAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<bool>.Error("Training course not found.", statusCode: 404));

        return Ok(ApiResponse<bool>.Ok(true, "Training course deleted."));
    }
}

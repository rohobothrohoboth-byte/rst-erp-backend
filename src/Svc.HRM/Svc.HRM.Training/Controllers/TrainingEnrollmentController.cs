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
[Route("api/v{version:apiVersion}/Enrollment")]
public class TrainingEnrollmentController : ControllerBase
{
    private readonly ITrainingService _service;

    public TrainingEnrollmentController(ITrainingService service)
    {
        _service = service;
    }

    [PerAuth("hr.training.list.view")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var enrollments = await _service.GetEnrollmentsAsync(ct);
        return Ok(ApiResponse<List<TrainingEnrollment>>.Ok(enrollments, "Training enrollments retrieved."));
    }

    [PerAuth("hr.training.list.view")]
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, CancellationToken ct)
    {
        var enrollments = await _service.GetEnrollmentsByEmployeeAsync(employeeId, ct);
        return Ok(ApiResponse<List<TrainingEnrollment>>.Ok(enrollments, "Training enrollments retrieved."));
    }

    [PerAuth("hr.training.list.enroll")]
    [HttpPost]
    public async Task<IActionResult> Enroll([FromBody] TrainingEnrollmentCreateDto dto, CancellationToken ct)
    {
        var enrollment = await _service.CreateEnrollmentAsync(dto, ct);
        return Ok(ApiResponse<TrainingEnrollment>.Ok(enrollment, "Employee enrolled."));
    }

    [PerAuth("hr.training.list.mod")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TrainingEnrollmentCreateDto dto, CancellationToken ct)
    {
        var enrollment = await _service.UpdateEnrollmentAsync(id, dto, ct);
        if (enrollment is null)
            return NotFound(ApiResponse<TrainingEnrollment>.Error("Training enrollment not found.", statusCode: 404));

        return Ok(ApiResponse<TrainingEnrollment>.Ok(enrollment, "Training enrollment updated."));
    }

    [PerAuth("hr.training.list.cancel")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteEnrollmentAsync(id, ct);
        if (!deleted)
            return NotFound(ApiResponse<bool>.Error("Training enrollment not found.", statusCode: 404));

        return Ok(ApiResponse<bool>.Ok(true, "Training enrollment cancelled."));
    }

    [PerAuth("hr.training.certificate.issue")]
    [HttpPost("{id}/certificate")]
    public async Task<IActionResult> IssueCertificate(Guid id, CancellationToken ct)
    {
        var enrollment = await _service.IssueCertificateAsync(id, ct);
        if (enrollment is null)
            return NotFound(ApiResponse<TrainingEnrollment>.Error("Training enrollment not found.", statusCode: 404));

        return Ok(ApiResponse<TrainingEnrollment>.Ok(enrollment, "Certificate issued."));
    }
}

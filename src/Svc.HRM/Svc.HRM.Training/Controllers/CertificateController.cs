using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Training.Models.DTOs;
using Svc.HRM.Training.Services;

namespace Svc.HRM.Training.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/certificates")]
public class CertificateController(ITrainingService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? employeeId, [FromQuery] Guid? programId, CancellationToken ct) =>
        Ok(await svc.GetCertificatesAsync(employeeId, programId, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var item = await svc.GetCertificateAsync(id, ct);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost("issue")]
    public async Task<IActionResult> Issue([FromBody] TrainingCertificateIssueDto dto, CancellationToken ct)
    {
        try
        {
            return Ok(await svc.IssueCertificateAsync(dto, ct));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("verify/{certificateNumber}")]
    public async Task<IActionResult> Verify(string certificateNumber, CancellationToken ct)
    {
        var item = await svc.VerifyCertificateAsync(certificateNumber, ct);
        return item == null ? NotFound() : Ok(item);
    }
}

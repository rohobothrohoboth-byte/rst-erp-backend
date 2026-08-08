using Asp.Versioning;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.HealthChecks;
using Shared.Helpers.Services;
namespace Svc.Auth.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/auth/v{version:apiVersion}/Alerts")]
[ApiVersion("1.0")]
public class AlertController : ControllerBase
{
    [HttpGet("sync-status")]
    public IActionResult GetSyncStatus()
    {
        var status = SyncHealthCheck.GetStatus();
        return Ok(ApiResponse<object>.Ok(status));
    }

    [HttpPost("test")]
    public async Task<IActionResult> TestAlert([FromServices] IAlertService alert)
    {
        await alert.SendSuccessAsync("Test Alert", "This is a test alert from the system");
        return Ok(ApiResponse<object>.Ok(null, "Test alert sent"));
    }
}
using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Queries;
using Svc.Auth.Services;
using Shared.Helpers.Services;
namespace Svc.Auth.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/auth/v{version:apiVersion}/Dashboard")]
[ApiVersion("1.0")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _med;
    private readonly ISetupService _setupService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IMediator med, ISetupService setupService, ILogger<DashboardController> logger)
    {
        _med = med;
        _setupService = setupService;
        _logger = logger;
    }

    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            _logger.LogInformation("Dashboard GetStats called");
            var stats = await _med.Send(new GetDashboardStatsQry());
            _logger.LogInformation("Dashboard GetStats completed successfully");
            return Ok(ApiResponse<object>.Ok(stats, "Dashboard stats retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dashboard GetStats FAILED");
            throw;
        }
    }

    [HttpGet("sync-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSyncStatus()
    {
        _logger.LogInformation("Dashboard GetSyncStatus called");
        var status = await _med.Send(new GetSyncStatusQry());
        return Ok(ApiResponse<object>.Ok(status, "Sync status retrieved successfully."));
    }

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHealth()
    {
        _logger.LogInformation("Dashboard GetHealth called");
        var health = await _med.Send(new GetSystemHealthQry());
        return Ok(ApiResponse<object>.Ok(health, "System health retrieved successfully."));
    }

    [HttpPost("sync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> TriggerSync()
    {
        _logger.LogInformation("Dashboard TriggerSync called");
        await _setupService.TriggerSyncAsync();
        return Ok(ApiResponse<object>.Ok(null, "Sync triggered successfully."));
    }

    [HttpDelete("cache")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearCache([FromServices] ICacheService cache)
    {
        _logger.LogInformation("Dashboard ClearCache called");
        await cache.RemoveByPatternAsync("Auth_*");
        return Ok(ApiResponse<object>.Ok(null, "Cache cleared successfully."));
    }
}
using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Commands.Setup;
using Svc.Auth.Queries.Setup;
using Svc.Auth.Services;

namespace Svc.Auth.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/auth/v{version:apiVersion}/Setup")]
[ApiVersion("1.0")]
public class SetupController : ControllerBase
{
    private readonly IMediator _med;
    private readonly ISetupService _setupService;

    public SetupController(IMediator med, ISetupService setupService)
    {
        _med = med;
        _setupService = setupService;
    }

    /// <summary>
    /// Check if system needs setup
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSetupStatus()
    {
        var status = await _setupService.GetSetupStatusAsync();
        return Ok(ApiResponse<object>.Ok(status));
    }

    /// <summary>
    /// Get required data for setup (modules, roles, etc.)
    /// </summary>
    [HttpGet("data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSetupData()
    {
        var response = await _med.Send(new GetSetupDataQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Complete system setup
    /// </summary>
    [HttpPost("complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteSetup([FromBody] CompleteSetupCmd request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var response = await _med.Send(request);
        return Ok(ApiResponse<object>.Ok(response, "System setup completed successfully!"));
    }

    /// <summary>
    /// Trigger manual sync from all services
    /// </summary>
    [HttpPost("sync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> TriggerSync()
    {
        await _setupService.TriggerSyncAsync();
        return Ok(ApiResponse<object>.Ok(null, "Sync triggered successfully."));
    }
}
// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Controllers\ExternalSystemController.cs
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Cor.HRMM.Commands;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Cor.HRMM.Services;
using Shared.Helpers.ExternalAccess;
using Cor.HRMM.Models.Entities;
namespace Cor.HRMM.Controllers;


[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/external")]
[ApiVersion("1.0")]
[Authorize(Roles = "admin")]
public class ExternalSystemController : ControllerBase
{
    private readonly IExternalSystemService _externalSystemService;
    private readonly ILogger<ExternalSystemController> _logger;

    public ExternalSystemController(
        IExternalSystemService externalSystemService,
        ILogger<ExternalSystemController> logger)
    {
        _externalSystemService = externalSystemService;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> RegisterSystem([FromBody] RegisterSystemRequest request)
    {
        var system = await _externalSystemService.RegisterSystemAsync(
            request.SystemName,
            request.AllowedEndpoints);

        return Ok(ApiResponse<ExternalSystem>.Ok(system, "External system registered successfully"));
    }

    [HttpPost("revoke/{apiKey}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeAccess(string apiKey)
    {
        await _externalSystemService.RevokeAccessAsync(apiKey);
        return Ok(ApiResponse<string>.Ok(null!, "API Key revoked successfully"));
    }
}

public class RegisterSystemRequest
{
    public string SystemName { get; set; } = string.Empty;
    public string[]? AllowedEndpoints { get; set; }
}

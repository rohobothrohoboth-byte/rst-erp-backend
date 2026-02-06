using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Queries;

namespace Svc.Auth.Controllers;

/// <summary>
/// AUTHORIZATION Permissions access end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/auth/v{version:apiVersion}/Permission")]
[ApiVersion("1.0")]
public class PermissionController(IMediator med) : ControllerBase
{
    [HttpGet("AllRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllRole()
    {
        var response = await med.Send(new RoleAllQry());
        return Ok(response);
    }

    [HttpGet("GetRole/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRole(string id)
    {
        var response = await med.Send(new RoleByIdQry { Id = id });
        if (response == null) { throw new DomainException($"ROLE with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("AllModule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllModule()
    {
        var response = await med.Send(new ModuleAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetModule/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetModule(Guid id)
    {
        var response = await med.Send(new ModuleByIdQry { Id = id });
        if (response == null) { throw new DomainException($"MODULE with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get List of Menu Permission by providing User Id
    /// </summary>
    [HttpGet("GetPerMenuByUser/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerMenuByUser(string id)
    {
        var response = await med.Send(new PerMenuByUserIdQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get List of Access Permission by providing User Id
    /// </summary>
    [HttpGet("GetPerApiByUser/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerApiByUser(string id)
    {
        var response = await med.Send(new PerApiByUserIdQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get List of Menu Permission by providing Module Id
    /// </summary>
    [HttpGet("GetPerMenuByMod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerMenuByMod(Guid id)
    {
        var response = await med.Send(new PerMenuByModIdQry { Id = id });
        if (response == null) { throw new DomainException($"MENU PERMISSION with Module id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get List of Access Permission by providing Menu Id
    /// </summary>
    [HttpGet("GetPerApiByMenu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerApiByMenu(Guid id)
    {
        var response = await med.Send(new PerApiByMenuIdQry { Id = id });
        if (response == null) { throw new DomainException($"ACCESS PERMISSION with Menu id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }




}
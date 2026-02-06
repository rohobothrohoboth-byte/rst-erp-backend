using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Queries;

namespace Svc.Auth.Controllers;

/// <summary>
/// End point to get the list of Names and Ids of Svc.Auth entities
/// </summary>

//[Authorize]
[ApiController]
[Route("api/auth/v{version:apiVersion}/Names")]
[ApiVersion("1.0")]
public class NameListController(IMediator med) : ControllerBase
{
    [HttpGet("AllModuleName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllModuleName()
    {
        var res = await med.Send(new ModuleNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetModuleName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetModuleName(Guid id)
    {
        var res = await med.Send(new ModuleNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"MODULE with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    [HttpGet("AllPerMenuName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPerMenuName()
    {
        var res = await med.Send(new PerMenuNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetPerMenuName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerMenuName(Guid id)
    {
        var res = await med.Send(new PerMenuNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"MENU PERMISSION with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    [HttpGet("AllPerApiName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPerApiName()
    {
        var res = await med.Send(new PerApiNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetPerApiName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerApiName(Guid id)
    {
        var res = await med.Send(new PerApiNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"ACCESS PERMISSION with id [{id}] NOT FOUND."); }
        return Ok(res);
    }



}
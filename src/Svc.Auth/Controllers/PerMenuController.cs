using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Commands;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Queries;

namespace Svc.Auth.Controllers;

/// <summary>
/// Menu Permission management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/auth/v{version:apiVersion}/PerMenu")]
[ApiVersion("1.0")]
public class PerMenuController(IMediator med) : ControllerBase
{
    [HttpGet("AllPerMenu")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPerMenu()
    {
        var response = await med.Send(new PerMenuAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetPerMenu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerMenu(Guid id)
    {
        var response = await med.Send(new PerMenuByIdQry { Id = id });
        if (response == null) { throw new DomainException($"MENU PERMISSION with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddPerMenu")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PerMenuAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PerMenuAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New MENU PERMISSION successfully created."));
    }

    [HttpPut("ModPerMenu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PerMenuModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PerMenuModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected MENU PERMISSION successfully updated."));
    }

    [HttpDelete("DelPerMenu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new PerMenuDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"MENU PERMISSION with Id {id} successfully deleted."));
    }
}
using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Commands;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Queries;

namespace Svc.Auth.Controllers;

/// <summary>
/// API Permission management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/auth/v{version:apiVersion}/PerApi")]
[ApiVersion("1.0")]
public class PerApiController(IMediator med) : ControllerBase
{
    [HttpGet("AllPerApi")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPerApi()
    {
        var response = await med.Send(new PerApiAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetPerApi/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerApi(Guid id)
    {
        var response = await med.Send(new PerApiByIdQry { Id = id });
        if (response == null) { throw new DomainException($"ACCESS PERMISSION with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddPerApi")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PerApiAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PerApiAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New ACCESS PERMISSION successfully created."));
    }

    [HttpPut("ModPerApi/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PerApiModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PerApiModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected ACCESS PERMISSION successfully updated."));
    }

    [HttpDelete("DelPerApi/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new PerApiDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"ACCESS PERMISSION with Id {id} successfully deleted."));
    }
}
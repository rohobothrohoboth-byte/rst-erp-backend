using Asp.Versioning;
using Cor.Module.Commands;
using Cor.Module.Helpers;
using Cor.Module.Models.DTOs;
using Cor.Module.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Module.Controllers;

/// <summary>
/// Period management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/module/v{version:apiVersion}/Period")]
[ApiVersion("1.0")]
public class PeriodController(IMediator med) : ControllerBase
{
    [HttpGet("AllPeriod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPeriod()
    {
        var response = await med.Send(new AllPeriodQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetPeriod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPeriod(Guid id)
    {
        var response = await med.Send(new PeriodByIdQry { Id = id });
        if (response == null) { throw new DomainException($"PERIOD with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }
    
    [HttpPost("AddPeriod")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddPeriodDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new AddPeriodCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New PERIOD successfully created."));
    }

    [HttpPut("ModPeriod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditPeriodDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new ModPeriodCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected PERIOD successfully updated."));

    }

    [HttpDelete("DelPeriod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DelPeriodCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"PERIOD with Id {id} successfully deleted."));
    }
}
using Asp.Versioning;
using Cor.Module.Commands;
using Cor.Module.Middlewares;
using Cor.Module.Models.DTOs;
using Cor.Module.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Module.Controllers;

/// <summary>
/// Holiday management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/module/v{version:apiVersion}/Holiday")]
[ApiVersion("1.0")]
public class HolidayController(IMediator med) : ControllerBase
{
    [HttpGet("AllHoliday")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllHoliday()
    {
        var response = await med.Send(new AllHolidayQry());
        return Ok(response);
    }

    [HttpGet("GetHoliday/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHoliday(Guid id)
    {
        var response = await med.Send(new HolidayByIdQry { Id = id });
        if (response == null)
        {
            throw new EntityNotFoundException("Holiday", id);
        }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddHoliday")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddHolidayDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new AddHolidayCmd { AddHolidayDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Holiday created successfully"));
    }

    [HttpPut("ModHoliday/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditHolidayDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new ModHolidayCmd { EditHolidayDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Holiday updated successfully"));
    }

    [HttpDelete("DelHoliday/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DelHolidayCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"Holiday with Id {id} deleted successfully"));
    }
}
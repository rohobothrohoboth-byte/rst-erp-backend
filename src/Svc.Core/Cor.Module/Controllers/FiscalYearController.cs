using Asp.Versioning;
using Cor.Module.Commands;
using Cor.Module.Helpers;
using Cor.Module.Models.DTOs;
using Cor.Module.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Module.Controllers;

/// <summary>
/// FiscalYear management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/module/v{version:apiVersion}/FiscalYear")]
[ApiVersion("1.0")]
public class FiscalYearController(IMediator med) : ControllerBase
{
    [HttpGet("AllFiscalYear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllFiscalYear()
    {
        var response = await med.Send(new AllFiscalYearsQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetFiscalYear/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFiscalYear(Guid id)
    {
        var response = await med.Send(new FiscalYearByIdQry { Id = id });
        if (response == null) { throw new DomainException($"FISCAL YEAR  with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddFiscalYear")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddFiscYearDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new AddFiscalYearCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New FISCAL YEAR successfully created."));
    }

    [HttpPut("ModFiscalYear/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditFiscYearDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new ModFiscalYearCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected FISCAL YEAR successfully updated."));
    }

    [HttpDelete("DelFiscalYear/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DelFiscalYearCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"FISCAL YEAR with Id {id} successfully deleted."));
    }
}

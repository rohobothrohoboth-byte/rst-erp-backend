using Asp.Versioning;
using Cor.Module.Commands;
using Cor.Module.Helpers;
using Cor.Module.Models.DTOs;
using Cor.Module.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Module.Controllers;

/// <summary>
/// Company management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/module/v{version:apiVersion}/Company")]
[ApiVersion("1.0")]
public class CompanyController(IMediator med) : ControllerBase
{
    [HttpGet("AllCompany")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllCompany()
    {
        var response = await med.Send(new AllCompsQry());
        //return Ok(response);
        return Ok(ApiResponse<object>.Ok(response));
    }
    
    [HttpGet("GetCompany/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var response = await med.Send(new CompByIdQry { Id = id });
        if (response == null) { throw new DomainException($"COMPANY with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }
    
    [HttpPost("AddCompany")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddCompDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new AddCompCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New COMPANY successfully created."));
    }

    [HttpPut("ModCompany/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditCompDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new ModCompCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected COMPANY successfully updated."));
    }

    [HttpDelete("DelCompany/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DelCompCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"COMPANY with Id {id} successfully deleted."));
    }
}
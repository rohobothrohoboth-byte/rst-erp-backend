using Asp.Versioning;
using Cor.Module.Commands;
using Cor.Module.Helpers;
using Cor.Module.Models.DTOs;
using Cor.Module.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Module.Controllers;

/// <summary>
/// Department management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/module/v{version:apiVersion}/Department")]
[ApiVersion("1.0")]
public class DepartmentController(IMediator med) : ControllerBase
{
    [HttpGet("AllDept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllDept()
    {
        var response = await med.Send(new AllDeptsQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetDept/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDept(Guid id)
    {
        var response = await med.Send(new DeptByIdQry { Id = id });
        if (response == null) { throw new DomainException($"DEPARTMENT with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddDept")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddDeptDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new AddDeptCmd { AddDeptDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New DEPARTMENT successfully created."));
    }
    
    [HttpPut("ModDept/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EdtDeptDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new ModDeptCmd { EdtDeptDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected DEPARTMENT successfully updated."));
    }

    [HttpDelete("DelDept/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DelDeptCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"DEPARTMENT with Id {id} successfully deleted."));
    }
}
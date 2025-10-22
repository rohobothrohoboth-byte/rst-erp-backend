using System.Data;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// Employees Management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/Employee")]
[ApiVersion("1.0")]
public class EmployeeController(IMediator med) : ControllerBase
{
    [HttpGet("AllEmployee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllEmployee()
    {
        var response = await med.Send(new EmployeeAllQry());
        return Ok(response);
    }

    [HttpGet("GetEmployee/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployee(Guid id)
    {
        var response = await med.Send(new EmployeeByIdQry { Id = id });
        if (response == null) { return NotFound(new { Error = $"Employee with Id {id} not found" }); }
        return Ok(response);
    }

    [HttpPost("AddEmployee")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EmployeeAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new EmployeeAddCmd { AddDto = addDto };
            var res = await med.Send(command);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Employee", Details = ex.Message });
        }
    }

    [HttpPut("ModEmployee/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modComp = await med.Send(new EmployeeModCmd { ModDto = modDto });
            return Ok(modComp);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the Employee was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Employee with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update Employee", Details = ex.Message });
        }
    }

    [HttpDelete("DelEmployee/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new EmployeeDelCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Employee with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete Employee", Details = ex.Message });
        }
    }
}
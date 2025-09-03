using Asp.Versioning;
using Cor.App.Commands.Dept;
using Cor.App.Queries;
using Cor.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Cor.API.Controllers;

//[Authorize]
[ApiController]
[Route("api/core/v{version:apiVersion}/department")]
[ApiVersion("1.0")]
public class DepartmentController(IMediator med) : ControllerBase
{
    [HttpPost("AddDept")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddDeptDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new AddDeptCmd { AddDeptDto = addDto };
            var deptId = await med.Send(command);
            return CreatedAtAction(nameof(GetDept), new { id = deptId }, deptId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create DEPARTMENT", Details = ex.Message });
        }
    }

    [HttpGet("AllDept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllDepts()
    {
        var depts = await med.Send(new AllDeptsQry());
        return Ok(depts);
    }

    [HttpGet("GetDept/id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDept(Guid id)
    {
        var dept = await med.Send(new DeptByIdQry { Id = id });
        if (dept == null)
        {
            return NotFound(new { Error = $"DEPARTMENT with Id {id} not found" });
        }
        return Ok(dept);
    }

    [HttpPut("ModDept/id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EdtDeptDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modDept = await med.Send(new ModDeptCmd { EdtDeptDto = modDto });
            return Ok(modDept);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the DEPARTMENT was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"DEPARTMENT with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update DEPARTMENT", Details = ex.Message });
        }
    }

    [HttpDelete("DelDept/id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new DelDeptCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"DEPARTMENT with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete DEPARTMENT", Details = ex.Message });
        }
    }
}

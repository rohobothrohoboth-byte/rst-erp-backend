using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// EMPLOYEE'S EXPERIENCE management end points
/// </summary>

[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpExp")]
[ApiVersion("1.0")]
public class EmpExpController(IMediator med) : ControllerBase
{
    [HttpGet("AllEmpExp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllEmpExp()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new EmpExpAllQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetEmpExp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpExp(Guid id)
    {
        var response = await med.Send(new EmpExpByIdQry { Id = id });
        if (response == null) { throw new DomainException("EMPLOYEE'S EXPERIENCE with given parameter NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddEmpExp")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EmpExpAddDto addDto)
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        addDto.EmpId = id;
        var command = new EmpExpAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New EMPLOYEE'S EXPERIENCE successfully created."));
    }

    [HttpPut("ModEmpExp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmpExpModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EmpExpModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S EXPERIENCE successfully updated."));
    }

    [HttpPut("ReviewExp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReviewExp(Guid id, [FromBody] EmpRevDto dto)
    {
        if (!ModelState.IsValid || dto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }
        await med.Send(new EmpExpRvwCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(null!, "Selected EMPLOYEE'S EXPERIANCE successfully REVIEWED."));
    }

    [HttpPut("ReviewExpAll/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReviewExpAll(Guid id, [FromBody] EmpRevDto dto)
    {
        if (!ModelState.IsValid || dto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }
        await med.Send(new EmpExpRvwAllCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(null!, "Selected EMPLOYEE'S EXPERIANCES successfully REVIEWED."));
    }

    [HttpDelete("DelEmpExp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new EmpExpDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Selected EMPLOYEE'S EXPERIENCE successfully deleted."));
    }
}
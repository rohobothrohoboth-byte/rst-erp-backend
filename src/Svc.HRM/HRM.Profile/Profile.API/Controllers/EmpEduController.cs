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
/// EMPLOYEE'S EDUCATION management end points
/// </summary>

[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpEdu")]
[ApiVersion("1.0")]
public class EmpEduController(IMediator med) : ControllerBase
{
    [HttpGet("AllEmpEdu")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllEmpEdu()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new EmpEduAllQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetEmpEdu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpEdu(Guid id)
    {
        var response = await med.Send(new EmpEduByIdQry { Id = id });
        if (response == null) { throw new DomainException("EMPLOYEE'S EDUCATION with given parameter NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddEmpEdu")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EmpEduAddDto addDto)
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        addDto.EmpId = id;
        var command = new EmpEduAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New EMPLOYEE'S EDUCATION successfully created."));
    }

    [HttpPut("ModEmpEdu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmpEduModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EmpEduModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S EDUCATION successfully updated."));
    }

    [HttpPut("ReviewEdu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReviewEdu(Guid id, [FromBody] EmpRevDto dto)
    {
        if (!ModelState.IsValid || dto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }
        await med.Send(new EmpEduRvwCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(null!, "Selected EMPLOYEE'S EDUCATION successfully REVIEWED."));
    }

    [HttpPut("ReviewEduAll/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReviewEduAll(Guid id, [FromBody] EmpRevDto dto)
    {
        if (!ModelState.IsValid || dto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }
        await med.Send(new EmpEduRvwAllCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(null!, "Selected EMPLOYEE'S EDUCATIONS successfully REVIEWED."));
    }

    [HttpDelete("DelEmpEdu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new EmpEduDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Selected EMPLOYEE'S EDUCATION successfully deleted."));
    }
}
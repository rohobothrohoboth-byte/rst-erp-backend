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
/// My Profile Management end points
/// </summary>

[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/MyProMod")]
[ApiVersion("1.0")]
public class MyProModController(IMediator med) : ControllerBase
{
    [HttpPost("EmpBioMod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpBioMod([FromForm] EmpBioModDto modDto)
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        modDto.Id = id;
        var response = await med.Send(new EmpBioModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "EMPLOYEE'S Biographical Info successfully updated."));
    }

    [HttpPost("EmpFinanceMod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpFinanceMod([FromBody] EmpFinanceModDto modDto)
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        modDto.Id = id;
        var response = await med.Send(new EmpFinanceModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "EMPLOYEE'S Biographical Info successfully updated."));
    }

    [HttpPost("EmContactMod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmContactMod([FromBody] EmContactModDto modDto)
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        modDto.EmployeeId = id;
        var response = await med.Send(new EmContactModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "EMPLOYEE'S Emergency Contact Info successfully updated."));
    }

    [HttpPost("EmpFamilyAdd")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpFamilyAdd([FromBody] EmpFamilyAddDto addDto)
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        addDto.EmployeeId = id;
        var response = await med.Send(new EmpFamilyAddCmd { AddDto = addDto });
        return Ok(ApiResponse<object>.Ok(response, "EMPLOYEE'S Family Info successfully Added."));
    }

    [HttpPut("EmpFamilyMod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpFamilyMod(Guid id, [FromBody] EmpFamilyModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var response = await med.Send(new EmpFamilyModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "EMPLOYEE'S Family Info successfully updated."));
    }

    [HttpDelete("EmpFamilyDel/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpFamilyDel(Guid id)
    {
        await med.Send(new EmpFamilyDelCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, "Selected EMPLOYEE'S Family Info successfully deleted."));
    }
}
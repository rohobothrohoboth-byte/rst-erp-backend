using Asp.Versioning;
using Common;
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
    [PerAuth("hr.emp.profile.mod")]
    [HttpPost("MyBioMod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MyBioMod([FromForm] MyBioModDto modDto)
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        modDto.Id = id;
        var response = await med.Send(new MyBioModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "EMPLOYEE'S Biographical Info successfully updated."));
    }

    [PerAuth("hr.emp.profile.mod")]
    [HttpPost("MyFinanceMod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MyFinanceMod([FromBody] MyFinanceModDto modDto)
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        modDto.Id = id;
        var response = await med.Send(new MyFinanceModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "EMPLOYEE'S Biographical Info successfully updated."));
    }

    [PerAuth("hr.emp.profile.mod")]
    [HttpPost("MyEmContMod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MyEmContMod([FromBody] MyEmContModDto modDto)
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        modDto.EmployeeId = id;
        var response = await med.Send(new MyEmContModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "EMPLOYEE'S Emergency Contact Info successfully updated."));
    }

    [PerAuth("hr.emp.profile.mod")]
    [HttpPost("MyFamilyAdd")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MyFamilyAdd([FromBody] MyFamilyAddDto addDto)
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        addDto.EmployeeId = id;
        var response = await med.Send(new MyFamilyAddCmd { AddDto = addDto });
        return Ok(ApiResponse<object>.Ok(response, "EMPLOYEE'S Family Info successfully Added."));
    }

    [PerAuth("hr.emp.profile.mod")]
    [HttpPut("MyFamilyMod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MyFamilyMod(Guid id, [FromBody] MyFamilyModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var response = await med.Send(new MyFamilyModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "EMPLOYEE'S Family Info successfully updated."));
    }

    [PerAuth("hr.emp.profile.mod")]
    [HttpDelete("MyFamilyDel/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MyFamilyDel(Guid id)
    {
        await med.Send(new MyFamilyDelCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, "Selected EMPLOYEE'S Family Info successfully deleted."));
    }
}
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
/// Employee's Profile Management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/Profile")]
[ApiVersion("1.0")]
public class ProfileController(IMediator med) : ControllerBase
{
    [HttpGet("GetEmpPhoto")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpPhoto()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new EmpPhotoQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Photo NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetPhotoThumbnail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPhotoThumbnail()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new EmpPhotoThumbnailQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Photo Thumbnail NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProfileInfo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileInfo()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProInfoQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Profile Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProOverview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProOverview()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProOverviewQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Overview Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProBasic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProBasic()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProBasicQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Basic Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProBio")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProBio()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProBioQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Biographical Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProEmContact")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProEmContact()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProEmContactQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Emergency Contact Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProFamily")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProFamily()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProFamilyQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Families Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetEmpGuaranty")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpGuaranty()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new EmpGuarantyQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Guarantor Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

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
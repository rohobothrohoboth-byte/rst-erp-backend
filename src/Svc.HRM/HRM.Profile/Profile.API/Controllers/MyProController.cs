using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Queries;

namespace Profile.API.Controllers;

/// <summary>
/// My Profile Display end points
/// </summary>

[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/MyPro")]
[ApiVersion("1.0")]
public class MyProController(IMediator med) : ControllerBase
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
        var response = await med.Send(new MyProInfoQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Profile Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProOverview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProOverview()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new MyProOverviewQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Overview Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProBasic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProBasic()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new MyProBasicQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Basic Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProBio")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProBio()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new MyProBioQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Biographical Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProEmContact")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProEmContact()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new MyProEmContQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Emergency Contact Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProFamily")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProFamily()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new MyProFamilyQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Families Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetEmpGuaranty")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpGuaranty()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new MyEmpGuarQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Guarantor Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }
}
using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Queries;

namespace Profile.API.Controllers;

/// <summary>
/// Employees Profile Display end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpPro")]
[ApiVersion("1.0")]
public class EmpProController(IMediator med) : ControllerBase
{
    [PerAuth("hr.emp.profile.view")]
    [HttpGet("GetEmpPhoto/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpPhoto(Guid id)
    {
        var response = await med.Send(new EmpPhotoQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Photo NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.emp.profile.view")]
    [HttpGet("GetPhotoThumbnail/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPhotoThumbnail(Guid id)
    {
        var response = await med.Send(new EmpPhotoThumbnailQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Photo Thumbnail NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.emp.profile.view")]
    [HttpGet("GetProfileInfo/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileInfo(Guid id)
    {
        var response = await med.Send(new MyProInfoQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Profile Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.emp.profile.view")]
    [HttpGet("GetProOverview/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProOverview(Guid id)
    {
        var response = await med.Send(new MyProOverviewQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Overview Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.emp.profile.view")]
    [HttpGet("GetProBasic/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProBasic(Guid id)
    {
        var response = await med.Send(new MyProBasicQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Basic Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.emp.profile.view")]
    [HttpGet("GetProBio/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProBio(Guid id)
    {
        var response = await med.Send(new MyProBioQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Biographical Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.emp.profile.view")]
    [HttpGet("GetProEmContact/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProEmContact(Guid id)
    {
        var response = await med.Send(new MyProEmContQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Emergency Contact Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.emp.profile.view")]
    [HttpGet("GetProFamily/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProFamily(Guid id)
    {
        var response = await med.Send(new MyProFamilyQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Families Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.emp.profile.view")]
    [HttpGet("GetEmpGuaranty/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpGuaranty(Guid id)
    {
        var response = await med.Send(new MyEmpGuarQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Guarantor Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }
}
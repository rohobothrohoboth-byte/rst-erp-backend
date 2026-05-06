using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Queries;

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
        return response == null ? throw new DomainException("EMPLOYEE PHOTO NOT FOUND.") : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetPhotoThumbnail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPhotoThumbnail()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new EmpPhotoThumbnailQry { Id = id });
        return response == null ? throw new DomainException("EMPLOYEE'S PHOTO THUMBNAIL NOT FOUND.") : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProfileInfo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileInfo()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProfileInfoQry { Id = id });
        return response == null ? throw new DomainException("EMPLOYEE'S Profile Info NOT FOUND.") : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProfileCard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileCard()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProfileCardQry { Id = id });
        return response == null ? throw new DomainException("EMPLOYEE'S Profile Info NOT FOUND.") : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProBasic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProBasic()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProBasicQry { Id = id });
        return response == null ? throw new DomainException("EMPLOYEE'S Profile Info NOT FOUND.") : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProSalary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProSalary()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProSalaryQry { Id = id });
        return response == null ? throw new DomainException("EMPLOYEE'S Profile Info NOT FOUND.") : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProAddress")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProAddress()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProAddressQry { Id = id });
        return response == null ? throw new DomainException("EMPLOYEE'S Profile Info NOT FOUND.") : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetProBio")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProBio()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var response = await med.Send(new ProBioQry { Id = id });
        return response == null ? throw new DomainException("EMPLOYEE'S Profile Info NOT FOUND.") : Ok(ApiResponse<object>.Ok(response));
    }




}
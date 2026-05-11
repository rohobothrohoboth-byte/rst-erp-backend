using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Queries;

namespace Profile.API.Controllers;

/// <summary>
/// Employee Information Update Management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpMod")]
[ApiVersion("1.0")]
public class EmpModController(IMediator med) : ControllerBase
{
    [HttpGet("EmpModBasic/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpModBasic(Guid id)
    {
        var response = await med.Send(new EmpModBasicQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Basic Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("EmpModBio/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpModBio(Guid id)
    {
        var response = await med.Send(new EmpModBioQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Biographical Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("EmpModGuar/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpModGuar(Guid id)
    {
        var response = await med.Send(new EmpModGuarQry { Id = id });
        return response == null ? Ok(ApiResponse<object>.Fail("EMPLOYEE'S Guarantor Info NOT FOUND.", null, 404)) : Ok(ApiResponse<object>.Ok(response));
    }


}
using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Queries;

namespace Profile.API.Controllers;

/// <summary>
/// HR Employee Reports end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpListRepo")]
[ApiVersion("1.0")]
public class EmpDashBrdRepoController(IMediator med) : ControllerBase
{
    [HttpGet("EmpDbRepo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EmpDbRepo()
    {
        var response = await med.Send(new EmpDbRepQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("PendEmpList")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PendEmpList()
    {
        var response = await med.Send(new EmpDbPendQry());
        return Ok(ApiResponse<object>.Ok(response));
    }



}

using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Queries;

namespace Recruit.API.Controllers;

/// <summary>
/// JOB APPLICANTS management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/Applicant")]
[ApiVersion("1.0")]
public class ApplicantController(IMediator med) : ControllerBase
{
    [HttpPost("AllIntApp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AllIntApp()
    {
        var response = await med.Send(new JobAppAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetIntApp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIntApp(Guid id)
    {
        var response = await med.Send(new JobAppByIdQry { Id = id });
        if (response == null) { throw new DomainException($"JOB APPLICATION with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }
}
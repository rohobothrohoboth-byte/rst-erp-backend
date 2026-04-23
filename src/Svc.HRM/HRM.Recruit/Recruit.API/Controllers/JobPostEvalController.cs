using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;

namespace Recruit.API.Controllers;

/// <summary>
/// JOB POSTING EVALUATION management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/JobPostEval")]
[ApiVersion("1.0")]
public class JobPostEvalController(IMediator med) : ControllerBase
{
    [HttpGet("JpStartEval/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JpStartEval(Guid id)
    {
        await med.Send(new JobPostStartEvalCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, $"Successfully STARTED EVALUATION for this JOB POSTING."));
    }




}
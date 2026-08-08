using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.Domain.DTOs;
using System.Security.Claims;

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

    [HttpPost("JpAppEvaluate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JpAppEvaluate([FromBody] JpAppEvalDto evalDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 })
        {
            throw new DomainException("AUTHORIZATION REQUIRED to gain access.");
            throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access.");
        }

        evalDto.EvaluatorId = Guid.Parse(empId!);
        await med.Send(new JobAppEvaluateCmd { EvalDto = evalDto });
        return Ok(ApiResponse<string>.Ok(null!, $"EVALUATION Successfully submitted for JOB APPLICATION."));
    }




}
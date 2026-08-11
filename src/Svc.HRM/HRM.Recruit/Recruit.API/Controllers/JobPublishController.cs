using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.Domain.DTOs;
using System.Security.Claims;

namespace Recruit.API.Controllers;

/// <summary>
/// JOB PUBLISH management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/JobPublish")]
[ApiVersion("1.0")]
public class JobPublishController(IMediator med) : ControllerBase
{
    [PerAuth("hr.recruit.posting.manage")]
    [HttpPost("PublishJobPost")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Publish([FromBody] PostPublish rvw)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 }) { throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access."); }

        rvw.ReviewById = Guid.Parse(empId!);
        var command = new JobPostPublishCmd { Rvw = rvw };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New JOB POSTING successfully PUBLISHED."));
    }

    [PerAuth("hr.recruit.posting.manage")]
    [HttpPost("PublishAllJobPost")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PublishAll([FromBody] PostPublish rvw)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 }) { throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access."); }

        rvw.ReviewById = Guid.Parse(empId!);
        var command = new JobPostPublishAllCmd { Rvw = rvw };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "All JOB POSTINGS successfully PUBLISHED."));
    }

    [PerAuth("hr.recruit.posting.manage")]
    [HttpPost("CloseJobPost/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Close(Guid id)
    {
        await med.Send(new JobPostingCloseCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, $"JOB POSTING with Id {id} successfully CLOSED."));
    }




}
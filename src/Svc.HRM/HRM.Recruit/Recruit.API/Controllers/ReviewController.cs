using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.Domain.DTOs;
using System.Security.Claims;

namespace Recruit.API.Controllers;

/// <summary>
/// REVIEWING end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/Review")]
[ApiVersion("1.0")]
public class ReviewController(IMediator med) : ControllerBase
{
    [HttpPost("WoFoPl")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> WoFoPl([FromBody] ReviewDto rvw)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 }) { throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access."); }

        rvw.ReviewById = Guid.Parse(empId!);
        var command = new WoFoPlReviewCmd { Rvw = rvw };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected WORKFORCE PLAN successfully REVIEWED."));
    }

    [HttpPost("JobReq")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JobReq([FromBody] ReviewDto rvw)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 }) { throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access."); }

        rvw.ReviewById = Guid.Parse(empId!);
        var command = new JobReqReviewCmd { Rvw = rvw };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected JOB REQUISITION successfully REVIEWED."));
    }

    [HttpPost("JobReqAll")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JobReqAll([FromBody] ReviewAllDto rvw)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 }) { throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access."); }

        rvw.ReviewById = Guid.Parse(empId!);
        var command = new JobReqReviewAllCmd { Rvw = rvw };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "All JOB REQUISITIONS successfully REVIEWED."));
    }



}
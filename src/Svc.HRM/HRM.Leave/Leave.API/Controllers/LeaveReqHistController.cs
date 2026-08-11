using Asp.Versioning;
using Common;
using Helpers;
using Leave.App.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// LEAVE REQUEST HISTORY end points
/// </summary>

[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/LvReqHist")]
[ApiVersion("1.0")]
public class LeaveReqHistController(IMediator med) : ControllerBase
{
    [HttpGet("MyReqHist")]
    [PerAuth("my.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MyReqHist()
    {
        if (!User.TryGetEmployeeId(out var empId)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        var res = await med.Send(new MyHistLeaveQry { Id = empId });
        return Ok(ApiResponse<object>.Ok(res));
    }

    [HttpGet("DeptReqHist")]
    [PerAuth("my.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeptReqHist()
    {
        if (!User.TryGetEmployeeId(out var empId)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        var res = await med.Send(new DeptHistLeaveQry { Id = empId });
        return Ok(ApiResponse<object>.Ok(res));
    }

    [HttpGet("BraReqHist")]
    [PerAuth("my.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BraReqHist()
    {
        if (!User.TryGetEmployeeId(out var empId)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        var res = await med.Send(new BraHistLeaveQry { Id = empId });
        return Ok(ApiResponse<object>.Ok(res));
    }

    [HttpGet("AllReqHist")]
    [PerAuth("my.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AllReqHist()
    {
        var response = await med.Send(new AllHistLeaveQry());
        return Ok(ApiResponse<object>.Ok(response));
    }
}
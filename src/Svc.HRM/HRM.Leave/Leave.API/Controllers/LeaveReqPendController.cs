using Asp.Versioning;
using Helpers;
using Leave.App.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// PENDING LEAVE REQUESTS end points
/// </summary>

[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/LvReqPend")]
[ApiVersion("1.0")]
public class LeaveReqPendController(IMediator med) : ControllerBase
{
    [HttpGet("MyPendReq")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MyPendReq()
    {
        if (!User.TryGetEmployeeId(out var empId)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }
        var res = await med.Send(new MyPendLeaveQry { Id = empId });
        if (res == null) { throw new NotFoundExc("PENDING LEAVE REQUEST NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(res));
    }

    [HttpGet("DeptPendReq")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeptPendReq()
    {
        if (!User.TryGetEmployeeId(out var empId)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        var res = await med.Send(new DeptPendLeaveQry { Id = empId });
        return Ok(ApiResponse<object>.Ok(res));
    }

    [HttpGet("BraPendReq")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BraPendReq()
    {
        if (!User.TryGetEmployeeId(out var empId)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        var res = await med.Send(new BraPendLeaveQry { Id = empId });
        return Ok(ApiResponse<object>.Ok(res));
    }

    [HttpGet("AllPendReq")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AllPendReq()
    {
        var response = await med.Send(new AllPendLeaveQry());
        return Ok(ApiResponse<object>.Ok(response));
    }
}
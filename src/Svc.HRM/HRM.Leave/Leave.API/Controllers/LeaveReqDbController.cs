using Asp.Versioning;
using Common;
using Helpers;
using Leave.App.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// PENDING LEAVE REQUESTS end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/LeaveReqDb")]
[ApiVersion("1.0")]
public class LeaveReqDbController(IMediator med) : ControllerBase
{
    [HttpGet("PendList")]
    [PerAuth("leave.approve.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PendList()
    {
        var response = await med.Send(new LvReqPendDbQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

     [HttpGet("OnLeaveList")]
     [PerAuth("leave.approve.view")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> OnLeaveList()
        {
            var response = await med.Send(new LvReqOnLeaveDbQry());
            return Ok(ApiResponse<object>.Ok(response));
        }


}
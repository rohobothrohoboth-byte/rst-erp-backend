using Asp.Versioning;
using Common;
using Helpers;
using Leave.App.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// Leave Dashboard endpoints - exposes leave data for other modules
/// </summary>
[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/dashboard")]
[ApiVersion("1.0")]
public class LeaveDashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveDashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get pending leave requests
    /// </summary>
    [HttpGet("pending")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingLeaveRequests([FromQuery] int limit = 10)
    {
        var response = await _mediator.Send(new GetPendingLeaveRequestsQry { Limit = limit });
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get employees on leave today
    /// </summary>
    [HttpGet("on-leave")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeesOnLeave()
    {
        var response = await _mediator.Send(new GetEmployeesOnLeaveQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get leave balance summary
    /// </summary>
    [HttpGet("balance-summary")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaveBalanceSummary()
    {
        var response = await _mediator.Send(new GetLeaveBalanceSummaryQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get leave statistics
    /// </summary>
    [HttpGet("statistics")]
    [PerAuth("hr.leave.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaveStatistics()
    {
        var response = await _mediator.Send(new GetLeaveStatisticsQry());
        return Ok(ApiResponse<object>.Ok(response));
    }
}
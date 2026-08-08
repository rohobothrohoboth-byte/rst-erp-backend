using Asp.Versioning;
using Helpers;
using Leave.App.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// Service-to-service leave integration for Attendance / Payroll.
/// </summary>
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/Integration")]
[ApiVersion("1.0")]
public class LeaveIntegrationController(IMediator med) : ControllerBase
{
    /// <summary>
    /// Approved leave overlapping a date range (optional employee filter).
    /// </summary>
    [HttpGet("ApprovedLeaves")]
    [AllowAnonymous] // secured via gateway/network; services call with API key headers when available
    public async Task<IActionResult> ApprovedLeaves(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] Guid? employeeId = null)
    {
        if (to < from)
            throw new ValException("to must be on or after from.");

        var response = await med.Send(new ApprovedLeavesInRangeQry
        {
            EmployeeId = employeeId,
            From = from,
            To = to
        });
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Unpaid leave day count for an employee in a payroll period.
    /// </summary>
    [HttpGet("UnpaidDays/{employeeId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> UnpaidDays(
        Guid employeeId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        if (to < from)
            throw new ValException("to must be on or after from.");

        var response = await med.Send(new UnpaidLeaveDaysQry
        {
            EmployeeId = employeeId,
            From = from,
            To = to
        });
        return Ok(ApiResponse<object>.Ok(response));
    }
}

using Asp.Versioning;
using Common;
using Helpers;
using Leave.App.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// LEAVE BALANCE MANAGEMENT
/// </summary>
[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/Balance")]
[ApiVersion("1.0")]
public class LeaveBalanceController : ControllerBase
{
    private readonly IMediator _med;

    public LeaveBalanceController(IMediator med)
    {
        _med = med;
    }

    [HttpGet("MyBalance")]
    [PerAuth("leave.balance.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyBalance()
    {
        if (!User.TryGetEmployeeId(out var id))
            throw new UnauthorizedException("Authorization required.");

        var response = await _med.Send(new EmpLeaveBalQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Employee/{employeeId:guid}")]
    [PerAuth("leave.balance.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeeBalance(Guid employeeId)
    {
        var response = await _med.Send(new EmpLeaveBalQry { Id = employeeId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    // Optional: Get all active policies for employee
    [HttpGet("MyPolicies")]
    [PerAuth("leave.balance.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPolicies()
    {
        if (!User.TryGetEmployeeId(out var id))
            throw new UnauthorizedException("Authorization required.");

        var response = await _med.Send(new EmpLeavePolicyByEmployeeQry { EmployeeId = id });
        return Ok(ApiResponse<object>.Ok(response));
    }
}
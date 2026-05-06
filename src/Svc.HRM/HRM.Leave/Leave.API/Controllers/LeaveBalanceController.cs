using Asp.Versioning;
using Helpers;
using Leave.App.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// LEAVE BALANCE management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/LeaveBalance")]
[ApiVersion("1.0")]
public class LeaveBalanceController(IMediator med) : ControllerBase
{
    [HttpGet("MyLeaveBalance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MyLeaveBalance()
    {
        if (!User.TryGetEmployeeId(out var id)) { return Ok(ApiResponse<object>.Fail("AUTHORIZATION REQUIRED to gain access. Please LOGIN!")); }

        var response = await med.Send(new EmpLeaveBalQry { Id = id });
        return response == null ? throw new DomainException("EMPLOYEE'S Profile Info NOT FOUND.") : Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetLeaveBalance/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeaveBalance(Guid id)
    {
        var response = await med.Send(new EmpLeaveBalQry { Id = id });
        return response == null ? throw new DomainException("EMPLOYEE'S Profile Info NOT FOUND.") : Ok(ApiResponse<object>.Ok(response));
    }


}
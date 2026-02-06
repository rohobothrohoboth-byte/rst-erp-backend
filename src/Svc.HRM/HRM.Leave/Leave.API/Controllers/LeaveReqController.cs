using Asp.Versioning;
using Helpers;
using Leave.App.Commands;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Leave.API.Controllers;

/// <summary>
/// LEAVE REQUEST management end points
/// </summary>

[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/LeaveRequest")]
[ApiVersion("1.0")]
public class LeaveReqController(IMediator med) : ControllerBase
{
    //[PerAuth("leave.req")]
    [HttpPost("AddNewReq")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddNewReq([FromBody] LeaveRequestAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 }) { throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access."); }

        var command = new LeaveRequestAddCmd
        {
            AddDto = addDto,
            EmpId = Guid.Parse(empId!)
        };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New LEAVE REQUEST successfully created."));
    }

    [HttpGet("MyLeaveReq")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MyLeaveReq()
    {
        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 }) { throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access."); }

        var rqst = new LeaveRequestMyQry { Id = Guid.Parse(empId!) };
        var response = await med.Send(rqst);
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetLeaveReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeaveReq(Guid id)
    {
        var response = await med.Send(new LeaveRequestByIdQry { Id = id });
        if (response == null) { throw new DomainException($"LEAVE REQUEST with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }




}
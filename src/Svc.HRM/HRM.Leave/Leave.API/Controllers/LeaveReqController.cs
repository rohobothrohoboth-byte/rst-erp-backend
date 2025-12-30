using Asp.Versioning;
using Common;
using Leave.App.Commands;
using Leave.App.Helpers;
using Leave.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Leave.API.Controllers;

/// <summary>
/// LEAVE REQUEST management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/LeaveRequest")]
[ApiVersion("1.0")]
public class LeaveReqController(IMediator med) : ControllerBase
{
    [HttpPost("NewReq")]
    //[PerAuth("leave.req")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> NewReq([FromBody] LeaveRequestAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 })
        {
            throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access.");
        }
        
        var command = new LeaveRequestAddCmd
        {
            AddDto = addDto,
            EmpId = Guid.Parse(empId!)
        };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New LEAVE TYPE successfully created."));
    }

}
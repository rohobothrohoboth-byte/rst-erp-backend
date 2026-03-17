using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Queries;

namespace Svc.Auth.Controllers;

/// <summary>
/// Employees Management by ADMIN end points
/// </summary>

//[Authorize(Roles = "admin")]
[ApiController]
[Route("api/auth/v{version:apiVersion}/AdminEmp")]
[ApiVersion("1.0")]
public class EmpController(IMediator med) : ControllerBase
{
    [HttpGet("AllEmployee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllEmployee()
    {
        var response = await med.Send(new EmpAllAdminQry());
        return Ok(ApiResponse<object>.Ok(response));
    }


}
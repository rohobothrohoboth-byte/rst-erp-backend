using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Helpers;
using Profile.App.Queries;

namespace Profile.API.Controllers;

/// <summary>
/// Employee Emergency Contacts end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpSearch")]
[ApiVersion("1.0")]
public class EmpSearchController(IMediator med) : ControllerBase
{

    [HttpGet("EmpByCode/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EmpByCode(string code)
    {
        if (code.Length != 10)
        {
            throw new DomainException($"EMPLOYEE Code should be 10 digits.");
        }
        var res = await med.Send(new SearchByCodeQry { Code = code });
        if (res == null) { throw new DomainException($"EMPLOYEE with Code [{code}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(res));
    }
}
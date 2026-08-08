using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Svc.HRM.Attendance.Controllers;

/// <summary>
/// Deprecated local leave API. Use HRM.Leave via Gateway: /hrm/leave/...
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/leave")]
[Obsolete("Attendance leave is deprecated. Use HRM.Leave (/hrm/leave) as the single leave source of truth.")]
public class LeaveController : ControllerBase
{
    /// <summary>
    /// All former attendance leave endpoints return 410 Gone.
    /// Canonical leave API: /hrm/leave/v1/...
    /// </summary>
    [HttpGet("{**catchAll}")]
    [HttpPost("{**catchAll}")]
    [HttpPut("{**catchAll}")]
    [HttpDelete("{**catchAll}")]
    [HttpPatch("{**catchAll}")]
    public IActionResult Gone(string? catchAll = null)
    {
        return StatusCode(StatusCodes.Status410Gone, new
        {
            success = false,
            message = "Attendance leave API has been removed. Use HRM.Leave via Gateway path /hrm/leave/...",
            redirect = "/hrm/leave/v1/Request/MyLeaveReq",
            path = catchAll
        });
    }

    [HttpGet("")]
    [HttpPost("")]
    public IActionResult GoneRoot() => Gone(null);
}

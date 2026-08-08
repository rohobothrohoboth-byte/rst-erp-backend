// Profile.API/Controllers/DashboardController.cs

using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Queries;

namespace Profile.API.Controllers;

/// <summary>
/// HR Dashboard endpoints
/// </summary>
[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/dashboard")]
[ApiVersion("1.0")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get recent activities
    /// </summary>
    [HttpGet("activities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentActivities([FromQuery] int limit = 10)
    {
        var response = await _mediator.Send(new GetRecentActivitiesQry { Limit = limit });
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get upcoming events
    /// </summary>
    [HttpGet("events/upcoming")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcomingEvents()
    {
        var response = await _mediator.Send(new GetUpcomingEventsQry());
        return Ok(ApiResponse<object>.Ok(response));
    }
}
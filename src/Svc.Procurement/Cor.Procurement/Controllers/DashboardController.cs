using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.Procurement.Queries;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Controllers;

[ApiController]
[Route("api/procurement/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class DashboardController : BaseApiController
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all dashboard data in one request
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] int recentActivitiesCount = 5,
        [FromQuery] int topVendorsCount = 5)
    {
        try
        {
            var query = new GetDashboardDataQuery
            {
                RecentActivitiesCount = recentActivitiesCount,
                TopVendorsCount = topVendorsCount
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetDashboard));
        }
    }

    /// <summary>
    /// Get dashboard stats only
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(DashboardStats), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var query = new GetDashboardDataQuery();
            var result = await _mediator.Send(query);
            return Ok(result.Stats);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetStats));
        }
    }

    /// <summary>
    /// Get recent activities
    /// </summary>
    [HttpGet("activities")]
    [ProducesResponseType(typeof(List<DashboardActivityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivities([FromQuery] int count = 10)
    {
        try
        {
            var query = new GetDashboardDataQuery { RecentActivitiesCount = count };
            var result = await _mediator.Send(query);
            return Ok(result.RecentActivities);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetActivities));
        }
    }

    /// <summary>
    /// Get vendor performance data
    /// </summary>
    [HttpGet("vendors")]
    [ProducesResponseType(typeof(List<DashboardVendorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVendors([FromQuery] int count = 5)
    {
        try
        {
            var query = new GetDashboardDataQuery { TopVendorsCount = count };
            var result = await _mediator.Send(query);
            return Ok(result.Vendors);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetVendors));
        }
    }
}
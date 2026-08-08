// Cor.CRM/Controllers/SalesForecastController.cs

using Asp.Versioning;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.CRM.Interfaces;
namespace Cor.CRM.Controllers;

[Authorize]
[ApiController]
[Route("api/core/crm/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class SalesForecastController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public SalesForecastController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get sales forecast data
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetForecast([FromQuery] string? period = "quarter")
    {
        try
        {
            var query = new SalesForecastQry { Period = period };
            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sales forecast");
            throw;
        }
    }
}
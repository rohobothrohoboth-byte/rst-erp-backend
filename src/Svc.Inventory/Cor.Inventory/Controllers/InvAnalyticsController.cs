using Asp.Versioning;
using Common;
using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Inventory.Controllers;

[ApiController]
[Route("api/inventory/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class InvAnalyticsController : ControllerBase
{
    private readonly IInvAnalyticsService _analyticsService;
    private readonly IValuationService _valuationService;
    private readonly ILogger<InvAnalyticsController> _logger;

    public InvAnalyticsController(
        IInvAnalyticsService analyticsService,
        IValuationService valuationService,
        ILogger<InvAnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _valuationService = valuationService;
        _logger = logger;
    }

    [HttpGet("stock-summary")]
    [PerAuth("inv.analytics.stock.view")]
    [ProducesResponseType(typeof(ApiResponse<StockSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockSummary()
    {
        try
        {
            var summary = await _analyticsService.GetStockSummaryAsync();
            return Ok(ApiResponse<StockSummaryDto>.Ok(summary));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock summary");
            return StatusCode(500, ApiResponse<StockSummaryDto>.Fail("Failed to get stock summary", statusCode: 500));
        }
    }

    [HttpGet("stock-value")]
    [PerAuth("inv.analytics.stock.value")]
    [ProducesResponseType(typeof(ApiResponse<ValuationReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockValue([FromQuery] Guid? warehouseId)
    {
        try
        {
            var report = await _valuationService.GetReportAsync(warehouseId);
            return Ok(ApiResponse<ValuationReportDto>.Ok(report));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock value");
            return StatusCode(500, ApiResponse<ValuationReportDto>.Fail("Failed to get stock value", statusCode: 500));
        }
    }

    [HttpGet("movement")]
    [PerAuth("inv.analytics.movement.view")]
    [ProducesResponseType(typeof(ApiResponse<MovementAnalysisDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMovement([FromQuery] int top = 5)
    {
        try
        {
            var analysis = await _analyticsService.GetMovementAnalysisAsync(top);
            return Ok(ApiResponse<MovementAnalysisDto>.Ok(analysis));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting movement analysis");
            return StatusCode(500, ApiResponse<MovementAnalysisDto>.Fail("Failed to get movement analysis", statusCode: 500));
        }
    }

    [HttpGet("forecast")]
    [PerAuth("inv.analytics.forecast.view")]
    [ProducesResponseType(typeof(ApiResponse<ForecastDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetForecast([FromQuery] int months = 3)
    {
        try
        {
            var forecast = await _analyticsService.GetForecastAsync(months);
            return Ok(ApiResponse<ForecastDto>.Ok(forecast));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forecast");
            return StatusCode(500, ApiResponse<ForecastDto>.Fail("Failed to get forecast", statusCode: 500));
        }
    }
}

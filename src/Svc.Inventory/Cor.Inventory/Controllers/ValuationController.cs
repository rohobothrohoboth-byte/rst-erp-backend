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
public class ValuationController : ControllerBase
{
    private readonly IValuationService _valuationService;
    private readonly ILogger<ValuationController> _logger;

    public ValuationController(IValuationService valuationService, ILogger<ValuationController> logger)
    {
        _valuationService = valuationService;
        _logger = logger;
    }

    [HttpGet("method")]
    [PerAuth("inv.valuation.method.view")]
    [ProducesResponseType(typeof(ApiResponse<ValuationMethodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMethod()
    {
        try
        {
            var method = await _valuationService.GetMethodAsync();
            return Ok(ApiResponse<ValuationMethodDto>.Ok(method));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting valuation method");
            return StatusCode(500, ApiResponse<ValuationMethodDto>.Fail("Failed to get valuation method", statusCode: 500));
        }
    }

    [HttpPut("method")]
    [PerAuth("inv.valuation.method.mod")]
    [ProducesResponseType(typeof(ApiResponse<ValuationMethodDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetMethod([FromBody] ValuationMethodDto dto)
    {
        try
        {
            var method = await _valuationService.SetMethodAsync(dto);
            return Ok(ApiResponse<ValuationMethodDto>.Ok(method, "Valuation method updated"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ValuationMethodDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting valuation method");
            return StatusCode(500, ApiResponse<ValuationMethodDto>.Fail("Failed to set valuation method", statusCode: 500));
        }
    }

    [HttpGet("report")]
    [PerAuth("inv.valuation.report.view")]
    [ProducesResponseType(typeof(ApiResponse<ValuationReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReport([FromQuery] Guid? warehouseId)
    {
        try
        {
            var report = await _valuationService.GetReportAsync(warehouseId);
            return Ok(ApiResponse<ValuationReportDto>.Ok(report));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting valuation report");
            return StatusCode(500, ApiResponse<ValuationReportDto>.Fail("Failed to get valuation report", statusCode: 500));
        }
    }
}

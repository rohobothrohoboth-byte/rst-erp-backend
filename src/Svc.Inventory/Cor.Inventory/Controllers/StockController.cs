using Asp.Versioning;
using Common;
using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Inventory.Controllers;

[ApiController]
[Route("api/inventory/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;
    private readonly ILogger<StockController> _logger;

    public StockController(IStockService stockService, ILogger<StockController> logger)
    {
        _stockService = stockService;
        _logger = logger;
    }

    [HttpGet("movements")]
    [PerAuth("inv.stock.view")]
    [ProducesResponseType(typeof(ApiResponse<List<StockMovementDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMovements([FromQuery] string? type, [FromQuery] Guid? warehouseId)
    {
        try
        {
            var movements = await _stockService.GetMovementsAsync(type, warehouseId);
            return Ok(ApiResponse<List<StockMovementDto>>.Ok(movements));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock movements");
            return StatusCode(500, ApiResponse<List<StockMovementDto>>.Fail("Failed to get stock movements", statusCode: 500));
        }
    }

    [HttpGet("levels")]
    [PerAuth("inv.stock.view")]
    [ProducesResponseType(typeof(ApiResponse<List<StockLevel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLevels([FromQuery] Guid? warehouseId)
    {
        try
        {
            var levels = await _stockService.GetStockLevelsAsync(warehouseId);
            return Ok(ApiResponse<List<StockLevel>>.Ok(levels));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock levels");
            return StatusCode(500, ApiResponse<List<StockLevel>>.Fail("Failed to get stock levels", statusCode: 500));
        }
    }

    [HttpPost("inbound")]
    [PerAuth("inv.stock.inbound.add")]
    [ProducesResponseType(typeof(ApiResponse<StockMovementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Inbound([FromBody] InboundStockDto dto)
    {
        try
        {
            var movement = await _stockService.InboundAsync(dto);
            return Ok(ApiResponse<StockMovementDto>.Ok(movement, "Inbound movement posted"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StockMovementDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting inbound movement");
            return StatusCode(500, ApiResponse<StockMovementDto>.Fail("Failed to post inbound movement", statusCode: 500));
        }
    }

    [HttpPost("outbound")]
    [PerAuth("inv.stock.outbound.add")]
    [ProducesResponseType(typeof(ApiResponse<StockMovementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Outbound([FromBody] OutboundStockDto dto)
    {
        try
        {
            var movement = await _stockService.OutboundAsync(dto);
            return Ok(ApiResponse<StockMovementDto>.Ok(movement, "Outbound movement posted"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StockMovementDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting outbound movement");
            return StatusCode(500, ApiResponse<StockMovementDto>.Fail("Failed to post outbound movement", statusCode: 500));
        }
    }

    [HttpPost("transfer")]
    [PerAuth("inv.stock.transfer.add")]
    [ProducesResponseType(typeof(ApiResponse<StockMovementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Transfer([FromBody] TransferStockDto dto)
    {
        try
        {
            var movement = await _stockService.TransferAsync(dto);
            return Ok(ApiResponse<StockMovementDto>.Ok(movement, "Transfer movement posted"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StockMovementDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting transfer movement");
            return StatusCode(500, ApiResponse<StockMovementDto>.Fail("Failed to post transfer movement", statusCode: 500));
        }
    }

    [HttpPost("adjustment")]
    [PerAuth("inv.stock.adjustment.add")]
    [ProducesResponseType(typeof(ApiResponse<StockMovementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Adjustment([FromBody] AdjustmentStockDto dto)
    {
        try
        {
            var movement = await _stockService.AdjustmentAsync(dto);
            return Ok(ApiResponse<StockMovementDto>.Ok(movement, "Adjustment movement posted"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StockMovementDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting adjustment movement");
            return StatusCode(500, ApiResponse<StockMovementDto>.Fail("Failed to post adjustment movement", statusCode: 500));
        }
    }
}

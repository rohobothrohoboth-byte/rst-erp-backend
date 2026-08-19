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
public class StockCountController : ControllerBase
{
    private readonly IStockService _stockService;
    private readonly ILogger<StockCountController> _logger;

    public StockCountController(IStockService stockService, ILogger<StockCountController> logger)
    {
        _stockService = stockService;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth("inv.stock.count.view")]
    [ProducesResponseType(typeof(ApiResponse<List<StockCountDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var counts = await _stockService.GetCountsAsync();
            return Ok(ApiResponse<List<StockCountDto>>.Ok(counts));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock counts");
            return StatusCode(500, ApiResponse<List<StockCountDto>>.Fail("Failed to get stock counts", statusCode: 500));
        }
    }

    [HttpGet("{id}")]
    [PerAuth("inv.stock.count.view")]
    [ProducesResponseType(typeof(ApiResponse<StockCountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var count = await _stockService.GetCountAsync(id);
            if (count == null)
                return NotFound(ApiResponse<StockCountDto>.Fail($"Stock count with ID '{id}' not found", statusCode: 404));

            return Ok(ApiResponse<StockCountDto>.Ok(count));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock count {Id}", id);
            return StatusCode(500, ApiResponse<StockCountDto>.Fail("Failed to get stock count", statusCode: 500));
        }
    }

    [HttpPost]
    [PerAuth("inv.stock.count.create")]
    [ProducesResponseType(typeof(ApiResponse<StockCountDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateStockCountDto dto)
    {
        try
        {
            var count = await _stockService.CreateCountAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = count.Id, version = "1.0" },
                ApiResponse<StockCountDto>.Ok(count, "Stock count created"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StockCountDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating stock count");
            return StatusCode(500, ApiResponse<StockCountDto>.Fail("Failed to create stock count", statusCode: 500));
        }
    }

    [HttpPut("{id}/lines")]
    [PerAuth("inv.stock.count.perform")]
    [ProducesResponseType(typeof(ApiResponse<StockCountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordLines(Guid id, [FromBody] RecordStockCountDto dto)
    {
        try
        {
            var count = await _stockService.RecordCountAsync(id, dto);
            return Ok(ApiResponse<StockCountDto>.Ok(count, "Counted quantities recorded"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<StockCountDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StockCountDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording stock count lines for {Id}", id);
            return StatusCode(500, ApiResponse<StockCountDto>.Fail("Failed to record stock count lines", statusCode: 500));
        }
    }

    [HttpPost("{id}/reconcile")]
    [PerAuth("inv.stock.count.reconcile")]
    [ProducesResponseType(typeof(ApiResponse<StockCountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reconcile(Guid id)
    {
        try
        {
            var count = await _stockService.ReconcileCountAsync(id);
            return Ok(ApiResponse<StockCountDto>.Ok(count, "Stock count reconciled"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<StockCountDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StockCountDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reconciling stock count {Id}", id);
            return StatusCode(500, ApiResponse<StockCountDto>.Fail("Failed to reconcile stock count", statusCode: 500));
        }
    }
}

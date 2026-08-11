using Asp.Versioning;
using Common;
using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Controllers;

[ApiController]
[Route("api/inventory/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class WarehouseZoneController : ControllerBase
{
    private readonly IWarehouseZoneService _zoneService;
    private readonly ILogger<WarehouseZoneController> _logger;

    public WarehouseZoneController(IWarehouseZoneService zoneService, ILogger<WarehouseZoneController> logger)
    {
        _zoneService = zoneService;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth("inv.warehouse.zone.view")]
    [ProducesResponseType(typeof(ApiResponse<List<WarehouseZoneDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? warehouseId)
    {
        try
        {
            var zones = await _zoneService.GetZonesAsync(warehouseId);
            return Ok(ApiResponse<List<WarehouseZoneDto>>.Ok(zones));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting warehouse zones");
            return StatusCode(500, ApiResponse<List<WarehouseZoneDto>>.Fail("Failed to get warehouse zones", statusCode: 500));
        }
    }

    [HttpGet("{id}")]
    [PerAuth("inv.warehouse.zone.view")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseZoneDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var zone = await _zoneService.GetZoneByIdAsync(id);
            if (zone == null)
                return NotFound(ApiResponse<WarehouseZoneDto>.Fail($"Warehouse zone with ID '{id}' not found", statusCode: 404));

            return Ok(ApiResponse<WarehouseZoneDto>.Ok(zone));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting warehouse zone {Id}", id);
            return StatusCode(500, ApiResponse<WarehouseZoneDto>.Fail("Failed to get warehouse zone", statusCode: 500));
        }
    }

    [HttpPost]
    [PerAuth("inv.warehouse.zone.add")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseZoneDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseZoneDto dto)
    {
        try
        {
            var zone = await _zoneService.CreateZoneAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = zone.Id, version = "1.0" },
                ApiResponse<WarehouseZoneDto>.Ok(zone, "Warehouse zone created"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<WarehouseZoneDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating warehouse zone");
            return StatusCode(500, ApiResponse<WarehouseZoneDto>.Fail("Failed to create warehouse zone", statusCode: 500));
        }
    }

    [HttpPut]
    [PerAuth("inv.warehouse.zone.mod")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseZoneDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromBody] UpdateWarehouseZoneDto dto)
    {
        try
        {
            var zone = await _zoneService.UpdateZoneAsync(dto);
            return Ok(ApiResponse<WarehouseZoneDto>.Ok(zone, "Warehouse zone updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<WarehouseZoneDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(ApiResponse<WarehouseZoneDto>.Fail("The warehouse zone was modified by another user", statusCode: 409));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating warehouse zone");
            return StatusCode(500, ApiResponse<WarehouseZoneDto>.Fail("Failed to update warehouse zone", statusCode: 500));
        }
    }

    [HttpDelete("{id}")]
    [PerAuth("inv.warehouse.zone.del")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _zoneService.DeleteZoneAsync(id);
            if (!result)
                return NotFound(ApiResponse<object>.Fail($"Warehouse zone with ID '{id}' not found", statusCode: 404));

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting warehouse zone {Id}", id);
            return StatusCode(500, ApiResponse<object>.Fail("Failed to delete warehouse zone", statusCode: 500));
        }
    }

    [HttpGet("{id}/bins")]
    [PerAuth("inv.warehouse.zone.bin")]
    [ProducesResponseType(typeof(ApiResponse<List<BinDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBins(Guid id)
    {
        try
        {
            var bins = await _zoneService.GetBinsAsync(id);
            return Ok(ApiResponse<List<BinDto>>.Ok(bins));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bins for zone {Id}", id);
            return StatusCode(500, ApiResponse<List<BinDto>>.Fail("Failed to get bins", statusCode: 500));
        }
    }

    [HttpPost("{id}/bins")]
    [PerAuth("inv.warehouse.zone.bin")]
    [ProducesResponseType(typeof(ApiResponse<BinDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateBin(Guid id, [FromBody] CreateBinDto dto)
    {
        try
        {
            var bin = await _zoneService.CreateBinAsync(id, dto);
            return Ok(ApiResponse<BinDto>.Ok(bin, "Bin created"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<BinDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<BinDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bin in zone {Id}", id);
            return StatusCode(500, ApiResponse<BinDto>.Fail("Failed to create bin", statusCode: 500));
        }
    }

    [HttpGet("layout/{warehouseId}")]
    [PerAuth("inv.warehouse.layout.view")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseLayoutDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLayout(Guid warehouseId)
    {
        try
        {
            var layout = await _zoneService.GetLayoutAsync(warehouseId);
            return Ok(ApiResponse<WarehouseLayoutDto>.Ok(layout));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting warehouse layout for {WarehouseId}", warehouseId);
            return StatusCode(500, ApiResponse<WarehouseLayoutDto>.Fail("Failed to get warehouse layout", statusCode: 500));
        }
    }
}

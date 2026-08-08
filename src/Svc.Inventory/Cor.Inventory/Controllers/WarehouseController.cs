using Asp.Versioning;
using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Cor.Inventory.Controllers;

[ApiController]
[Route("api/inventory/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    private readonly ILogger<WarehouseController> _logger;

    public WarehouseController(IWarehouseService warehouseService, ILogger<WarehouseController> logger)
    {
        _warehouseService = warehouseService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<WarehouseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return Ok(new { success = true, data = warehouses });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting warehouses");
            return StatusCode(500, new { success = false, message = "Failed to get warehouses" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
                return NotFound(new { success = false, message = $"Warehouse with ID '{id}' not found" });

            return Ok(new { success = true, data = warehouse });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting warehouse {Id}", id);
            return StatusCode(500, new { success = false, message = "Failed to get warehouse" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseDto dto)
    {
        try
        {
            var warehouse = await _warehouseService.CreateWarehouseAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = warehouse.Id },
                new { success = true, data = warehouse });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating warehouse");
            return StatusCode(500, new { success = false, message = "Failed to create warehouse" });
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromBody] UpdateWarehouseDto dto)
    {
        try
        {
            var warehouse = await _warehouseService.UpdateWarehouseAsync(dto);
            return Ok(new { success = true, data = warehouse });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { success = false, message = "The warehouse was modified by another user" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating warehouse");
            return StatusCode(500, new { success = false, message = "Failed to update warehouse" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _warehouseService.DeleteWarehouseAsync(id);
            if (!result)
                return NotFound(new { success = false, message = $"Warehouse with ID '{id}' not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting warehouse {Id}", id);
            return StatusCode(500, new { success = false, message = "Failed to delete warehouse" });
        }
    }
}
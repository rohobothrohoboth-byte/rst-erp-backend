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
public class UnitController : ControllerBase
{
    private readonly IUnitService _unitService;
    private readonly ILogger<UnitController> _logger;

    public UnitController(IUnitService unitService, ILogger<UnitController> logger)
    {
        _unitService = unitService;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth("inv.products.unit.view")]
    [ProducesResponseType(typeof(ApiResponse<List<UnitDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var units = await _unitService.GetAllAsync();
            return Ok(ApiResponse<List<UnitDto>>.Ok(units));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting units");
            return StatusCode(500, ApiResponse<List<UnitDto>>.Fail("Failed to get units", statusCode: 500));
        }
    }

    [HttpGet("{id}")]
    [PerAuth("inv.products.unit.view")]
    [ProducesResponseType(typeof(ApiResponse<UnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var unit = await _unitService.GetByIdAsync(id);
            if (unit == null)
                return NotFound(ApiResponse<UnitDto>.Fail($"Unit with ID '{id}' not found", statusCode: 404));

            return Ok(ApiResponse<UnitDto>.Ok(unit));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unit {Id}", id);
            return StatusCode(500, ApiResponse<UnitDto>.Fail("Failed to get unit", statusCode: 500));
        }
    }

    [HttpPost]
    [PerAuth("inv.products.unit.add")]
    [ProducesResponseType(typeof(ApiResponse<UnitDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUnitDto dto)
    {
        try
        {
            var unit = await _unitService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = unit.Id, version = "1.0" },
                ApiResponse<UnitDto>.Ok(unit, "Unit created"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<UnitDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating unit");
            return StatusCode(500, ApiResponse<UnitDto>.Fail("Failed to create unit", statusCode: 500));
        }
    }

    [HttpPut]
    [PerAuth("inv.products.unit.mod")]
    [ProducesResponseType(typeof(ApiResponse<UnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromBody] UpdateUnitDto dto)
    {
        try
        {
            var unit = await _unitService.UpdateAsync(dto);
            return Ok(ApiResponse<UnitDto>.Ok(unit, "Unit updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<UnitDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(ApiResponse<UnitDto>.Fail("The unit was modified by another user", statusCode: 409));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating unit");
            return StatusCode(500, ApiResponse<UnitDto>.Fail("Failed to update unit", statusCode: 500));
        }
    }

    [HttpDelete("{id}")]
    [PerAuth("inv.products.unit.del")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _unitService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<object>.Fail($"Unit with ID '{id}' not found", statusCode: 404));

            return Ok(ApiResponse<object>.Ok(null, "Unit deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting unit {Id}", id);
            return StatusCode(500, ApiResponse<object>.Fail("Failed to delete unit", statusCode: 500));
        }
    }
}

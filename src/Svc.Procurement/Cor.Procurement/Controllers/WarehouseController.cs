using Asp.Versioning;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cor.Procurement.Controllers;

[ApiController]
[Route("api/procurement/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class WarehouseController : BaseApiController
{
    private readonly ProcurementDbContext _context;

    public WarehouseController(
        ProcurementDbContext context,
        ILogger<WarehouseController> logger)
        : base(logger)
    {
        _context = context;
    }

    /// <summary>
    /// Get all warehouses from local copy
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<WarehouseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var warehouses = await _context.LocalWarehouses
                .Where(w => !w.IsDeleted && w.IsActive)
                .OrderBy(w => w.Name)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Code = w.Code,
                    Location = w.Location,
                    Address = w.Address,
                    City = w.City,
                    State = w.State,
                    Country = w.Country,
                    ZipCode = w.ZipCode,
                    Phone = w.Phone,
                    Email = w.Email,
                    WarehouseType = w.WarehouseType,
                    Status = w.Status,
                    IsActive = w.IsActive,
                    SyncedAt = w.SyncedAt,
                    SourceId = w.SourceId,
                    DateAdd = w.DateAdd,
                    DateMod = w.DateMod
                })
                .ToListAsync();

            return Ok(new { success = true, data = warehouses });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting warehouses");
            return StatusCode(500, new { success = false, message = "Failed to get warehouses" });
        }
    }

    /// <summary>
    /// Get warehouse by ID from local copy
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var warehouse = await _context.LocalWarehouses
                .Where(w => w.Id == id && !w.IsDeleted && w.IsActive)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Code = w.Code,
                    Location = w.Location,
                    Address = w.Address,
                    City = w.City,
                    State = w.State,
                    Country = w.Country,
                    ZipCode = w.ZipCode,
                    Phone = w.Phone,
                    Email = w.Email,
                    WarehouseType = w.WarehouseType,
                    Status = w.Status,
                    IsActive = w.IsActive,
                    SyncedAt = w.SyncedAt,
                    SourceId = w.SourceId,
                    DateAdd = w.DateAdd,
                    DateMod = w.DateMod
                })
                .FirstOrDefaultAsync();

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

    /// <summary>
    /// Get active warehouses only
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<WarehouseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive()
    {
        try
        {
            var warehouses = await _context.LocalWarehouses
                .Where(w => !w.IsDeleted && w.IsActive)
                .OrderBy(w => w.Name)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Code = w.Code,
                    Location = w.Location,
                    Address = w.Address,
                    City = w.City,
                    State = w.State,
                    Country = w.Country,
                    ZipCode = w.ZipCode,
                    Phone = w.Phone,
                    Email = w.Email,
                    WarehouseType = w.WarehouseType,
                    Status = w.Status,
                    IsActive = w.IsActive,
                    SyncedAt = w.SyncedAt,
                    SourceId = w.SourceId,
                    DateAdd = w.DateAdd,
                    DateMod = w.DateMod
                })
                .ToListAsync();

            return Ok(new { success = true, data = warehouses });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active warehouses");
            return StatusCode(500, new { success = false, message = "Failed to get active warehouses" });
        }
    }
}
// Controllers/CostCenterController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Shared.Helpers.Services;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class CostCenterController : BaseApiController
{
    private readonly CachedReferenceDataService _cachedService;
    private readonly ILogger<CostCenterController> _logger;

    public CostCenterController(
        IMediator mediator,
        ILogger<CostCenterController> logger,
        CachedReferenceDataService cachedService)
        : base(mediator, logger)
    {
        _cachedService = cachedService;
        _logger = logger;
    }

    /// <summary>
    /// Get all cost centers (CACHED)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CostCenterDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "isActive", "departmentId" })]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isActive = true,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sortBy = "Name",
        [FromQuery] string? sortDirection = "ASC")
    {
        try
        {
            _logger.LogInformation("📊 Getting cost centers - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            // ✅ Get from cache
            var costCenters = await _cachedService.GetCachedCostCentersAsync();

            // ✅ Apply filters in memory (since we're working with cached list)
            var filtered = costCenters.AsEnumerable();

            if (isActive.HasValue)
            {
                filtered = filtered.Where(x => x.IsActive == isActive.Value);
            }

            if (departmentId.HasValue)
            {
                filtered = filtered.Where(x => x.DepartmentId == departmentId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                filtered = filtered.Where(x =>
                    (x.Name?.ToLower().Contains(term) ?? false) ||
                    (x.Code?.ToLower().Contains(term) ?? false) ||
                    (x.Description?.ToLower().Contains(term) ?? false));
            }

            // ✅ Apply sorting in memory
            filtered = sortDirection?.ToUpper() == "ASC"
                ? filtered.OrderBy(x => GetPropertyValue(x, sortBy ?? "Name"))
                : filtered.OrderByDescending(x => GetPropertyValue(x, sortBy ?? "Name"));

            // ✅ Apply pagination
            var totalCount = filtered.Count();
            var items = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            _logger.LogInformation("✅ Retrieved {Count} cost centers (Total: {Total})", items.Count, totalCount);

            return Ok(new
            {
                success = true,
                data = items,
                totalCount = totalCount,
                page = page,
                pageSize = pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting cost centers");
            return HandleException(ex, "GetAllCostCenters");
        }
    }

    // ✅ Helper method for sorting
    private object GetPropertyValue(CostCenterDto obj, string propertyName)
    {
        return propertyName switch
        {
            "Id" => obj.Id,
            "Code" => obj.Code ?? string.Empty,
            "Name" => obj.Name ?? string.Empty,
            "Description" => obj.Description ?? string.Empty,
            "IsActive" => obj.IsActive,
            "DateAdd" => obj.DateAdd,
            _ => obj.Name ?? string.Empty
        };
    }

    /// <summary>
    /// Get cost center by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CostCenterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            _logger.LogInformation("📄 Getting cost center by ID: {Id}", id);

            // ✅ Try from cache first
            var costCenters = await _cachedService.GetCachedCostCentersAsync();
            var result = costCenters.FirstOrDefault(x => x.Id == id);

            if (result == null)
            {
                _logger.LogDebug("Cost center not found in cache, checking database");
                result = await Mediator.Send(new GetCostCenterByIdQry { Id = id });
            }

            if (result == null)
            {
                return NotFound(new { success = false, message = $"Cost center with ID '{id}' not found" });
            }

            return Ok(new { success = true, data = result });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { success = false, message = $"Cost center with ID '{id}' not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting cost center by ID: {Id}", id);
            return HandleException(ex, "GetCostCenterById", id);
        }
    }

    /// <summary>
    /// Get cost centers by department
    /// </summary>
    [HttpGet("ByDepartment/{departmentId}")]
    [ProducesResponseType(typeof(List<CostCenterDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "isActive" })]
    public async Task<IActionResult> GetByDepartment(
        Guid departmentId,
        [FromQuery] bool? isActive = true)
    {
        try
        {
            _logger.LogInformation("📊 Getting cost centers for department: {DepartmentId}", departmentId);

            var result = await Mediator.Send(new GetCostCentersByDepartmentQry
            {
                DepartmentId = departmentId,
                IsActive = isActive
            });

            _logger.LogInformation("✅ Retrieved {Count} cost centers for department", result.Count);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting cost centers for department: {DepartmentId}", departmentId);
            return HandleException(ex, "GetCostCentersByDepartment", departmentId);
        }
    }

    /// <summary>
    /// Create a new cost center
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CostCenterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddCostCenterDto dto)
    {
        try
        {
            _logger.LogInformation("📝 Creating cost center: {Name}", dto.Name);

            // Validate
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new { success = false, message = "Name is required" });
            }

            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                return BadRequest(new { success = false, message = "Code is required" });
            }

            var result = await Mediator.Send(new AddCostCenterCmd { AddDto = dto });

            // ✅ Invalidate cache after creating
            await _cachedService.InvalidateCostCentersCacheAsync();

            _logger.LogInformation("✅ Cost center created: {Name} (Id: {Id})", result.Name, result.Id);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Cost center created successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Validation error creating cost center");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error creating cost center");
            return HandleException(ex, "CreateCostCenter");
        }
    }

    /// <summary>
    /// Update a cost center
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(CostCenterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditCostCenterDto dto)
    {
        try
        {
            _logger.LogInformation("📝 Updating cost center: {Id} - {Name}", dto.Id, dto.Name);

            // Validate
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new { success = false, message = "Name is required" });
            }

            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                return BadRequest(new { success = false, message = "Code is required" });
            }

            var result = await Mediator.Send(new EditCostCenterCmd { EditDto = dto });

            // ✅ Invalidate cache after updating
            await _cachedService.InvalidateCostCentersCacheAsync();

            _logger.LogInformation("✅ Cost center updated: {Name} (Id: {Id})", result.Name, result.Id);

            return Ok(new
            {
                success = true,
                message = "Cost center updated successfully",
                data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "⚠️ Cost center not found: {Id}", dto.Id);
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Validation error updating cost center");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error updating cost center: {Id}", dto.Id);
            return HandleException(ex, "UpdateCostCenter", dto.Id);
        }
    }

    /// <summary>
    /// Delete a cost center
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("🗑️ Deleting cost center: {Id}", id);

            var result = await Mediator.Send(new DeleteCostCenterCmd { Id = id });

            if (!result)
            {
                _logger.LogWarning("⚠️ Cost center not found: {Id}", id);
                return NotFound(new { success = false, message = $"Cost center with ID '{id}' not found" });
            }

            // ✅ Invalidate cache after deleting
            await _cachedService.InvalidateCostCentersCacheAsync();

            _logger.LogInformation("✅ Cost center deleted: {Id}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Cannot delete cost center: {Id}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error deleting cost center: {Id}", id);
            return HandleException(ex, "DeleteCostCenter", id);
        }
    }

    /// <summary>
    /// Invalidate cost centers cache
    /// </summary>
    [HttpPost("InvalidateCache")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> InvalidateCache()
    {
        try
        {
            await _cachedService.InvalidateCostCentersCacheAsync();
            _logger.LogInformation("🗑️ Cost centers cache invalidated");

            return Ok(new
            {
                success = true,
                message = "Cost centers cache invalidated successfully",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error invalidating cache");
            return HandleException(ex, "InvalidateCache");
        }
    }
}
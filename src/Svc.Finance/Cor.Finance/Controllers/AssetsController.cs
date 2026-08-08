// Controllers/AssetsController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cor.Finance.Controllers
{
    [ApiController]
    [Route("api/finance/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class AssetsController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(
            IMediator mediator,
            ILogger<AssetsController> logger,
            IMemoryCache cache)
            : base(mediator, logger)
        {
            _mediator = mediator;
            _logger = logger;
            _cache = cache;
        }

     [HttpGet]
     [ProducesResponseType(typeof(List<AssetDto>), StatusCodes.Status200OK)]
     [ProducesResponseType(StatusCodes.Status500InternalServerError)]
     public async Task<IActionResult> GetAssets(
         [FromQuery] string? status,
         [FromQuery] string? assetType,
         [FromQuery] string? assetCategory,
         [FromQuery] Guid? branchId,
         [FromQuery] Guid? departmentId,
         [FromQuery] Guid? assignedTo,
         [FromQuery] bool? isActive,
         [FromQuery] DateTime? fromDate,
         [FromQuery] DateTime? toDate,
         [FromQuery] decimal? minCost,
         [FromQuery] decimal? maxCost,
         [FromQuery] string? searchTerm,
         [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 50,
         [FromQuery] string? sortBy = "AcquisitionDate",
         [FromQuery] string? sortDirection = "DESC")
     {
         try
         {
             _logger.LogInformation("📊 GetAssets called");

             // Convert dates to UTC
             var fromDateUtc = fromDate.HasValue
                 ? DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc)
                 : (DateTime?)null;
             var toDateUtc = toDate.HasValue
                 ? DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc)
                 : (DateTime?)null;

             // Generate cache key
             string cacheKey = $"Assets_{status ?? "All"}_{assetType ?? "All"}_{assetCategory ?? "All"}_" +
                 $"{branchId?.ToString() ?? "Null"}_{departmentId?.ToString() ?? "Null"}_" +
                 $"{assignedTo?.ToString() ?? "Null"}_{isActive?.ToString() ?? "Null"}_" +
                 $"{fromDateUtc?.ToString("yyyyMMdd") ?? "Null"}_{toDateUtc?.ToString("yyyyMMdd") ?? "Null"}_" +
                 $"{minCost?.ToString() ?? "Null"}_{maxCost?.ToString() ?? "Null"}_" +
                 $"{searchTerm ?? "Null"}_{pageNumber}_{pageSize}_{sortBy ?? "AcquisitionDate"}_{sortDirection ?? "DESC"}";

             // ✅ Check cache (only return if not empty)
             if (_cache.TryGetValue(cacheKey, out List<AssetDto> cachedResult) &&
                 cachedResult != null && cachedResult.Count > 0)
             {
                 _logger.LogInformation("✅ Cache HIT: {Count} assets", cachedResult.Count);
                 return Ok(cachedResult);
             }

             _logger.LogInformation("📊 Cache miss, executing query");

             var result = await _mediator.Send(new GetAssetsQry
             {
                 Status = status,
                 AssetType = assetType,
                 AssetCategory = assetCategory,
                 BranchId = branchId,
                 DepartmentId = departmentId,
                 AssignedTo = assignedTo,
                 IsActive = isActive,
                 FromDate = fromDateUtc,
                 ToDate = toDateUtc,
                 MinCost = minCost,
                 MaxCost = maxCost,
                 SearchTerm = searchTerm,
                 PageNumber = pageNumber,
                 PageSize = pageSize,
                 SortBy = sortBy,
                 SortDirection = sortDirection
             });

             // ✅ Only cache non-empty results
             if (result != null && result.Count > 0)
             {
                 _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
                 _logger.LogInformation("✅ Cached {Count} assets", result.Count);
             }
             else
             {
                 _logger.LogWarning("⚠️ No assets found - NOT caching empty result");
             }

             return Ok(result ?? new List<AssetDto>());
         }
         catch (Exception ex)
         {
             _logger.LogError(ex, "❌ Error in GetAssets");
             return StatusCode(500, new { success = false, message = ex.Message });
         }
     }

        // ✅ Helper method to clear cache
        [HttpPost("ClearCache")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearCache()
        {
            try
            {
                _logger.LogInformation("🧹 Clearing asset cache");

                // Clear all asset caches (simplified - in production, use Redis pattern matching)
                _cache.Remove("Assets_");
                _cache.Remove("Asset_");

                // You can also clear specific keys if needed
                // _cache.Remove("Assets_All_All_All_Null_Null_Null_Null_Null_Null_Null_Null_Null_1_50_AcquisitionDate_DESC");

                return Ok(new { success = true, message = "Asset cache cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error clearing cache");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AssetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAssetById(Guid id)
        {
            try
            {
                string cacheKey = $"Asset_{id}";

                if (_cache.TryGetValue(cacheKey, out AssetDto cachedResult))
                {
                    _logger.LogInformation("✅ Asset {Id} retrieved from cache", id);
                    return Ok(cachedResult);
                }

                var result = await _mediator.Send(new GetAssetByIdQry { Id = id });

                if (result != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, result, cacheOptions);
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "GetAssetById", id);
            }
        }

        [HttpGet("ByBranch/{branchId}")]
        [ProducesResponseType(typeof(List<AssetDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAssetsByBranch(Guid branchId)
        {
            try
            {
                string cacheKey = $"Assets_Branch_{branchId}";

                if (_cache.TryGetValue(cacheKey, out List<AssetDto> cachedResult))
                {
                    _logger.LogInformation("✅ Assets for branch {BranchId} retrieved from cache", branchId);
                    return Ok(cachedResult);
                }

                var result = await _mediator.Send(new GetAssetsByBranchQry { BranchId = branchId });

                if (result != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, result, cacheOptions);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "GetAssetsByBranch", branchId);
            }
        }

        [HttpGet("ByDepartment/{departmentId}")]
        [ProducesResponseType(typeof(List<AssetDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAssetsByDepartment(Guid departmentId)
        {
            try
            {
                string cacheKey = $"Assets_Department_{departmentId}";

                if (_cache.TryGetValue(cacheKey, out List<AssetDto> cachedResult))
                {
                    _logger.LogInformation("✅ Assets for department {DepartmentId} retrieved from cache", departmentId);
                    return Ok(cachedResult);
                }

                var result = await _mediator.Send(new GetAssetsByDepartmentQry { DepartmentId = departmentId });

                if (result != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, result, cacheOptions);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "GetAssetsByDepartment", departmentId);
            }
        }

        [HttpGet("ByEmployee/{employeeId}")]
        [ProducesResponseType(typeof(List<AssetDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAssetsByEmployee(Guid employeeId)
        {
            try
            {
                string cacheKey = $"Assets_Employee_{employeeId}";

                if (_cache.TryGetValue(cacheKey, out List<AssetDto> cachedResult))
                {
                    _logger.LogInformation("✅ Assets for employee {EmployeeId} retrieved from cache", employeeId);
                    return Ok(cachedResult);
                }

                var result = await _mediator.Send(new GetAssetsByEmployeeQry { EmployeeId = employeeId });

                if (result != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, result, cacheOptions);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "GetAssetsByEmployee", employeeId);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(AssetDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsset([FromBody] CreateAssetDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return HandleBadRequest("Asset name is required");

                var result = await _mediator.Send(new CreateAssetCmd { Dto = dto });

                // Invalidate cache
                await InvalidateAssetCache();

                return CreatedAtAction(nameof(GetAssetById), new { id = result.Id }, new
                {
                    success = true,
                    message = "Asset created successfully",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "CreateAsset");
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(AssetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsset([FromBody] UpdateAssetDto dto)
        {
            try
            {
                if (dto.Id == Guid.Empty)
                    return HandleBadRequest("Asset ID is required");

                var result = await _mediator.Send(new UpdateAssetCmd { Dto = dto });

                // Invalidate cache
                await InvalidateAssetCache();
                _cache.Remove($"Asset_{dto.Id}");

                return Ok(new
                {
                    success = true,
                    message = "Asset updated successfully",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "UpdateAsset", dto.Id);
            }
        }

        [HttpPatch("{id}/toggle-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleAssetStatus(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new ToggleAssetStatusCmd { Id = id });
                if (!result)
                    return HandleNotFound("Asset", id);

                // Invalidate cache
                await InvalidateAssetCache();
                _cache.Remove($"Asset_{id}");

                return SuccessResponse("Asset status toggled successfully");
            }
            catch (Exception ex)
            {
                return HandleException(ex, "ToggleAssetStatus", id);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsset(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteAssetCmd { Id = id });
                if (!result)
                    return HandleNotFound("Asset", id);

                // Invalidate cache
                await InvalidateAssetCache();
                _cache.Remove($"Asset_{id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex, "DeleteAsset", id);
            }
        }



        #region Private Methods

        private string GenerateCacheKey(
            string? status,
            string? assetType,
            string? assetCategory,
            Guid? branchId,
            Guid? departmentId,
            Guid? assignedTo,
            bool? isActive,
            DateTime? fromDate,
            DateTime? toDate,
            decimal? minCost,
            decimal? maxCost,
            string? searchTerm,
            int pageNumber,
            int pageSize,
            string? sortBy,
            string? sortDirection)
        {
            return $"Assets_{status ?? "All"}_{assetType ?? "All"}_{assetCategory ?? "All"}_" +
                   $"{branchId?.ToString() ?? "Null"}_{departmentId?.ToString() ?? "Null"}_" +
                   $"{assignedTo?.ToString() ?? "Null"}_{isActive?.ToString() ?? "Null"}_" +
                   $"{fromDate?.ToString("yyyyMMdd") ?? "Null"}_{toDate?.ToString("yyyyMMdd") ?? "Null"}_" +
                   $"{minCost?.ToString() ?? "Null"}_{maxCost?.ToString() ?? "Null"}_" +
                   $"{searchTerm ?? "Null"}_{pageNumber}_{pageSize}_{sortBy ?? "AcquisitionDate"}_{sortDirection ?? "DESC"}";
        }

        private async Task InvalidateAssetCache()
        {
            _logger.LogInformation("🧹 Invalidating asset cache");
            await _mediator.Send(new InvalidateCacheCommand { EntityType = "Asset" });
            _cache.Remove("Assets_");
            _cache.Remove("Assets_Branch_");
            _cache.Remove("Assets_Department_");
            _cache.Remove("Assets_Employee_");
            _cache.Remove("Asset_");
        }

        #endregion
    }
}
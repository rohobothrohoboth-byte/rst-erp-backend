using Common;
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

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class BudgetController : BaseApiController
{
     private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BudgetController> _logger;

        public BudgetController(
            IMediator mediator,
            ILogger<BudgetController> logger,
            IMemoryCache cache)
            : base(mediator, logger)
        {
            _mediator = mediator;
            _logger = logger;
            _cache = cache;
        }
    [HttpGet("All")]
        [PerAuth("fnm.gl.budget.view")]
        [ProducesResponseType(typeof(PaginatedResponse<BudgetDto>), StatusCodes.Status200OK)] // ✅ FIXED
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ResponseCache(Duration = 300, VaryByQueryKeys = new[] {
            "status", "branchId", "departmentId", "fiscalYear",
            "fromDate", "toDate", "periodId", "pageNumber", "pageSize"
        })]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status,
            [FromQuery] Guid? branchId,
            [FromQuery] Guid? departmentId,
            [FromQuery] string? fiscalYear,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] Guid? periodId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20) // ✅ Add pagination params
        {
            try
            {
                // ✅ Clamp page size
                if (pageSize > 100) pageSize = 100;
                if (pageNumber < 1) pageNumber = 1;

                // ✅ Convert dates to UTC
                var fromDateUtc = fromDate.HasValue
                    ? DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc)
                    : (DateTime?)null;
                var toDateUtc = toDate.HasValue
                    ? DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc)
                    : (DateTime?)null;

                string cacheKey = GenerateCacheKey(status, branchId, departmentId, fiscalYear,
                    fromDateUtc, toDateUtc, periodId, pageNumber, pageSize);

                if (_cache.TryGetValue(cacheKey, out PaginatedResponse<BudgetDto> cachedResult))
                {
                    _logger.LogInformation("✅ Budgets retrieved from cache for key: {CacheKey}", cacheKey);
                    return Ok(cachedResult);
                }

                _logger.LogInformation("📊 Cache miss, executing query for key: {CacheKey}", cacheKey);

                var result = await _mediator.Send(new GetAllBudgetsQry
                {
                    Status = status,
                    BranchId = branchId,
                    DepartmentId = departmentId,
                    FiscalYear = fiscalYear,
                    FromDate = fromDateUtc,
                    ToDate = toDateUtc,
                    PeriodId = periodId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

                if (result != null && result.Items.Count > 0)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                        .SetPriority(CacheItemPriority.Normal);

                    _cache.Set(cacheKey, result, cacheOptions);
                    _logger.LogInformation("✅ Retrieved {Count} budgets and cached", result.Items.Count);
                }
                else
                {
                    _logger.LogWarning("⚠️ Query returned 0 budgets - NOT caching empty result");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllBudgets for BudgetController");
                return StatusCode(500, new {
                    success = false,
                    message = "An error occurred while retrieving budgets",
                    error = ex.Message
                });
            }
        }

        // ✅ GET BY ID - same as before (returns single BudgetDto)
        [HttpGet("{id}")]
        [PerAuth("fnm.gl.budget.view")]
        [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                string cacheKey = $"Budget_{id}";

                if (_cache.TryGetValue(cacheKey, out BudgetDto cachedResult))
                {
                    _logger.LogInformation("✅ Budget {Id} retrieved from cache", id);
                    return Ok(cachedResult);
                }

                var result = await _mediator.Send(new GetBudgetByIdQry { Id = id });

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
                return HandleException(ex, "GetBudgetById", id);
            }
        }

        // ✅ GET BY BRANCH - returns PaginatedResponse
        [HttpGet("ByBranch/{branchId}")]
        [PerAuth("fnm.gl.budget.view")]
        [ProducesResponseType(typeof(PaginatedResponse<BudgetDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByBranch(
            Guid branchId,
            [FromQuery] Guid? periodId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (pageSize > 100) pageSize = 100;
                if (pageNumber < 1) pageNumber = 1;

                string cacheKey = $"Budgets_Branch_{branchId}_{periodId?.ToString() ?? "All"}_{pageNumber}_{pageSize}";

                if (_cache.TryGetValue(cacheKey, out PaginatedResponse<BudgetDto> cachedResult))
                {
                    _logger.LogInformation("✅ Budgets for branch {BranchId} retrieved from cache", branchId);
                    return Ok(cachedResult);
                }

                var result = await _mediator.Send(new GetBudgetsByBranchQry
                {
                    BranchId = branchId,
                    PeriodId = periodId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

               if (result != null && result.Count > 0)
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
                return HandleException(ex, "GetBudgetsByBranch", branchId);
            }
        }

    [HttpGet("ByPeriod")]
    [PerAuth("fnm.gl.budget.view")]
    [ProducesResponseType(typeof(List<BudgetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPeriod([FromQuery] Guid periodId)
    {
        try
        {
            string cacheKey = $"Budgets_Period_{periodId}";

            if (_cache.TryGetValue(cacheKey, out List<BudgetDto> cachedResult))
            {
                _logger.LogInformation("? Budgets for period {PeriodId} retrieved from cache", periodId);
                return Ok(cachedResult);
            }

            var result = await _mediator.Send(new GetBudgetsByPeriodQry { PeriodId = periodId });

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
            return HandleException(ex, "GetBudgetsByPeriod");
        }
    }

    [HttpPost]
    [PerAuth("fnm.gl.budget.add")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddBudgetDto dto)
    {
        try
        {
            if (dto.PeriodId == Guid.Empty)
                return HandleBadRequest("PeriodId is required");

            var result = await _mediator.Send(new AddBudgetCmd { AddDto = dto });

            // Invalidate cache after creating
            await InvalidateBudgetCache();

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Budget created successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateBudget");
        }
    }

    [HttpPut]
    [PerAuth("fnm.gl.budget.mod")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditBudgetDto dto)
    {
        try
        {
            if (dto.PeriodId == Guid.Empty)
                return HandleBadRequest("PeriodId is required");

            var result = await _mediator.Send(new EditBudgetCmd { EditDto = dto });

            // Invalidate cache after updating
            await InvalidateBudgetCache();
            // Also remove individual budget cache
            _cache.Remove($"Budget_{dto.Id}");

            return Ok(new { success = true, message = "Budget updated successfully", data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateBudget", dto.Id);
        }
    }

    [HttpPatch("{id}/toggle-status")]
    [PerAuth("fnm.gl.budget.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new ToggleBudgetStatusCmd { Id = id });
            if (!result)
                return HandleNotFound("Budget", id);

            // Invalidate cache after status change
            await InvalidateBudgetCache();
            _cache.Remove($"Budget_{id}");

            return SuccessResponse("Budget status toggled successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ToggleBudgetStatus", id);
        }
    }

    [HttpDelete("{id}")]
    [PerAuth("fnm.gl.budget.del")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteBudgetCmd { Id = id });
            if (!result)
                return HandleNotFound("Budget", id);

            // Invalidate cache after deleting
            await InvalidateBudgetCache();
            _cache.Remove($"Budget_{id}");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteBudget", id);
        }
    }

    [HttpPost("ClearCache")]
    [PerAuth("fnm.gl.budget.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearCache()
    {
        try
        {
            _logger.LogInformation("?? Clearing budget cache");
            await _mediator.Send(new InvalidateCacheCommand { EntityType = "Budget" });
            return Ok(new { success = true, message = "Budget cache cleared successfully" });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ClearCache");
        }
    }

      #region Private Methods

        private string GenerateCacheKey(
            string? status,
            Guid? branchId,
            Guid? departmentId,
            string? fiscalYear,
            DateTime? fromDate,
            DateTime? toDate,
            Guid? periodId,
            int pageNumber,
            int pageSize)
        {
            return $"Budgets_{status ?? "All"}_{branchId?.ToString() ?? "Null"}_{departmentId?.ToString() ?? "Null"}_{fiscalYear ?? "All"}_{fromDate?.ToString("yyyyMMdd") ?? "Null"}_{toDate?.ToString("yyyyMMdd") ?? "Null"}_{periodId?.ToString() ?? "Null"}_{pageNumber}_{pageSize}";
        }

        private async Task InvalidateBudgetCache()
        {
            _logger.LogInformation("🧹 Invalidating budget cache");
            await _mediator.Send(new InvalidateCacheCommand { EntityType = "Budget" });
            _cache.Remove("Budgets_");
            _cache.Remove("Budgets_Branch_");
            _cache.Remove("Budgets_Period_");
        }

        #endregion
    }
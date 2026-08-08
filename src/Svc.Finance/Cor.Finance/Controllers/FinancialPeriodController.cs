using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Models.Enums;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Shared.Helpers.Services;
using Cor.Finance.Persistence;
using Microsoft.EntityFrameworkCore;  // ✅ Required for SumAsync, CountAsync
using System;  // ✅ Required for nullable types
using System.Linq;  // ✅ Required for LINQ
namespace Cor.Finance.Controllers;

[ApiController]
[Authorize]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class PeriodClosingController : BaseApiController
{
    private readonly CachedReferenceDataService _cachedService;
    private readonly ILogger<PeriodClosingController> _logger;

    private readonly FinanceDbContext _context;
    public PeriodClosingController(
        IMediator mediator,
        ILogger<PeriodClosingController> logger,FinanceDbContext context,
        CachedReferenceDataService cachedService)
        : base(mediator, logger)
    {
        _cachedService = cachedService;
        _logger = logger;
        _context = context;
    }

    // ============================================================
    // GET ENDPOINTS
    // ============================================================

    /// <summary>
    /// Get all financial periods (CACHED - No Pagination)
    /// </summary>
    // PeriodClosingController.cs - GetAll endpoint

    [HttpGet("All")]
    [ProducesResponseType(typeof(List<FinancialPeriodDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "isClosed", "periodType", "search" })]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isClosed = null,
        [FromQuery] string? periodType = null,
        [FromQuery] string? search = null)
    {
        try
        {
            _logger.LogInformation("🔍 GetAll called with filters: isClosed={IsClosed}, periodType={PeriodType}, search={Search}",
                isClosed, periodType, search);

            // ✅ Use cached service
            var periods = await _cachedService.GetCachedFinancialPeriodsAsync();

            _logger.LogInformation($"📊 Retrieved {periods.Count} periods from cache before filtering");

            // Apply filters
            if (isClosed.HasValue)
            {
                periods = periods.Where(x => x.IsClosed == isClosed.Value).ToList();
                _logger.LogInformation($"📊 After IsClosed filter: {periods.Count} periods");
            }

            if (!string.IsNullOrEmpty(periodType))
            {
                periods = periods.Where(x => x.PeriodType == periodType).ToList();
                _logger.LogInformation($"📊 After PeriodType filter: {periods.Count} periods");
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                periods = periods.Where(x =>
                    (x.Name?.ToLower().Contains(searchLower) ?? false) ||
                    (x.Notes?.ToLower().Contains(searchLower) ?? false)
                ).ToList();
                _logger.LogInformation($"📊 After Search filter: {periods.Count} periods");
            }

            _logger.LogInformation("✅ Retrieved {Count} periods from cache", periods.Count);

            // ✅ Return with success response
            return SuccessResponse(periods);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in GetAll");
            return HandleException(ex, nameof(GetAll));
        }
    }

// PeriodClosingController.cs

[HttpGet("debug/periods")]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> DebugPeriods()
{
    try
    {
        // Get all periods (including deleted)
        var all = await _context.FinancialPeriods.ToListAsync();

        // Get active periods
        var active = await _context.FinancialPeriods
            .Where(x => !x.IsDeleted)
            .ToListAsync();

        return Ok(new
        {
            allCount = all.Count,
            activeCount = active.Count,
            allPeriods = all.Select(p => new { p.Id, p.Name, p.StartDate, p.EndDate, p.IsClosed, p.IsDeleted }),
            activePeriods = active.Select(p => new { p.Id, p.Name, p.StartDate, p.EndDate, p.IsClosed })
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}

[HttpPost("clear-cache")]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> ClearCache()
{
    try
    {
        await _cachedService.InvalidatePeriodsCacheAsync();
        _logger.LogInformation("🗑️ Period cache cleared manually");

        // Also clear the specific cache key directly
        await _cachedService.InvalidateReferenceDataAsync();

        return Ok(new { success = true, message = "Cache cleared successfully" });
    }
    catch (Exception ex)
    {
        return BadRequest(new { success = false, error = ex.Message });
    }
}

    /// <summary>
    /// Get all financial periods with pagination and filtering (CACHED)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<FinancialPeriodDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "search", "periodType", "isClosed", "status", "fromDate", "toDate", "page", "pageSize", "sortBy", "sortOrder" })]
    public async Task<IActionResult> GetPeriods(
        [FromQuery] string? search = null,
        [FromQuery] string? periodType = null,
        [FromQuery] bool? isClosed = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "StartDate",
        [FromQuery] string? sortOrder = "DESC")
    {
        try
        {
            // ✅ Use cached service
            var periods = await _cachedService.GetCachedFinancialPeriodsAsync();

            // Apply filters
            if (isClosed.HasValue)
            {
                periods = periods.Where(x => x.IsClosed == isClosed.Value).ToList();
            }

            if (!string.IsNullOrEmpty(periodType))
            {
                periods = periods.Where(x => x.PeriodType == periodType).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                periods = periods.Where(x =>
                    (x.Status ?? "").Equals(status, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                periods = periods.Where(x =>
                    (x.Name?.ToLower().Contains(searchLower) ?? false) ||
                    (x.Notes?.ToLower().Contains(searchLower) ?? false)
                ).ToList();
            }

            // Apply sorting
            periods = sortBy?.ToLower() switch
            {
                "name" => sortOrder?.ToUpper() == "DESC"
                    ? periods.OrderByDescending(x => x.Name).ToList()
                    : periods.OrderBy(x => x.Name).ToList(),
                "startdate" => sortOrder?.ToUpper() == "DESC"
                    ? periods.OrderByDescending(x => x.StartDate).ToList()
                    : periods.OrderBy(x => x.StartDate).ToList(),
                "enddate" => sortOrder?.ToUpper() == "DESC"
                    ? periods.OrderByDescending(x => x.EndDate).ToList()
                    : periods.OrderBy(x => x.EndDate).ToList(),
                _ => sortOrder?.ToUpper() == "DESC"
                    ? periods.OrderByDescending(x => x.StartDate).ToList()
                    : periods.OrderBy(x => x.StartDate).ToList()
            };

            var totalCount = periods.Count;
            var pagedPeriods = periods
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            _logger.LogInformation("✅ Retrieved {Count} periods from cache (Page {Page}/{TotalPages})",
                pagedPeriods.Count, page, (int)Math.Ceiling((double)totalCount / pageSize));

            return PaginatedResponse(
                pagedPeriods,
                totalCount,
                page,
                pageSize
            );
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetPeriods));
        }
    }

    /// <summary>
    /// Get financial period by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FinancialPeriodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            // ✅ Try from cache first
            var periods = await _cachedService.GetCachedFinancialPeriodsAsync();
            var result = periods.FirstOrDefault(x => x.Id == id);

            if (result == null)
            {
                // Fallback to database if not in cache
                result = await Mediator.Send(new GetFinancialPeriodByIdQry { Id = id });
            }

            return SuccessResponse(result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return HandleNotFound("Financial Period", id);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById), id);
        }
    }

    /// <summary>
    /// Get active financial period for a specific date (CACHED)
    /// </summary>
    [HttpGet("Active")]
    [ProducesResponseType(typeof(FinancialPeriodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "date" })]
    public async Task<IActionResult> GetActive([FromQuery] DateTime? date = null)
    {
        try
        {
            var periods = await _cachedService.GetCachedFinancialPeriodsAsync();
            var targetDate = date ?? DateTime.UtcNow.Date;

            var result = periods.FirstOrDefault(x =>
                x.StartDate <= targetDate && x.EndDate >= targetDate && !x.IsClosed
            );

            if (result == null)
            {
                var dateStr = date?.ToString("yyyy-MM-dd") ?? "today";
                return NotFound(new
                {
                    success = false,
                    message = $"No active financial period found for {dateStr}",
                    date = dateStr
                });
            }

            return SuccessResponse(result, "Active period found");
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetActive));
        }
    }

    /// <summary>
    /// Get active financial period with statistics (CACHED)
    /// </summary>
    [HttpGet("ActiveWithStats")]
    [ProducesResponseType(typeof(ActivePeriodWithStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "date" })]
    public async Task<IActionResult> GetActiveWithStats([FromQuery] DateTime? date = null)
    {
        try
        {
            var periods = await _cachedService.GetCachedFinancialPeriodsAsync();
            var targetDate = date ?? DateTime.UtcNow.Date;

            var period = periods.FirstOrDefault(x =>
                x.StartDate <= targetDate && x.EndDate >= targetDate && !x.IsClosed
            );

            if (period == null)
            {
                var dateStr = date?.ToString("yyyy-MM-dd") ?? "today";
                return NotFound(new
                {
                    success = false,
                    message = $"No active financial period found for {dateStr}",
                    date = dateStr
                });
            }

            // Get stats from database (not cached since it changes frequently)
            var stats = await Mediator.Send(new GetPeriodStatsQry { PeriodId = period.Id });

            var result = new ActivePeriodWithStatsDto
            {
                Period = period,
                Stats = stats
            };

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetActiveWithStats));
        }
    }

    /// <summary>
    /// Get period statistics
    /// </summary>
    [HttpGet("{id:guid}/stats")]
    [ProducesResponseType(typeof(PeriodStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStats(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new GetPeriodStatsQry { PeriodId = id });
            return SuccessResponse(result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return HandleNotFound("Financial Period", id);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetStats), id);
        }
    }

   // In PeriodClosingController.cs


 /// <summary>
 /// Get summary of all periods in a fiscal year (CACHED)
 /// </summary>
 [HttpGet("year-summary/{year}")]
 [ProducesResponseType(typeof(YearPeriodSummaryDto), StatusCodes.Status200OK)]
 [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
 public async Task<IActionResult> GetYearSummary(int year)
 {
     try
     {
         var periods = await _cachedService.GetCachedFinancialPeriodsAsync();

         var yearPeriods = periods
             .Where(x => x.StartDate.Year == year || x.EndDate.Year == year)
             .ToList();

         var periodIds = yearPeriods.Select(p => p.Id).ToList();

         // ✅ FIX: Use a separate query for financial summary
         var financialSummary = new YearFinancialSummaryDto();

         if (periodIds.Any())
         {
             var totalDebit = await _context.JournalEntries
                 .Where(j => !j.IsDeleted && periodIds.Contains(j.PeriodId ?? Guid.Empty))
                 .SumAsync(j => (decimal?)j.TotalDebit) ?? 0m;

             var totalCredit = await _context.JournalEntries
                 .Where(j => !j.IsDeleted && periodIds.Contains(j.PeriodId ?? Guid.Empty))
                 .SumAsync(j => (decimal?)j.TotalCredit) ?? 0m;

             var totalTransactions = await _context.JournalEntries
                 .Where(j => !j.IsDeleted && periodIds.Contains(j.PeriodId ?? Guid.Empty))
                 .CountAsync();

             financialSummary = new YearFinancialSummaryDto
             {
                 TotalDebit = totalDebit,
                 TotalCredit = totalCredit,
                 NetBalance = totalDebit - totalCredit,
                 TotalTransactions = totalTransactions
             };
         }

         var summary = new YearPeriodSummaryDto
         {
             Year = year,
             TotalPeriods = yearPeriods.Count,
             ClosedPeriods = yearPeriods.Count(x => x.IsClosed),
             OpenPeriods = yearPeriods.Count(x => !x.IsClosed),
             Periods = yearPeriods,
             FirstPeriodStart = yearPeriods.Any() ? yearPeriods.Min(x => x.StartDate) : null,
             LastPeriodEnd = yearPeriods.Any() ? yearPeriods.Max(x => x.EndDate) : null,
             FinancialSummary = financialSummary
         };

         _logger.LogInformation("✅ Retrieved year summary for {Year}: {Count} periods, {Transactions} transactions",
             year, yearPeriods.Count, financialSummary.TotalTransactions);
         return SuccessResponse(summary);
     }
     catch (Exception ex)
     {
         return HandleException(ex, nameof(GetYearSummary));
     }
 }

    /// <summary>
    /// Compare two periods
    /// </summary>
    [HttpGet("compare")]
    [ProducesResponseType(typeof(PeriodComparisonDto), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "period1Id", "period2Id" })]
    public async Task<IActionResult> ComparePeriods(
        [FromQuery] Guid period1Id,
        [FromQuery] Guid period2Id)
    {
        try
        {
            var result = await Mediator.Send(new ComparePeriodsQry
            {
                Period1Id = period1Id,
                Period2Id = period2Id
            });
            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(ComparePeriods));
        }
    }

    /// <summary>
    /// Validate if period can be closed
    /// </summary>
    [HttpGet("{id:guid}/validate-close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateClose(Guid id)
    {
        try
        {
            var (canClose, reason) = await Mediator.Send(new ValidatePeriodCloseQry { PeriodId = id });

            return SuccessResponse(new
            {
                canClose = canClose,
                reason = reason,
                periodId = id
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return HandleNotFound("Financial Period", id);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(ValidateClose), id);
        }
    }

    /// <summary>
    /// Get audit trail for a period
    /// </summary>
    [HttpGet("{id:guid}/audit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAuditTrail(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var result = await Mediator.Send(new GetPeriodAuditQry
            {
                PeriodId = id,
                Page = page,
                PageSize = pageSize
            });

            return PaginatedResponse(result, result.Count, page, pageSize);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetAuditTrail), id);
        }
    }

    // ============================================================
    // POST ENDPOINTS
    // ============================================================

    /// <summary>
    /// Create a new financial period
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FinancialPeriodDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] AddFinancialPeriodDto dto)
    {
        try
        {
            if (!IsValidDateRange(dto.StartDate, dto.EndDate))
            {
                return HandleBadRequest("Start date must be before end date");
            }

            if (!Enum.TryParse<PeriodType>(dto.PeriodType, true, out _))
            {
                return HandleBadRequest($"Invalid PeriodType: {dto.PeriodType}. Valid values: Monthly, Quarterly, Yearly, Custom");
            }

            var result = await Mediator.Send(new AddFinancialPeriodCmd { AddDto = dto });

            // ✅ Invalidate cache after creating
            await _cachedService.InvalidatePeriodsCacheAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                new
                {
                    success = true,
                    message = "Financial period created successfully",
                    data = result
                }
            );
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("overlaps"))
        {
            return Conflict(new
            {
                success = false,
                message = ex.Message,
                conflictType = "OverlappingPeriod"
            });
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    /// <summary>
    /// Close a financial period
    /// </summary>
    [HttpPost("{id:guid}/close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Close(Guid id, [FromBody] ClosePeriodDto? dto = null)
    {
        try
        {
            var (canClose, reason) = await Mediator.Send(new ValidatePeriodCloseQry { PeriodId = id });

            if (!canClose && !(dto?.ForceClose ?? false))
            {
                return BadRequest(new
                {
                    success = false,
                    message = reason ?? "Period cannot be closed",
                    canForceClose = true,
                    hint = "Use ForceClose=true to override"
                });
            }

            var result = await Mediator.Send(new CloseFinancialPeriodCmd
            {
                Id = id,
                CloseDto = dto ?? new ClosePeriodDto()
            });

            if (!result)
                return HandleNotFound("Financial Period", id);

            // ✅ Invalidate cache after closing
            await _cachedService.InvalidatePeriodsCacheAsync();

            return SuccessResponse("Financial period closed successfully");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already closed"))
        {
            return Conflict(new
            {
                success = false,
                message = ex.Message,
                periodId = id,
                isClosed = true
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return HandleNotFound("Financial Period", id);
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Close), id);
        }
    }

    /// <summary>
    /// Open (reopen) a financial period
    /// </summary>
    [HttpPost("{id:guid}/open")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Open(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new OpenFinancialPeriodCmd { Id = id });

            if (!result)
                return HandleNotFound("Financial Period", id);

            // ✅ Invalidate cache after opening
            await _cachedService.InvalidatePeriodsCacheAsync();

            return SuccessResponse("Financial period opened successfully");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already open"))
        {
            return Conflict(new
            {
                success = false,
                message = ex.Message,
                periodId = id,
                isOpen = true
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return HandleNotFound("Financial Period", id);
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Open), id);
        }
    }

    /// <summary>
    /// Lock a period (prevents any changes - more restrictive than close)
    /// </summary>
    [HttpPost("{id:guid}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LockPeriod(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new LockFinancialPeriodCmd { Id = id });

            if (!result)
                return HandleNotFound("Financial Period", id);

            // ✅ Invalidate cache after locking
            await _cachedService.InvalidatePeriodsCacheAsync();

            return SuccessResponse("Financial period locked successfully");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return HandleNotFound("Financial Period", id);
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(LockPeriod), id);
        }
    }

    /// <summary>
    /// Unlock a period
    /// </summary>
    [HttpPost("{id:guid}/unlock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UnlockPeriod(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new UnlockFinancialPeriodCmd { Id = id });

            if (!result)
                return HandleNotFound("Financial Period", id);

            // ✅ Invalidate cache after unlocking
            await _cachedService.InvalidatePeriodsCacheAsync();

            return SuccessResponse("Financial period unlocked successfully");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return HandleNotFound("Financial Period", id);
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(UnlockPeriod), id);
        }
    }

    /// <summary>
    /// Perform year-end closing (creates new fiscal year)
    /// </summary>
    [HttpPost("year-end")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin,FinanceManager")]
    public async Task<IActionResult> YearEndClose([FromBody] YearEndCloseDto dto)
    {
        try
        {
            var result = await Mediator.Send(new YearEndCloseCmd { Dto = dto });

            // ✅ Invalidate cache after year-end close
            await _cachedService.InvalidatePeriodsCacheAsync();

            return SuccessResponse(result, "Year-end closing completed successfully");
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(YearEndClose));
        }
    }

    /// <summary>
    /// Close multiple periods at once
    /// </summary>
    [HttpPost("bulk-close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkClose([FromBody] BulkPeriodCloseDto dto)
    {
        try
        {
            var result = await Mediator.Send(new BulkClosePeriodsCmd { Dto = dto });

            // ✅ Invalidate cache after bulk close
            await _cachedService.InvalidatePeriodsCacheAsync();

            return SuccessResponse(result, $"{result.ClosedCount} periods closed successfully");
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(BulkClose));
        }
    }

    /// <summary>
    /// Open multiple periods at once
    /// </summary>
    [HttpPost("bulk-open")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkOpen([FromBody] BulkPeriodOpenDto dto)
    {
        try
        {
            var result = await Mediator.Send(new BulkOpenPeriodsCmd { Dto = dto });

            // ✅ Invalidate cache after bulk open
            await _cachedService.InvalidatePeriodsCacheAsync();

            return SuccessResponse(result, $"{result.OpenedCount} periods opened successfully");
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(BulkOpen));
        }
    }

    /// <summary>
    /// Configure auto-close schedule
    /// </summary>
    [HttpPost("auto-close/config")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin,FinanceManager")]
    public async Task<IActionResult> ConfigureAutoClose([FromBody] AutoCloseConfigDto dto)
    {
        try
        {
            var result = await Mediator.Send(new ConfigureAutoCloseCmd { Dto = dto });
            return SuccessResponse(result, "Auto-close configuration saved successfully");
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(ConfigureAutoClose));
        }
    }

    /// <summary>
    /// Reverse a closed period (admin only)
    /// </summary>
    [HttpPost("{id:guid}/reverse")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReversePeriod(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new ReversePeriodCmd { Id = id });

            // ✅ Invalidate cache after reversing
            await _cachedService.InvalidatePeriodsCacheAsync();

            return SuccessResponse(result, "Period reversed successfully");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return HandleNotFound("Financial Period", id);
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(ReversePeriod), id);
        }
    }

    /// <summary>
    /// Transfer balances between periods
    /// </summary>
    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TransferBalance([FromBody] PeriodTransferDto dto)
    {
        try
        {
            var result = await Mediator.Send(new TransferPeriodBalanceCmd { Dto = dto });

            // ✅ Invalidate cache after transfer
            await _cachedService.InvalidatePeriodsCacheAsync();

            return SuccessResponse(result, "Balance transferred successfully");
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(TransferBalance));
        }
    }

    // ============================================================
    // PUT ENDPOINTS
    // ============================================================

    /// <summary>
    /// Update a financial period
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(FinancialPeriodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromBody] EditFinancialPeriodDto dto)
    {
        try
        {
            if (!IsValidDateRange(dto.StartDate, dto.EndDate))
            {
                return HandleBadRequest("Start date must be before end date");
            }

            if (!Enum.TryParse<PeriodType>(dto.PeriodType, true, out _))
            {
                return HandleBadRequest($"Invalid PeriodType: {dto.PeriodType}. Valid values: Monthly, Quarterly, Yearly, Custom");
            }

            var result = await Mediator.Send(new EditFinancialPeriodCmd { EditDto = dto });

            // ✅ Invalidate cache after updating
            await _cachedService.InvalidatePeriodsCacheAsync();

            return SuccessResponse(result, "Financial period updated successfully");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return HandleNotFound("Financial Period", dto.Id);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("closed"))
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                periodId = dto.Id,
                isClosed = true
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("overlaps"))
        {
            return Conflict(new
            {
                success = false,
                message = ex.Message,
                conflictType = "OverlappingPeriod"
            });
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Update), dto.Id);
        }
    }

    // ============================================================
    // DELETE ENDPOINTS
    // ============================================================

    /// <summary>
    /// Delete a financial period
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new DeleteFinancialPeriodCmd { Id = id });

            if (!result)
                return HandleNotFound("Financial Period", id);

            // ✅ Invalidate cache after deleting
            await _cachedService.InvalidatePeriodsCacheAsync();

            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("closed"))
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                periodId = id,
                isClosed = true,
                hint = "Cannot delete a closed period"
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("journal entries"))
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                periodId = id,
                hint = "Cannot delete a period with existing journal entries"
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return HandleNotFound("Financial Period", id);
        }
        catch (InvalidOperationException ex)
        {
            return HandleBadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete), id);
        }
    }

    /// <summary>
    /// Export period data
    /// </summary>
    [HttpGet("{id:guid}/export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Export(Guid id)
    {
        try
        {
            var period = await Mediator.Send(new GetFinancialPeriodByIdQry { Id = id });
            var stats = await Mediator.Send(new GetPeriodStatsQry { PeriodId = id });

            var exportData = new
            {
                period = new
                {
                    period.Id,
                    period.Name,
                    StartDate = period.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = period.EndDate.ToString("yyyy-MM-dd"),
                    IsClosed = period.IsClosed,
                    PeriodType = period.PeriodType,
                    Status = period.Status,
                    ClosedDate = period.ClosedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A",
                    CreatedAt = period.DateAdd.ToString("yyyy-MM-dd HH:mm:ss"),
                    LastModified = period.DateMod?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A",
                    period.Notes
                },
                summary = new
                {
                    TotalJournalEntries = stats.TotalJournalEntries,
                    PostedEntries = stats.PostedEntries,
                    UnpostedEntries = stats.UnpostedEntries,
                    TotalTransactions = stats.TotalTransactions,
                    TotalDebit = stats.TotalDebit.ToString("F2"),
                    TotalCredit = stats.TotalCredit.ToString("F2"),
                    NetBalance = stats.NetBalance.ToString("F2"),
                    PeriodStart = stats.PeriodStart.ToString("yyyy-MM-dd"),
                    PeriodEnd = stats.PeriodEnd.ToString("yyyy-MM-dd"),
                    DaysRemaining = stats.DaysRemaining,
                    CompletionPercentage = stats.CompletionPercentage.ToString("F1") + "%",
                    CanBeClosed = stats.CanBeClosed ? "Yes" : "No",
                    ClosingReason = stats.ClosingReason ?? "None"
                },
                exportedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                exportedBy = User?.Identity?.Name ?? "System"
            };

            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = System.Text.Json.JsonSerializer.Serialize(exportData, jsonOptions);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var fileName = $"period-{period.Name}-{DateTime.UtcNow:yyyyMMddHHmmss}.json";

            return File(bytes, "application/json; charset=utf-8", fileName);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return HandleNotFound("Financial Period", id);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Export), id);
        }
    }

    // ============================================================
    // HELPER METHODS
    // ============================================================

    private bool IsValidDateRange(DateTime start, DateTime end)
    {
        return start < end;
    }
}

public class PaginatedResponse<T>
{
    public bool Success { get; set; } = true;
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public PaginationInfo Pagination { get; set; } = new();
}

public class PaginationInfo
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
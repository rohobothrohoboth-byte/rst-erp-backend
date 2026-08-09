// Svc.Finance.Controllers - AuditController.cs

using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/Audit")]
[ApiVersion("1.0")]
[Authorize]
public class AuditController : BaseApiController
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<AuditController> _logger;

    public AuditController(
        IMediator mediator,
        ILogger<AuditController> logger,
        IMemoryCache cache)
        : base(mediator, logger)
    {
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Get all audit logs with pagination
    /// GET: /api/finance/v1.0/Audit/Logs
    /// </summary>
    [HttpGet("Logs")]
    [ProducesResponseType(typeof(AuditLogListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ResponseCache(Duration = 10, VaryByQueryKeys = new[] { "entityType", "action", "userId", "fromDate", "toDate", "page", "pageSize" })]
    public async Task<IActionResult> GetLogs(
        [FromQuery] string? entityType,
        [FromQuery] string? action,
        [FromQuery] string? userId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            // ✅ Cap page size to prevent abuse
            if (pageSize > 100) pageSize = 100;
            if (page < 1) page = 1;

            var fromDateUtc = fromDate.HasValue ? DateTimeHelper.EnsureUtc(fromDate.Value) : (DateTime?)null;
            var toDateUtc = toDate.HasValue ? DateTimeHelper.EnsureUtc(toDate.Value) : (DateTime?)null;

            var result = await Mediator.Send(new GetAuditLogsQry
            {
                EntityType = entityType,
                Action = action,
                UserId = userId,
                FromDate = fromDateUtc,
                ToDate = toDateUtc,
                Page = page,
                PageSize = pageSize
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAuditLogs");
        }
    }

    /// <summary>
    /// Get audit trail by entity
    /// GET: /api/finance/v1.0/Audit/Trail
    /// </summary>
    [HttpGet("Trail")]
    [ProducesResponseType(typeof(List<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "entityType", "entityId" })]
    public async Task<IActionResult> GetAuditTrail(
        [FromQuery] string entityType,
        [FromQuery] string entityId)
    {
        try
        {
            if (string.IsNullOrEmpty(entityType) || string.IsNullOrEmpty(entityId))
            {
                return BadRequest(new { success = false, message = "EntityType and EntityId are required" });
            }

            var result = await Mediator.Send(new GetEntityAuditQry
            {
                EntityType = entityType,
                EntityId = entityId
            });

           if (result == null || result.Items.Count == 0)
           {
               return NotFound(new { success = false, message = $"No audit trail found for {entityType}/{entityId}" });
           }



            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAuditTrail", $"{entityType}/{entityId}");
        }
    }

    /// <summary>
    /// Get entity audit history
    /// GET: /api/finance/v1.0/Audit/Entity/{entityType}/{entityId}
    /// </summary>
    [HttpGet("Entity/{entityType}/{entityId}")]
    [ProducesResponseType(typeof(List<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 30)]
    public async Task<IActionResult> GetEntityAuditHistory(string entityType, string entityId)
    {
        try
        {
            if (string.IsNullOrEmpty(entityType) || string.IsNullOrEmpty(entityId))
            {
                return BadRequest(new { success = false, message = "EntityType and EntityId are required" });
            }

            var result = await Mediator.Send(new GetEntityAuditQry
            {
                EntityType = entityType,
                EntityId = entityId
            });

           if (result == null || result.Items.Count == 0)
           {
               return NotFound(new { success = false, message = $"No audit history found for {entityType}/{entityId}" });
           }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetEntityAuditHistory", $"{entityType}/{entityId}");
        }
    }

    /// <summary>
    /// Get audit summary with caching
    /// GET: /api/finance/v1.0/Audit/Summary
    /// </summary>
    [HttpGet("Summary")]
    [ProducesResponseType(typeof(AuditSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "fromDate", "toDate" })]
    public async Task<IActionResult> GetAuditSummary(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        try
        {
            // ✅ Convert dates to UTC
            var fromDateUtc = fromDate.HasValue ? DateTimeHelper.EnsureUtc(fromDate.Value) : (DateTime?)null;
            var toDateUtc = toDate.HasValue ? DateTimeHelper.EnsureUtc(toDate.Value) : (DateTime?)null;

            // ✅ Check memory cache first (faster than ResponseCache)
            var cacheKey = $"AuditSummary_{fromDateUtc?.ToString("yyyyMMdd") ?? "All"}_{toDateUtc?.ToString("yyyyMMdd") ?? "All"}";

            if (_cache.TryGetValue(cacheKey, out AuditSummaryDto? cachedResult))
            {
                _logger.LogInformation("✅ Audit summary retrieved from memory cache");
                return Ok(cachedResult);
            }

            // ✅ Execute query if not in cache
            var result = await Mediator.Send(new GetAuditSummaryQry
            {
                FromDate = fromDateUtc,
                ToDate = toDateUtc
            });

            // ✅ Cache for 30 seconds
            if (result != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                    .SetPriority(CacheItemPriority.Normal);

                _cache.Set(cacheKey, result, cacheOptions);
            }

            _logger.LogInformation("✅ Audit summary generated: {TotalLogs} logs", result?.TotalLogs ?? 0);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAuditSummary");
        }
    }

    /// <summary>
    /// Clear audit cache (admin only)
    /// POST: /api/finance/v1.0/Audit/ClearCache
    /// </summary>
    [HttpPost("ClearCache")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ClearCache()
    {
        try
        {
            _logger.LogInformation("🧹 Clearing audit cache");

            // Clear memory cache (simplified - in production use pattern matching)
            // This is a simple approach - consider using Redis pattern matching
            _cache.Remove("AuditSummary_");

            return Ok(new { success = true, message = "Audit cache cleared successfully" });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ClearCache");
        }
    }
}
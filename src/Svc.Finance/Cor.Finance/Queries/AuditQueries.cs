using MediatR;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;
using Microsoft.Extensions.Logging;
using Cor.Finance.Models.Enums;
namespace Cor.Finance.Queries;

public class GetAuditLogsQry : IRequest<AuditLogListResponse>
{
    public string? EntityType { get; set; }
    public string? Action { get; set; }
    public string? UserId { get; set; }
    public string? EntityId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}



public class GetEntityAuditQry : IRequest<AuditLogListResponse>
{
    public string EntityType { get; set; } = string.Empty;
    public string EntityId   { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class GetAuditSummaryQry : IRequest<AuditSummaryDto>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

// ==================== HANDLERS ====================

/// <summary>
/// ✅ OPTIMIZED: GetAuditLogsHandler with pagination and projection
/// </summary>
public class GetAuditLogsHandler : IRequestHandler<GetAuditLogsQry, AuditLogListResponse>
{
    private readonly FinanceDbContext _context;

    public GetAuditLogsHandler(FinanceDbContext context) => _context = context;

    public async Task<AuditLogListResponse> Handle(GetAuditLogsQry request, CancellationToken ct)
    {
        var page     = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        var query = _context.AuditLogs
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrEmpty(request.EntityType))
            query = query.Where(x => x.EntityType == request.EntityType);

        if (!string.IsNullOrEmpty(request.EntityId))
            query = query.Where(x => x.EntityId == request.EntityId);

       if (!string.IsNullOrEmpty(request.UserId) && Guid.TryParse(request.UserId, out var userIdGuid))
           query = query.Where(x => x.UserId == userIdGuid);

        if (!string.IsNullOrEmpty(request.Action))
            query = query.Where(x => x.Action == request.Action);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.ActionDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.ActionDate <= request.ToDate.Value);

        var totalCount = await query.CountAsync(ct);

        var logs = await query
            .OrderByDescending(x => x.ActionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AuditLogDto
            {
                Id          = x.Id,
                UserId      = x.UserId.HasValue ? x.UserId.Value.ToString() : string.Empty,
                UserName    = x.UserName,
                UserEmail   = x.UserEmail,
                UserRole    = x.UserRole,
                EntityType  = x.EntityType,
                EntityId    = x.EntityId ?? string.Empty,
                Action      = x.Action,
                ActionDate  = x.ActionDate,
                IpAddress   = x.IpAddress,
                RequestId   = x.RequestId,
                DurationMs  = (int?)x.DurationMs,
                Status      = x.Status.ToString(),
                ErrorMessage = x.ErrorMessage,
                // List view: deliberately skip large JSON payloads
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);

        return new AuditLogListResponse
        {
            Items      = logs,
            TotalCount = totalCount,
            Page       = page,
            PageSize   = pageSize,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }
}

/// <summary>
/// ✅ OPTIMIZED: GetEntityAuditHandler with efficient query
/// </summary>
public class GetEntityAuditHandler : IRequestHandler<GetEntityAuditQry, AuditLogListResponse>
{
    private readonly FinanceDbContext _context;

    public GetEntityAuditHandler(FinanceDbContext context) => _context = context;

    public async Task<AuditLogListResponse> Handle(GetEntityAuditQry request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.EntityType) || string.IsNullOrWhiteSpace(request.EntityId))
           return new AuditLogListResponse { Page = 1, PageSize = 50 };

        var page     = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        var query = _context.AuditLogs
            .AsNoTracking()
            .Where(x => x.EntityType == request.EntityType
                     && x.EntityId   == request.EntityId
                     && !x.IsDeleted);

        var totalCount = await query.CountAsync(ct);

        var logs = await query
            .OrderByDescending(x => x.ActionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AuditLogDto
            {
                Id          = x.Id,
                EntityType  = x.EntityType,
                EntityId    = x.EntityId ?? string.Empty,
                Action      = x.Action,
                UserId      = x.UserId.HasValue ? x.UserId.Value.ToString() : string.Empty,
                UserName    = x.UserName,
                UserEmail   = x.UserEmail,
                UserRole    = x.UserRole,
                IpAddress   = x.IpAddress,
                ActionDate  = x.ActionDate,
                RequestId   = x.RequestId,
                DurationMs  = (int?)x.DurationMs,
                Status      = x.Status.ToString(),
                ErrorMessage = x.ErrorMessage,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);

        return new AuditLogListResponse
        {
            Items      = logs,
            TotalCount = totalCount,
            Page       = page,
            PageSize   = pageSize,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }
}

/// <summary>
/// ✅ OPTIMIZED: GetAuditSummaryHandler with efficient aggregations
/// </summary>
// Queries/Audit/GetAuditSummaryHandler.cs - SQL Version
// Queries/Audit/GetAuditSummaryHandler.cs - EF Core Version

public class GetAuditSummaryHandler : IRequestHandler<GetAuditSummaryQry, AuditSummaryDto>
{
    private readonly FinanceDbContext _context;

    public GetAuditSummaryHandler(FinanceDbContext context) => _context = context;

    public async Task<AuditSummaryDto> Handle(GetAuditSummaryQry request, CancellationToken ct)
    {
        var query = _context.AuditLogs
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.ActionDate >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            query = query.Where(x => x.ActionDate <= request.ToDate.Value);

        // ✅ All aggregates computed server-side, over the ENTIRE filtered set.
        var totals = await query
            .GroupBy(x => 1)
            .Select(g => new
            {
                Total      = g.Count(),
                Success    = g.Count(x => x.Status == AuditStatus.Success),
                Failed     = g.Count(x => x.Status == AuditStatus.Failed),
                Pending    = g.Count(x => x.Status == AuditStatus.Pending),
                UniqueUsers = g.Select(x => x.UserId).Distinct().Count()
            })
            .FirstOrDefaultAsync(ct) ?? new { Total = 0, Success = 0, Failed = 0, Pending = 0, UniqueUsers = 0 };

        var byAction = await query
            .Where(x => x.Action != null)
            .GroupBy(x => x.Action!)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToListAsync(ct);

        var byEntity = await query
            .Where(x => x.EntityType != null)
            .GroupBy(x => x.EntityType!)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToListAsync(ct);

        var byUser = await query
            .Where(x => x.UserName != null)
            .GroupBy(x => x.UserName!)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToListAsync(ct);

        var recent = await query
            .OrderByDescending(x => x.ActionDate)
            .Take(5)
            .Select(x => new AuditLogDto
            {
                Id          = x.Id,
                UserId      = x.UserId.HasValue ? x.UserId.Value.ToString() : string.Empty,
                UserName    = x.UserName,
                EntityType  = x.EntityType,
                EntityId    = x.EntityId ?? string.Empty,
                Action      = x.Action,
                ActionDate  = x.ActionDate,
                Status      = x.Status.ToString(),
                ErrorMessage = x.ErrorMessage
            })
            .ToListAsync(ct);

        return new AuditSummaryDto
        {
            TotalLogs      = totals.Total,
            SuccessLogs    = totals.Success,
            FailureLogs    = totals.Failed,
            PendingLogs    = totals.Pending,   // ✅ honest name, no "Warning" lie
            UniqueUsers    = totals.UniqueUsers,
            ActionsByType   = byAction.ToDictionary(g => g.Key, g => g.Count),
            ActionsByEntity = byEntity.ToDictionary(g => g.Key, g => g.Count),
            ActionsByUser   = byUser.ToDictionary(g => g.Key, g => g.Count),
            RecentLogs      = recent
        };
    }
}
// ✅ Helper classes for SQL results
public class AuditCountResult
{
    public int TotalLogs { get; set; }
    public int SuccessLogs { get; set; }
    public int FailureLogs { get; set; }
    public int WarningLogs { get; set; }
    public int UniqueUsers { get; set; }
}

public class KeyValueResult
{
    public string Key { get; set; } = string.Empty;
    public int Count { get; set; }
}
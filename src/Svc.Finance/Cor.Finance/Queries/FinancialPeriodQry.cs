// Queries/FinancialPeriodQry.cs - COMPLETE FIX ✅

using MediatR;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Models.Enums;
using Cor.Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

// ==================== QUERIES ====================

public class GetFinancialPeriodsQry : IRequest<(List<FinancialPeriodDto> Data, int Total, int Page, int TotalPages)>
{
    public string? Search { get; set; }
    public string? PeriodType { get; set; }
    public bool? IsClosed { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

public class GetFinancialPeriodByIdQry : IRequest<FinancialPeriodDto>
{
    public Guid Id { get; set; }
}

public class GetPeriodStatsQry : IRequest<PeriodStatsDto>
{
    public Guid PeriodId { get; set; }
}

public class GetPeriodEntriesQry : IRequest<List<JournalEntryDto>>
{
    public Guid PeriodId { get; set; }
}

public class GetPeriodAuditQry : IRequest<List<AuditLogDto>>
{
    public Guid PeriodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class ValidatePeriodCloseQry : IRequest<(bool CanClose, string? Reason)>
{
    public Guid PeriodId { get; set; }
}

public class GetYearPeriodSummaryQry : IRequest<YearPeriodSummaryDto>
{
    public int Year { get; set; }
}

public class ComparePeriodsQry : IRequest<PeriodComparisonDto>
{
    public Guid Period1Id { get; set; }
    public Guid Period2Id { get; set; }
}

// ==================== ADDITIONAL QUERIES ====================

public class GetAllFinancialPeriodsQry : IRequest<List<FinancialPeriodDto>>
{
    public string? Search { get; set; }
    public string? PeriodType { get; set; }
    public bool? IsClosed { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetActiveFinancialPeriodQry : IRequest<FinancialPeriodDto?>
{
    public DateTime? Date { get; set; }
}

public class GetActiveFinancialPeriodWithStatsQry : IRequest<ActivePeriodWithStatsDto?>
{
    public DateTime? Date { get; set; }
}

public class ExportPeriodQry : IRequest<object>
{
    public Guid PeriodId { get; set; }
}

// ==================== HELPERS ====================

public static class PeriodHelpers
{
    public static string GetStatusString(FinancialPeriod period)
    {
        if (period.IsClosed)
            return "Closed";

        return period.Status switch
        {
            PeriodStatus.OPEN => "Open",
            PeriodStatus.LOCKED => "Locked",
            PeriodStatus.PENDING => "Pending",
            PeriodStatus.DRAFT => "Draft",
            PeriodStatus.CLOSED => "Closed",
            _ => "Unknown"
        };
    }

    public static FinancialPeriodDto MapToDto(FinancialPeriod period)
    {
        return new FinancialPeriodDto
        {
            Id = period.Id,
            Name = period.Name,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            PeriodType = period.PeriodType.ToString(),
            Status = GetStatusString(period),
            IsClosed = period.IsClosed,
            ClosedDate = period.ClosedDate,
            DateAdd = period.DateAdd,
            DateMod = period.DateMod,
            Notes = period.Notes
        };
    }
}

// ==================== HANDLERS ====================

public class GetFinancialPeriodsHandler : IRequestHandler<GetFinancialPeriodsQry,
    (List<FinancialPeriodDto> Data, int Total, int Page, int TotalPages)>
{
    private readonly FinanceDbContext _context;

    public GetFinancialPeriodsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<(List<FinancialPeriodDto> Data, int Total, int Page, int TotalPages)>
        Handle(GetFinancialPeriodsQry request, CancellationToken ct)
    {
        var query = _context.FinancialPeriods
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x => x.Name.Contains(request.Search));
        }

        // Apply PeriodType filter
        if (!string.IsNullOrEmpty(request.PeriodType))
        {
            if (Enum.TryParse<PeriodType>(request.PeriodType, true, out var periodType))
            {
                query = query.Where(x => x.PeriodType == periodType);
            }
        }

        // Apply Status filter
        if (!string.IsNullOrEmpty(request.Status))
        {
            if (Enum.TryParse<PeriodStatus>(request.Status, true, out var status))
            {
                query = query.Where(x => x.Status == status);
            }
        }

        // Apply IsClosed filter
        if (request.IsClosed.HasValue)
        {
            query = query.Where(x => x.IsClosed == request.IsClosed.Value);
        }

        // Apply date filters
        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.StartDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.EndDate <= request.ToDate.Value);
        }

        var total = await query.CountAsync(ct);

        // Apply sorting
        var sortBy = request.SortBy ?? "StartDate";
        var sortOrder = request.SortOrder ?? "DESC";

        query = sortOrder.ToUpper() == "DESC"
            ? query.OrderByDescending(x => EF.Property<object>(x, sortBy))
            : query.OrderBy(x => EF.Property<object>(x, sortBy));

        // ✅ FIX: Get data first, then map
        var periods = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var dtos = periods.Select(PeriodHelpers.MapToDto).ToList();
        var totalPages = (int)Math.Ceiling((double)total / request.PageSize);

        return (dtos, total, request.Page, totalPages);
    }
}

public class GetAllFinancialPeriodsHandler : IRequestHandler<GetAllFinancialPeriodsQry, List<FinancialPeriodDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllFinancialPeriodsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<FinancialPeriodDto>> Handle(GetAllFinancialPeriodsQry request, CancellationToken ct)
    {
        var query = _context.FinancialPeriods
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x => x.Name.Contains(request.Search));
        }

        if (!string.IsNullOrEmpty(request.PeriodType))
        {
            if (Enum.TryParse<PeriodType>(request.PeriodType, true, out var periodType))
            {
                query = query.Where(x => x.PeriodType == periodType);
            }
        }

        if (request.IsClosed.HasValue)
        {
            query = query.Where(x => x.IsClosed == request.IsClosed.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.StartDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.EndDate <= request.ToDate.Value);
        }

        var periods = await query
            .OrderByDescending(x => x.StartDate)
            .ToListAsync(ct);

        return periods.Select(PeriodHelpers.MapToDto).ToList();
    }
}

public class GetActiveFinancialPeriodHandler : IRequestHandler<GetActiveFinancialPeriodQry, FinancialPeriodDto?>
{
    private readonly FinanceDbContext _context;

    public GetActiveFinancialPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialPeriodDto?> Handle(GetActiveFinancialPeriodQry request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => !x.IsDeleted && !x.IsClosed, ct);

        if (period == null)
            return null;

        return PeriodHelpers.MapToDto(period);
    }
}

public class GetFinancialPeriodByIdHandler : IRequestHandler<GetFinancialPeriodByIdQry, FinancialPeriodDto>
{
    private readonly FinanceDbContext _context;

    public GetFinancialPeriodByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialPeriodDto> Handle(GetFinancialPeriodByIdQry request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Financial period with ID '{request.Id}' not found");

        return PeriodHelpers.MapToDto(period);
    }
}

public class GetPeriodStatsHandler : IRequestHandler<GetPeriodStatsQry, PeriodStatsDto>
{
    private readonly FinanceDbContext _context;

    public GetPeriodStatsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PeriodStatsDto> Handle(GetPeriodStatsQry request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.PeriodId && !x.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Financial period with ID '{request.PeriodId}' not found");

        var entries = await _context.JournalEntries
            .Where(x => x.PeriodId == request.PeriodId && !x.IsDeleted)
            .Include(x => x.Lines)
            .ToListAsync(ct);

        var totalEntries = entries.Count;
        var postedEntries = entries.Count(x => x.IsPosted);
        var unpostedEntries = totalEntries - postedEntries;

        var totalTransactions = entries.Sum(x => x.Lines?.Count ?? 0);
        var totalDebit = entries.SelectMany(x => x.Lines ?? new List<JournalLine>()).Sum(x => x.Debit);
        var totalCredit = entries.SelectMany(x => x.Lines ?? new List<JournalLine>()).Sum(x => x.Credit);

        var today = DateTime.UtcNow;
        var totalDays = (period.EndDate - period.StartDate).Days;
        var elapsedDays = (today - period.StartDate).Days;
        var daysRemaining = Math.Max(0, totalDays - elapsedDays);

        var completionPercentage = totalEntries > 0
            ? Math.Min(100, (double)postedEntries / totalEntries * 100)
            : 0;

        var canBeClosed = unpostedEntries == 0 && !period.IsClosed;

        return new PeriodStatsDto
        {
            TotalJournalEntries = totalEntries,
            PostedEntries = postedEntries,
            UnpostedEntries = unpostedEntries,
            TotalTransactions = totalTransactions,
            TotalDebit = totalDebit,
            TotalCredit = totalCredit,
            PeriodStart = period.StartDate,
            PeriodEnd = period.EndDate,
            DaysRemaining = daysRemaining,
            CompletionPercentage = completionPercentage,
            CanBeClosed = canBeClosed,
            ClosingReason = unpostedEntries > 0
                ? $"{unpostedEntries} unposted journal entries found"
                : null
        };
    }
}

public class GetPeriodEntriesHandler : IRequestHandler<GetPeriodEntriesQry, List<JournalEntryDto>>
{
    private readonly FinanceDbContext _context;

    public GetPeriodEntriesHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<JournalEntryDto>> Handle(GetPeriodEntriesQry request, CancellationToken ct)
    {
        var entries = await _context.JournalEntries
            .Where(x => x.PeriodId == request.PeriodId && !x.IsDeleted)
            .Include(x => x.Lines)
            .ThenInclude(x => x.Account)
            .OrderByDescending(x => x.EntryDate)
            .ToListAsync(ct);

        return entries.Select(e => new JournalEntryDto
        {
            Id = e.Id,
            Reference = e.Reference,
            Description = e.Description,
            EntryDate = e.EntryDate,
            IsPosted = e.IsPosted,
            TotalDebit = e.TotalDebit,
            TotalCredit = e.TotalCredit,
            DateAdd = e.DateAdd,
            DateMod = e.DateMod,
            Lines = e.Lines?.Select(l => new JournalLineDto
            {
                AccountId = l.AccountId,
                AccountCode = l.Account?.Code,
                AccountName = l.Account?.Name,
                Debit = l.Debit,
                Credit = l.Credit,
                Description = l.Description
            }).ToList() ?? new List<JournalLineDto>()
        }).ToList();
    }
}

public class GetPeriodAuditHandler : IRequestHandler<GetPeriodAuditQry, List<AuditLogDto>>
{
    private readonly FinanceDbContext _context;

    public GetPeriodAuditHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuditLogDto>> Handle(GetPeriodAuditQry request, CancellationToken ct)
    {
        var logs = await _context.AuditLogs
            .Where(x => x.EntityType == "FinancialPeriod" &&
                        x.EntityId == request.PeriodId.ToString())
            .OrderByDescending(x => x.ActionDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return logs.Select(log => new AuditLogDto
        {
            Id = log.Id,
            EntityType = log.EntityType,
            EntityId = log.EntityId ?? string.Empty,
            Action = log.Action,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            UserId = log.UserId?.ToString() ?? string.Empty,
            UserName = log.UserName,
            IpAddress = log.IpAddress,
            ActionDate = log.ActionDate,
            DateAdd = log.DateAdd,
            DateMod = log.DateMod
        }).ToList();
    }
}

public class ValidatePeriodCloseHandler : IRequestHandler<ValidatePeriodCloseQry, (bool CanClose, string? Reason)>
{
    private readonly FinanceDbContext _context;

    public ValidatePeriodCloseHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<(bool CanClose, string? Reason)> Handle(ValidatePeriodCloseQry request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.PeriodId && !x.IsDeleted, ct);

        if (period == null)
            return (false, "Period not found");

        if (period.IsClosed)
            return (false, "Period is already closed");

        var hasUnposted = await _context.JournalEntries
            .AnyAsync(x => x.PeriodId == request.PeriodId && !x.IsPosted && !x.IsDeleted, ct);

        if (hasUnposted)
            return (false, "Period has unposted journal entries");

        return (true, null);
    }
}

public class GetYearPeriodSummaryHandler : IRequestHandler<GetYearPeriodSummaryQry, YearPeriodSummaryDto>
{
    private readonly FinanceDbContext _context;

    public GetYearPeriodSummaryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<YearPeriodSummaryDto> Handle(GetYearPeriodSummaryQry request, CancellationToken ct)
    {
        var startDate = new DateTime(request.Year, 1, 1);
        var endDate = new DateTime(request.Year, 12, 31);

        var periods = await _context.FinancialPeriods
            .Where(x => !x.IsDeleted &&
                        x.StartDate >= startDate &&
                        x.EndDate <= endDate)
            .OrderBy(x => x.StartDate)
            .ToListAsync(ct);

        var periodDtos = periods.Select(PeriodHelpers.MapToDto).ToList();

        return new YearPeriodSummaryDto
        {
            Year = request.Year,
            Periods = periodDtos,
            TotalPeriods = periods.Count,
            ClosedPeriods = periods.Count(p => p.IsClosed),
            OpenPeriods = periods.Count(p => !p.IsClosed),
            FirstPeriodStart = periods.Any() ? periods.First().StartDate : null,
            LastPeriodEnd = periods.Any() ? periods.Last().EndDate : null
        };
    }
}

public class ComparePeriodsHandler : IRequestHandler<ComparePeriodsQry, PeriodComparisonDto>
{
    private readonly FinanceDbContext _context;

    public ComparePeriodsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PeriodComparisonDto> Handle(ComparePeriodsQry request, CancellationToken ct)
    {
        var period1 = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Period1Id && !x.IsDeleted, ct);

        var period2 = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Period2Id && !x.IsDeleted, ct);

        if (period1 == null || period2 == null)
            throw new InvalidOperationException("One or both periods not found");

        var period1Stats = await GetPeriodStats(request.Period1Id, ct);
        var period2Stats = await GetPeriodStats(request.Period2Id, ct);

        var diff = period1Stats.NetBalance - period2Stats.NetBalance;
        var percentageChange = period2Stats.NetBalance != 0
            ? (period1Stats.NetBalance - period2Stats.NetBalance) / Math.Abs(period2Stats.NetBalance) * 100
            : 0;

        return new PeriodComparisonDto
        {
            Period1 = new PeriodComparisonItem
            {
                Id = period1.Id,
                Name = period1.Name,
                TotalDebit = period1Stats.TotalDebit,
                TotalCredit = period1Stats.TotalCredit,
                NetBalance = period1Stats.NetBalance,
                JournalEntryCount = period1Stats.TotalJournalEntries,
                VoucherCount = period1Stats.TotalTransactions
            },
            Period2 = new PeriodComparisonItem
            {
                Id = period2.Id,
                Name = period2.Name,
                TotalDebit = period2Stats.TotalDebit,
                TotalCredit = period2Stats.TotalCredit,
                NetBalance = period2Stats.NetBalance,
                JournalEntryCount = period2Stats.TotalJournalEntries,
                VoucherCount = period2Stats.TotalTransactions
            },
            Comparison = new PeriodComparisonResult
            {
                BalanceDifference = diff,
                PercentageChange = percentageChange,
                Status = percentageChange > 5 ? "Increased" :
                        percentageChange < -5 ? "Decreased" : "Stable"
            }
        };
    }

    private async Task<PeriodStatsDto> GetPeriodStats(Guid periodId, CancellationToken ct)
    {
        var entries = await _context.JournalEntries
            .Where(x => x.PeriodId == periodId && !x.IsDeleted)
            .Include(x => x.Lines)
            .ToListAsync(ct);

        var totalEntries = entries.Count;
        var postedEntries = entries.Count(x => x.IsPosted);
        var unpostedEntries = totalEntries - postedEntries;

        var totalTransactions = entries.Sum(x => x.Lines?.Count ?? 0);
        var totalDebit = entries.SelectMany(x => x.Lines ?? new List<JournalLine>()).Sum(x => x.Debit);
        var totalCredit = entries.SelectMany(x => x.Lines ?? new List<JournalLine>()).Sum(x => x.Credit);
        var netBalance = totalDebit - totalCredit;

        return new PeriodStatsDto
        {
            TotalJournalEntries = totalEntries,
            PostedEntries = postedEntries,
            UnpostedEntries = unpostedEntries,
            TotalTransactions = totalTransactions,
            TotalDebit = totalDebit,
            TotalCredit = totalCredit,
            NetBalance = netBalance,
            CanBeClosed = unpostedEntries == 0
        };
    }
}

public class ExportPeriodHandler : IRequestHandler<ExportPeriodQry, object>
{
    private readonly FinanceDbContext _context;

    public ExportPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<object> Handle(ExportPeriodQry request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.PeriodId && !x.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {request.PeriodId} not found");

        var entries = await _context.JournalEntries
            .Where(x => x.PeriodId == request.PeriodId && !x.IsDeleted)
            .Include(x => x.Lines)
            .ToListAsync(ct);

        var totalDebit = entries
            .SelectMany(x => x.Lines ?? new List<JournalLine>())
            .Sum(x => x.Debit);

        var totalCredit = entries
            .SelectMany(x => x.Lines ?? new List<JournalLine>())
            .Sum(x => x.Credit);

        return new
        {
            period = new
            {
                period.Id,
                period.Name,
                period.StartDate,
                period.EndDate,
                PeriodType = period.PeriodType.ToString(),
                period.IsClosed,
                period.ClosedDate,
                period.DateAdd,
                period.DateMod,
                period.Notes
            },
            summary = new
            {
                totalEntries = entries.Count,
                postedEntries = entries.Count(x => x.IsPosted),
                unpostedEntries = entries.Count(x => !x.IsPosted),
                totalDebit = totalDebit,
                totalCredit = totalCredit,
                netBalance = totalDebit - totalCredit
            },
            entries = entries.Select(e => new
            {
                e.Id,
                e.Reference,
                e.Description,
                e.EntryDate,
                e.IsPosted,
                e.TotalDebit,
                e.TotalCredit,
                e.DateAdd,
                Lines = e.Lines != null && e.Lines.Any()
                    ? e.Lines.Select(l => new
                    {
                        l.Debit,
                        l.Credit,
                        l.Description
                    })
                    : Enumerable.Empty<object>()
            }),
            exportedAt = DateTime.UtcNow,
            exportedBy = "System"
        };
    }
}

public class GetActiveFinancialPeriodWithStatsHandler : IRequestHandler<GetActiveFinancialPeriodWithStatsQry, ActivePeriodWithStatsDto?>
{
    private readonly FinanceDbContext _context;

    public GetActiveFinancialPeriodWithStatsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ActivePeriodWithStatsDto?> Handle(GetActiveFinancialPeriodWithStatsQry request, CancellationToken ct)
    {
        var checkDate = request.Date ?? DateTime.UtcNow.Date;

        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                      !x.IsClosed &&
                                      x.StartDate <= checkDate &&
                                      x.EndDate >= checkDate, ct);

        if (period == null)
            return null;

        var entries = await _context.JournalEntries
            .Where(x => x.PeriodId == period.Id && !x.IsDeleted)
            .Include(x => x.Lines)
            .ToListAsync(ct);

        var totalEntries = entries.Count;
        var postedEntries = entries.Count(x => x.IsPosted);
        var unpostedEntries = totalEntries - postedEntries;

        var totalTransactions = entries.Sum(x => x.Lines?.Count ?? 0);
        var totalDebit = entries.SelectMany(x => x.Lines ?? new List<JournalLine>()).Sum(x => x.Debit);
        var totalCredit = entries.SelectMany(x => x.Lines ?? new List<JournalLine>()).Sum(x => x.Credit);
        var netBalance = totalDebit - totalCredit;

        var today = DateTime.UtcNow.Date;
        var totalDays = (period.EndDate - period.StartDate).Days;
        var daysElapsed = (today - period.StartDate).Days;
        var daysRemaining = Math.Max(0, totalDays - daysElapsed);

        var completionPercentage = totalEntries > 0
            ? Math.Min(100, (double)postedEntries / totalEntries * 100)
            : 0;

        var timeProgressPercentage = totalDays > 0
            ? Math.Min(100, (double)daysElapsed / totalDays * 100)
            : 0;

        var canBeClosed = unpostedEntries == 0 && !period.IsClosed;

        var uniqueAccounts = entries
            .SelectMany(x => x.Lines ?? new List<JournalLine>())
            .Select(x => x.AccountId)
            .Distinct()
            .Count();

        var firstEntryDate = entries.Any() ? entries.Min(x => x.EntryDate) : (DateTime?)null;
        var lastEntryDate = entries.Any() ? entries.Max(x => x.EntryDate) : (DateTime?)null;

        var periodDto = PeriodHelpers.MapToDto(period);
        periodDto.TotalEntries = totalEntries;
        periodDto.PostedEntries = postedEntries;
        periodDto.UnpostedEntries = unpostedEntries;

        var statsDto = new PeriodStatsDto
        {
            TotalJournalEntries = totalEntries,
            PostedEntries = postedEntries,
            UnpostedEntries = unpostedEntries,
            TotalTransactions = totalTransactions,
            TotalDebit = totalDebit,
            TotalCredit = totalCredit,
            NetBalance = netBalance,
            PeriodStart = period.StartDate,
            PeriodEnd = period.EndDate,
            DaysRemaining = daysRemaining,
            CompletionPercentage = completionPercentage,
            CanBeClosed = canBeClosed,
            ClosingReason = unpostedEntries > 0
                ? $"{unpostedEntries} unposted journal entries found"
                : null,
            TotalAccountsUsed = uniqueAccounts,
            TotalUniqueAccounts = uniqueAccounts,
            FirstEntryDate = firstEntryDate,
            LastEntryDate = lastEntryDate
        };

        return new ActivePeriodWithStatsDto
        {
            Period = periodDto,
            Stats = statsDto,
            HasUnpostedEntries = unpostedEntries > 0,
            DaysRemaining = daysRemaining,
            CompletionPercentage = completionPercentage,
            CanBeClosed = canBeClosed,
            ClosingReason = unpostedEntries > 0
                ? $"{unpostedEntries} unposted journal entries found"
                : null,
            TotalDays = totalDays,
            DaysElapsed = Math.Max(0, daysElapsed),
            TimeProgressPercentage = timeProgressPercentage
        };
    }
}
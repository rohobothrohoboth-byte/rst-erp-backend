using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Cor.Finance.Models.Entities;
namespace Cor.Finance.Queries;

// ==================== QUERIES ====================

public class GetAllJournalEntriesQry : IRequest<PagedResult<JournalEntryDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "EntryDate";
    public string? SortOrder { get; set; } = "DESC";
    public Guid? PeriodId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsPosted { get; set; }
    public bool? IsApproved { get; set; }
    public string? EntryType { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}

public class GetJournalEntryByIdQry : IRequest<JournalEntryDto?>
{
    public Guid Id { get; set; }
}

public class GetUnpostedJournalEntriesQry : IRequest<List<JournalEntryDto>>
{
}

public class GetJournalEntriesByPeriodQry : IRequest<List<JournalEntryDto>>
{
    public Guid PeriodId { get; set; }
}

public class GetJournalEntryByReferenceQry : IRequest<JournalEntryDto>
{
    public string Reference { get; set; } = string.Empty;
}

public class GetJournalEntrySummaryQry : IRequest<JournalEntrySummaryDto>
{
    public Guid? PeriodId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class ExportJournalEntriesQry : IRequest<List<JournalEntryDto>>
{
    public Guid? PeriodId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

// ============================================================
// HANDLERS
// ============================================================

// ==================== GET ALL JOURNAL ENTRIES (WITH PAGINATION) ====================
public class GetAllJournalEntriesHandler : IRequestHandler<GetAllJournalEntriesQry, PagedResult<JournalEntryDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAllJournalEntriesHandler> _logger;

    public GetAllJournalEntriesHandler(FinanceDbContext context, ILogger<GetAllJournalEntriesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<JournalEntryDto>> Handle(GetAllJournalEntriesQry request, CancellationToken ct)
    {
        try
        {
            // ? FIX: Use Include to load all data in one query (eliminates N+1)
            var query = _context.JournalEntries
                .Include(x => x.Period)
                .Include(x => x.Lines.Where(l => !l.IsDeleted))
                    .ThenInclude(l => l.Account)
                .Where(x => !x.IsDeleted)
                .AsQueryable();


            // Apply filters
            if (request.PeriodId.HasValue)
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.EntryDate >= EnsureUtc(request.FromDate.Value));

            if (request.ToDate.HasValue)
                query = query.Where(x => x.EntryDate <= EnsureUtc(request.ToDate.Value));

            if (request.IsPosted.HasValue)
                query = query.Where(x => x.IsPosted == request.IsPosted.Value);

            if (request.IsApproved.HasValue)
                query = query.Where(x => x.IsApproved == request.IsApproved.Value);

            if (!string.IsNullOrEmpty(request.EntryType))
                query = query.Where(x => x.EntryType == request.EntryType);

            if (request.MinAmount.HasValue)
                query = query.Where(x => x.TotalDebit >= request.MinAmount.Value || x.TotalCredit >= request.MinAmount.Value);

            if (request.MaxAmount.HasValue)
                query = query.Where(x => x.TotalDebit <= request.MaxAmount.Value || x.TotalCredit <= request.MaxAmount.Value);

            // Get total count before pagination
            var totalCount = await query.CountAsync(ct);

            // Apply sorting
            query = request.SortBy?.ToLower() switch
            {
                "reference" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Reference)
                    : query.OrderBy(x => x.Reference),
                "entrydate" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.EntryDate)
                    : query.OrderBy(x => x.EntryDate),
                "description" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Description)
                    : query.OrderBy(x => x.Description),
                "entrytype" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.EntryType)
                    : query.OrderBy(x => x.EntryType),
                _ => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.EntryDate)
                    : query.OrderBy(x => x.EntryDate)
            };

            // Apply pagination
            var entries = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(ct);

            _logger.LogInformation("Retrieved {Count} journal entries (Page {Page}/{TotalPages})",
                entries.Count, request.Page, (int)Math.Ceiling((double)totalCount / request.PageSize));

            return new PagedResult<JournalEntryDto>
            {
                Data = entries.Select(MapToDto).ToList(),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journal entries");
            throw;
        }
    }

    private static JournalEntryDto MapToDto(JournalEntry entry)
    {
        return new JournalEntryDto
        {
            Id = entry.Id,
            Reference = entry.Reference,
            EntryDate = entry.EntryDate,
            Description = entry.Description,
            EntryType = entry.EntryType,
            TotalDebit = entry.TotalDebit,
            TotalCredit = entry.TotalCredit,
            IsPosted = entry.IsPosted,
            IsApproved = entry.IsApproved,
            IsReversed = entry.IsReversed,
            PostedDate = entry.PostedDate,
            ApprovedDate = entry.ApprovedDate,
            ApprovedBy = entry.ApprovedBy,
            ReversedDate = entry.ReversedDate,
            ReversedBy = entry.ReversedBy,
            RejectionReason = entry.RejectionReason,
            PeriodId = entry.PeriodId,
            PeriodName = entry.Period?.Name,
            BranchId = entry.BranchId,
            DepartmentId = entry.DepartmentId,
            EmployeeId = entry.EmployeeId,
            RowVersion = entry.RowVersion ,
            Lines = entry.Lines?.Where(l => !l.IsDeleted).Select(line => new JournalLineDto
            {
                Id = line.Id,
                AccountId = line.AccountId,
                AccountCode = line.Account?.Code,
                AccountName = line.Account?.Name,
                Direction = line.Direction,
                Amount = line.Amount,
                Description = line.Description
            }).ToList() ?? new List<JournalLineDto>(),
            DateAdd = entry.DateAdd,
            DateMod = entry.DateMod
        };
    }

    private static DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        return dateTime.ToUniversalTime();
    }
}

// ==================== GET JOURNAL ENTRY BY ID ====================
public class GetJournalEntryByIdHandler : IRequestHandler<GetJournalEntryByIdQry, JournalEntryDto?>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetJournalEntryByIdHandler> _logger;

    public GetJournalEntryByIdHandler(FinanceDbContext context, ILogger<GetJournalEntryByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<JournalEntryDto?> Handle(GetJournalEntryByIdQry request, CancellationToken ct)
    {
        try
        {
            var entry = await _context.JournalEntries
                .Include(x => x.Period)
                .Include(x => x.Lines.Where(l => !l.IsDeleted))
                    .ThenInclude(l => l.Account)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (entry == null)
                return null;

            return MapToDto(entry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journal entry {Id}", request.Id);
            throw;
        }
    }

    private static JournalEntryDto MapToDto(JournalEntry entry)
    {
        return new JournalEntryDto
        {
            Id = entry.Id,
            Reference = entry.Reference,
            EntryDate = entry.EntryDate,
            Description = entry.Description,
            EntryType = entry.EntryType,
            TotalDebit = entry.TotalDebit,
            TotalCredit = entry.TotalCredit,
            IsPosted = entry.IsPosted,
            IsApproved = entry.IsApproved,
            IsReversed = entry.IsReversed,
            PostedDate = entry.PostedDate,
            ApprovedDate = entry.ApprovedDate,
            ApprovedBy = entry.ApprovedBy,
            ReversedDate = entry.ReversedDate,
            ReversedBy = entry.ReversedBy,
            RejectionReason = entry.RejectionReason,
            PeriodId = entry.PeriodId,
            PeriodName = entry.Period?.Name,
            BranchId = entry.BranchId,
            DepartmentId = entry.DepartmentId,
            EmployeeId = entry.EmployeeId,
            RowVersion = entry.RowVersion ,
            Lines = entry.Lines?.Where(l => !l.IsDeleted).Select(line => new JournalLineDto
            {
                Id = line.Id,
                AccountId = line.AccountId,
                AccountCode = line.Account?.Code,
                AccountName = line.Account?.Name,
                Direction = line.Direction,
                Amount = line.Amount,
                Description = line.Description
            }).ToList() ?? new List<JournalLineDto>(),
            DateAdd = entry.DateAdd,
            DateMod = entry.DateMod
        };
    }
}

// ==================== GET JOURNAL ENTRY SUMMARY (OPTIMIZED) ====================
public class GetJournalEntrySummaryHandler : IRequestHandler<GetJournalEntrySummaryQry, JournalEntrySummaryDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetJournalEntrySummaryHandler> _logger;

    public GetJournalEntrySummaryHandler(FinanceDbContext context, ILogger<GetJournalEntrySummaryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<JournalEntrySummaryDto> Handle(GetJournalEntrySummaryQry request, CancellationToken ct)
    {
        try
        {
            var query = _context.JournalEntries
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (request.PeriodId.HasValue)
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.EntryDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.EntryDate <= request.ToDate.Value);

            // ? FIX: Use database aggregation instead of fetching all data
            var result = await query
                .GroupBy(x => 1)
                .Select(g => new
                {
                    TotalEntries = g.Count(),
                    PostedEntries = g.Count(x => x.IsPosted),
                    UnpostedEntries = g.Count(x => !x.IsPosted),
                    ApprovedEntries = g.Count(x => x.IsApproved),
                    RejectedEntries = g.Count(x => x.IsApproved == false && x.RejectionReason != null),
                    TotalDebit = g.Sum(x => x.TotalDebit),
                    TotalCredit = g.Sum(x => x.TotalCredit)
                })
                .FirstOrDefaultAsync(ct) ?? new
                {
                    TotalEntries = 0,
                    PostedEntries = 0,
                    UnpostedEntries = 0,
                    ApprovedEntries = 0,
                    RejectedEntries = 0,
                    TotalDebit = 0m,
                    TotalCredit = 0m
                };

            // Get period name if needed
            var periodName = request.PeriodId.HasValue
                ? await _context.FinancialPeriods
                    .Where(p => p.Id == request.PeriodId.Value && !p.IsDeleted)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync(ct)
                : null;

            // Get breakdown by type (still efficient)
            var entriesByType = await query
                .GroupBy(x => x.EntryType ?? "Unknown")
                .Select(g => new { Type = g.Key, Count = g.Count(), Amount = g.Sum(x => x.TotalDebit) })
                .ToListAsync(ct);

            return new JournalEntrySummaryDto
            {
                TotalEntries = result.TotalEntries,
                PostedEntries = result.PostedEntries,
                UnpostedEntries = result.UnpostedEntries,
                ApprovedEntries = result.ApprovedEntries,
                RejectedEntries = result.RejectedEntries,
                TotalDebit = result.TotalDebit,
                TotalCredit = result.TotalCredit,
                NetBalance = result.TotalDebit - result.TotalCredit,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                PeriodId = request.PeriodId,
                PeriodName = periodName,
                EntriesByType = entriesByType.ToDictionary(x => x.Type, x => x.Count),
                AmountByType = entriesByType.ToDictionary(x => x.Type, x => x.Amount),
                EntriesByDate = new Dictionary<string, int>(),
                AmountByDate = new Dictionary<string, decimal>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journal entry summary");
            throw;
        }
    }
}

// ==================== GET UNPOSTED JOURNAL ENTRIES ====================
public class GetUnpostedJournalEntriesHandler : IRequestHandler<GetUnpostedJournalEntriesQry, List<JournalEntryDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetUnpostedJournalEntriesHandler> _logger;

    public GetUnpostedJournalEntriesHandler(FinanceDbContext context, ILogger<GetUnpostedJournalEntriesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<JournalEntryDto>> Handle(GetUnpostedJournalEntriesQry request, CancellationToken ct)
    {
        try
        {
            var entries = await _context.JournalEntries
                .Include(x => x.Period)
                .Include(x => x.Lines.Where(l => !l.IsDeleted))
                    .ThenInclude(l => l.Account)
                .Where(x => !x.IsPosted && !x.IsDeleted)
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync(ct);

            return entries.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unposted journal entries");
            throw;
        }
    }

    private static JournalEntryDto MapToDto(JournalEntry entry)
    {
        return new JournalEntryDto
        {
            Id = entry.Id,
            Reference = entry.Reference,
            EntryDate = entry.EntryDate,
            Description = entry.Description,
            EntryType = entry.EntryType,
            TotalDebit = entry.TotalDebit,
            TotalCredit = entry.TotalCredit,
            IsPosted = entry.IsPosted,
            IsApproved = entry.IsApproved,
            IsReversed = entry.IsReversed,
            PostedDate = entry.PostedDate,
            ApprovedDate = entry.ApprovedDate,
            ApprovedBy = entry.ApprovedBy,
            ReversedDate = entry.ReversedDate,
            ReversedBy = entry.ReversedBy,
            RejectionReason = entry.RejectionReason,
            PeriodId = entry.PeriodId,
            PeriodName = entry.Period?.Name,
            BranchId = entry.BranchId,
            DepartmentId = entry.DepartmentId,
            EmployeeId = entry.EmployeeId,
            RowVersion = entry.RowVersion ,
            Lines = entry.Lines?.Where(l => !l.IsDeleted).Select(line => new JournalLineDto
            {
                Id = line.Id,
                AccountId = line.AccountId,
                AccountCode = line.Account?.Code,
                AccountName = line.Account?.Name,
                Direction = line.Direction,
                Amount = line.Amount,
                Description = line.Description
            }).ToList() ?? new List<JournalLineDto>(),
            DateAdd = entry.DateAdd,
            DateMod = entry.DateMod
        };
    }
}

// ==================== GET JOURNAL ENTRIES BY PERIOD ====================
public class GetJournalEntriesByPeriodHandler : IRequestHandler<GetJournalEntriesByPeriodQry, List<JournalEntryDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetJournalEntriesByPeriodHandler> _logger;

    public GetJournalEntriesByPeriodHandler(FinanceDbContext context, ILogger<GetJournalEntriesByPeriodHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<JournalEntryDto>> Handle(GetJournalEntriesByPeriodQry request, CancellationToken ct)
    {
        try
        {
            var entries = await _context.JournalEntries
                .Include(x => x.Period)
                .Include(x => x.Lines.Where(l => !l.IsDeleted))
                    .ThenInclude(l => l.Account)
                .Where(x => x.PeriodId == request.PeriodId && !x.IsDeleted)
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync(ct);

            return entries.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journal entries for period {PeriodId}", request.PeriodId);
            throw;
        }
    }

    private static JournalEntryDto MapToDto(JournalEntry entry)
    {
        return new JournalEntryDto
        {
            Id = entry.Id,
            Reference = entry.Reference,
            EntryDate = entry.EntryDate,
            Description = entry.Description,
            EntryType = entry.EntryType,
            TotalDebit = entry.TotalDebit,
            TotalCredit = entry.TotalCredit,
            IsPosted = entry.IsPosted,
            IsApproved = entry.IsApproved,
            IsReversed = entry.IsReversed,
            PostedDate = entry.PostedDate,
            ApprovedDate = entry.ApprovedDate,
            ApprovedBy = entry.ApprovedBy,
            ReversedDate = entry.ReversedDate,
            ReversedBy = entry.ReversedBy,
            RejectionReason = entry.RejectionReason,
            PeriodId = entry.PeriodId,
            PeriodName = entry.Period?.Name,
            BranchId = entry.BranchId,
            DepartmentId = entry.DepartmentId,
            EmployeeId = entry.EmployeeId,
            RowVersion = entry.RowVersion ,
            Lines = entry.Lines?.Where(l => !l.IsDeleted).Select(line => new JournalLineDto
            {
                Id = line.Id,
                AccountId = line.AccountId,
                AccountCode = line.Account?.Code,
                AccountName = line.Account?.Name,
                Direction = line.Direction,
                Amount = line.Amount,
                Description = line.Description
            }).ToList() ?? new List<JournalLineDto>(),
            DateAdd = entry.DateAdd,
            DateMod = entry.DateMod
        };
    }
}

// ==================== GET JOURNAL ENTRY BY REFERENCE ====================
public class GetJournalEntryByReferenceHandler : IRequestHandler<GetJournalEntryByReferenceQry, JournalEntryDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetJournalEntryByReferenceHandler> _logger;

    public GetJournalEntryByReferenceHandler(FinanceDbContext context, ILogger<GetJournalEntryByReferenceHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<JournalEntryDto> Handle(GetJournalEntryByReferenceQry request, CancellationToken ct)
    {
        try
        {
            var entry = await _context.JournalEntries
                .Include(x => x.Period)
                .Include(x => x.Lines.Where(l => !l.IsDeleted))
                    .ThenInclude(l => l.Account)
                .FirstOrDefaultAsync(x => x.Reference == request.Reference && !x.IsDeleted, ct);

            if (entry == null)
                throw new InvalidOperationException($"Journal entry with reference '{request.Reference}' not found");

            return MapToDto(entry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journal entry by reference {Reference}", request.Reference);
            throw;
        }
    }

    private static JournalEntryDto MapToDto(JournalEntry entry)
    {
        return new JournalEntryDto
        {
            Id = entry.Id,
            Reference = entry.Reference,
            EntryDate = entry.EntryDate,
            Description = entry.Description,
            EntryType = entry.EntryType,
            TotalDebit = entry.TotalDebit,
            TotalCredit = entry.TotalCredit,
            IsPosted = entry.IsPosted,
            IsApproved = entry.IsApproved,
            IsReversed = entry.IsReversed,
            PostedDate = entry.PostedDate,
            ApprovedDate = entry.ApprovedDate,
            ApprovedBy = entry.ApprovedBy,
            ReversedDate = entry.ReversedDate,
            ReversedBy = entry.ReversedBy,
            RejectionReason = entry.RejectionReason,
            PeriodId = entry.PeriodId,
            PeriodName = entry.Period?.Name,
            BranchId = entry.BranchId,
            DepartmentId = entry.DepartmentId,
            EmployeeId = entry.EmployeeId,
            RowVersion = entry.RowVersion,
            Lines = entry.Lines?.Where(l => !l.IsDeleted).Select(line => new JournalLineDto
            {
                Id = line.Id,
                AccountId = line.AccountId,
                AccountCode = line.Account?.Code,
                AccountName = line.Account?.Name,
                Direction = line.Direction,
                Amount = line.Amount,
                Description = line.Description
            }).ToList() ?? new List<JournalLineDto>(),
            DateAdd = entry.DateAdd,
            DateMod = entry.DateMod
        };
    }
}

// ==================== EXPORT JOURNAL ENTRIES ====================
public class ExportJournalEntriesHandler : IRequestHandler<ExportJournalEntriesQry, List<JournalEntryDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<ExportJournalEntriesHandler> _logger;

    public ExportJournalEntriesHandler(FinanceDbContext context, ILogger<ExportJournalEntriesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<JournalEntryDto>> Handle(ExportJournalEntriesQry request, CancellationToken ct)
    {
        try
        {
            var query = _context.JournalEntries
                .Include(x => x.Period)
                .Include(x => x.Lines.Where(l => !l.IsDeleted))
                    .ThenInclude(l => l.Account)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (request.PeriodId.HasValue)
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.EntryDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.EntryDate <= request.ToDate.Value);

            var entries = await query
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync(ct);

            _logger.LogInformation("Exported {Count} journal entries", entries.Count);

            return entries.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting journal entries");
            throw;
        }
    }

    private static JournalEntryDto MapToDto(JournalEntry entry)
    {
        return new JournalEntryDto
        {
            Id = entry.Id,
            Reference = entry.Reference,
            EntryDate = entry.EntryDate,
            Description = entry.Description,
            EntryType = entry.EntryType,
            TotalDebit = entry.TotalDebit,
            TotalCredit = entry.TotalCredit,
            IsPosted = entry.IsPosted,
            IsApproved = entry.IsApproved,
            IsReversed = entry.IsReversed,
            PostedDate = entry.PostedDate,
            ApprovedDate = entry.ApprovedDate,
            ApprovedBy = entry.ApprovedBy,
            ReversedDate = entry.ReversedDate,
            ReversedBy = entry.ReversedBy,
            RejectionReason = entry.RejectionReason,
            PeriodId = entry.PeriodId,
            PeriodName = entry.Period?.Name,
            BranchId = entry.BranchId,
            DepartmentId = entry.DepartmentId,
            EmployeeId = entry.EmployeeId,
            RowVersion = entry.RowVersion ,
            Lines = entry.Lines?.Where(l => !l.IsDeleted).Select(line => new JournalLineDto
            {
                Id = line.Id,
                AccountId = line.AccountId,
                AccountCode = line.Account?.Code,
                AccountName = line.Account?.Name,
                Direction = line.Direction,
                Amount = line.Amount,
                Description = line.Description
            }).ToList() ?? new List<JournalLineDto>(),
            DateAdd = entry.DateAdd,
            DateMod = entry.DateMod
        };
    }
}
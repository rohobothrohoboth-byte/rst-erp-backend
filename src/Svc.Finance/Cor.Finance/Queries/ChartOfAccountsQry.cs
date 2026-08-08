// Queries/ChartOfAccountsQry.cs - FIXED ✅

using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

// ============================================================
// QUERIES
// ============================================================

public class GetAllChartOfAccountsQry : IRequest<PaginatedResponse<ChartOfAccountsDto>>
{
    public bool? IsActive { get; set; }
    public string? Type { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "Code";
    public string? SortOrder { get; set; } = "ASC";
}

public class GetChartOfAccountsByIdQry : IRequest<ChartOfAccountsDto>
{
    public Guid Id { get; set; }
}

public class GetChartOfAccountsByCodeQry : IRequest<ChartOfAccountsDto>
{
    public string Code { get; set; } = string.Empty;
}

public class GetChartOfAccountsByTypeQry : IRequest<List<ChartOfAccountsDto>>
{
    public string AccountType { get; set; } = default!;
}

public class GetChartOfAccountsHierarchyQry : IRequest<List<ChartOfAccountsHierarchyDto>>
{
}

public class GetChartOfAccountsUsageQry : IRequest<ChartOfAccountsUsageDto>
{
    public Guid Id { get; set; }
}

public class CanDeleteChartOfAccountsQry : IRequest<CanDeleteResultDto>
{
    public Guid Id { get; set; }
}

public class ExportChartOfAccountsQry : IRequest<List<ChartOfAccountsDto>>
{
    public string? Type { get; set; }
    public bool? IsActive { get; set; }
}

// Queries/ChartOfAccountsQry.cs - GetChartOfAccountsUsageHandler

public class GetAllChartOfAccountsHandler : IRequestHandler<GetAllChartOfAccountsQry, PaginatedResponse<ChartOfAccountsDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAllChartOfAccountsHandler> _logger;

    public GetAllChartOfAccountsHandler(FinanceDbContext context, ILogger<GetAllChartOfAccountsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<ChartOfAccountsDto>> Handle(GetAllChartOfAccountsQry request, CancellationToken ct)
    {
        try
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Min(100, Math.Max(1, request.PageSize));

            var query = _context.ChartOfAccounts
                .Where(x => !x.IsDeleted)
                .Include(x => x.Category)
                .AsQueryable();

            // Apply filters
            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            if (!string.IsNullOrEmpty(request.Type))
                query = query.Where(x => x.AccountType == request.Type);

            if (!string.IsNullOrEmpty(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(x =>
                    (x.Code != null && x.Code.ToLower().Contains(search)) ||
                    (x.Name != null && x.Name.ToLower().Contains(search)) ||
                    (x.NameAm != null && x.NameAm.ToLower().Contains(search))
                );
            }

            // ✅ FIX: Get total count FIRST (sequential, not parallel)
            var totalCount = await query.CountAsync(ct);

            // Apply sorting
            query = request.SortBy?.ToLower() switch
            {
                "code" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Code)
                    : query.OrderBy(x => x.Code),
                "name" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Name)
                    : query.OrderBy(x => x.Name),
                "type" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.AccountType)
                    : query.OrderBy(x => x.AccountType),
                "level" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Level)
                    : query.OrderBy(x => x.Level),
                _ => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Code)
                    : query.OrderBy(x => x.Code)
            };

            // ✅ FIX: Get paginated data SECOND (sequential, not parallel)
            var accounts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ChartOfAccountsDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Name ?? string.Empty,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    AccountType = x.AccountType ?? string.Empty,
                    AccountSubType = x.AccountSubType,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Level = x.Level,
                    OpeningBalance = x.OpeningBalance,
                    CurrentBalance = x.CurrentBalance,
                    OpeningBalanceDate = x.OpeningBalanceDate,
                    PeriodId = x.PeriodId,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.Name : null,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Retrieved {Count} chart of accounts (Page {Page}/{TotalPages})",
                accounts.Count, page, (int)Math.Ceiling((double)totalCount / pageSize));

            return new PaginatedResponse<ChartOfAccountsDto>
            {
                Data = accounts,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = page < (int)Math.Ceiling((double)totalCount / pageSize),
                HasPreviousPage = page > 1
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chart of accounts");
            throw;
        }
    }
}

// ============================================================
// GET CHART OF ACCOUNTS USAGE HANDLER - FIXED ✅
// ============================================================

public class GetChartOfAccountsUsageHandler : IRequestHandler<GetChartOfAccountsUsageQry, ChartOfAccountsUsageDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetChartOfAccountsUsageHandler> _logger;

    public GetChartOfAccountsUsageHandler(FinanceDbContext context, ILogger<GetChartOfAccountsUsageHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ChartOfAccountsUsageDto> Handle(GetChartOfAccountsUsageQry request, CancellationToken ct)
    {
        try
        {
            // ✅ FIX: Get account info FIRST
            var chartOfAccounts = await _context.ChartOfAccounts
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .Select(x => new { x.Code, x.Name, x.CategoryId })
                .FirstOrDefaultAsync(ct);

            if (chartOfAccounts == null)
                throw new InvalidOperationException($"Chart of Accounts with ID '{request.Id}' not found");

            // ✅ FIX: Get journal lines SECOND (sequential)
            var journalLines = await _context.JournalLines
                .Where(x => x.AccountId == request.Id && !x.IsDeleted)
                .Select(x => new { x.Amount, x.Direction })
                .ToListAsync(ct);

            var journalLineCount = journalLines.Count;

            var totalDebit = journalLines
                .Where(x => x.Direction == "Debit")
                .Sum(x => x.Amount);

            var totalCredit = journalLines
                .Where(x => x.Direction == "Credit")
                .Sum(x => x.Amount);

            // ✅ FIX: Check for children THIRD (sequential)
            var hasChildren = await _context.ChartOfAccounts
                .AnyAsync(x => x.ParentId == request.Id && !x.IsDeleted, ct);

            var canBeDeleted = journalLineCount == 0 && !hasChildren;

            string? reason = null;
            if (!canBeDeleted)
            {
                if (hasChildren && journalLineCount > 0)
                    reason = $"Chart of Accounts has {journalLineCount} journal line(s) and child accounts.";
                else if (hasChildren)
                    reason = "Chart of Accounts has child accounts. Please delete or reassign child accounts first.";
                else if (journalLineCount > 0)
                    reason = $"Chart of Accounts is used in {journalLineCount} journal line(s). Please remove or reassign these transactions first.";
            }

            _logger.LogInformation("✅ Chart of Accounts usage retrieved for {Code}: {Count} lines",
                chartOfAccounts.Code, journalLineCount);

            return new ChartOfAccountsUsageDto
            {
                ChartOfAccountsId = request.Id,
                AccountId = request.Id.ToString(),
                Code = chartOfAccounts.Code ?? string.Empty,
                Name = chartOfAccounts.Name ?? "Unknown",
                AccountName = chartOfAccounts.Name ?? "Unknown",
                AccountCode = chartOfAccounts.Code ?? string.Empty,
                JournalLineCount = journalLineCount,
                TransactionCount = journalLineCount,
                JournalEntryCount = journalLineCount,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                HasChildren = hasChildren,
                CanDelete = canBeDeleted,
                CanBeDeleted = canBeDeleted,
                Reason = reason
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chart of accounts usage for ID: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// GET CHART OF ACCOUNTS BY ID HANDLER - FIXED ✅
// ============================================================

public class GetChartOfAccountsByIdHandler : IRequestHandler<GetChartOfAccountsByIdQry, ChartOfAccountsDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetChartOfAccountsByIdHandler> _logger;

    public GetChartOfAccountsByIdHandler(FinanceDbContext context, ILogger<GetChartOfAccountsByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ChartOfAccountsDto> Handle(GetChartOfAccountsByIdQry request, CancellationToken ct)
    {
        try
        {
            var chartOfAccounts = await _context.ChartOfAccounts
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .Select(x => new ChartOfAccountsDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Name ?? string.Empty,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    AccountType = x.AccountType ?? string.Empty,
                    AccountSubType = x.AccountSubType,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Level = x.Level,
                    OpeningBalance = x.OpeningBalance,
                    CurrentBalance = x.CurrentBalance,
                    OpeningBalanceDate = x.OpeningBalanceDate,
                    PeriodId = x.PeriodId,
                    CategoryId = x.CategoryId, // ✅ ADD THIS
                    CategoryName = x.Category != null ? x.Category.Name : null, // ✅ ADD THIS
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .FirstOrDefaultAsync(ct);

            if (chartOfAccounts == null)
                throw new InvalidOperationException($"Chart of Accounts with ID '{request.Id}' not found");

            return chartOfAccounts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chart of accounts by ID: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// GET CHART OF ACCOUNTS BY CODE HANDLER - FIXED ✅
// ============================================================

public class GetChartOfAccountsByCodeHandler : IRequestHandler<GetChartOfAccountsByCodeQry, ChartOfAccountsDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetChartOfAccountsByCodeHandler> _logger;

    public GetChartOfAccountsByCodeHandler(FinanceDbContext context, ILogger<GetChartOfAccountsByCodeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ChartOfAccountsDto> Handle(GetChartOfAccountsByCodeQry request, CancellationToken ct)
    {
        try
        {
            var chartOfAccounts = await _context.ChartOfAccounts
                .Where(x => x.Code == request.Code && !x.IsDeleted)
                .Select(x => new ChartOfAccountsDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Name ?? string.Empty,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    AccountType = x.AccountType ?? string.Empty,
                    AccountSubType = x.AccountSubType,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Level = x.Level,
                    OpeningBalance = x.OpeningBalance,
                    CurrentBalance = x.CurrentBalance,
                    OpeningBalanceDate = x.OpeningBalanceDate,
                    PeriodId = x.PeriodId,
                    CategoryId = x.CategoryId, // ✅ ADD THIS
                    CategoryName = x.Category != null ? x.Category.Name : null, // ✅ ADD THIS
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .FirstOrDefaultAsync(ct);

            if (chartOfAccounts == null)
                throw new InvalidOperationException($"Chart of Accounts with code '{request.Code}' not found");

            return chartOfAccounts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chart of accounts by code: {Code}", request.Code);
            throw;
        }
    }
}

// ============================================================
// GET CHART OF ACCOUNTS BY TYPE HANDLER - FIXED ✅
// ============================================================

public class GetChartOfAccountsByTypeHandler : IRequestHandler<GetChartOfAccountsByTypeQry, List<ChartOfAccountsDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetChartOfAccountsByTypeHandler> _logger;

    public GetChartOfAccountsByTypeHandler(FinanceDbContext context, ILogger<GetChartOfAccountsByTypeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ChartOfAccountsDto>> Handle(GetChartOfAccountsByTypeQry request, CancellationToken ct)
    {
        try
        {
            var accounts = await _context.ChartOfAccounts
                .Where(x => x.AccountType == request.AccountType && !x.IsDeleted)
                .OrderBy(x => x.Code)
                .Select(x => new ChartOfAccountsDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Name ?? string.Empty,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    AccountType = x.AccountType ?? string.Empty,
                    AccountSubType = x.AccountSubType,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Level = x.Level,
                    OpeningBalance = x.OpeningBalance,
                    CurrentBalance = x.CurrentBalance,
                    OpeningBalanceDate = x.OpeningBalanceDate,
                    PeriodId = x.PeriodId,
                    CategoryId = x.CategoryId, // ✅ ADD THIS
                    CategoryName = x.Category != null ? x.Category.Name : null, // ✅ ADD THIS
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Retrieved {Count} chart of accounts of type '{Type}'", accounts.Count, request.AccountType);
            return accounts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chart of accounts by type: {Type}", request.AccountType);
            throw;
        }
    }
}

// ============================================================
// GET CHART OF ACCOUNTS HIERARCHY HANDLER - FIXED ✅
// ============================================================

// Queries/ChartOfAccountsQry.cs - GetChartOfAccountsHierarchyHandler

// Queries/ChartOfAccountsQry.cs - GetChartOfAccountsHierarchyHandler

public class GetChartOfAccountsHierarchyHandler : IRequestHandler<GetChartOfAccountsHierarchyQry, List<ChartOfAccountsHierarchyDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetChartOfAccountsHierarchyHandler> _logger;

    public async Task<List<ChartOfAccountsHierarchyDto>> Handle(GetChartOfAccountsHierarchyQry request, CancellationToken ct)
    {
        try
        {
            // ✅ FIX: Include Category in the query
            var accounts = await _context.ChartOfAccounts
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Code)
                .Select(x => new ChartOfAccountsHierarchyDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Name ?? string.Empty,
                    NameAm = x.NameAm,
                    AccountType = x.AccountType,
                    AccountSubType = x.AccountSubType,
                    Description = x.Description,
                    Level = x.Level,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    OpeningBalance = x.OpeningBalance,
                    CurrentBalance = x.CurrentBalance,
                    OpeningBalanceDate = x.OpeningBalanceDate,
                    CategoryId = x.CategoryId, // ✅ ADD THIS
                    CategoryName = x.Category != null ? x.Category.Name : null, // ✅ ADD THIS
                    Children = new List<ChartOfAccountsHierarchyDto>(),
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            // Build hierarchy
            var accountDict = accounts.ToDictionary(x => x.Id);
            var rootAccounts = new List<ChartOfAccountsHierarchyDto>();

            foreach (var acc in accounts)
            {
                if (acc.ParentId.HasValue && accountDict.ContainsKey(acc.ParentId.Value))
                {
                    accountDict[acc.ParentId.Value].Children.Add(acc);
                }
                else
                {
                    rootAccounts.Add(acc);
                }
            }

            // Sort children by code
            foreach (var acc in accountDict.Values)
            {
                acc.Children = acc.Children.OrderBy(x => x.Code).ToList();
            }

            _logger.LogInformation("✅ Retrieved chart of accounts hierarchy with {Count} root nodes", rootAccounts.Count);
            return rootAccounts.OrderBy(x => x.Code).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chart of accounts hierarchy");
            throw;
        }
    }
}

// ============================================================
// EXPORT CHART OF ACCOUNTS HANDLER - FIXED ✅
// ============================================================

public class ExportChartOfAccountsHandler : IRequestHandler<ExportChartOfAccountsQry, List<ChartOfAccountsDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<ExportChartOfAccountsHandler> _logger;

    public ExportChartOfAccountsHandler(FinanceDbContext context, ILogger<ExportChartOfAccountsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ChartOfAccountsDto>> Handle(ExportChartOfAccountsQry request, CancellationToken ct)
    {
        try
        {
            var query = _context.ChartOfAccounts
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            if (!string.IsNullOrEmpty(request.Type))
                query = query.Where(x => x.AccountType == request.Type);

            var accounts = await query
                .OrderBy(x => x.Code)
                .Select(x => new ChartOfAccountsDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Name ?? string.Empty,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    AccountType = x.AccountType ?? string.Empty,
                    AccountSubType = x.AccountSubType,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Level = x.Level,
                    OpeningBalance = x.OpeningBalance,
                    CurrentBalance = x.CurrentBalance,
                    OpeningBalanceDate = x.OpeningBalanceDate,
                    PeriodId = x.PeriodId,
                    CategoryId = x.CategoryId, // ✅ ADD THIS
                    CategoryName = x.Category != null ? x.Category.Name : null, // ✅ ADD THIS
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Exported {Count} chart of accounts", accounts.Count);
            return accounts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting chart of accounts");
            throw;
        }
    }
}
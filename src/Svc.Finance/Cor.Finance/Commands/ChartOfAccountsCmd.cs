// Commands/ChartOfAccountsCmd.cs - WITH CACHE INVALIDATION ✅

using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Cor.Finance.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class AddChartOfAccountsCmd : IRequest<ChartOfAccountsDto>
{
    public AddChartOfAccountsDto AddDto { get; set; } = new();
}

public class EditChartOfAccountsCmd : IRequest<ChartOfAccountsDto>
{
    public EditChartOfAccountsDto EditDto { get; set; } = new();
}

public class DeleteChartOfAccountsCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ToggleChartOfAccountsActiveCmd : IRequest<ToggleActiveResultDto>
{
    public Guid Id { get; set; }
}

public class BulkDeleteChartOfAccountsCmd : IRequest<BulkDeleteResultDto>
{
    public List<Guid> Ids { get; set; } = new();
}

public class BulkAddChartOfAccountsCmd : IRequest<List<ChartOfAccountsDto>>
{
    public List<AddChartOfAccountsDto> AddDtos { get; set; } = new();
}

// ============================================================
// ADD CHART OF ACCOUNTS HANDLER - WITH CACHE INVALIDATION ✅
// ============================================================

public class AddChartOfAccountsHandler : IRequestHandler<AddChartOfAccountsCmd, ChartOfAccountsDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<AddChartOfAccountsHandler> _logger;
    private readonly IMemoryCache _cache;

    public AddChartOfAccountsHandler(
        FinanceDbContext context,
        ILogger<AddChartOfAccountsHandler> logger,
        IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ChartOfAccountsDto> Handle(AddChartOfAccountsCmd request, CancellationToken ct)
    {
        try
        {
            var exists = await _context.ChartOfAccounts
                .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);

            if (exists)
                throw new InvalidOperationException($"Chart of Accounts with code '{request.AddDto.Code}' already exists");

            var chartOfAccounts = new ChartOfAccounts
            {
                Id = Guid.NewGuid(),
                Code = request.AddDto.Code,
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                Description = request.AddDto.Description,
                AccountType = request.AddDto.AccountType,
                AccountSubType = request.AddDto.AccountSubType,
                IsActive = request.AddDto.IsActive,
                ParentId = request.AddDto.ParentId,
                Level = request.AddDto.Level,
                OpeningBalance = request.AddDto.OpeningBalance ?? 0,
                CurrentBalance = request.AddDto.OpeningBalance ?? 0,
                OpeningBalanceDate = request.AddDto.OpeningBalanceDate,
                PeriodId = request.AddDto.PeriodId,
                CategoryId = request.AddDto.CategoryId,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ChartOfAccounts.Add(chartOfAccounts);
            await _context.SaveChangesAsync(ct);

            // ✅ Invalidate cache after successful creation
            InvalidateCache();

            _logger.LogInformation("✅ Chart of Accounts created: {Code} - {Name}", chartOfAccounts.Code, chartOfAccounts.Name);

            // Get category name for response
            string? categoryName = null;
            if (chartOfAccounts.CategoryId.HasValue)
            {
                var category = await _context.AccountCategories
                    .Where(x => x.Id == chartOfAccounts.CategoryId.Value && !x.IsDeleted)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync(ct);
                categoryName = category;
            }

            return new ChartOfAccountsDto
            {
                Id = chartOfAccounts.Id,
                Code = chartOfAccounts.Code ?? string.Empty,
                Name = chartOfAccounts.Name ?? string.Empty,
                NameAm = chartOfAccounts.NameAm,
                Description = chartOfAccounts.Description,
                AccountType = chartOfAccounts.AccountType ?? string.Empty,
                AccountSubType = chartOfAccounts.AccountSubType,
                IsActive = chartOfAccounts.IsActive,
                ParentId = chartOfAccounts.ParentId,
                Level = chartOfAccounts.Level,
                OpeningBalance = chartOfAccounts.OpeningBalance,
                CurrentBalance = chartOfAccounts.CurrentBalance,
                OpeningBalanceDate = chartOfAccounts.OpeningBalanceDate,
                PeriodId = chartOfAccounts.PeriodId,
                CategoryId = chartOfAccounts.CategoryId,
                CategoryName = categoryName,
                DateAdd = chartOfAccounts.DateAdd,
                DateMod = chartOfAccounts.DateMod
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating chart of accounts");
            throw;
        }
    }

    private void InvalidateCache()
    {
        _cache.Remove("accounts_list");
        _cache.Remove("accounts_hierarchy");
        _logger.LogInformation("🗑️ Accounts cache invalidated after create");
    }
}

// ============================================================
// EDIT CHART OF ACCOUNTS HANDLER - WITH CACHE INVALIDATION ✅
// ============================================================

public class EditChartOfAccountsHandler : IRequestHandler<EditChartOfAccountsCmd, ChartOfAccountsDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<EditChartOfAccountsHandler> _logger;
    private readonly IMemoryCache _cache;

    public EditChartOfAccountsHandler(
        FinanceDbContext context,
        ILogger<EditChartOfAccountsHandler> logger,
        IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ChartOfAccountsDto> Handle(EditChartOfAccountsCmd request, CancellationToken ct)
    {
        try
        {
            var chartOfAccounts = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

            if (chartOfAccounts == null)
                throw new InvalidOperationException($"Chart of Accounts with ID '{request.EditDto.Id}' not found");

            var exists = await _context.ChartOfAccounts
                .AnyAsync(x => x.Code == request.EditDto.Code && x.Id != request.EditDto.Id && !x.IsDeleted, ct);

            if (exists)
                throw new InvalidOperationException($"Chart of Accounts with code '{request.EditDto.Code}' already exists");

            chartOfAccounts.Code = request.EditDto.Code;
            chartOfAccounts.Name = request.EditDto.Name;
            chartOfAccounts.NameAm = request.EditDto.NameAm;
            chartOfAccounts.Description = request.EditDto.Description;
            chartOfAccounts.AccountType = request.EditDto.AccountType;
            chartOfAccounts.AccountSubType = request.EditDto.AccountSubType;
            chartOfAccounts.IsActive = request.EditDto.IsActive;
            chartOfAccounts.ParentId = request.EditDto.ParentId;
            chartOfAccounts.Level = request.EditDto.Level;
            chartOfAccounts.OpeningBalance = request.EditDto.OpeningBalance ?? 0;
            chartOfAccounts.CurrentBalance = request.EditDto.CurrentBalance ?? 0;
            chartOfAccounts.OpeningBalanceDate = request.EditDto.OpeningBalanceDate;
            chartOfAccounts.PeriodId = request.EditDto.PeriodId;
            chartOfAccounts.CategoryId = request.EditDto.CategoryId;
            chartOfAccounts.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            // ✅ Invalidate cache after successful update
            InvalidateCache();

            _logger.LogInformation("✅ Chart of Accounts updated: {Code} - {Name}", chartOfAccounts.Code, chartOfAccounts.Name);

            // Get category name for response
            string? categoryName = null;
            if (chartOfAccounts.CategoryId.HasValue)
            {
                var category = await _context.AccountCategories
                    .Where(x => x.Id == chartOfAccounts.CategoryId.Value && !x.IsDeleted)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync(ct);
                categoryName = category;
            }

            return new ChartOfAccountsDto
            {
                Id = chartOfAccounts.Id,
                Code = chartOfAccounts.Code ?? string.Empty,
                Name = chartOfAccounts.Name ?? string.Empty,
                NameAm = chartOfAccounts.NameAm,
                Description = chartOfAccounts.Description,
                AccountType = chartOfAccounts.AccountType ?? string.Empty,
                AccountSubType = chartOfAccounts.AccountSubType,
                IsActive = chartOfAccounts.IsActive,
                ParentId = chartOfAccounts.ParentId,
                Level = chartOfAccounts.Level,
                OpeningBalance = chartOfAccounts.OpeningBalance,
                CurrentBalance = chartOfAccounts.CurrentBalance,
                OpeningBalanceDate = chartOfAccounts.OpeningBalanceDate,
                PeriodId = chartOfAccounts.PeriodId,
                CategoryId = chartOfAccounts.CategoryId,
                CategoryName = categoryName,
                DateAdd = chartOfAccounts.DateAdd,
                DateMod = chartOfAccounts.DateMod
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating chart of accounts: {Id}", request.EditDto.Id);
            throw;
        }
    }

    private void InvalidateCache()
    {
        _cache.Remove("accounts_list");
        _cache.Remove("accounts_hierarchy");
        _logger.LogInformation("🗑️ Accounts cache invalidated after update");
    }
}

// ============================================================
// DELETE CHART OF ACCOUNTS HANDLER - WITH CACHE INVALIDATION ✅
// ============================================================

public class DeleteChartOfAccountsHandler : IRequestHandler<DeleteChartOfAccountsCmd, bool>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<DeleteChartOfAccountsHandler> _logger;
    private readonly IMemoryCache _cache;

    public DeleteChartOfAccountsHandler(
        FinanceDbContext context,
        ILogger<DeleteChartOfAccountsHandler> logger,
        IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteChartOfAccountsCmd request, CancellationToken ct)
    {
        try
        {
            var chartOfAccounts = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (chartOfAccounts == null)
                return false;

            var hasEntries = await _context.JournalLines
                .AnyAsync(x => x.AccountId == request.Id && !x.IsDeleted, ct);

            if (hasEntries)
                throw new InvalidOperationException("Cannot delete chart of accounts with journal entries");

            var hasChildren = await _context.ChartOfAccounts
                .AnyAsync(x => x.ParentId == request.Id && !x.IsDeleted, ct);

            if (hasChildren)
                throw new InvalidOperationException("Cannot delete chart of accounts with child accounts");

            chartOfAccounts.IsDeleted = true;
            chartOfAccounts.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            // ✅ Invalidate cache after successful delete
            InvalidateCache();

            _logger.LogInformation("✅ Chart of Accounts deleted: {Code} - {Name}", chartOfAccounts.Code, chartOfAccounts.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting chart of accounts: {Id}", request.Id);
            throw;
        }
    }

    private void InvalidateCache()
    {
        _cache.Remove("accounts_list");
        _cache.Remove("accounts_hierarchy");
        _logger.LogInformation("🗑️ Accounts cache invalidated after delete");
    }
}

// ============================================================
// TOGGLE CHART OF ACCOUNTS ACTIVE HANDLER - WITH CACHE INVALIDATION ✅
// ============================================================

public class ToggleChartOfAccountsActiveHandler : IRequestHandler<ToggleChartOfAccountsActiveCmd, ToggleActiveResultDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<ToggleChartOfAccountsActiveHandler> _logger;
    private readonly IMemoryCache _cache;

    public ToggleChartOfAccountsActiveHandler(
        FinanceDbContext context,
        ILogger<ToggleChartOfAccountsActiveHandler> logger,
        IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ToggleActiveResultDto> Handle(ToggleChartOfAccountsActiveCmd request, CancellationToken ct)
    {
        try
        {
            var chartOfAccounts = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (chartOfAccounts == null)
                throw new InvalidOperationException($"Chart of Accounts with ID '{request.Id}' not found");

            chartOfAccounts.IsActive = !chartOfAccounts.IsActive;
            chartOfAccounts.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            // ✅ Invalidate cache after toggle
            InvalidateCache();

            _logger.LogInformation("✅ Chart of Accounts status toggled: {Code} - IsActive: {IsActive}",
                chartOfAccounts.Code, chartOfAccounts.IsActive);

            return new ToggleActiveResultDto
            {
                Id = chartOfAccounts.Id,
                IsActive = chartOfAccounts.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling chart of accounts status: {Id}", request.Id);
            throw;
        }
    }

    private void InvalidateCache()
    {
        _cache.Remove("accounts_list");
        _cache.Remove("accounts_hierarchy");
        _logger.LogInformation("🗑️ Accounts cache invalidated after toggle");
    }
}

// ============================================================
// BULK DELETE CHART OF ACCOUNTS HANDLER - WITH CACHE INVALIDATION ✅
// ============================================================

public class BulkDeleteChartOfAccountsHandler : IRequestHandler<BulkDeleteChartOfAccountsCmd, BulkDeleteResultDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<BulkDeleteChartOfAccountsHandler> _logger;
    private readonly IMemoryCache _cache;

    public BulkDeleteChartOfAccountsHandler(
        FinanceDbContext context,
        ILogger<BulkDeleteChartOfAccountsHandler> logger,
        IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<BulkDeleteResultDto> Handle(BulkDeleteChartOfAccountsCmd request, CancellationToken ct)
    {
        var result = new BulkDeleteResultDto();
        var errors = new List<BulkDeleteErrorDto>();

        foreach (var id in request.Ids)
        {
            try
            {
                var chartOfAccounts = await _context.ChartOfAccounts
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

                if (chartOfAccounts == null)
                {
                    errors.Add(new BulkDeleteErrorDto { Id = id, Error = $"Chart of Accounts with ID '{id}' not found" });
                    result.FailedCount++;
                    continue;
                }

                var hasEntries = await _context.JournalLines
                    .AnyAsync(x => x.AccountId == id && !x.IsDeleted, ct);

                if (hasEntries)
                {
                    errors.Add(new BulkDeleteErrorDto { Id = id, Error = $"Chart of Accounts '{chartOfAccounts.Code}' has journal entries" });
                    result.FailedCount++;
                    continue;
                }

                var hasChildren = await _context.ChartOfAccounts
                    .AnyAsync(x => x.ParentId == id && !x.IsDeleted, ct);

                if (hasChildren)
                {
                    errors.Add(new BulkDeleteErrorDto { Id = id, Error = $"Chart of Accounts '{chartOfAccounts.Code}' has child accounts" });
                    result.FailedCount++;
                    continue;
                }

                chartOfAccounts.IsDeleted = true;
                chartOfAccounts.DateMod = DateTime.UtcNow;
                result.DeletedCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new BulkDeleteErrorDto { Id = id, Error = $"Error deleting chart of accounts '{id}': {ex.Message}" });
                result.FailedCount++;
            }
        }

        await _context.SaveChangesAsync(ct);
        result.Errors = errors;

        // ✅ Invalidate cache after bulk delete
        InvalidateCache();

        _logger.LogInformation("✅ Bulk delete completed: {DeletedCount} deleted, {FailedCount} failed",
            result.DeletedCount, result.FailedCount);

        return result;
    }

    private void InvalidateCache()
    {
        _cache.Remove("accounts_list");
        _cache.Remove("accounts_hierarchy");
        _logger.LogInformation("🗑️ Accounts cache invalidated after bulk delete");
    }
}

// ============================================================
// BULK ADD CHART OF ACCOUNTS HANDLER - WITH CACHE INVALIDATION ✅
// ============================================================

public class BulkAddChartOfAccountsHandler : IRequestHandler<BulkAddChartOfAccountsCmd, List<ChartOfAccountsDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<BulkAddChartOfAccountsHandler> _logger;
    private readonly IMemoryCache _cache;

    public BulkAddChartOfAccountsHandler(
        FinanceDbContext context,
        ILogger<BulkAddChartOfAccountsHandler> logger,
        IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<ChartOfAccountsDto>> Handle(BulkAddChartOfAccountsCmd request, CancellationToken ct)
    {
        var results = new List<ChartOfAccountsDto>();
        var errors = new List<string>();

        foreach (var dto in request.AddDtos)
        {
            try
            {
                var exists = await _context.ChartOfAccounts
                    .AnyAsync(x => x.Code == dto.Code && !x.IsDeleted, ct);

                if (exists)
                {
                    errors.Add($"Chart of Accounts with code '{dto.Code}' already exists.");
                    continue;
                }

                var chartOfAccounts = new ChartOfAccounts
                {
                    Id = Guid.NewGuid(),
                    Code = dto.Code,
                    Name = dto.Name,
                    NameAm = dto.NameAm,
                    AccountType = dto.AccountType,
                    AccountSubType = dto.AccountSubType,
                    Description = dto.Description,
                    IsActive = true,
                    ParentId = dto.ParentId,
                    Level = dto.Level,
                    OpeningBalance = dto.OpeningBalance ?? 0,
                    CurrentBalance = dto.CurrentBalance ?? 0,
                    OpeningBalanceDate = dto.OpeningBalanceDate,
                    CategoryId = dto.CategoryId, // ✅ ADD THIS
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false,
                    RowVersion = string.Empty
                };

                _context.ChartOfAccounts.Add(chartOfAccounts);

                // Get category name for response
                string? categoryName = null;
                if (chartOfAccounts.CategoryId.HasValue)
                {
                    var category = await _context.AccountCategories
                        .Where(x => x.Id == chartOfAccounts.CategoryId.Value && !x.IsDeleted)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync(ct);
                    categoryName = category;
                }

                results.Add(new ChartOfAccountsDto
                {
                    Id = chartOfAccounts.Id,
                    Code = chartOfAccounts.Code ?? string.Empty,
                    Name = chartOfAccounts.Name ?? string.Empty,
                    NameAm = chartOfAccounts.NameAm,
                    AccountType = chartOfAccounts.AccountType ?? string.Empty,
                    AccountSubType = chartOfAccounts.AccountSubType,
                    IsActive = chartOfAccounts.IsActive,
                    ParentId = chartOfAccounts.ParentId,
                    Level = chartOfAccounts.Level,
                    Description = chartOfAccounts.Description,
                    OpeningBalance = chartOfAccounts.OpeningBalance,
                    CurrentBalance = chartOfAccounts.CurrentBalance,
                    OpeningBalanceDate = chartOfAccounts.OpeningBalanceDate,
                    CategoryId = chartOfAccounts.CategoryId,
                    CategoryName = categoryName,
                    DateAdd = chartOfAccounts.DateAdd,
                    DateMod = chartOfAccounts.DateMod
                });
            }
            catch (Exception ex)
            {
                errors.Add($"Error creating chart of accounts '{dto.Code}': {ex.Message}");
            }
        }

        await _context.SaveChangesAsync(ct);

        // ✅ Invalidate cache after bulk add
        InvalidateCache();

        if (errors.Any())
        {
            _logger.LogWarning("⚠️ Bulk add completed with errors: {Errors}", string.Join("; ", errors));
        }

        _logger.LogInformation("✅ Bulk add completed: {Count} chart of accounts created", results.Count);

        return results;
    }

    private void InvalidateCache()
    {
        _cache.Remove("accounts_list");
        _cache.Remove("accounts_hierarchy");
        _logger.LogInformation("🗑️ Accounts cache invalidated after bulk add");
    }
}
// Commands/AccountCategoryCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class AddAccountCategoryCmd : IRequest<AccountCategoryDto>
{
    public AddAccountCategoryDto AddDto { get; set; } = new();
}

public class EditAccountCategoryCmd : IRequest<AccountCategoryDto>
{
    public EditAccountCategoryDto EditDto { get; set; } = new();
}

public class DeleteAccountCategoryCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ToggleAccountCategoryActiveCmd : IRequest<ToggleActiveResultDto>
{
    public Guid Id { get; set; }
}

public class BulkDeleteAccountCategoryCmd : IRequest<BulkDeleteResultDto>
{
    public List<Guid> Ids { get; set; } = new();
}

public class BulkAddAccountCategoryCmd : IRequest<List<AccountCategoryDto>>
{
    public List<AddAccountCategoryDto> AddDtos { get; set; } = new();
}

// ============================================================
// ADD ACCOUNT CATEGORY HANDLER
// ============================================================

public class AddAccountCategoryHandler : IRequestHandler<AddAccountCategoryCmd, AccountCategoryDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<AddAccountCategoryHandler> _logger;

    public AddAccountCategoryHandler(FinanceDbContext context, ILogger<AddAccountCategoryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountCategoryDto> Handle(AddAccountCategoryCmd request, CancellationToken ct)
    {
        try
        {
            var exists = await _context.AccountCategories
                .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);

            if (exists)
                throw new InvalidOperationException($"Account Category with code '{request.AddDto.Code}' already exists");

            var category = new AccountCategory
            {
                Id = Guid.NewGuid(),
                Code = request.AddDto.Code,
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                Description = request.AddDto.Description,
                Type = request.AddDto.Type,
                IsActive = request.AddDto.IsActive,
                ParentId = request.AddDto.ParentId,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.AccountCategories.Add(category);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("✅ Account Category created: {Code} - {Name}", category.Code, category.Name);

            return new AccountCategoryDto
            {
                Id = category.Id,
                Code = category.Code,
                Name = category.Name,
                NameAm = category.NameAm,
                Description = category.Description,
                Type = category.Type,
                IsActive = category.IsActive,
                ParentId = category.ParentId,
                DateAdd = category.DateAdd,
                DateMod = category.DateMod
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account category");
            throw;
        }
    }
}

// ============================================================
// EDIT ACCOUNT CATEGORY HANDLER
// ============================================================

public class EditAccountCategoryHandler : IRequestHandler<EditAccountCategoryCmd, AccountCategoryDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<EditAccountCategoryHandler> _logger;

    public EditAccountCategoryHandler(FinanceDbContext context, ILogger<EditAccountCategoryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountCategoryDto> Handle(EditAccountCategoryCmd request, CancellationToken ct)
    {
        try
        {
            var category = await _context.AccountCategories
                .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

            if (category == null)
                throw new InvalidOperationException($"Account Category with ID '{request.EditDto.Id}' not found");

            var exists = await _context.AccountCategories
                .AnyAsync(x => x.Code == request.EditDto.Code && x.Id != request.EditDto.Id && !x.IsDeleted, ct);

            if (exists)
                throw new InvalidOperationException($"Account Category with code '{request.EditDto.Code}' already exists");

            category.Code = request.EditDto.Code;
            category.Name = request.EditDto.Name;
            category.NameAm = request.EditDto.NameAm;
            category.Description = request.EditDto.Description;
            category.Type = request.EditDto.Type;
            category.IsActive = request.EditDto.IsActive;
            category.ParentId = request.EditDto.ParentId;
            category.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("✅ Account Category updated: {Code} - {Name}", category.Code, category.Name);

            return new AccountCategoryDto
            {
                Id = category.Id,
                Code = category.Code,
                Name = category.Name,
                NameAm = category.NameAm,
                Description = category.Description,
                Type = category.Type,
                IsActive = category.IsActive,
                ParentId = category.ParentId,
                DateAdd = category.DateAdd,
                DateMod = category.DateMod
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account category: {Id}", request.EditDto.Id);
            throw;
        }
    }
}

// ============================================================
// DELETE ACCOUNT CATEGORY HANDLER
// ============================================================

public class DeleteAccountCategoryHandler : IRequestHandler<DeleteAccountCategoryCmd, bool>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<DeleteAccountCategoryHandler> _logger;

    public DeleteAccountCategoryHandler(FinanceDbContext context, ILogger<DeleteAccountCategoryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteAccountCategoryCmd request, CancellationToken ct)
    {
        try
        {
            var category = await _context.AccountCategories
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (category == null)
                return false;

            // Check if category has child categories
            var hasChildren = await _context.AccountCategories
                .AnyAsync(x => x.ParentId == request.Id && !x.IsDeleted, ct);

            if (hasChildren)
                throw new InvalidOperationException("Cannot delete category with child categories");

            // Check if category has accounts
            var hasAccounts = await _context.ChartOfAccounts
                .AnyAsync(x => x.CategoryId == request.Id && !x.IsDeleted, ct);

            if (hasAccounts)
                throw new InvalidOperationException("Cannot delete category with associated accounts");

            category.IsDeleted = true;
            category.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("✅ Account Category deleted: {Code} - {Name}", category.Code, category.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account category: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// TOGGLE ACCOUNT CATEGORY ACTIVE HANDLER
// ============================================================

public class ToggleAccountCategoryActiveHandler : IRequestHandler<ToggleAccountCategoryActiveCmd, ToggleActiveResultDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<ToggleAccountCategoryActiveHandler> _logger;

    public ToggleAccountCategoryActiveHandler(FinanceDbContext context, ILogger<ToggleAccountCategoryActiveHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ToggleActiveResultDto> Handle(ToggleAccountCategoryActiveCmd request, CancellationToken ct)
    {
        try
        {
            var category = await _context.AccountCategories
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (category == null)
                throw new InvalidOperationException($"Account Category with ID '{request.Id}' not found");

            category.IsActive = !category.IsActive;
            category.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("✅ Account Category status toggled: {Code} - IsActive: {IsActive}",
                category.Code, category.IsActive);

            return new ToggleActiveResultDto
            {
                Id = category.Id,
                IsActive = category.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling account category status: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// BULK DELETE ACCOUNT CATEGORY HANDLER
// ============================================================

public class BulkDeleteAccountCategoryHandler : IRequestHandler<BulkDeleteAccountCategoryCmd, BulkDeleteResultDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<BulkDeleteAccountCategoryHandler> _logger;

    public BulkDeleteAccountCategoryHandler(FinanceDbContext context, ILogger<BulkDeleteAccountCategoryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BulkDeleteResultDto> Handle(BulkDeleteAccountCategoryCmd request, CancellationToken ct)
    {
        var result = new BulkDeleteResultDto();
        var errors = new List<BulkDeleteErrorDto>();

        foreach (var id in request.Ids)
        {
            try
            {
                var category = await _context.AccountCategories
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

                if (category == null)
                {
                    errors.Add(new BulkDeleteErrorDto { Id = id, Error = $"Account Category with ID '{id}' not found" });
                    result.FailedCount++;
                    continue;
                }

                var hasChildren = await _context.AccountCategories
                    .AnyAsync(x => x.ParentId == id && !x.IsDeleted, ct);

                if (hasChildren)
                {
                    errors.Add(new BulkDeleteErrorDto { Id = id, Error = $"Category '{category.Code}' has child categories" });
                    result.FailedCount++;
                    continue;
                }

                var hasAccounts = await _context.ChartOfAccounts
                    .AnyAsync(x => x.CategoryId == id && !x.IsDeleted, ct);

                if (hasAccounts)
                {
                    errors.Add(new BulkDeleteErrorDto { Id = id, Error = $"Category '{category.Code}' has associated accounts" });
                    result.FailedCount++;
                    continue;
                }

                category.IsDeleted = true;
                category.DateMod = DateTime.UtcNow;
                result.DeletedCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new BulkDeleteErrorDto { Id = id, Error = $"Error deleting category '{id}': {ex.Message}" });
                result.FailedCount++;
            }
        }

        await _context.SaveChangesAsync(ct);
        result.Errors = errors;

        _logger.LogInformation("✅ Bulk delete completed: {DeletedCount} deleted, {FailedCount} failed",
            result.DeletedCount, result.FailedCount);

        return result;
    }
}

// ============================================================
// BULK ADD ACCOUNT CATEGORY HANDLER
// ============================================================

public class BulkAddAccountCategoryHandler : IRequestHandler<BulkAddAccountCategoryCmd, List<AccountCategoryDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<BulkAddAccountCategoryHandler> _logger;

    public BulkAddAccountCategoryHandler(FinanceDbContext context, ILogger<BulkAddAccountCategoryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AccountCategoryDto>> Handle(BulkAddAccountCategoryCmd request, CancellationToken ct)
    {
        var results = new List<AccountCategoryDto>();
        var errors = new List<string>();

        foreach (var dto in request.AddDtos)
        {
            try
            {
                var exists = await _context.AccountCategories
                    .AnyAsync(x => x.Code == dto.Code && !x.IsDeleted, ct);

                if (exists)
                {
                    errors.Add($"Account Category with code '{dto.Code}' already exists.");
                    continue;
                }

                var category = new AccountCategory
                {
                    Id = Guid.NewGuid(),
                    Code = dto.Code,
                    Name = dto.Name,
                    NameAm = dto.NameAm,
                    Description = dto.Description,
                    Type = dto.Type,
                    IsActive = dto.IsActive,
                    ParentId = dto.ParentId,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.AccountCategories.Add(category);

                results.Add(new AccountCategoryDto
                {
                    Id = category.Id,
                    Code = category.Code,
                    Name = category.Name,
                    NameAm = category.NameAm,
                    Description = category.Description,
                    Type = category.Type,
                    IsActive = category.IsActive,
                    ParentId = category.ParentId,
                    DateAdd = category.DateAdd,
                    DateMod = category.DateMod
                });
            }
            catch (Exception ex)
            {
                errors.Add($"Error creating account category '{dto.Code}': {ex.Message}");
            }
        }

        await _context.SaveChangesAsync(ct);

        if (errors.Any())
        {
            _logger.LogWarning("⚠️ Bulk add completed with errors: {Errors}", string.Join("; ", errors));
        }

        _logger.LogInformation("✅ Bulk add completed: {Count} account categories created", results.Count);

        return results;
    }
}
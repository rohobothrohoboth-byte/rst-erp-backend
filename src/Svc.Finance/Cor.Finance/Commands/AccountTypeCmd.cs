// Cor.Finance/Commands/AccountTypeCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

// ============================================================
// ACCOUNT TYPE COMMANDS
// ============================================================

public class CreateAccountTypeCmd : IRequest<AccountTypeDto>
{
    public CreateAccountTypeDto Dto { get; set; } = new();
}

public class UpdateAccountTypeCmd : IRequest<AccountTypeDto>
{
    public UpdateAccountTypeDto Dto { get; set; } = new();
}

public class DeleteAccountTypeCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ToggleAccountTypeActiveCmd : IRequest<ToggleActiveResultDto>
{
    public Guid Id { get; set; }
}

public class BulkDeleteAccountTypesCmd : IRequest<BulkDeleteResultDto>
{
    public List<Guid> Ids { get; set; } = new();
}

// ============================================================
// ACCOUNT SUBTYPE COMMANDS
// ============================================================

public class CreateAccountSubtypeCmd : IRequest<AccountSubtypeDto>
{
    public CreateAccountSubtypeDto Dto { get; set; } = new();
}

public class UpdateAccountSubtypeCmd : IRequest<AccountSubtypeDto>
{
    public UpdateAccountSubtypeDto Dto { get; set; } = new();
}

public class DeleteAccountSubtypeCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ToggleAccountSubtypeActiveCmd : IRequest<ToggleActiveResultDto>
{
    public Guid Id { get; set; }
}

// ============================================================
// CREATE ACCOUNT TYPE HANDLER
// ============================================================

public class CreateAccountTypeHandler : IRequestHandler<CreateAccountTypeCmd, AccountTypeDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<CreateAccountTypeHandler> _logger;

    public CreateAccountTypeHandler(FinanceDbContext context, ILogger<CreateAccountTypeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountTypeDto> Handle(CreateAccountTypeCmd request, CancellationToken ct)
    {
        try
        {
            var exists = await _context.AccountTypes
                .AnyAsync(x => x.Code == request.Dto.Code && !x.IsDeleted, ct);

            if (exists)
                throw new InvalidOperationException($"Account Type with code '{request.Dto.Code}' already exists");

            var accountType = new AccountType
            {
                Id = Guid.NewGuid(),
                Code = request.Dto.Code,
                Name = request.Dto.Name,
                NameAm = request.Dto.NameAm,
                Description = request.Dto.Description,
                Category = request.Dto.Category,
                NormalBalance = request.Dto.NormalBalance,
                IsActive = request.Dto.IsActive,
                SortOrder = request.Dto.SortOrder,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.AccountTypes.Add(accountType);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("✅ Account Type created: {Code} - {Name}", accountType.Code, accountType.Name);

            return new AccountTypeDto
            {
                Id = accountType.Id,
                Code = accountType.Code,
                Name = accountType.Name,
                NameAm = accountType.NameAm,
                Description = accountType.Description,
                Category = accountType.Category,
                NormalBalance = accountType.NormalBalance,
                IsActive = accountType.IsActive,
                SortOrder = accountType.SortOrder,
                DateAdd = accountType.DateAdd,
                DateMod = accountType.DateMod,
                SubtypeCount = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account type");
            throw;
        }
    }
}

// ============================================================
// UPDATE ACCOUNT TYPE HANDLER
// ============================================================

public class UpdateAccountTypeHandler : IRequestHandler<UpdateAccountTypeCmd, AccountTypeDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<UpdateAccountTypeHandler> _logger;

    public UpdateAccountTypeHandler(FinanceDbContext context, ILogger<UpdateAccountTypeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountTypeDto> Handle(UpdateAccountTypeCmd request, CancellationToken ct)
    {
        try
        {
            var accountType = await _context.AccountTypes
                .FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct);

            if (accountType == null)
                throw new InvalidOperationException($"Account Type with ID '{request.Dto.Id}' not found");

            var exists = await _context.AccountTypes
                .AnyAsync(x => x.Code == request.Dto.Code && x.Id != request.Dto.Id && !x.IsDeleted, ct);

            if (exists)
                throw new InvalidOperationException($"Account Type with code '{request.Dto.Code}' already exists");

            accountType.Code = request.Dto.Code;
            accountType.Name = request.Dto.Name;
            accountType.NameAm = request.Dto.NameAm;
            accountType.Description = request.Dto.Description;
            accountType.Category = request.Dto.Category;
            accountType.NormalBalance = request.Dto.NormalBalance;
            accountType.IsActive = request.Dto.IsActive;
            accountType.SortOrder = request.Dto.SortOrder;
            accountType.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            var subtypeCount = await _context.AccountSubtypes
                .CountAsync(x => x.AccountTypeId == accountType.Id && !x.IsDeleted, ct);

            _logger.LogInformation("✅ Account Type updated: {Code} - {Name}", accountType.Code, accountType.Name);

            return new AccountTypeDto
            {
                Id = accountType.Id,
                Code = accountType.Code,
                Name = accountType.Name,
                NameAm = accountType.NameAm,
                Description = accountType.Description,
                Category = accountType.Category,
                NormalBalance = accountType.NormalBalance,
                IsActive = accountType.IsActive,
                SortOrder = accountType.SortOrder,
                DateAdd = accountType.DateAdd,
                DateMod = accountType.DateMod,
                SubtypeCount = subtypeCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account type: {Id}", request.Dto.Id);
            throw;
        }
    }
}

// ============================================================
// DELETE ACCOUNT TYPE HANDLER
// ============================================================

public class DeleteAccountTypeHandler : IRequestHandler<DeleteAccountTypeCmd, bool>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<DeleteAccountTypeHandler> _logger;

    public DeleteAccountTypeHandler(FinanceDbContext context, ILogger<DeleteAccountTypeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteAccountTypeCmd request, CancellationToken ct)
    {
        try
        {
            var accountType = await _context.AccountTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (accountType == null)
                return false;

            var hasSubtypes = await _context.AccountSubtypes
                .AnyAsync(x => x.AccountTypeId == request.Id && !x.IsDeleted, ct);

            if (hasSubtypes)
                throw new InvalidOperationException("Cannot delete account type with associated subtypes");

            accountType.IsDeleted = true;
            accountType.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("✅ Account Type deleted: {Code} - {Name}", accountType.Code, accountType.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account type: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// TOGGLE ACCOUNT TYPE ACTIVE HANDLER
// ============================================================

public class ToggleAccountTypeActiveHandler : IRequestHandler<ToggleAccountTypeActiveCmd, ToggleActiveResultDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<ToggleAccountTypeActiveHandler> _logger;

    public ToggleAccountTypeActiveHandler(FinanceDbContext context, ILogger<ToggleAccountTypeActiveHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ToggleActiveResultDto> Handle(ToggleAccountTypeActiveCmd request, CancellationToken ct)
    {
        try
        {
            var accountType = await _context.AccountTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (accountType == null)
                throw new InvalidOperationException($"Account Type with ID '{request.Id}' not found");

            accountType.IsActive = !accountType.IsActive;
            accountType.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("✅ Account Type status toggled: {Code} - IsActive: {IsActive}",
                accountType.Code, accountType.IsActive);

            return new ToggleActiveResultDto
            {
                Id = accountType.Id,
                IsActive = accountType.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling account type status: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// BULK DELETE ACCOUNT TYPES HANDLER
// ============================================================

public class BulkDeleteAccountTypesHandler : IRequestHandler<BulkDeleteAccountTypesCmd, BulkDeleteResultDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<BulkDeleteAccountTypesHandler> _logger;

    public BulkDeleteAccountTypesHandler(FinanceDbContext context, ILogger<BulkDeleteAccountTypesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BulkDeleteResultDto> Handle(BulkDeleteAccountTypesCmd request, CancellationToken ct)
    {
        var result = new BulkDeleteResultDto();
        var errors = new List<BulkDeleteErrorDto>();

        foreach (var id in request.Ids)
        {
            try
            {
                var accountType = await _context.AccountTypes
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

                if (accountType == null)
                {
                    errors.Add(new BulkDeleteErrorDto { Id = id, Error = $"Account Type with ID '{id}' not found" });
                    result.FailedCount++;
                    continue;
                }

                var hasSubtypes = await _context.AccountSubtypes
                    .AnyAsync(x => x.AccountTypeId == id && !x.IsDeleted, ct);

                if (hasSubtypes)
                {
                    errors.Add(new BulkDeleteErrorDto { Id = id, Error = $"Type '{accountType.Code}' has associated subtypes" });
                    result.FailedCount++;
                    continue;
                }

                accountType.IsDeleted = true;
                accountType.DateMod = DateTime.UtcNow;
                result.DeletedCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new BulkDeleteErrorDto { Id = id, Error = $"Error deleting type '{id}': {ex.Message}" });
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
// CREATE ACCOUNT SUBTYPE HANDLER
// ============================================================

public class CreateAccountSubtypeHandler : IRequestHandler<CreateAccountSubtypeCmd, AccountSubtypeDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<CreateAccountSubtypeHandler> _logger;

    public CreateAccountSubtypeHandler(FinanceDbContext context, ILogger<CreateAccountSubtypeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountSubtypeDto> Handle(CreateAccountSubtypeCmd request, CancellationToken ct)
    {
        try
        {
            var exists = await _context.AccountSubtypes
                .AnyAsync(x => x.Code == request.Dto.Code && !x.IsDeleted, ct);

            if (exists)
                throw new InvalidOperationException($"Account Subtype with code '{request.Dto.Code}' already exists");

            var accountType = await _context.AccountTypes
                .FirstOrDefaultAsync(x => x.Id == request.Dto.AccountTypeId && !x.IsDeleted, ct);

            if (accountType == null)
                throw new InvalidOperationException($"Account Type with ID '{request.Dto.AccountTypeId}' not found");

            var subtype = new AccountSubtype
            {
                Id = Guid.NewGuid(),
                Code = request.Dto.Code,
                Name = request.Dto.Name,
                NameAm = request.Dto.NameAm,
                Description = request.Dto.Description,
                AccountTypeId = request.Dto.AccountTypeId,
                IsActive = request.Dto.IsActive,
                SortOrder = request.Dto.SortOrder,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.AccountSubtypes.Add(subtype);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("✅ Account Subtype created: {Code} - {Name}", subtype.Code, subtype.Name);

            return new AccountSubtypeDto
            {
                Id = subtype.Id,
                Code = subtype.Code,
                Name = subtype.Name,
                NameAm = subtype.NameAm,
                Description = subtype.Description,
                AccountTypeId = subtype.AccountTypeId,
                AccountTypeName = accountType.Name,
                AccountTypeCode = accountType.Code,
                IsActive = subtype.IsActive,
                SortOrder = subtype.SortOrder,
                DateAdd = subtype.DateAdd,
                DateMod = subtype.DateMod
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account subtype");
            throw;
        }
    }
}

// ============================================================
// UPDATE ACCOUNT SUBTYPE HANDLER
// ============================================================

public class UpdateAccountSubtypeHandler : IRequestHandler<UpdateAccountSubtypeCmd, AccountSubtypeDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<UpdateAccountSubtypeHandler> _logger;

    public UpdateAccountSubtypeHandler(FinanceDbContext context, ILogger<UpdateAccountSubtypeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountSubtypeDto> Handle(UpdateAccountSubtypeCmd request, CancellationToken ct)
    {
        try
        {
            var subtype = await _context.AccountSubtypes
                .FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct);

            if (subtype == null)
                throw new InvalidOperationException($"Account Subtype with ID '{request.Dto.Id}' not found");

            var exists = await _context.AccountSubtypes
                .AnyAsync(x => x.Code == request.Dto.Code && x.Id != request.Dto.Id && !x.IsDeleted, ct);

            if (exists)
                throw new InvalidOperationException($"Account Subtype with code '{request.Dto.Code}' already exists");

            var accountType = await _context.AccountTypes
                .FirstOrDefaultAsync(x => x.Id == request.Dto.AccountTypeId && !x.IsDeleted, ct);

            if (accountType == null)
                throw new InvalidOperationException($"Account Type with ID '{request.Dto.AccountTypeId}' not found");

            subtype.Code = request.Dto.Code;
            subtype.Name = request.Dto.Name;
            subtype.NameAm = request.Dto.NameAm;
            subtype.Description = request.Dto.Description;
            subtype.AccountTypeId = request.Dto.AccountTypeId;
            subtype.IsActive = request.Dto.IsActive;
            subtype.SortOrder = request.Dto.SortOrder;
            subtype.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("✅ Account Subtype updated: {Code} - {Name}", subtype.Code, subtype.Name);

            return new AccountSubtypeDto
            {
                Id = subtype.Id,
                Code = subtype.Code,
                Name = subtype.Name,
                NameAm = subtype.NameAm,
                Description = subtype.Description,
                AccountTypeId = subtype.AccountTypeId,
                AccountTypeName = accountType.Name,
                AccountTypeCode = accountType.Code,
                IsActive = subtype.IsActive,
                SortOrder = subtype.SortOrder,
                DateAdd = subtype.DateAdd,
                DateMod = subtype.DateMod
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account subtype: {Id}", request.Dto.Id);
            throw;
        }
    }
}

// ============================================================
// DELETE ACCOUNT SUBTYPE HANDLER
// ============================================================

public class DeleteAccountSubtypeHandler : IRequestHandler<DeleteAccountSubtypeCmd, bool>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<DeleteAccountSubtypeHandler> _logger;

    public DeleteAccountSubtypeHandler(FinanceDbContext context, ILogger<DeleteAccountSubtypeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteAccountSubtypeCmd request, CancellationToken ct)
    {
        try
        {
            var subtype = await _context.AccountSubtypes
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (subtype == null)
                return false;

            subtype.IsDeleted = true;
            subtype.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("✅ Account Subtype deleted: {Code} - {Name}", subtype.Code, subtype.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account subtype: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// TOGGLE ACCOUNT SUBTYPE ACTIVE HANDLER
// ============================================================

public class ToggleAccountSubtypeActiveHandler : IRequestHandler<ToggleAccountSubtypeActiveCmd, ToggleActiveResultDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<ToggleAccountSubtypeActiveHandler> _logger;

    public ToggleAccountSubtypeActiveHandler(FinanceDbContext context, ILogger<ToggleAccountSubtypeActiveHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ToggleActiveResultDto> Handle(ToggleAccountSubtypeActiveCmd request, CancellationToken ct)
    {
        try
        {
            var subtype = await _context.AccountSubtypes
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (subtype == null)
                throw new InvalidOperationException($"Account Subtype with ID '{request.Id}' not found");

            subtype.IsActive = !subtype.IsActive;
            subtype.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("✅ Account Subtype status toggled: {Code} - IsActive: {IsActive}",
                subtype.Code, subtype.IsActive);

            return new ToggleActiveResultDto
            {
                Id = subtype.Id,
                IsActive = subtype.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling account subtype status: {Id}", request.Id);
            throw;
        }
    }
}
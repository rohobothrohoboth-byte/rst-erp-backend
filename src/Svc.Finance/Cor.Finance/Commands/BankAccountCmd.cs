// Cor.Finance/Commands/BankAccountCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Cor.Finance.Queries;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class AddBankAccountCmd : IRequest<BankAccountDto>
{
    public AddBankAccountDto AddDto { get; set; } = default!;
}

public class EditBankAccountCmd : IRequest<BankAccountDto>
{
    public EditBankAccountDto EditDto { get; set; } = default!;
}

public class DeleteBankAccountCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ToggleBankAccountActiveCmd : IRequest<ToggleActiveResultDto>
{
    public Guid Id { get; set; }
}

public class UpdateBankAccountBalanceCmd : IRequest<BankAccountDto>
{
    public UpdateBankAccountBalanceDto Dto { get; set; } = default!;
}

public class BulkCreateBankAccountsCmd : IRequest<BulkCreateBankAccountResultDto>
{
    public List<AddBankAccountDto> Dtos { get; set; } = new();
}

public class BulkDeleteBankAccountsCmd : IRequest<BulkDeleteResultDto>
{
    public List<Guid> Ids { get; set; } = new();
}

// ============================================================
// RESPONSE DTOs
// ============================================================

public class BulkCreateBankAccountResultDto
{
    public int CreatedCount { get; set; }
    public int FailedCount { get; set; }
    public List<BulkCreateErrorDto> Errors { get; set; } = new();
}

public class BulkCreateErrorDto
{
    public string? AccountNumber { get; set; }
    public string? Error { get; set; }
}

public class BulkDeleteResultDto
{
    public int DeletedCount { get; set; }
    public int FailedCount { get; set; }
    public List<BulkDeleteErrorDto> Errors { get; set; } = new();
}

public class BulkDeleteErrorDto
{
    public Guid Id { get; set; }
    public string? Error { get; set; }
}

public class ToggleActiveResultDto
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}

// ============================================================
// HANDLERS
// ============================================================

public class AddBankAccountHandler : IRequestHandler<AddBankAccountCmd, BankAccountDto>
{
    private readonly FinanceDbContext _context;

    public AddBankAccountHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccountDto> Handle(AddBankAccountCmd request, CancellationToken ct)
    {
        // ✅ Check if account number already exists
        var exists = await _context.BankAccounts
            .AnyAsync(x => x.AccountNumber == request.AddDto.AccountNumber && !x.IsDeleted, ct);

        if (exists)
            throw new InvalidOperationException($"Account with number '{request.AddDto.AccountNumber}' already exists.");

        // ✅ Validate Period if provided
        if (request.AddDto.PeriodId.HasValue)
        {
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == request.AddDto.PeriodId.Value && !p.IsDeleted, ct);

            if (period == null)
                throw new InvalidOperationException($"Period with ID '{request.AddDto.PeriodId}' not found");

            if (period.IsClosed)
                throw new InvalidOperationException($"Cannot create account in a closed period: {period.Name}");
        }

        // ✅ Create account with all fields
        var account = new BankAccount
        {
            Id = Guid.NewGuid(),
            AccountName = request.AddDto.AccountName,
            AccountNumber = request.AddDto.AccountNumber,
            BankName = request.AddDto.BankName,
            AccountType = request.AddDto.AccountType,
            GLCode = request.AddDto.GLCode,
            OpeningBalance = request.AddDto.OpeningBalance,
            CurrentBalance = request.AddDto.OpeningBalance,
            AvailableBalance = request.AddDto.OpeningBalance,
            Currency = request.AddDto.Currency ?? "USD",
            IsActive = true,
            IsDefault = request.AddDto.IsDefault,
            BranchId = request.AddDto.BranchId,
            AccountId = request.AddDto.AccountId,
            Description = request.AddDto.Description,
            IBAN = request.AddDto.IBAN,
            SwiftCode = request.AddDto.SwiftCode,
            BankAddress = request.AddDto.BankAddress,
            OverdraftLimit = request.AddDto.OverdraftLimit,
            PeriodId = request.AddDto.PeriodId,
            IsReconciled = false,
            DateAdd = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.BankAccounts.Add(account);
        await _context.SaveChangesAsync(ct);

        // ✅ Create opening balance transaction
        if (request.AddDto.OpeningBalance > 0)
        {
            var transaction = new BankTransaction
            {
                Id = Guid.NewGuid(),
                BankAccountId = account.Id,
                TransactionDate = DateTime.UtcNow,
                TransactionType = "OpeningBalance",
                Amount = request.AddDto.OpeningBalance,
                Description = "Opening balance",
                Reference = $"OPEN-{DateTime.UtcNow:yyyyMMdd}",
                PeriodId = request.AddDto.PeriodId,
                IsDeleted = false,
                DateAdd = DateTime.UtcNow
            };
            _context.BankTransactions.Add(transaction);
            await _context.SaveChangesAsync(ct);
        }

        return await MapToDto(account, ct);
    }

    private async Task<BankAccountDto> MapToDto(BankAccount account, CancellationToken ct)
    {
        var branchName = "";
        if (account.BranchId.HasValue)
        {
            var branch = await _context.LocalBranches
                .FirstOrDefaultAsync(x => x.Id == account.BranchId.Value && !x.IsDeleted, ct);
            branchName = branch?.Name ?? "";
        }

        var periodName = "";
        if (account.PeriodId.HasValue)
        {
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(x => x.Id == account.PeriodId.Value && !x.IsDeleted, ct);
            periodName = period?.Name ?? "";
        }

        var accountCode = "";
        if (account.AccountId.HasValue)
        {
            var chartAccount = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(x => x.Id == account.AccountId.Value && !x.IsDeleted, ct);
            accountCode = chartAccount?.Code ?? "";
        }

        return new BankAccountDto
        {
            Id = account.Id,
            AccountName = account.AccountName,
            AccountNumber = account.AccountNumber,
            BankName = account.BankName,
            AccountType = account.AccountType,
            GLCode = account.GLCode,
            OpeningBalance = account.OpeningBalance,
            CurrentBalance = account.CurrentBalance,
            AvailableBalance = account.AvailableBalance,
            Currency = account.Currency,
            IsActive = account.IsActive,
            IsDefault = account.IsDefault,
            BranchId = account.BranchId,
            BranchName = branchName,
            AccountId = account.AccountId,
            AccountCode = accountCode,
            Description = account.Description,
            IBAN = account.IBAN,
            SwiftCode = account.SwiftCode,
            BankAddress = account.BankAddress,
            LastReconciledDate = account.LastReconciledDate,
            OverdraftLimit = account.OverdraftLimit,
            IsReconciled = account.IsReconciled,
            PeriodId = account.PeriodId,
            PeriodName = periodName,
            DateAdd = account.DateAdd,
            DateMod = account.DateMod,
            SyncedAt = account.SyncedAt
        };
    }
}

public class EditBankAccountHandler : IRequestHandler<EditBankAccountCmd, BankAccountDto>
{
    private readonly FinanceDbContext _context;

    public EditBankAccountHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccountDto> Handle(EditBankAccountCmd request, CancellationToken ct)
    {
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (account == null)
            throw new InvalidOperationException($"Bank account with ID '{request.EditDto.Id}' not found");

        // ✅ Check if account number already exists (excluding current)
        var exists = await _context.BankAccounts
            .AnyAsync(x => x.AccountNumber == request.EditDto.AccountNumber && x.Id != request.EditDto.Id && !x.IsDeleted, ct);

        if (exists)
            throw new InvalidOperationException($"Account with number '{request.EditDto.AccountNumber}' already exists.");

        // ✅ Validate Period if changed
        if (request.EditDto.PeriodId.HasValue && account.PeriodId != request.EditDto.PeriodId)
        {
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == request.EditDto.PeriodId.Value && !p.IsDeleted, ct);

            if (period == null)
                throw new InvalidOperationException($"Period with ID '{request.EditDto.PeriodId}' not found");

            if (period.IsClosed)
                throw new InvalidOperationException($"Cannot move account to a closed period: {period.Name}");
        }

        // ✅ Update all fields
        account.AccountName = request.EditDto.AccountName;
        account.AccountNumber = request.EditDto.AccountNumber;
        account.BankName = request.EditDto.BankName;
        account.AccountType = request.EditDto.AccountType;
        account.GLCode = request.EditDto.GLCode;
        account.Currency = request.EditDto.Currency ?? "USD";
        account.IsActive = request.EditDto.IsActive;
        account.IsDefault = request.EditDto.IsDefault;
        account.BranchId = request.EditDto.BranchId;
        account.AccountId = request.EditDto.AccountId;
        account.Description = request.EditDto.Description;
        account.IBAN = request.EditDto.IBAN;
        account.SwiftCode = request.EditDto.SwiftCode;
        account.BankAddress = request.EditDto.BankAddress;
        account.OverdraftLimit = request.EditDto.OverdraftLimit;
        account.PeriodId = request.EditDto.PeriodId;
        account.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return await MapToDto(account, ct);
    }

    private async Task<BankAccountDto> MapToDto(BankAccount account, CancellationToken ct)
    {
        var branchName = "";
        if (account.BranchId.HasValue)
        {
            var branch = await _context.LocalBranches
                .FirstOrDefaultAsync(x => x.Id == account.BranchId.Value && !x.IsDeleted, ct);
            branchName = branch?.Name ?? "";
        }

        var periodName = "";
        if (account.PeriodId.HasValue)
        {
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(x => x.Id == account.PeriodId.Value && !x.IsDeleted, ct);
            periodName = period?.Name ?? "";
        }

        var accountCode = "";
        if (account.AccountId.HasValue)
        {
            var chartAccount = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(x => x.Id == account.AccountId.Value && !x.IsDeleted, ct);
            accountCode = chartAccount?.Code ?? "";
        }

        return new BankAccountDto
        {
            Id = account.Id,
            AccountName = account.AccountName,
            AccountNumber = account.AccountNumber,
            BankName = account.BankName,
            AccountType = account.AccountType,
            GLCode = account.GLCode,
            OpeningBalance = account.OpeningBalance,
            CurrentBalance = account.CurrentBalance,
            AvailableBalance = account.AvailableBalance,
            Currency = account.Currency,
            IsActive = account.IsActive,
            IsDefault = account.IsDefault,
            BranchId = account.BranchId,
            BranchName = branchName,
            AccountId = account.AccountId,
            AccountCode = accountCode,
            Description = account.Description,
            IBAN = account.IBAN,
            SwiftCode = account.SwiftCode,
            BankAddress = account.BankAddress,
            LastReconciledDate = account.LastReconciledDate,
            OverdraftLimit = account.OverdraftLimit,
            IsReconciled = account.IsReconciled,
            PeriodId = account.PeriodId,
            PeriodName = periodName,
            DateAdd = account.DateAdd,
            DateMod = account.DateMod,
            SyncedAt = account.SyncedAt
        };
    }
}

public class DeleteBankAccountHandler : IRequestHandler<DeleteBankAccountCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteBankAccountHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteBankAccountCmd request, CancellationToken ct)
    {
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (account == null)
            return false;

        // ✅ Check if account has transactions
        var hasTransactions = await _context.BankTransactions
            .AnyAsync(x => x.BankAccountId == request.Id && !x.IsDeleted, ct);

        if (hasTransactions)
            throw new InvalidOperationException("Cannot delete bank account with existing transactions. Archive it instead.");

        account.IsDeleted = true;
        account.IsActive = false;
        account.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

public class ToggleBankAccountActiveHandler : IRequestHandler<ToggleBankAccountActiveCmd, ToggleActiveResultDto>
{
    private readonly FinanceDbContext _context;

    public ToggleBankAccountActiveHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ToggleActiveResultDto> Handle(ToggleBankAccountActiveCmd request, CancellationToken ct)
    {
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (account == null)
            throw new InvalidOperationException($"Bank account with ID '{request.Id}' not found");

        account.IsActive = !account.IsActive;
        account.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return new ToggleActiveResultDto
        {
            Id = account.Id,
            IsActive = account.IsActive
        };
    }
}

public class UpdateBankAccountBalanceHandler : IRequestHandler<UpdateBankAccountBalanceCmd, BankAccountDto>
{
    private readonly FinanceDbContext _context;

    public UpdateBankAccountBalanceHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccountDto> Handle(UpdateBankAccountBalanceCmd request, CancellationToken ct)
    {
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct);

        if (account == null)
            throw new InvalidOperationException($"Bank account with ID '{request.Dto.Id}' not found");

        // ✅ Validate Period if provided
        if (request.Dto.PeriodId.HasValue)
        {
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == request.Dto.PeriodId.Value && !p.IsDeleted, ct);

            if (period == null)
                throw new InvalidOperationException($"Period with ID '{request.Dto.PeriodId}' not found");

            if (period.IsClosed)
                throw new InvalidOperationException($"Cannot update balance in a closed period: {period.Name}");
        }

        var oldBalance = account.CurrentBalance;
        account.CurrentBalance = request.Dto.NewBalance;
        account.AvailableBalance = request.Dto.NewBalance;
        account.DateMod = DateTime.UtcNow;

        // ✅ Create adjustment transaction
        var adjustmentAmount = request.Dto.NewBalance - oldBalance;
        if (adjustmentAmount != 0)
        {
            var transaction = new BankTransaction
            {
                Id = Guid.NewGuid(),
                BankAccountId = account.Id,
                TransactionDate = DateTime.UtcNow,
                TransactionType = adjustmentAmount > 0 ? "AdjustmentCredit" : "AdjustmentDebit",
                Amount = Math.Abs(adjustmentAmount),
                Description = request.Dto.Reason ?? "Manual balance adjustment",
                Reference = $"ADJ-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
                PeriodId = request.Dto.PeriodId,
                IsDeleted = false,
                DateAdd = DateTime.UtcNow
            };
            _context.BankTransactions.Add(transaction);
        }

        await _context.SaveChangesAsync(ct);

        return await MapToDto(account, ct);
    }

    private async Task<BankAccountDto> MapToDto(BankAccount account, CancellationToken ct)
    {
        var branchName = "";
        if (account.BranchId.HasValue)
        {
            var branch = await _context.LocalBranches
                .FirstOrDefaultAsync(x => x.Id == account.BranchId.Value && !x.IsDeleted, ct);
            branchName = branch?.Name ?? "";
        }

        var periodName = "";
        if (account.PeriodId.HasValue)
        {
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(x => x.Id == account.PeriodId.Value && !x.IsDeleted, ct);
            periodName = period?.Name ?? "";
        }

        var accountCode = "";
        if (account.AccountId.HasValue)
        {
            var chartAccount = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(x => x.Id == account.AccountId.Value && !x.IsDeleted, ct);
            accountCode = chartAccount?.Code ?? "";
        }

        return new BankAccountDto
        {
            Id = account.Id,
            AccountName = account.AccountName,
            AccountNumber = account.AccountNumber,
            BankName = account.BankName,
            AccountType = account.AccountType,
            GLCode = account.GLCode,
            OpeningBalance = account.OpeningBalance,
            CurrentBalance = account.CurrentBalance,
            AvailableBalance = account.AvailableBalance,
            Currency = account.Currency,
            IsActive = account.IsActive,
            IsDefault = account.IsDefault,
            BranchId = account.BranchId,
            BranchName = branchName,
            AccountId = account.AccountId,
            AccountCode = accountCode,
            Description = account.Description,
            IBAN = account.IBAN,
            SwiftCode = account.SwiftCode,
            BankAddress = account.BankAddress,
            LastReconciledDate = account.LastReconciledDate,
            OverdraftLimit = account.OverdraftLimit,
            IsReconciled = account.IsReconciled,
            PeriodId = account.PeriodId,
            PeriodName = periodName,
            DateAdd = account.DateAdd,
            DateMod = account.DateMod,
            SyncedAt = account.SyncedAt
        };
    }
}

public class BulkCreateBankAccountsHandler : IRequestHandler<BulkCreateBankAccountsCmd, BulkCreateBankAccountResultDto>
{
    private readonly FinanceDbContext _context;

    public BulkCreateBankAccountsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BulkCreateBankAccountResultDto> Handle(BulkCreateBankAccountsCmd request, CancellationToken ct)
    {
        var result = new BulkCreateBankAccountResultDto();
        var errors = new List<BulkCreateErrorDto>();

        foreach (var dto in request.Dtos)
        {
            try
            {
                // ✅ Check if account number already exists
                var exists = await _context.BankAccounts
                    .AnyAsync(x => x.AccountNumber == dto.AccountNumber && !x.IsDeleted, ct);

                if (exists)
                {
                    errors.Add(new BulkCreateErrorDto { AccountNumber = dto.AccountNumber, Error = "Account number already exists" });
                    continue;
                }

                // ✅ Validate Period if provided
                if (dto.PeriodId.HasValue)
                {
                    var period = await _context.FinancialPeriods
                        .FirstOrDefaultAsync(p => p.Id == dto.PeriodId.Value && !p.IsDeleted, ct);

                    if (period == null)
                    {
                        errors.Add(new BulkCreateErrorDto { AccountNumber = dto.AccountNumber, Error = $"Period with ID '{dto.PeriodId}' not found" });
                        continue;
                    }

                    if (period.IsClosed)
                    {
                        errors.Add(new BulkCreateErrorDto { AccountNumber = dto.AccountNumber, Error = $"Period '{period.Name}' is closed" });
                        continue;
                    }
                }

                var account = new BankAccount
                {
                    Id = Guid.NewGuid(),
                    AccountName = dto.AccountName,
                    AccountNumber = dto.AccountNumber,
                    BankName = dto.BankName,
                    AccountType = dto.AccountType,
                    GLCode = dto.GLCode,
                    OpeningBalance = dto.OpeningBalance,
                    CurrentBalance = dto.OpeningBalance,
                    AvailableBalance = dto.OpeningBalance,
                    Currency = dto.Currency ?? "USD",
                    IsActive = true,
                    IsDefault = dto.IsDefault,
                    BranchId = dto.BranchId,
                    AccountId = dto.AccountId,
                    Description = dto.Description,
                    IBAN = dto.IBAN,
                    SwiftCode = dto.SwiftCode,
                    BankAddress = dto.BankAddress,
                    OverdraftLimit = dto.OverdraftLimit,
                    PeriodId = dto.PeriodId,
                    IsReconciled = false,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.BankAccounts.Add(account);
                result.CreatedCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new BulkCreateErrorDto { AccountNumber = dto.AccountNumber, Error = ex.Message });
            }
        }

        await _context.SaveChangesAsync(ct);
        result.Errors = errors;
        result.FailedCount = errors.Count;

        return result;
    }
}


public class SetDefaultBankAccountCmd : IRequest<BankAccountDto>
{
    public Guid Id { get; set; }
}

public class SetDefaultBankAccountHandler : IRequestHandler<SetDefaultBankAccountCmd, BankAccountDto>
{
    private readonly FinanceDbContext _context;

    public SetDefaultBankAccountHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccountDto> Handle(SetDefaultBankAccountCmd request, CancellationToken ct)
    {
        // ✅ Reset all default flags
        await _context.BankAccounts
            .Where(x => !x.IsDeleted && x.IsDefault)
            .ForEachAsync(x => x.IsDefault = false, ct);

        // ✅ Set the new default
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (account == null)
            throw new InvalidOperationException($"Bank account with ID '{request.Id}' not found");

        account.IsDefault = true;
        account.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        // ✅ Map to DTO
        var handler = new GetBankAccountByIdHandler(_context);
        return await handler.Handle(new GetBankAccountByIdQry { Id = account.Id }, ct);
    }
}
public class BulkDeleteBankAccountsHandler : IRequestHandler<BulkDeleteBankAccountsCmd, BulkDeleteResultDto>
{
    private readonly FinanceDbContext _context;

    public BulkDeleteBankAccountsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BulkDeleteResultDto> Handle(BulkDeleteBankAccountsCmd request, CancellationToken ct)
    {
        var result = new BulkDeleteResultDto();
        var errors = new List<BulkDeleteErrorDto>();

        foreach (var id in request.Ids)
        {
            try
            {
                var account = await _context.BankAccounts
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

                if (account == null)
                {
                    errors.Add(new BulkDeleteErrorDto { Id = id, Error = "Account not found" });
                    continue;
                }

                // ✅ Check if has transactions
                var hasTransactions = await _context.BankTransactions
                    .AnyAsync(x => x.BankAccountId == id && !x.IsDeleted, ct);

                if (hasTransactions)
                {
                    errors.Add(new BulkDeleteErrorDto { Id = id, Error = "Account has transactions - cannot delete" });
                    continue;
                }

                account.IsDeleted = true;
                account.IsActive = false;
                account.DateMod = DateTime.UtcNow;
                result.DeletedCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new BulkDeleteErrorDto { Id = id, Error = ex.Message });
            }
        }

        await _context.SaveChangesAsync(ct);
        result.Errors = errors;
        result.FailedCount = errors.Count;

        return result;
    }
}
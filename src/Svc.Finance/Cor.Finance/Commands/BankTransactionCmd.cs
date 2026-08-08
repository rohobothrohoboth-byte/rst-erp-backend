// Cor.Finance/Commands/BankTransactionCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class AddBankTransactionCmd : IRequest<BankTransactionDto>
{
    public AddBankTransactionDto AddDto { get; set; } = default!;
}

public class EditBankTransactionCmd : IRequest<BankTransactionDto>
{
    public EditBankTransactionDto EditDto { get; set; } = default!;
}

public class DeleteBankTransactionCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ReconcileBankTransactionCmd : IRequest<bool>
{
    public ReconcileBankTransactionDto ReconcileDto { get; set; } = default!;
}

public class BulkReconcileCmd : IRequest<BulkReconcileResultDto>
{
    public BulkReconcileDto Dto { get; set; } = default!;
}

public class VoidTransactionCmd : IRequest<BankTransactionDto>
{
    public Guid Id { get; set; }
    public VoidTransactionDto Dto { get; set; } = default!;
}

// ============================================================
// RESPONSE DTOs
// ============================================================

public class BulkReconcileResultDto
{
    public int ReconciledCount { get; set; }
    public int FailedCount { get; set; }
    public List<BulkReconcileErrorDto> Errors { get; set; } = new();
}

public class BulkReconcileErrorDto
{
    public Guid TransactionId { get; set; }
    public string? Error { get; set; }
}

// ============================================================
// ADD BANK TRANSACTION HANDLER
// ============================================================

public class AddBankTransactionHandler : IRequestHandler<AddBankTransactionCmd, BankTransactionDto>
{
    private readonly FinanceDbContext _context;

    public AddBankTransactionHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BankTransactionDto> Handle(AddBankTransactionCmd request, CancellationToken ct)
    {
        // ✅ STEP 1: VALIDATE PERIOD EXISTS AND IS OPEN
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == request.AddDto.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {request.AddDto.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot create bank transaction in a closed period: {period.Name}");

        // ✅ STEP 2: VALIDATE TRANSACTION DATE IS WITHIN PERIOD RANGE
        var transactionDate = request.AddDto.TransactionDate;
        if (transactionDate < period.StartDate || transactionDate > period.EndDate)
            throw new InvalidOperationException(
                $"Transaction date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

        // ✅ STEP 3: CHECK IF BANK ACCOUNT EXISTS AND IS ACTIVE
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == request.AddDto.BankAccountId && !x.IsDeleted && x.IsActive, ct);

        if (account == null)
            throw new InvalidOperationException($"Bank account with ID '{request.AddDto.BankAccountId}' not found or inactive.");

        // ✅ STEP 4: CREATE TRANSACTION
        var transaction = new BankTransaction
        {
            Id = Guid.NewGuid(),
            BankAccountId = request.AddDto.BankAccountId,
            TransactionDate = request.AddDto.TransactionDate,
            TransactionType = request.AddDto.TransactionType,
            Amount = request.AddDto.Amount,
            Description = request.AddDto.Description,
            Reference = request.AddDto.Reference ?? $"TXN-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
            PaymentMethod = request.AddDto.PaymentMethod,
            CheckNumber = request.AddDto.CheckNumber,
            BankReference = request.AddDto.BankReference,
            PeriodId = period.Id,
            IsReconciled = false,
            ReconciliationDate = null,
            Status = TransactionStatus.Pending,
            DateAdd = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.BankTransactions.Add(transaction);

        // ✅ STEP 5: UPDATE ACCOUNT BALANCE
        if (request.AddDto.TransactionType == "Deposit" || request.AddDto.TransactionType == "Replenishment" || request.AddDto.TransactionType == "OpeningBalance")
        {
            account.CurrentBalance += request.AddDto.Amount;
            account.AvailableBalance += request.AddDto.Amount;
        }
        else if (request.AddDto.TransactionType == "Withdrawal" || request.AddDto.TransactionType == "Expense" || request.AddDto.TransactionType == "Transfer")
        {
            if (account.AvailableBalance < request.AddDto.Amount)
                throw new InvalidOperationException($"Insufficient balance. Available: {account.AvailableBalance}, Attempted: {request.AddDto.Amount}");

            account.CurrentBalance -= request.AddDto.Amount;
            account.AvailableBalance -= request.AddDto.Amount;
        }
        else if (request.AddDto.TransactionType == "Adjustment")
        {
            // Adjustments can be positive or negative
            if (request.AddDto.Amount > 0)
            {
                account.CurrentBalance += request.AddDto.Amount;
                account.AvailableBalance += request.AddDto.Amount;
            }
            else
            {
                if (account.AvailableBalance < Math.Abs(request.AddDto.Amount))
                    throw new InvalidOperationException($"Insufficient balance for adjustment. Available: {account.AvailableBalance}");

                account.CurrentBalance -= Math.Abs(request.AddDto.Amount);
                account.AvailableBalance -= Math.Abs(request.AddDto.Amount);
            }
        }

        account.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return await MapToDto(transaction, period, ct);
    }

    private async Task<BankTransactionDto> MapToDto(BankTransaction transaction, FinancialPeriod period, CancellationToken ct)
    {
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == transaction.BankAccountId && !x.IsDeleted, ct);

        return new BankTransactionDto
        {
            Id = transaction.Id,
            BankAccountId = transaction.BankAccountId,
            BankAccountName = account?.AccountName,
            TransactionDate = transaction.TransactionDate,
            TransactionType = transaction.TransactionType,
            Amount = transaction.Amount,
            Description = transaction.Description,
            Reference = transaction.Reference,
            PaymentMethod = transaction.PaymentMethod,
            CheckNumber = transaction.CheckNumber,
            BankReference = transaction.BankReference,
            BalanceAfter = transaction.BalanceAfter,
            PeriodId = transaction.PeriodId,
            PeriodName = period?.Name,
            IsReconciled = transaction.IsReconciled,
            ReconciliationDate = transaction.ReconciliationDate,
            Status = transaction.Status.ToString(),
            DateAdd = transaction.DateAdd,
            DateMod = transaction.DateMod,
        };
    }
}

// ============================================================
// EDIT BANK TRANSACTION HANDLER
// ============================================================
public class EditBankTransactionHandler : IRequestHandler<EditBankTransactionCmd, BankTransactionDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<EditBankTransactionHandler> _logger;

    public EditBankTransactionHandler(FinanceDbContext context, ILogger<EditBankTransactionHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BankTransactionDto> Handle(EditBankTransactionCmd request, CancellationToken ct)
    {
        // ✅ 1. GET THE TRANSACTION
        var transaction = await _context.BankTransactions
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (transaction == null)
            throw new InvalidOperationException($"Bank transaction with ID '{request.EditDto.Id}' not found");

        if (transaction.IsReconciled)
            throw new InvalidOperationException("Cannot edit a reconciled transaction.");

        // ✅ 2. GET THE PERIOD (ONLY ONCE)
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == request.EditDto.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {request.EditDto.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot use closed period: {period.Name}");

        // ✅ 3. VALIDATE TRANSACTION DATE WITHIN PERIOD
        if (request.EditDto.TransactionDate < period.StartDate || request.EditDto.TransactionDate > period.EndDate)
            throw new InvalidOperationException(
                $"Transaction date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

        // ✅ 4. GET THE BANK ACCOUNT
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == transaction.BankAccountId && !x.IsDeleted && x.IsActive, ct);

        if (account == null)
            throw new InvalidOperationException($"Bank account with ID '{transaction.BankAccountId}' not found or inactive");

        // ✅ 5. STORE OLD AND NEW VALUES
        var oldAmount = transaction.Amount;
        var oldType = transaction.TransactionType;
        var newAmount = request.EditDto.Amount;
        var newType = request.EditDto.TransactionType;
        var isPeriodChanging = transaction.PeriodId != request.EditDto.PeriodId;

        // ✅ 6. VALIDATE BALANCE BEFORE ANY CHANGES
        await ValidateBalanceAsync(account, oldType, oldAmount, newType, newAmount, ct);

        // ✅ 7. REVERSE OLD TRANSACTION EFFECT
        await ReverseTransactionEffectAsync(account, oldType, oldAmount, ct);

        // ✅ 8. APPLY NEW TRANSACTION EFFECT
        await ApplyTransactionEffectAsync(account, newType, newAmount, ct);

        // ✅ 9. UPDATE TRANSACTION FIELDS
        transaction.TransactionDate = request.EditDto.TransactionDate;
        transaction.TransactionType = newType;
        transaction.Amount = newAmount;
        transaction.Description = request.EditDto.Description;
        transaction.Reference = request.EditDto.Reference ?? transaction.Reference;
        transaction.PaymentMethod = request.EditDto.PaymentMethod ?? transaction.PaymentMethod;
        transaction.CheckNumber = request.EditDto.CheckNumber ?? transaction.CheckNumber;
        transaction.BankReference = request.EditDto.BankReference ?? transaction.BankReference;
        transaction.PeriodId = period.Id;
        transaction.DateMod = DateTime.UtcNow;

        // ✅ 10. UPDATE ACCOUNT
        account.DateMod = DateTime.UtcNow;

        // ✅ 11. SAVE CHANGES
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "✅ Bank transaction {TransactionId} updated: {OldType} {OldAmount:C} -> {NewType} {NewAmount:C}",
            transaction.Id, oldType, oldAmount, newType, newAmount);

        // ✅ 12. RETURN DTO
        return await MapToDto(transaction, period, account, ct);
    }

    // ============================================================
    // PRIVATE HELPER METHODS
    // ============================================================

    private async Task ValidateBalanceAsync(
        BankAccount account,
        string oldType,
        decimal oldAmount,
        string newType,
        decimal newAmount,
        CancellationToken ct)
    {
        // Check if the transaction is a debit (withdrawal/expense)
        var isOldDebit = IsDebitTransaction(oldType);
        var isNewDebit = IsDebitTransaction(newType);

        // If both are debit transactions, check if we need more balance
        if (isOldDebit && isNewDebit)
        {
            // If new amount is greater than old amount, we need extra balance
            if (newAmount > oldAmount)
            {
                var additionalAmount = newAmount - oldAmount;
                if (account.AvailableBalance + oldAmount < newAmount)
                {
                    throw new InvalidOperationException(
                        $"Insufficient balance. Available: {account.AvailableBalance + oldAmount:C}, Required: {newAmount:C}");
                }
            }
        }
        // If changing from credit to debit, check if there's enough balance
        else if (!isOldDebit && isNewDebit)
        {
            if (account.AvailableBalance < newAmount)
            {
                throw new InvalidOperationException(
                    $"Insufficient balance. Available: {account.AvailableBalance:C}, Required: {newAmount:C}");
            }
        }

        await Task.CompletedTask;
    }

    private async Task ReverseTransactionEffectAsync(
        BankAccount account,
        string transactionType,
        decimal amount,
        CancellationToken ct)
    {
        if (IsCreditTransaction(transactionType))
        {
            // Reverse credit: subtract
            account.CurrentBalance -= amount;
            account.AvailableBalance -= amount;
        }
        else if (IsDebitTransaction(transactionType))
        {
            // Reverse debit: add back
            account.CurrentBalance += amount;
            account.AvailableBalance += amount;
        }
        else if (transactionType == "Adjustment")
        {
            if (amount > 0)
            {
                account.CurrentBalance -= amount;
                account.AvailableBalance -= amount;
            }
            else
            {
                account.CurrentBalance += Math.Abs(amount);
                account.AvailableBalance += Math.Abs(amount);
            }
        }

        await Task.CompletedTask;
    }

    private async Task ApplyTransactionEffectAsync(
        BankAccount account,
        string transactionType,
        decimal amount,
        CancellationToken ct)
    {
        if (IsCreditTransaction(transactionType))
        {
            // Apply credit: add
            account.CurrentBalance += amount;
            account.AvailableBalance += amount;
        }
        else if (IsDebitTransaction(transactionType))
        {
            // Apply debit: subtract
            if (account.AvailableBalance < amount)
                throw new InvalidOperationException(
                    $"Insufficient balance. Available: {account.AvailableBalance:C}, Required: {amount:C}");

            account.CurrentBalance -= amount;
            account.AvailableBalance -= amount;
        }
        else if (transactionType == "Adjustment")
        {
            if (amount > 0)
            {
                account.CurrentBalance += amount;
                account.AvailableBalance += amount;
            }
            else
            {
                var absAmount = Math.Abs(amount);
                if (account.AvailableBalance < absAmount)
                    throw new InvalidOperationException(
                        $"Insufficient balance for adjustment. Available: {account.AvailableBalance:C}, Required: {absAmount:C}");

                account.CurrentBalance -= absAmount;
                account.AvailableBalance -= absAmount;
            }
        }

        await Task.CompletedTask;
    }

    private static bool IsCreditTransaction(string type)
    {
        return type == "Deposit" ||
               type == "Replenishment" ||
               type == "OpeningBalance" ||
               type == "Interest" ||
               type == "Refund" ||
               type == "TransferIn";
    }

    private static bool IsDebitTransaction(string type)
    {
        return type == "Withdrawal" ||
               type == "Expense" ||
               type == "Transfer" ||
               type == "Payment" ||
               type == "Fee" ||
               type == "Tax" ||
               type == "TransferOut";
    }

    private async Task<BankTransactionDto> MapToDto(
        BankTransaction transaction,
        FinancialPeriod period,
        BankAccount account,
        CancellationToken ct)
    {
        // Get period name (use the provided period or fetch if null)
        string? periodName = period?.Name;
        if (period == null)
        {
            var p = await _context.FinancialPeriods
                .FirstOrDefaultAsync(x => x.Id == transaction.PeriodId && !x.IsDeleted, ct);
            periodName = p?.Name;
        }

        return new BankTransactionDto
        {
            Id = transaction.Id,
            BankAccountId = transaction.BankAccountId,
            BankAccountName = account?.AccountName,
            BankAccountNumber = account?.AccountNumber,
            TransactionDate = transaction.TransactionDate,
            TransactionType = transaction.TransactionType,
            Amount = transaction.Amount,
            Description = transaction.Description,
            Reference = transaction.Reference,
            PaymentMethod = transaction.PaymentMethod,
            CheckNumber = transaction.CheckNumber,
            BankReference = transaction.BankReference,
            BalanceAfter = transaction.BalanceAfter,
            PeriodId = transaction.PeriodId,
            PeriodName = periodName,
            IsReconciled = transaction.IsReconciled,
            ReconciliationDate = transaction.ReconciliationDate,
            Status = transaction.Status.ToString(),
            CreatedByUserName = transaction.CreatedByUserName,
            DateAdd = transaction.DateAdd,
            DateMod = transaction.DateMod,
        };
    }
}

// ============================================================
// DELETE BANK TRANSACTION HANDLER
// ============================================================

public class DeleteBankTransactionHandler : IRequestHandler<DeleteBankTransactionCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteBankTransactionHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteBankTransactionCmd request, CancellationToken ct)
    {
        var transaction = await _context.BankTransactions
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (transaction == null)
            return false;

        if (transaction.IsReconciled)
            throw new InvalidOperationException("Cannot delete a reconciled transaction.");

        // ✅ VALIDATE PERIOD IS OPEN BEFORE DELETING
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == transaction.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {transaction.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot delete transaction in a closed period: {period.Name}");

        // ✅ Reverse the transaction effect on account balance
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == transaction.BankAccountId && !x.IsDeleted && x.IsActive, ct);

        if (account != null)
        {
            if (transaction.TransactionType == "Deposit" || transaction.TransactionType == "Replenishment" || transaction.TransactionType == "OpeningBalance")
            {
                account.CurrentBalance -= transaction.Amount;
                account.AvailableBalance -= transaction.Amount;
            }
            else if (transaction.TransactionType == "Withdrawal" || transaction.TransactionType == "Expense" || transaction.TransactionType == "Transfer")
            {
                account.CurrentBalance += transaction.Amount;
                account.AvailableBalance += transaction.Amount;
            }
            else if (transaction.TransactionType == "Adjustment")
            {
                if (transaction.Amount > 0)
                {
                    account.CurrentBalance -= transaction.Amount;
                    account.AvailableBalance -= transaction.Amount;
                }
                else
                {
                    account.CurrentBalance += Math.Abs(transaction.Amount);
                    account.AvailableBalance += Math.Abs(transaction.Amount);
                }
            }

            account.DateMod = DateTime.UtcNow;
        }

        transaction.IsDeleted = true;
        transaction.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

// ============================================================
// RECONCILE BANK TRANSACTION HANDLER
// ============================================================

public class ReconcileBankTransactionHandler : IRequestHandler<ReconcileBankTransactionCmd, bool>
{
    private readonly FinanceDbContext _context;

    public ReconcileBankTransactionHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ReconcileBankTransactionCmd request, CancellationToken ct)
    {
        var transaction = await _context.BankTransactions
            .FirstOrDefaultAsync(x => x.Id == request.ReconcileDto.Id && !x.IsDeleted, ct);

        if (transaction == null)
            return false;

        // ✅ VALIDATE PERIOD IS OPEN BEFORE RECONCILING
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == transaction.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {transaction.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot reconcile transaction in a closed period: {period.Name}");

        transaction.IsReconciled = request.ReconcileDto.IsReconciled;
        transaction.ReconciliationDate = request.ReconcileDto.IsReconciled
            ? (request.ReconcileDto.ReconciliationDate ?? DateTime.UtcNow)
            : null;

        if (request.ReconcileDto.IsReconciled)
        {
            transaction.Status = TransactionStatus.Reconciled;
        }
        else
        {
            transaction.Status = TransactionStatus.Pending;
        }

        transaction.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

// ============================================================
// BULK RECONCILE HANDLER
// ============================================================

public class BulkReconcileHandler : IRequestHandler<BulkReconcileCmd, BulkReconcileResultDto>
{
    private readonly FinanceDbContext _context;

    public BulkReconcileHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BulkReconcileResultDto> Handle(BulkReconcileCmd request, CancellationToken ct)
    {
        var result = new BulkReconcileResultDto();
        var errors = new List<BulkReconcileErrorDto>();

        foreach (var transactionId in request.Dto.TransactionIds)
        {
            try
            {
                var transaction = await _context.BankTransactions
                    .FirstOrDefaultAsync(x => x.Id == transactionId && !x.IsDeleted, ct);

                if (transaction == null)
                {
                    errors.Add(new BulkReconcileErrorDto
                    {
                        TransactionId = transactionId,
                        Error = "Transaction not found"
                    });
                    continue;
                }

                if (transaction.IsReconciled)
                {
                    errors.Add(new BulkReconcileErrorDto
                    {
                        TransactionId = transactionId,
                        Error = "Transaction already reconciled"
                    });
                    continue;
                }

                // Validate period is open
                var period = await _context.FinancialPeriods
                    .FirstOrDefaultAsync(p => p.Id == transaction.PeriodId && !p.IsDeleted, ct);

                if (period == null || period.IsClosed)
                {
                    errors.Add(new BulkReconcileErrorDto
                    {
                        TransactionId = transactionId,
                        Error = $"Period {period?.Name ?? "not found"} is closed"
                    });
                    continue;
                }

                transaction.IsReconciled = request.Dto.IsReconciled;
                transaction.ReconciliationDate = request.Dto.IsReconciled
                    ? (request.Dto.ReconciliationDate ?? DateTime.UtcNow)
                    : null;

                transaction.Status = request.Dto.IsReconciled
                    ? TransactionStatus.Reconciled
                    : TransactionStatus.Pending;

                transaction.DateMod = DateTime.UtcNow;

                result.ReconciledCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new BulkReconcileErrorDto
                {
                    TransactionId = transactionId,
                    Error = ex.Message
                });
            }
        }

        await _context.SaveChangesAsync(ct);
        result.Errors = errors;
        result.FailedCount = errors.Count;

        return result;
    }
}

// ============================================================
// VOID TRANSACTION HANDLER
// ============================================================

public class VoidTransactionHandler : IRequestHandler<VoidTransactionCmd, BankTransactionDto>
{
    private readonly FinanceDbContext _context;

    public VoidTransactionHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BankTransactionDto> Handle(VoidTransactionCmd request, CancellationToken ct)
    {
        var transaction = await _context.BankTransactions
            .Include(x => x.Period)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (transaction == null)
            throw new InvalidOperationException($"Bank transaction with ID '{request.Id}' not found");

        if (transaction.IsReconciled)
            throw new InvalidOperationException("Cannot void a reconciled transaction.");

        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == transaction.PeriodId && !p.IsDeleted, ct);

        if (period != null && period.IsClosed)
            throw new InvalidOperationException($"Cannot void transaction in a closed period: {period.Name}");

        // ✅ Reverse the transaction effect on account balance
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == transaction.BankAccountId && !x.IsDeleted && x.IsActive, ct);

        if (account != null)
        {
            if (transaction.TransactionType == "Deposit" || transaction.TransactionType == "Replenishment" || transaction.TransactionType == "OpeningBalance")
            {
                account.CurrentBalance -= transaction.Amount;
                account.AvailableBalance -= transaction.Amount;
            }
            else if (transaction.TransactionType == "Withdrawal" || transaction.TransactionType == "Expense" || transaction.TransactionType == "Transfer")
            {
                account.CurrentBalance += transaction.Amount;
                account.AvailableBalance += transaction.Amount;
            }
            else if (transaction.TransactionType == "Adjustment")
            {
                if (transaction.Amount > 0)
                {
                    account.CurrentBalance -= transaction.Amount;
                    account.AvailableBalance -= transaction.Amount;
                }
                else
                {
                    account.CurrentBalance += Math.Abs(transaction.Amount);
                    account.AvailableBalance += Math.Abs(transaction.Amount);
                }
            }

            account.DateMod = DateTime.UtcNow;
        }

        // ✅ Soft delete the transaction
        transaction.IsDeleted = true;
        transaction.DateMod = DateTime.UtcNow;
        transaction.Status = TransactionStatus.Cancelled;

        await _context.SaveChangesAsync(ct);

        return new BankTransactionDto
        {
            Id = transaction.Id,
            BankAccountId = transaction.BankAccountId,
            TransactionDate = transaction.TransactionDate,
            TransactionType = transaction.TransactionType,
            Amount = transaction.Amount,
            Description = transaction.Description,
            Reference = transaction.Reference,
            PaymentMethod = transaction.PaymentMethod,
            CheckNumber = transaction.CheckNumber,
            BankReference = transaction.BankReference,
            BalanceAfter = transaction.BalanceAfter,
            PeriodId = transaction.PeriodId,
            PeriodName = period?.Name,
            IsReconciled = transaction.IsReconciled,
            ReconciliationDate = transaction.ReconciliationDate,
            Status = transaction.Status.ToString(),
            DateAdd = transaction.DateAdd,
            DateMod = transaction.DateMod,
        };
    }
}
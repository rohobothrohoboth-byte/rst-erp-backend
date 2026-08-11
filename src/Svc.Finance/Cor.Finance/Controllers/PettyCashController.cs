// Cor.Finance/Controllers/PettyCashController.cs
using Common;
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;
using Cor.Finance.Models.Enums;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class PettyCashController : BaseApiController
{
    private readonly FinanceDbContext _context;
    private readonly IMediator _mediator;

    public PettyCashController(IMediator mediator, ILogger<PettyCashController> logger, FinanceDbContext context)
        : base(mediator, logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator;
    }

    /// <summary>
    /// Get petty cash balance
    /// </summary>
    [HttpGet("Balance")]
    [PerAuth("fnm.cash.petty.view")]
    [ProducesResponseType(typeof(PettyCashDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance([FromQuery] Guid? bankAccountId)
    {
        try
        {
            if (_context == null)
                return BadRequest(new { message = "Database context not initialized" });

            // Get current period info
            var currentPeriod = await GetCurrentPeriodAsync();
            var periodInfo = currentPeriod != null ? new
            {
                currentPeriod.Id,
                currentPeriod.Name,
                currentPeriod.StartDate,
                currentPeriod.EndDate,
                currentPeriod.IsClosed,
                Status = currentPeriod.Status.ToString()
            } : null;

            // Find petty cash account
            var query = _context.BankAccounts
                .Where(x => !x.IsDeleted && x.IsActive && x.AccountType == "Cash");

            if (bankAccountId.HasValue)
                query = query.Where(x => x.Id == bankAccountId.Value);
            else
                query = query.Where(x => x.AccountName.Contains("Petty Cash"));

            var account = await query.FirstOrDefaultAsync();

            if (account == null)
            {
                return Ok(new PettyCashDto
                {
                    Id = Guid.Empty,
                    Balance = 0,
                    TotalExpenses = 0,
                    TotalReplenishments = 0,
                    DateAdd = DateTime.UtcNow,
                    PeriodInfo = periodInfo
                });
            }

            // Get transactions - filtered by current period if period exists
            var transactionsQuery = _context.BankTransactions
                .Where(x => x.BankAccountId == account.Id && !x.IsDeleted);

            if (currentPeriod != null)
            {
                transactionsQuery = transactionsQuery
                    .Where(x => x.TransactionDate >= currentPeriod.StartDate &&
                                x.TransactionDate <= currentPeriod.EndDate);
            }

            var transactions = await transactionsQuery.ToListAsync();

            var totalExpenses = transactions
                .Where(x => x.TransactionType == "Expense" || x.TransactionType == "Withdrawal")
                .Sum(x => x.Amount);

            var totalReplenishments = transactions
                .Where(x => x.TransactionType == "Replenishment" || x.TransactionType == "Deposit")
                .Sum(x => x.Amount);

            return Ok(new PettyCashDto
            {
                Id = account.Id,
                Balance = account.CurrentBalance,
                TotalExpenses = totalExpenses,
                TotalReplenishments = totalReplenishments,
                DateAdd = account.DateAdd,
                DateMod = account.DateMod,
                PeriodInfo = periodInfo
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPettyCashBalance");
        }
    }

    /// <summary>
    /// Get all petty cash transactions with period filtering
    /// </summary>
    [HttpGet("Transactions")]
    [PerAuth("fnm.cash.petty.view")]
    [ProducesResponseType(typeof(List<BankTransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] Guid? bankAccountId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status,
        [FromQuery] bool includeAllPeriods = false)
    {
        try
        {
            // Find petty cash account
            var query = _context.BankAccounts
                .Where(x => !x.IsDeleted && x.IsActive && x.AccountType == "Cash");

            if (bankAccountId.HasValue)
                query = query.Where(x => x.Id == bankAccountId.Value);
            else
                query = query.Where(x => x.AccountName.Contains("Petty Cash"));

            var account = await query.FirstOrDefaultAsync();

            if (account == null)
                return Ok(new List<BankTransactionDto>());

            var transactionsQuery = _context.BankTransactions
                .Where(x => x.BankAccountId == account.Id && !x.IsDeleted);

            // Apply period filtering
            if (!includeAllPeriods)
            {
                var currentPeriod = await GetCurrentPeriodAsync();
                if (currentPeriod != null)
                {
                    transactionsQuery = transactionsQuery
                        .Where(x => x.TransactionDate >= currentPeriod.StartDate &&
                                    x.TransactionDate <= currentPeriod.EndDate);
                }
            }

            if (fromDate.HasValue)
                transactionsQuery = transactionsQuery.Where(x => x.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                transactionsQuery = transactionsQuery.Where(x => x.TransactionDate <= toDate.Value);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TransactionStatus>(status, true, out var transactionStatus))
                transactionsQuery = transactionsQuery.Where(x => x.Status == transactionStatus);

            var transactions = await transactionsQuery
                .OrderByDescending(x => x.TransactionDate)
                .ToListAsync();

            var result = transactions.Select(x => new BankTransactionDto
            {
                Id = x.Id,
                BankAccountId = x.BankAccountId,
                BankAccountName = account.AccountName,
                TransactionDate = x.TransactionDate,
                TransactionType = x.TransactionType,
                Amount = x.Amount,
                Description = x.Description,
                Reference = x.Reference,
                IsReconciled = x.IsReconciled,
                ReconciliationDate = x.ReconciliationDate,
                Status = x.Status.ToString(),
                PeriodId = x.PeriodId,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod,
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetPettyCashTransactions");
        }
    }

    /// <summary>
    /// Record petty cash transaction with period validation
    /// </summary>
    [HttpPost("Transaction")]
    [PerAuth("fnm.cash.petty.add")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RecordTransaction([FromBody] AddPettyCashTransactionDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid transaction data" });

            if (dto.Amount <= 0)
                return BadRequest(new { message = "Amount must be greater than zero" });

            if (string.IsNullOrEmpty(dto.Description))
                return BadRequest(new { message = "Description is required" });

            if (string.IsNullOrEmpty(dto.TransactionType))
                return BadRequest(new { message = "Transaction type is required" });

            // ✅ VALIDATE PERIOD
          var transactionDate = dto.TransactionDate == default(DateTime)
              ? DateTime.UtcNow
              : dto.TransactionDate;
            var period = await GetPeriodForDateAsync(transactionDate);

            if (period == null)
                return BadRequest(new
                {
                    message = $"No active financial period found for date {transactionDate:yyyy-MM-dd}",
                    date = transactionDate,
                    hint = "Please ensure the transaction date falls within an active financial period"
                });

            if (period.IsClosed)
                return Conflict(new
                {
                    message = $"Cannot record transaction in closed period: {period.Name}",
                    periodId = period.Id,
                    periodName = period.Name,
                    periodStart = period.StartDate,
                    periodEnd = period.EndDate,
                    hint = "Please reopen the period or select a different date"
                });

            if (period.Status == PeriodStatus.LOCKED)
                return Conflict(new
                {
                    message = $"Period is locked: {period.Name}",
                    periodId = period.Id,
                    periodName = period.Name,
                    hint = "Please unlock the period first"
                });

            // Find or create petty cash account
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(x => x.AccountType == "Cash" && x.AccountName.Contains("Petty Cash") && !x.IsDeleted);

            if (account == null)
            {
                account = new BankAccount
                {
                    Id = Guid.NewGuid(),
                    AccountName = "Petty Cash",
                    AccountNumber = "PC-001",
                    BankName = "Cash on Hand",
                    AccountType = "Cash",
                    OpeningBalance = 0,
                    CurrentBalance = 0,
                    IsActive = true,
                    IsDeleted = false,
                    DateAdd = DateTime.UtcNow,
                };
                _context.BankAccounts.Add(account);
                await _context.SaveChangesAsync();
            }

            // Validate balance for expenses/withdrawals
            if (dto.TransactionType == "Expense" || dto.TransactionType == "Withdrawal")
            {
                if (account.CurrentBalance < dto.Amount)
                    return BadRequest(new { message = $"Insufficient petty cash balance. Current: {account.CurrentBalance:C}, Attempted: {dto.Amount:C}" });
            }

            // ✅ Create transaction WITH period reference
            var transaction = new BankTransaction
            {
                Id = Guid.NewGuid(),
                BankAccountId = account.Id,
                TransactionDate = transactionDate,
                TransactionType = dto.TransactionType,
                Amount = dto.Amount,
                Description = dto.Description,
                Reference = $"PC-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
                IsReconciled = false,
                ReconciliationDate = null,
                Status = TransactionStatus.Pending,
                PeriodId = period.Id, // ✅ Link to period
                IsDeleted = false,
                DateAdd = DateTime.UtcNow,
            };

            _context.BankTransactions.Add(transaction);

            // Update balance
            if (dto.TransactionType == "Replenishment" || dto.TransactionType == "Deposit")
                account.CurrentBalance += dto.Amount;
            else if (dto.TransactionType == "Expense" || dto.TransactionType == "Withdrawal")
                account.CurrentBalance -= dto.Amount;

            account.DateMod = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Petty cash transaction recorded successfully",
                transactionId = transaction.Id,
                newBalance = account.CurrentBalance,
                amount = dto.Amount,
                type = dto.TransactionType,
                description = dto.Description,
                date = transactionDate,
                period = new
                {
                    period.Id,
                    period.Name,
                    period.StartDate,
                    period.EndDate,
                    IsClosed = period.IsClosed
                }
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "RecordPettyCashTransaction");
        }
    }



// Cor.Finance/Controllers/PettyCashController.cs

/// <summary>
/// Approve a petty cash transaction
/// </summary>
[HttpPost("Transaction/{id:guid}/approve")]
[PerAuth("fnm.cash.petty.view")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> ApproveTransaction(Guid id)
{
    try
    {
        var transaction = await _context.BankTransactions
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (transaction == null)
            return NotFound(new { message = $"Transaction with ID '{id}' not found" });

        if (transaction.Status == TransactionStatus.Completed)
            return BadRequest(new { message = "Transaction is already approved" });

        if (transaction.Status == TransactionStatus.Cancelled)
            return BadRequest(new { message = "Cannot approve a cancelled transaction" });

        transaction.Status = TransactionStatus.Completed;
        transaction.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync();

      //  _logger.LogInformation("✅ Transaction approved: {Id} - {Description}", id, transaction.Description);

        return Ok(new
        {
            success = true,
            message = "Transaction approved successfully",
            transactionId = id,
            status = transaction.Status.ToString()
        });
    }
    catch (Exception ex)
    {
        return HandleException(ex, nameof(ApproveTransaction), id);
    }
}

/// <summary>
/// Reject a petty cash transaction
/// </summary>
[HttpPost("Transaction/{id:guid}/reject")]
[PerAuth("fnm.cash.petty.view")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> RejectTransaction(Guid id)
{
    try
    {
        var transaction = await _context.BankTransactions
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (transaction == null)
            return NotFound(new { message = $"Transaction with ID '{id}' not found" });

        if (transaction.Status == TransactionStatus.Cancelled)
            return BadRequest(new { message = "Transaction is already rejected" });

        if (transaction.Status == TransactionStatus.Completed)
            return BadRequest(new { message = "Cannot reject an approved transaction" });

        // Reverse the balance change
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == transaction.BankAccountId && !x.IsDeleted);

        if (account != null)
        {
            if (transaction.TransactionType == "Expense" || transaction.TransactionType == "Withdrawal")
                account.CurrentBalance += transaction.Amount; // Add back the amount
            else if (transaction.TransactionType == "Replenishment" || transaction.TransactionType == "Deposit")
                account.CurrentBalance -= transaction.Amount; // Subtract the amount

            account.DateMod = DateTime.UtcNow;
        }

        transaction.Status = TransactionStatus.Cancelled;
        transaction.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync();

     //   _logger.LogInformation("✅ Transaction rejected: {Id} - {Description}", id, transaction.Description);

        return Ok(new
        {
            success = true,
            message = "Transaction rejected successfully",
            transactionId = id,
            status = transaction.Status.ToString(),
            newBalance = account?.CurrentBalance ?? 0
        });
    }
    catch (Exception ex)
    {
        return HandleException(ex, nameof(RejectTransaction), id);
    }
}

/// <summary>
/// Delete a petty cash transaction (soft delete)
/// </summary>
[HttpDelete("Transaction/{id:guid}")]
[PerAuth("fnm.cash.petty.view")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> DeleteTransaction(Guid id)
{
    try
    {
        var transaction = await _context.BankTransactions
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (transaction == null)
            return NotFound(new { message = $"Transaction with ID '{id}' not found" });

        if (transaction.Status == TransactionStatus.Completed)
            return BadRequest(new { message = "Cannot delete an approved transaction" });

        // Reverse the balance change if not already cancelled
        if (transaction.Status != TransactionStatus.Cancelled)
        {
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(x => x.Id == transaction.BankAccountId && !x.IsDeleted);

            if (account != null)
            {
                if (transaction.TransactionType == "Expense" || transaction.TransactionType == "Withdrawal")
                    account.CurrentBalance += transaction.Amount;
                else if (transaction.TransactionType == "Replenishment" || transaction.TransactionType == "Deposit")
                    account.CurrentBalance -= transaction.Amount;

                account.DateMod = DateTime.UtcNow;
            }
        }

        transaction.IsDeleted = true;
        transaction.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync();

       // _logger.LogInformation("✅ Transaction deleted: {Id} - {Description}", id, transaction.Description);

        return Ok(new
        {
            success = true,
            message = "Transaction deleted successfully",
            transactionId = id
        });
    }
    catch (Exception ex)
    {
        return HandleException(ex, nameof(DeleteTransaction), id);
    }
}
    /// <summary>
    /// Replenish petty cash with period validation
    /// </summary>
    [HttpPost("Replenish")]
    [PerAuth("fnm.cash.petty.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Replenish([FromBody] ReplenishPettyCashDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid replenish data" });

            if (dto.Amount <= 0)
                return BadRequest(new { message = "Amount must be greater than zero" });

            if (string.IsNullOrEmpty(dto.Description))
                return BadRequest(new { message = "Description is required" });

            var transactionDate = dto.TransactionDate ?? DateTime.UtcNow;

            // ✅ VALIDATE PERIOD
            var period = await GetPeriodForDateAsync(transactionDate);

            if (period == null)
                return BadRequest(new
                {
                    message = $"No active financial period found for date {transactionDate:yyyy-MM-dd}",
                    date = transactionDate,
                    hint = "Please ensure the replenishment date falls within an active financial period"
                });

            if (period.IsClosed)
                return Conflict(new
                {
                    message = $"Cannot replenish in closed period: {period.Name}",
                    periodId = period.Id,
                    periodName = period.Name,
                    periodStart = period.StartDate,
                    periodEnd = period.EndDate,
                    hint = "Please reopen the period or select a different date"
                });

            if (period.Status == PeriodStatus.LOCKED)
                return Conflict(new
                {
                    message = $"Period is locked: {period.Name}",
                    periodId = period.Id,
                    periodName = period.Name,
                    hint = "Please unlock the period first"
                });

            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(x => x.AccountType == "Cash" && x.AccountName.Contains("Petty Cash") && !x.IsDeleted);

            if (account == null)
                return BadRequest(new { message = "Petty cash account not found. Please create one first." });

            // ✅ Create replenishment transaction WITH period reference
            var transaction = new BankTransaction
            {
                Id = Guid.NewGuid(),
                BankAccountId = account.Id,
                TransactionDate = transactionDate,
                TransactionType = "Replenishment",
                Amount = dto.Amount,
                Description = dto.Description,
                Reference = $"REP-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
                IsReconciled = false,
                ReconciliationDate = null,
                Status = TransactionStatus.Completed,
                PeriodId = period.Id, // ✅ Link to period
                IsDeleted = false,
                DateAdd = DateTime.UtcNow,
            };

            _context.BankTransactions.Add(transaction);
            account.CurrentBalance += dto.Amount;
            account.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Petty cash replenished with {dto.Amount:C} successfully",
                newBalance = account.CurrentBalance,
                amount = dto.Amount,
                date = transactionDate,
                period = new
                {
                    period.Id,
                    period.Name,
                    period.StartDate,
                    period.EndDate,
                    IsClosed = period.IsClosed
                }
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ReplenishPettyCash");
        }
    }

    // ==================== HELPER METHODS ====================

    /// <summary>
    /// Get the current active financial period
    /// </summary>
    private async Task<FinancialPeriod?> GetCurrentPeriodAsync()
    {
        var today = DateTime.UtcNow.Date;

        return await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                      !x.IsClosed &&
                                      x.StartDate <= today &&
                                      x.EndDate >= today);
    }

    /// <summary>
    /// Get the financial period for a specific date
    /// </summary>
    private async Task<FinancialPeriod?> GetPeriodForDateAsync(DateTime date)
    {
        var utcDate = date.Date;

        return await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                      x.StartDate <= utcDate &&
                                      x.EndDate >= utcDate);
    }
}
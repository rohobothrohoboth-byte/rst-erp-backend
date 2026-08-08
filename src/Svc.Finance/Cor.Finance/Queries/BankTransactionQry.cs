// Cor.Finance/Queries/BankTransactionQry.cs - FIXED

using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;

namespace Cor.Finance.Queries;

// ============================================================
// QUERIES
// ============================================================

public class GetAllBankTransactionsQry : IRequest<List<BankTransactionDto>>
{
    public Guid? BankAccountId { get; set; }
    public bool? IsReconciled { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? TransactionType { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Status { get; set; }
    public Guid? PeriodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class GetBankTransactionByIdQry : IRequest<BankTransactionDto>
{
    public Guid Id { get; set; }
}

public class GetBankTransactionsByPeriodQry : IRequest<List<BankTransactionDto>>
{
    public Guid PeriodId { get; set; }
}

public class GetReconciliationSummaryQry : IRequest<ReconciliationSummaryDto>
{
    public Guid? BankAccountId { get; set; }
    public Guid? PeriodId { get; set; }
}

public class GetTransactionStatsQry : IRequest<TransactionStatsDto>
{
    public Guid? BankAccountId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetTransactionTypesQry : IRequest<List<string>>
{
}

public class ExportTransactionsQry : IRequest<List<BankTransactionDto>>
{
    public Guid? BankAccountId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetDailyTransactionSummaryQry : IRequest<List<DailyTransactionSummaryDto>>
{
    public Guid? BankAccountId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetRecentTransactionsQry : IRequest<List<BankTransactionDto>>
{
    public Guid? BankAccountId { get; set; }
    public int Count { get; set; } = 10;
}

// ============================================================
// DTOs
// ============================================================

public class DailyTransactionSummaryDto
{
    public DateTime Date { get; set; }
    public int TransactionCount { get; set; }
    public decimal TotalDeposits { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public decimal NetChange { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
}

// ============================================================
// GET ALL BANK TRANSACTIONS HANDLER
// ============================================================

public class GetAllBankTransactionsHandler : IRequestHandler<GetAllBankTransactionsQry, List<BankTransactionDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllBankTransactionsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankTransactionDto>> Handle(GetAllBankTransactionsQry request, CancellationToken ct)
    {
        var query = _context.BankTransactions
            .Include(x => x.Period)
            .Include(x => x.BankAccount)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.PeriodId.HasValue)
            query = query.Where(x => x.PeriodId == request.PeriodId.Value);

        if (request.BankAccountId.HasValue)
            query = query.Where(x => x.BankAccountId == request.BankAccountId.Value);

        if (request.IsReconciled.HasValue)
            query = query.Where(x => x.IsReconciled == request.IsReconciled.Value);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.TransactionDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.TransactionDate <= request.ToDate.Value);

        if (!string.IsNullOrEmpty(request.TransactionType))
            query = query.Where(x => x.TransactionType == request.TransactionType);

        if (!string.IsNullOrEmpty(request.PaymentMethod))
            query = query.Where(x => x.PaymentMethod == request.PaymentMethod);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status.ToString() == request.Status);

        var transactions = await query
            .OrderByDescending(x => x.TransactionDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var result = new List<BankTransactionDto>();
        foreach (var transaction in transactions)
        {
            result.Add(await MapToDto(transaction, ct));
        }

        return result;
    }

    private async Task<BankTransactionDto> MapToDto(BankTransaction transaction, CancellationToken ct)
    {
        // ✅ FIX: Added OrderBy to eliminate warning
        var account = await _context.BankAccounts
            .OrderBy(x => x.AccountName)  // ✅ Added OrderBy
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
            PeriodName = transaction.Period?.Name,
            IsReconciled = transaction.IsReconciled,
            ReconciliationDate = transaction.ReconciliationDate,
            Status = transaction.Status.ToString(),
            DateAdd = transaction.DateAdd,
            DateMod = transaction.DateMod,
        };
    }
}

// ============================================================
// GET BANK TRANSACTION BY ID HANDLER
// ============================================================

public class GetBankTransactionByIdHandler : IRequestHandler<GetBankTransactionByIdQry, BankTransactionDto>
{
    private readonly FinanceDbContext _context;

    public GetBankTransactionByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BankTransactionDto> Handle(GetBankTransactionByIdQry request, CancellationToken ct)
    {
        // ✅ FIX: Added OrderBy to eliminate warning
        var transaction = await _context.BankTransactions
            .OrderBy(x => x.TransactionDate)  // ✅ Added OrderBy
            .Include(x => x.Period)
            .Include(x => x.BankAccount)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (transaction == null)
            throw new InvalidOperationException($"Bank transaction with ID '{request.Id}' not found");

        return await MapToDto(transaction, ct);
    }

    private async Task<BankTransactionDto> MapToDto(BankTransaction transaction, CancellationToken ct)
    {
        // ✅ FIX: Added OrderBy to eliminate warning
        var account = await _context.BankAccounts
            .OrderBy(x => x.AccountName)  // ✅ Added OrderBy
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
            PeriodName = transaction.Period?.Name,
            IsReconciled = transaction.IsReconciled,
            ReconciliationDate = transaction.ReconciliationDate,
            Status = transaction.Status.ToString(),
            DateAdd = transaction.DateAdd,
            DateMod = transaction.DateMod,
        };
    }
}

// ============================================================
// GET BANK TRANSACTIONS BY PERIOD HANDLER
// ============================================================

public class GetBankTransactionsByPeriodHandler : IRequestHandler<GetBankTransactionsByPeriodQry, List<BankTransactionDto>>
{
    private readonly FinanceDbContext _context;

    public GetBankTransactionsByPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankTransactionDto>> Handle(GetBankTransactionsByPeriodQry request, CancellationToken ct)
    {
        var transactions = await _context.BankTransactions
            .Include(x => x.Period)
            .Include(x => x.BankAccount)
            .Where(x => x.PeriodId == request.PeriodId && !x.IsDeleted)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(ct);

        var result = new List<BankTransactionDto>();
        foreach (var transaction in transactions)
        {
            result.Add(await MapToDto(transaction, ct));
        }

        return result;
    }

    private async Task<BankTransactionDto> MapToDto(BankTransaction transaction, CancellationToken ct)
    {
        // ✅ FIX: Added OrderBy to eliminate warning
        var account = await _context.BankAccounts
            .OrderBy(x => x.AccountName)  // ✅ Added OrderBy
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
            PeriodName = transaction.Period?.Name,
            IsReconciled = transaction.IsReconciled,
            ReconciliationDate = transaction.ReconciliationDate,
            Status = transaction.Status.ToString(),
            DateAdd = transaction.DateAdd,
            DateMod = transaction.DateMod,
        };
    }
}

// ============================================================
// GET RECONCILIATION SUMMARY HANDLER
// ============================================================

public class GetReconciliationSummaryHandler : IRequestHandler<GetReconciliationSummaryQry, ReconciliationSummaryDto>
{
    private readonly FinanceDbContext _context;

    public GetReconciliationSummaryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ReconciliationSummaryDto> Handle(GetReconciliationSummaryQry request, CancellationToken ct)
    {
        var query = _context.BankTransactions
            .Include(x => x.Period)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.BankAccountId.HasValue)
            query = query.Where(x => x.BankAccountId == request.BankAccountId.Value);

        if (request.PeriodId.HasValue)
            query = query.Where(x => x.PeriodId == request.PeriodId.Value);

        var transactions = await query.ToListAsync(ct);

        var totalTransactions = transactions.Count;
        var reconciledCount = transactions.Count(x => x.IsReconciled);
        var unreconciledCount = totalTransactions - reconciledCount;
        var totalAmount = transactions.Sum(x => x.Amount);
        var reconciledAmount = transactions.Where(x => x.IsReconciled).Sum(x => x.Amount);
        var unreconciledAmount = totalAmount - reconciledAmount;
        var progress = totalTransactions > 0 ? (decimal)reconciledCount / totalTransactions * 100 : 0;

        string accountName = "";
        string periodName = "";

        if (request.BankAccountId.HasValue)
        {
            // ✅ FIX: Added OrderBy to eliminate warning
            var account = await _context.BankAccounts
                .OrderBy(x => x.AccountName)  // ✅ Added OrderBy
                .FirstOrDefaultAsync(x => x.Id == request.BankAccountId.Value && !x.IsDeleted, ct);
            accountName = account?.AccountName ?? "";
        }

        if (request.PeriodId.HasValue)
        {
            // ✅ FIX: Added OrderBy to eliminate warning
            var period = await _context.FinancialPeriods
                .OrderBy(x => x.Name)  // ✅ Added OrderBy
                .FirstOrDefaultAsync(x => x.Id == request.PeriodId.Value && !x.IsDeleted, ct);
            periodName = period?.Name ?? "";
        }

        return new ReconciliationSummaryDto
        {
            BankAccountId = request.BankAccountId,
            BankAccountName = accountName,
            PeriodId = request.PeriodId ?? Guid.Empty,
            PeriodName = periodName,
            TotalTransactions = totalTransactions,
            ReconciledCount = reconciledCount,
            UnreconciledCount = unreconciledCount,
            TotalAmount = totalAmount,
            ReconciledAmount = reconciledAmount,
            UnreconciledAmount = unreconciledAmount,
            ReconciliationProgress = Math.Round(progress, 2),
        };
    }
}

// ============================================================
// GET TRANSACTION STATS HANDLER
// ============================================================

public class GetTransactionStatsHandler : IRequestHandler<GetTransactionStatsQry, TransactionStatsDto>
{
    private readonly FinanceDbContext _context;

    public GetTransactionStatsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionStatsDto> Handle(GetTransactionStatsQry request, CancellationToken ct)
    {
        var query = _context.BankTransactions
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.BankAccountId.HasValue)
            query = query.Where(x => x.BankAccountId == request.BankAccountId.Value);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.TransactionDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.TransactionDate <= request.ToDate.Value);

        var transactions = await query.ToListAsync(ct);

        var totalTransactions = transactions.Count;
        var reconciledCount = transactions.Count(x => x.IsReconciled);
        var unreconciledCount = totalTransactions - reconciledCount;

        var deposits = transactions.Where(x => x.TransactionType == "Deposit" || x.TransactionType == "Replenishment" || x.TransactionType == "OpeningBalance");
        var withdrawals = transactions.Where(x => x.TransactionType == "Withdrawal" || x.TransactionType == "Expense" || x.TransactionType == "Transfer");

        var totalDeposits = deposits.Sum(x => x.Amount);
        var totalWithdrawals = withdrawals.Sum(x => x.Amount);

        var amounts = transactions.Select(x => x.Amount).ToList();
        var avgAmount = amounts.Any() ? amounts.Average() : 0;
        var minAmount = amounts.Any() ? amounts.Min() : 0;
        var maxAmount = amounts.Any() ? amounts.Max() : 0;

        var firstDate = transactions.Any() ? transactions.Min(x => x.TransactionDate) : (DateTime?)null;
        var lastDate = transactions.Any() ? transactions.Max(x => x.TransactionDate) : (DateTime?)null;

        var byType = transactions
            .GroupBy(x => x.TransactionType)
            .ToDictionary(g => g.Key, g => g.Count());

        var byTypeAmount = transactions
            .GroupBy(x => x.TransactionType)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

        var byStatus = transactions
            .GroupBy(x => x.Status)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        return new TransactionStatsDto
        {
            TotalTransactions = totalTransactions,
            ReconciledCount = reconciledCount,
            UnreconciledCount = unreconciledCount,
            TotalDeposits = totalDeposits,
            TotalWithdrawals = totalWithdrawals,
            NetChange = totalDeposits - totalWithdrawals,
            AverageTransactionAmount = (decimal)avgAmount,
            MinTransactionAmount = minAmount,
            MaxTransactionAmount = maxAmount,
            FirstTransactionDate = firstDate,
            LastTransactionDate = lastDate,
            TransactionsByType = byType,
            AmountByType = byTypeAmount,
            TransactionsByStatus = byStatus
        };
    }
}

// ============================================================
// GET TRANSACTION TYPES HANDLER
// ============================================================

public class GetTransactionTypesHandler : IRequestHandler<GetTransactionTypesQry, List<string>>
{
    private readonly FinanceDbContext _context;

    public GetTransactionTypesHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> Handle(GetTransactionTypesQry request, CancellationToken ct)
    {
        var types = await _context.BankTransactions
            .Where(x => !x.IsDeleted)
            .Select(x => x.TransactionType)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);

        if (!types.Any())
        {
            types = new List<string>
            {
                "Deposit",
                "Withdrawal",
                "Transfer",
                "Expense",
                "Replenishment",
                "OpeningBalance",
                "Adjustment",
                "Interest",
                "Fee"
            };
        }

        return types;
    }
}

// ============================================================
// EXPORT TRANSACTIONS HANDLER
// ============================================================

public class ExportTransactionsHandler : IRequestHandler<ExportTransactionsQry, List<BankTransactionDto>>
{
    private readonly FinanceDbContext _context;

    public ExportTransactionsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankTransactionDto>> Handle(ExportTransactionsQry request, CancellationToken ct)
    {
        var query = _context.BankTransactions
            .Include(x => x.Period)
            .Include(x => x.BankAccount)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.BankAccountId.HasValue)
            query = query.Where(x => x.BankAccountId == request.BankAccountId.Value);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.TransactionDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.TransactionDate <= request.ToDate.Value);

        var transactions = await query
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(ct);

        var result = new List<BankTransactionDto>();
        foreach (var transaction in transactions)
        {
            result.Add(await MapToDto(transaction, ct));
        }

        return result;
    }

    private async Task<BankTransactionDto> MapToDto(BankTransaction transaction, CancellationToken ct)
    {
        // ✅ FIX: Added OrderBy to eliminate warning
        var account = await _context.BankAccounts
            .OrderBy(x => x.AccountName)  // ✅ Added OrderBy
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
            PeriodName = transaction.Period?.Name,
            IsReconciled = transaction.IsReconciled,
            ReconciliationDate = transaction.ReconciliationDate,
            Status = transaction.Status.ToString(),
            DateAdd = transaction.DateAdd,
            DateMod = transaction.DateMod,
        };
    }
}

// ============================================================
// GET DAILY TRANSACTION SUMMARY HANDLER
// ============================================================

public class GetDailyTransactionSummaryHandler : IRequestHandler<GetDailyTransactionSummaryQry, List<DailyTransactionSummaryDto>>
{
    private readonly FinanceDbContext _context;

    public GetDailyTransactionSummaryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<DailyTransactionSummaryDto>> Handle(GetDailyTransactionSummaryQry request, CancellationToken ct)
    {
        var query = _context.BankTransactions
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.BankAccountId.HasValue)
            query = query.Where(x => x.BankAccountId == request.BankAccountId.Value);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.TransactionDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.TransactionDate <= request.ToDate.Value);

        var transactions = await query
            .OrderBy(x => x.TransactionDate)
            .ToListAsync(ct);

        var dailySummary = transactions
            .GroupBy(x => x.TransactionDate.Date)
            .Select(g => new DailyTransactionSummaryDto
            {
                Date = g.Key,
                TransactionCount = g.Count(),
                TotalDeposits = g.Where(x => x.TransactionType == "Deposit" || x.TransactionType == "Replenishment" || x.TransactionType == "OpeningBalance").Sum(x => x.Amount),
                TotalWithdrawals = g.Where(x => x.TransactionType == "Withdrawal" || x.TransactionType == "Expense" || x.TransactionType == "Transfer").Sum(x => x.Amount),
                NetChange = g.Where(x => x.TransactionType == "Deposit" || x.TransactionType == "Replenishment" || x.TransactionType == "OpeningBalance").Sum(x => x.Amount)
                            - g.Where(x => x.TransactionType == "Withdrawal" || x.TransactionType == "Expense" || x.TransactionType == "Transfer").Sum(x => x.Amount)
            })
            .OrderBy(x => x.Date)
            .ToList();

        // Calculate running balances
        decimal runningBalance = 0;
        foreach (var day in dailySummary)
        {
            day.OpeningBalance = runningBalance;
            runningBalance += day.NetChange;
            day.ClosingBalance = runningBalance;
        }

        return dailySummary;
    }
}

// ============================================================
// GET RECENT TRANSACTIONS HANDLER
// ============================================================

public class GetRecentTransactionsHandler : IRequestHandler<GetRecentTransactionsQry, List<BankTransactionDto>>
{
    private readonly FinanceDbContext _context;

    public GetRecentTransactionsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankTransactionDto>> Handle(GetRecentTransactionsQry request, CancellationToken ct)
    {
        var query = _context.BankTransactions
            .Include(x => x.Period)
            .Include(x => x.BankAccount)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.BankAccountId.HasValue)
            query = query.Where(x => x.BankAccountId == request.BankAccountId.Value);

        var transactions = await query
            .OrderByDescending(x => x.TransactionDate)
            .Take(request.Count)
            .ToListAsync(ct);

        var result = new List<BankTransactionDto>();
        foreach (var transaction in transactions)
        {
            result.Add(await MapToDto(transaction, ct));
        }

        return result;
    }

    private async Task<BankTransactionDto> MapToDto(BankTransaction transaction, CancellationToken ct)
    {
        // ✅ FIX: Added OrderBy to eliminate warning
        var account = await _context.BankAccounts
            .OrderBy(x => x.AccountName)  // ✅ Added OrderBy
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
            PeriodName = transaction.Period?.Name,
            IsReconciled = transaction.IsReconciled,
            ReconciliationDate = transaction.ReconciliationDate,
            Status = transaction.Status.ToString(),
            DateAdd = transaction.DateAdd,
            DateMod = transaction.DateMod,
        };
    }
}
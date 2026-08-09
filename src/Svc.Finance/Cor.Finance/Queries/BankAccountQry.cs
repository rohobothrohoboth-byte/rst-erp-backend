// Cor.Finance/Queries/BankAccountQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;

namespace Cor.Finance.Queries;

// ============================================================
// QUERIES
// ============================================================

public class GetAllBankAccountsQry : IRequest<PaginatedResponse<BankAccountDto>>
{
    public bool? IsActive { get; set; }
    public Guid? BranchId { get; set; }
    public string? AccountType { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "AccountName";
    public string? SortOrder { get; set; } = "ASC";
}

public class GetBankAccountByIdQry : IRequest<BankAccountDto>
{
    public Guid Id { get; set; }
}

public class GetBankAccountByNumberQry : IRequest<BankAccountDto>
{
    public string AccountNumber { get; set; } = string.Empty;
}

public class GetBankAccountBalanceQry : IRequest<BankAccountBalanceDto>
{
    public Guid Id { get; set; }
    public Guid? PeriodId { get; set; }
    public DateTime? AsOfDate { get; set; }
}

public class GetBankAccountSummaryQry : IRequest<BankAccountSummaryDto>
{
    public Guid Id { get; set; }
    public Guid? PeriodId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetBankAccountsByBranchQry : IRequest<List<BankAccountDto>>
{
    public Guid BranchId { get; set; }
    public bool? IsActive { get; set; }
}

public class GetBankAccountTypesQry : IRequest<List<string>>
{
}

public class ExportBankAccountsQry : IRequest<List<BankAccountDto>>
{
    public bool? IsActive { get; set; }
    public Guid? BranchId { get; set; }
}

public class BankAccountHasTransactionsQry : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class GetPeriodByIdQry : IRequest<FinancialPeriodDto?>
{
    public Guid Id { get; set; }
}

// ============================================================
// HANDLERS
// ============================================================

public class GetAllBankAccountsHandler : IRequestHandler<GetAllBankAccountsQry, PaginatedResponse<BankAccountDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllBankAccountsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<BankAccountDto>> Handle(GetAllBankAccountsQry request, CancellationToken ct)
    {
        var query = _context.BankAccounts
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (request.BranchId.HasValue)
            query = query.Where(x => x.BranchId == request.BranchId.Value);

        if (!string.IsNullOrEmpty(request.AccountType))
            query = query.Where(x => x.AccountType == request.AccountType);

        if (!string.IsNullOrEmpty(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(x =>
                x.AccountName.ToLower().Contains(search) ||
                x.AccountNumber.ToLower().Contains(search) ||
                x.BankName.ToLower().Contains(search) ||
                (x.IBAN != null && x.IBAN.ToLower().Contains(search)) ||
                (x.Description != null && x.Description.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync(ct);

        // Apply sorting
        query = request.SortOrder?.ToUpper() == "DESC"
            ? query.OrderByDescending(x => EF.Property<object>(x, request.SortBy ?? "AccountName"))
            : query.OrderBy(x => EF.Property<object>(x, request.SortBy ?? "AccountName"));

        var accounts = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var result = new List<BankAccountDto>();
        foreach (var account in accounts)
        {
            result.Add(await MapToDto(account, ct));
        }

        return new PaginatedResponse<BankAccountDto>
        {
            Data = result,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            HasNextPage = request.Page < (int)Math.Ceiling((double)totalCount / request.PageSize),
            HasPreviousPage = request.Page > 1
        };
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

public class GetBankAccountByIdHandler : IRequestHandler<GetBankAccountByIdQry, BankAccountDto>
{
    private readonly FinanceDbContext _context;

    public GetBankAccountByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccountDto> Handle(GetBankAccountByIdQry request, CancellationToken ct)
    {
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (account == null)
            throw new InvalidOperationException($"Bank account with ID '{request.Id}' not found");

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

public class GetBankAccountByNumberHandler : IRequestHandler<GetBankAccountByNumberQry, BankAccountDto>
{
    private readonly FinanceDbContext _context;

    public GetBankAccountByNumberHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccountDto> Handle(GetBankAccountByNumberQry request, CancellationToken ct)
    {
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.AccountNumber == request.AccountNumber && !x.IsDeleted, ct);

        if (account == null)
            throw new InvalidOperationException($"Bank account with number '{request.AccountNumber}' not found");

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

public class GetBankAccountBalanceHandler : IRequestHandler<GetBankAccountBalanceQry, BankAccountBalanceDto>
{
    private readonly FinanceDbContext _context;

    public GetBankAccountBalanceHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccountBalanceDto> Handle(GetBankAccountBalanceQry request, CancellationToken ct)
    {
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (account == null)
            throw new InvalidOperationException($"Bank account with ID '{request.Id}' not found");

        var periodName = "";
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.PeriodId && !x.IsDeleted, ct);

        if (period != null)
            periodName = period.Name;

        var asOfDate = request.AsOfDate ?? DateTime.UtcNow;

        var transactionsQuery = _context.BankTransactions
            .Where(x => x.BankAccountId == request.Id && !x.IsDeleted)
            .Where(x => x.TransactionDate <= asOfDate);

        if (request.PeriodId.HasValue)
            transactionsQuery = transactionsQuery.Where(x => x.PeriodId == request.PeriodId.Value);

        var transactions = await transactionsQuery.ToListAsync(ct);

        var totalDeposits = transactions
            .Where(x => x.TransactionType == "Deposit" || x.TransactionType == "Replenishment" || x.TransactionType == "OpeningBalance")
            .Sum(x => x.Amount);

        var totalWithdrawals = transactions
            .Where(x => x.TransactionType == "Withdrawal" || x.TransactionType == "Expense" || x.TransactionType == "Transfer")
            .Sum(x => x.Amount);

        var netChange = totalDeposits - totalWithdrawals;

        return new BankAccountBalanceDto
        {
            Id = account.Id,
            AccountName = account.AccountName,
            AccountNumber = account.AccountNumber,
            CurrentBalance = account.CurrentBalance,
            AvailableBalance = account.AvailableBalance,
            OpeningBalance = account.OpeningBalance,
            TotalDeposits = totalDeposits,
            TotalWithdrawals = totalWithdrawals,
            NetChange = netChange,
            AsOfDate = asOfDate,
            PeriodId = request.PeriodId,
            PeriodName = periodName
        };
    }
}

public class GetBankAccountSummaryHandler : IRequestHandler<GetBankAccountSummaryQry, BankAccountSummaryDto>
{
    private readonly FinanceDbContext _context;

    public GetBankAccountSummaryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccountSummaryDto> Handle(GetBankAccountSummaryQry request, CancellationToken ct)
    {
        var account = await _context.BankAccounts
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (account == null)
            throw new InvalidOperationException($"Bank account with ID '{request.Id}' not found");

        var transactionsQuery = _context.BankTransactions
            .Where(x => x.BankAccountId == request.Id && !x.IsDeleted)
            .AsQueryable();

        if (request.FromDate.HasValue)
            transactionsQuery = transactionsQuery.Where(x => x.TransactionDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            transactionsQuery = transactionsQuery.Where(x => x.TransactionDate <= request.ToDate.Value);

        if (request.PeriodId.HasValue)
            transactionsQuery = transactionsQuery.Where(x => x.PeriodId == request.PeriodId.Value);

        var transactions = await transactionsQuery.ToListAsync(ct);

        var totalDebit = transactions
            .Where(x => x.TransactionType == "Withdrawal" || x.TransactionType == "Expense" || x.TransactionType == "Transfer")
            .Sum(x => x.Amount);

        var totalCredit = transactions
            .Where(x => x.TransactionType == "Deposit" || x.TransactionType == "Replenishment" || x.TransactionType == "OpeningBalance")
            .Sum(x => x.Amount);

        var balances = transactions
            .OrderBy(x => x.TransactionDate)
            .Select(x => x.Amount)
            .ToList();

        var avgBalance = balances.Any() ? balances.Average() : 0;
        var minBalance = balances.Any() ? balances.Min() : 0;
        var maxBalance = balances.Any() ? balances.Max() : 0;

        return new BankAccountSummaryDto
        {
            Id = account.Id,
            AccountName = account.AccountName,
            AccountNumber = account.AccountNumber,
            TotalTransactions = transactions.Count,
            TotalDebit = totalDebit,
            TotalCredit = totalCredit,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            AverageBalance = (decimal)avgBalance,
            MinBalance = minBalance,
            MaxBalance = maxBalance
        };
    }
}

public class GetBankAccountsByBranchHandler : IRequestHandler<GetBankAccountsByBranchQry, List<BankAccountDto>>
{
    private readonly FinanceDbContext _context;

    public GetBankAccountsByBranchHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankAccountDto>> Handle(GetBankAccountsByBranchQry request, CancellationToken ct)
    {
        var query = _context.BankAccounts
            .Where(x => !x.IsDeleted && x.BranchId == request.BranchId)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        var accounts = await query
            .OrderBy(x => x.AccountName)
            .ToListAsync(ct);

        var result = new List<BankAccountDto>();
        foreach (var account in accounts)
        {
            result.Add(await MapToDto(account, ct));
        }

        return result;
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

public class GetBankAccountTypesHandler : IRequestHandler<GetBankAccountTypesQry, List<string>>
{
    private readonly FinanceDbContext _context;

    public GetBankAccountTypesHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> Handle(GetBankAccountTypesQry request, CancellationToken ct)
    {
        var types = await _context.BankAccounts
            .Where(x => !x.IsDeleted)
            .Select(x => x.AccountType)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);

        if (!types.Any())
        {
            types = new List<string>
            {
                "Checking",
                "Savings",
                "Cash",
                "Petty Cash",
                "Investment",
                "Loan",
                "Credit Card",
                "Merchant Account"
            };
        }

        return types;
    }
}

public class ExportBankAccountsHandler : IRequestHandler<ExportBankAccountsQry, List<BankAccountDto>>
{
    private readonly FinanceDbContext _context;

    public ExportBankAccountsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankAccountDto>> Handle(ExportBankAccountsQry request, CancellationToken ct)
    {
        var query = _context.BankAccounts
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (request.BranchId.HasValue)
            query = query.Where(x => x.BranchId == request.BranchId.Value);

        var accounts = await query
            .OrderBy(x => x.AccountName)
            .ToListAsync(ct);

        var result = new List<BankAccountDto>();
        foreach (var account in accounts)
        {
            result.Add(await MapToDto(account, ct));
        }

        return result;
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
// Queries/BankAccountQry.cs - Add this query

public class GetDefaultBankAccountsQry : IRequest<List<BankAccountDto>>
{
}

public class GetDefaultBankAccountsHandler : IRequestHandler<GetDefaultBankAccountsQry, List<BankAccountDto>>
{
    private readonly FinanceDbContext _context;

    public GetDefaultBankAccountsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankAccountDto>> Handle(GetDefaultBankAccountsQry request, CancellationToken ct)
    {
        var accounts = await _context.BankAccounts
            .Where(x => !x.IsDeleted && x.IsDefault)
            .OrderBy(x => x.AccountName)
            .ToListAsync(ct);

        var result = new List<BankAccountDto>();
        foreach (var account in accounts)
        {
            // Use the same mapping logic as other handlers
            var handler = new GetBankAccountByIdHandler(_context);
            result.Add(await handler.Handle(new GetBankAccountByIdQry { Id = account.Id }, ct));
        }

        return result;
    }
}
public class BankAccountHasTransactionsHandler : IRequestHandler<BankAccountHasTransactionsQry, bool>
{
    private readonly FinanceDbContext _context;

    public BankAccountHasTransactionsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(BankAccountHasTransactionsQry request, CancellationToken ct)
    {
        var hasTransactions = await _context.BankTransactions
            .AnyAsync(x => x.BankAccountId == request.Id && !x.IsDeleted, ct);

        return hasTransactions;
    }
}

public class GetPeriodByIdHandler : IRequestHandler<GetPeriodByIdQry, FinancialPeriodDto?>
{
    private readonly FinanceDbContext _context;

    public GetPeriodByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialPeriodDto?> Handle(GetPeriodByIdQry request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (period == null)
            return null;

        return new FinancialPeriodDto
        {
            Id = period.Id,
            Name = period.Name,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            PeriodType = period.PeriodType.ToString(),
            IsClosed = period.IsClosed,
            Status = period.Status.ToString(),
            ClosedDate = period.ClosedDate,
            DateAdd = period.DateAdd,
            DateMod = period.DateMod
        };
    }
}
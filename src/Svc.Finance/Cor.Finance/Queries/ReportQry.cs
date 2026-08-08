using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Helpers;
namespace Cor.Finance.Queries;



// ==================== INCOME STATEMENT ====================

public class GetIncomeStatementQry : IRequest<IncomeStatementDto>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? BranchId { get; set; }
}

public class GetIncomeStatementHandler : IRequestHandler<GetIncomeStatementQry, IncomeStatementDto>
{
    private readonly FinanceDbContext _context;

    public GetIncomeStatementHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IncomeStatementDto> Handle(GetIncomeStatementQry request, CancellationToken ct)
    {
        // Convert dates to UTC
        var startDateUtc = request.StartDate.ToUtc();
        var endDateUtc = request.EndDate.ToUtc().Date.AddDays(1).AddTicks(-1); // End of day

        // Get all journal entries in the period
        var entries = await _context.JournalEntries
            .Where(x => !x.IsDeleted && x.IsPosted &&
                x.EntryDate >= startDateUtc && x.EntryDate <= endDateUtc)
            .ToListAsync(ct);

        var entryIds = entries.Select(e => e.Id).ToList();

        // Get all journal lines
        var lines = await _context.JournalLines
            .Where(x => entryIds.Contains(x.JournalEntryId) && !x.IsDeleted)
            .Include(x => x.Account)
            .ToListAsync(ct);

        // Separate revenue and expenses
        var revenueLines = lines
            .Where(x => x.Account != null && x.Account.AccountType == "Revenue")
            .GroupBy(x => new { x.AccountId, x.Account!.Name, x.Account.Code })
            .Select(g => new IncomeStatementLineDto
            {
                AccountName = g.Key.Name,
                AccountCode = g.Key.Code,
                Amount = g.Sum(x => x.Direction == "Credit" ? x.Amount : -x.Amount)
            })
            .ToList();

        var expenseLines = lines
            .Where(x => x.Account != null && x.Account.AccountType == "Expense")
            .GroupBy(x => new { x.AccountId, x.Account!.Name, x.Account.Code })
            .Select(g => new IncomeStatementLineDto
            {
                AccountName = g.Key.Name,
                AccountCode = g.Key.Code,
                Amount = g.Sum(x => x.Direction == "Debit" ? x.Amount : -x.Amount)
            })
            .ToList();

        var totalRevenue = revenueLines.Sum(x => x.Amount);
        var totalExpenses = expenseLines.Sum(x => x.Amount);
        var grossProfit = totalRevenue - totalExpenses;

        // Calculate percentages
        foreach (var line in revenueLines)
        {
            line.Percentage = totalRevenue > 0 ? (line.Amount / totalRevenue) * 100 : 0;
        }
        foreach (var line in expenseLines)
        {
            line.Percentage = totalExpenses > 0 ? (line.Amount / totalExpenses) * 100 : 0;
        }

        return new IncomeStatementDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalRevenue = totalRevenue,
            TotalExpenses = totalExpenses,
            GrossProfit = grossProfit,
            NetIncome = grossProfit,
            RevenueLines = revenueLines,
            ExpenseLines = expenseLines
        };
    }
}

// ==================== BALANCE SHEET ====================

public class GetBalanceSheetQry : IRequest<BalanceSheetDto>
{
    public DateTime AsOfDate { get; set; }
    public Guid? BranchId { get; set; }
}

public class GetBalanceSheetHandler : IRequestHandler<GetBalanceSheetQry, BalanceSheetDto>
{
    private readonly FinanceDbContext _context;

    public GetBalanceSheetHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BalanceSheetDto> Handle(GetBalanceSheetQry request, CancellationToken ct)
    {
        // Convert to UTC
        var asOfDateUtc = request.AsOfDate.ToUtc().Date.AddDays(1).AddTicks(-1); // End of day

        // Get all accounts
        var accounts = await _context.ChartOfAccounts
            .Where(x => !x.IsDeleted && x.IsActive)
            .ToListAsync(ct);

        // Get all journal entries up to the date
        var entries = await _context.JournalEntries
            .Where(x => !x.IsDeleted && x.IsPosted && x.EntryDate <= asOfDateUtc)
            .ToListAsync(ct);

        var entryIds = entries.Select(e => e.Id).ToList();

        // Get all journal lines
        var lines = await _context.JournalLines
            .Where(x => entryIds.Contains(x.JournalEntryId) && !x.IsDeleted)
            .Include(x => x.Account)
            .ToListAsync(ct);

        // Calculate balances by account
        var accountBalances = accounts.ToDictionary(
            a => a.Id,
            a => new BalanceSheetLineDto
            {
                AccountName = a.Name,
                AccountCode = a.Code,
                Amount = a.OpeningBalance ?? 0
            }
        );

        foreach (var line in lines)
        {
            if (accountBalances.ContainsKey(line.AccountId))
            {
                var balance = accountBalances[line.AccountId];
                if (line.Direction == "Debit")
                    balance.Amount += line.Amount;
                else
                    balance.Amount -= line.Amount;
            }
        }

        // Group by account type
        var assetLines = new List<BalanceSheetLineDto>();
        var liabilityLines = new List<BalanceSheetLineDto>();
        var equityLines = new List<BalanceSheetLineDto>();

        foreach (var account in accounts)
        {
            if (accountBalances.ContainsKey(account.Id))
            {
                var line = accountBalances[account.Id];
                switch (account.AccountType)
                {
                    case "Asset":
                        assetLines.Add(line);
                        break;
                    case "Liability":
                        liabilityLines.Add(line);
                        break;
                    case "Equity":
                        equityLines.Add(line);
                        break;
                }
            }
        }

        return new BalanceSheetDto
        {
            AsOfDate = request.AsOfDate,
            Assets = new BalanceSheetSectionDto
            {
                Name = "Assets",
                Lines = assetLines,
                Total = assetLines.Sum(x => x.Amount)
            },
            Liabilities = new BalanceSheetSectionDto
            {
                Name = "Liabilities",
                Lines = liabilityLines,
                Total = liabilityLines.Sum(x => x.Amount)
            },
            Equity = new BalanceSheetSectionDto
            {
                Name = "Equity",
                Lines = equityLines,
                Total = equityLines.Sum(x => x.Amount)
            },
            TotalAssets = assetLines.Sum(x => x.Amount),
            TotalLiabilities = liabilityLines.Sum(x => x.Amount),
            TotalEquity = equityLines.Sum(x => x.Amount)
        };
    }
}

// ==================== EXPENSE REPORT ====================

public class GetExpenseReportQry : IRequest<ExpenseReportDto>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
}

public class GetExpenseReportHandler : IRequestHandler<GetExpenseReportQry, ExpenseReportDto>
{
    private readonly FinanceDbContext _context;

    public GetExpenseReportHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseReportDto> Handle(GetExpenseReportQry request, CancellationToken ct)
    {
        // Convert dates to UTC
        var startDateUtc = request.StartDate.ToUtc();
        var endDateUtc = request.EndDate.ToUtc().Date.AddDays(1).AddTicks(-1); // End of day

        var query = _context.Expenses
            .Where(x => !x.IsDeleted && x.ExpenseDate >= startDateUtc && x.ExpenseDate <= endDateUtc)
            .AsQueryable();

        if (request.BranchId.HasValue)
            query = query.Where(x => x.BranchId == request.BranchId.Value);

        if (request.DepartmentId.HasValue)
            query = query.Where(x => x.DepartmentId == request.DepartmentId.Value);

        var expenses = await query.ToListAsync(ct);

        // Get category names
        var categoryIds = expenses.Select(x => x.ExpenseCategoryId).Distinct();
        var categories = await _context.ExpenseCategories
            .Where(x => categoryIds.Contains(x.Id) && !x.IsDeleted)
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

        var totalExpenses = expenses.Sum(x => x.Amount);

        // Group by category
        var byCategory = expenses
            .GroupBy(x => x.ExpenseCategoryId)
            .Select(g => new ExpenseByCategoryDto
            {
                CategoryName = categories.ContainsKey(g.Key) ? categories[g.Key] : "Unknown",
                TotalAmount = g.Sum(x => x.Amount),
                TransactionCount = g.Count(),
                Percentage = totalExpenses > 0 ? (g.Sum(x => x.Amount) / totalExpenses) * 100 : 0
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToList();

        // Group by department
        var departments = await _context.LocalDepartments
            .Where(x => expenses.Select(e => e.DepartmentId).Contains(x.Id) && !x.IsDeleted)
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

        var byDepartment = expenses
            .Where(x => x.DepartmentId.HasValue)
           .GroupBy(x => x.DepartmentId!.Value)
            .Select(g => new ExpenseByDepartmentDto
            {
                DepartmentName = departments.ContainsKey(g.Key) ? departments[g.Key] : "Unknown",
                TotalAmount = g.Sum(x => x.Amount),
                TransactionCount = g.Count(),
                Percentage = totalExpenses > 0 ? (g.Sum(x => x.Amount) / totalExpenses) * 100 : 0
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToList();

        return new ExpenseReportDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalExpenses = totalExpenses,
            ExpensesByCategory = byCategory,
            ExpensesByDepartment = byDepartment
        };
    }
}

// ==================== CASH FLOW STATEMENT ====================

public class GetCashFlowStatementQry : IRequest<CashFlowStatementDto>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? BranchId { get; set; }
}

public class GetCashFlowStatementHandler : IRequestHandler<GetCashFlowStatementQry, CashFlowStatementDto>
{
    private readonly FinanceDbContext _context;

    public GetCashFlowStatementHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<CashFlowStatementDto> Handle(GetCashFlowStatementQry request, CancellationToken ct)
    {
        // Convert dates to UTC
        var startDateUtc = request.StartDate.ToUtc();
        var endDateUtc = request.EndDate.ToUtc().Date.AddDays(1).AddTicks(-1); // End of day

        // Get all journal entries in the period
        var entries = await _context.JournalEntries
            .Where(x => !x.IsDeleted && x.IsPosted &&
                x.EntryDate >= startDateUtc && x.EntryDate <= endDateUtc)
            .ToListAsync(ct);

        var entryIds = entries.Select(e => e.Id).ToList();

        // Get all journal lines
        var lines = await _context.JournalLines
            .Where(x => entryIds.Contains(x.JournalEntryId) && !x.IsDeleted)
            .Include(x => x.Account)
            .ToListAsync(ct);

        // Separate by account type for cash flow
        var operatingLines = new List<CashFlowLineDto>();
        var investingLines = new List<CashFlowLineDto>();
        var financingLines = new List<CashFlowLineDto>();

        foreach (var line in lines)
        {
            if (line.Account == null) continue;

            var amount = line.Direction == "Debit" ? -line.Amount : line.Amount;
            var description = $"{line.Account.Code} - {line.Account.Name}";

            switch (line.Account.AccountType)
            {
                case "Asset":
                    if (line.Account.AccountSubType == "Current Asset")
                    {
                        operatingLines.Add(new CashFlowLineDto { Description = description, Amount = amount });
                    }
                    else
                    {
                        investingLines.Add(new CashFlowLineDto { Description = description, Amount = amount });
                    }
                    break;

                case "Liability":
                    if (line.Account.AccountSubType == "Current Liability")
                    {
                        operatingLines.Add(new CashFlowLineDto { Description = description, Amount = amount });
                    }
                    else
                    {
                        financingLines.Add(new CashFlowLineDto { Description = description, Amount = amount });
                    }
                    break;

                case "Equity":
                    financingLines.Add(new CashFlowLineDto { Description = description, Amount = amount });
                    break;

                case "Revenue":
                    operatingLines.Add(new CashFlowLineDto { Description = description, Amount = amount });
                    break;

                case "Expense":
                    operatingLines.Add(new CashFlowLineDto { Description = description, Amount = amount });
                    break;
            }
        }

        var beginningCashBalance = await GetBeginningCashBalance(startDateUtc, ct);

        var totalOperating = operatingLines.Sum(x => x.Amount);
        var totalInvesting = investingLines.Sum(x => x.Amount);
        var totalFinancing = financingLines.Sum(x => x.Amount);
        var netCashFlow = totalOperating + totalInvesting + totalFinancing;
        var endingCashBalance = beginningCashBalance + netCashFlow;

        return new CashFlowStatementDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            OperatingActivities = new CashFlowSectionDto
            {
                Name = "Operating Activities",
                Lines = operatingLines,
                Total = totalOperating
            },
            InvestingActivities = new CashFlowSectionDto
            {
                Name = "Investing Activities",
                Lines = investingLines,
                Total = totalInvesting
            },
            FinancingActivities = new CashFlowSectionDto
            {
                Name = "Financing Activities",
                Lines = financingLines,
                Total = totalFinancing
            },
            NetCashFlow = netCashFlow,
            BeginningCashBalance = beginningCashBalance,
            EndingCashBalance = endingCashBalance
        };
    }

    private async Task<decimal> GetBeginningCashBalance(DateTime startDateUtc, CancellationToken ct)
    {
        // Get all cash accounts
        var cashAccounts = await _context.ChartOfAccounts
            .Where(x => !x.IsDeleted && (x.Code.StartsWith("1") || x.Name.Contains("Cash")))
            .ToListAsync(ct);

        if (!cashAccounts.Any())
            return 0;

        var accountIds = cashAccounts.Select(a => a.Id).ToList();

        // Get all journal entries before the start date
        var entries = await _context.JournalEntries
            .Where(x => !x.IsDeleted && x.IsPosted && x.EntryDate < startDateUtc)
            .ToListAsync(ct);

        var entryIds = entries.Select(e => e.Id).ToList();

        // Get all journal lines for cash accounts
        var lines = await _context.JournalLines
            .Where(x => entryIds.Contains(x.JournalEntryId) && !x.IsDeleted && accountIds.Contains(x.AccountId))
            .Include(x => x.Account)
            .ToListAsync(ct);

        // Calculate total cash balance
        var balance = 0m;
        foreach (var line in lines)
        {
            if (line.Account != null && line.Account.AccountType == "Asset")
            {
                balance += line.Direction == "Debit" ? line.Amount : -line.Amount;
            }
            else
            {
                balance += line.Direction == "Credit" ? line.Amount : -line.Amount;
            }
        }

        // Add opening balances from accounts
        foreach (var account in cashAccounts)
        {
            balance += account.OpeningBalance ?? 0;
        }

        return balance;
    }
}

// ==================== BUDGET VS ACTUAL ====================

public class GetBudgetVsActualQry : IRequest<BudgetVsActualDto>
{
    public Guid BudgetId { get; set; }
    public DateTime? ActualEndDate { get; set; }
}

public class GetBudgetVsActualHandler : IRequestHandler<GetBudgetVsActualQry, BudgetVsActualDto>
{
    private readonly FinanceDbContext _context;

    public GetBudgetVsActualHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetVsActualDto> Handle(GetBudgetVsActualQry request, CancellationToken ct)
    {
        // Get budget
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(x => x.Id == request.BudgetId && !x.IsDeleted, ct);

        if (budget == null)
            throw new InvalidOperationException($"Budget with ID '{request.BudgetId}' not found");

        // Get budget lines
        var budgetLines = await _context.BudgetLines
            .Where(x => x.BudgetId == budget.Id && !x.IsDeleted)
            .Include(x => x.Account)
            .ToListAsync(ct);

        // Get actual expenses for the budget period
        var actualEndDate = request.ActualEndDate?.ToUtc() ?? DateTime.UtcNow;
        var startDateUtc = budget.StartDate.ToUtc();
        var endDateUtc = actualEndDate.Date.AddDays(1).AddTicks(-1);

        var actualExpenses = await _context.Expenses
            .Where(x => !x.IsDeleted && x.ExpenseDate >= startDateUtc && x.ExpenseDate <= endDateUtc)
            .ToListAsync(ct);

        // Calculate actual by account
        var actualByAccount = actualExpenses
            .GroupBy(x => x.ExpenseCategoryId)
            .Select(g => new { AccountId = g.Key, Amount = g.Sum(x => x.Amount) })
            .ToDictionary(x => x.AccountId, x => x.Amount);

        // Build budget vs actual lines
        var lines = new List<BudgetVsActualLineDto>();
        decimal totalBudget = 0;
        decimal totalActual = 0;

        foreach (var line in budgetLines)
        {
            var actualAmount = actualByAccount.ContainsKey(line.AccountId) ? actualByAccount[line.AccountId] : 0;
            var variance = actualAmount - line.AllocatedAmount;

            lines.Add(new BudgetVsActualLineDto
            {
                AccountName = line.Account?.Name ?? "Unknown",
                AccountCode = line.Account?.Code ?? "",
                BudgetAmount = line.AllocatedAmount,
                ActualAmount = actualAmount,
                Variance = variance,
                VariancePercentage = line.AllocatedAmount > 0 ? (variance / line.AllocatedAmount) * 100 : 0
            });

            totalBudget += line.AllocatedAmount;
            totalActual += actualAmount;
        }

        var totalVariance = totalActual - totalBudget;

        return new BudgetVsActualDto
        {
            BudgetName = budget.Name,
            StartDate = budget.StartDate,
            EndDate = budget.EndDate,
            BudgetAmount = totalBudget,
            ActualAmount = totalActual,
            Variance = totalVariance,
            VariancePercentage = totalBudget > 0 ? (totalVariance / totalBudget) * 100 : 0,
            Lines = lines
        };
    }
}
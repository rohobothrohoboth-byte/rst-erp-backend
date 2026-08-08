namespace Cor.Finance.Models.DTOs;

// ==================== INCOME STATEMENT ====================

public class IncomeStatementDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal NetIncome { get; set; }
    public List<IncomeStatementLineDto> RevenueLines { get; set; } = new();
    public List<IncomeStatementLineDto> ExpenseLines { get; set; } = new();
}

public class IncomeStatementLineDto
{
    public string AccountName { get; set; } = default!;
    public string AccountCode { get; set; } = default!;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

// ==================== BALANCE SHEET ====================

public class BalanceSheetDto
{
    public DateTime AsOfDate { get; set; }
    public BalanceSheetSectionDto Assets { get; set; } = new();
    public BalanceSheetSectionDto Liabilities { get; set; } = new();
    public BalanceSheetSectionDto Equity { get; set; } = new();
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
}

public class BalanceSheetSectionDto
{
    public string Name { get; set; } = default!;
    public List<BalanceSheetLineDto> Lines { get; set; } = new();
    public decimal Total { get; set; }
}

public class BalanceSheetLineDto
{
    public string AccountName { get; set; } = default!;
    public string AccountCode { get; set; } = default!;
    public decimal Amount { get; set; }
}

// ==================== CASH FLOW STATEMENT ====================

public class CashFlowStatementDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CashFlowSectionDto OperatingActivities { get; set; } = new();
    public CashFlowSectionDto InvestingActivities { get; set; } = new();
    public CashFlowSectionDto FinancingActivities { get; set; } = new();
    public decimal NetCashFlow { get; set; }
    public decimal BeginningCashBalance { get; set; }
    public decimal EndingCashBalance { get; set; }
}

public class CashFlowSectionDto
{
    public string Name { get; set; } = default!;
    public List<CashFlowLineDto> Lines { get; set; } = new();
    public decimal Total { get; set; }
}

public class CashFlowLineDto
{
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
}

// ==================== EXPENSE REPORT ====================

public class ExpenseReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalExpenses { get; set; }
    public List<ExpenseByCategoryDto> ExpensesByCategory { get; set; } = new();
    public List<ExpenseByDepartmentDto> ExpensesByDepartment { get; set; } = new();
}

public class ExpenseByCategoryDto
{
    public string CategoryName { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public decimal Percentage { get; set; }
}

public class ExpenseByDepartmentDto
{
    public string DepartmentName { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public decimal Percentage { get; set; }
}

// ==================== BUDGET VS ACTUAL ====================

public class BudgetVsActualDto
{
    public string BudgetName { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal BudgetAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercentage { get; set; }
    public List<BudgetVsActualLineDto> Lines { get; set; } = new();
}

public class BudgetVsActualLineDto
{
    public string AccountName { get; set; } = default!;
    public string AccountCode { get; set; } = default!;
    public decimal BudgetAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercentage { get; set; }
}
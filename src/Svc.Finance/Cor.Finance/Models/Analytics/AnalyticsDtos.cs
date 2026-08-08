// Models/Analytics/AnalyticsDtos.cs
using Cor.Finance.Models.DTOs;

namespace Cor.Finance.Models.Analytics;

public class AnalyticsDashboardDto
{
    // Sales Revenue (Accounts Receivable)
    public decimal MonthlyRevenue { get; set; }
    public int MonthlyCount { get; set; }
    public decimal AverageInvoice { get; set; }
    public decimal OverdueAmount { get; set; }
    public decimal YearlyRevenue { get; set; }
    public double MonthOverMonthGrowth { get; set; }
       public double PurchaseMonthOverMonthGrowth { get; set; }
     public string? CacheKey { get; set; }
    public DateTime? CacheDate { get; set; }
    // Purchase Expenses (Supplier Bills)
    public decimal MonthlyPurchaseExpense { get; set; }
    public int MonthlyPurchaseCount { get; set; }
    public decimal AveragePurchaseInvoice { get; set; }
    public decimal PurchaseOverdueAmount { get; set; }
    public decimal YearlyPurchaseExpense { get; set; }


    // Operating Expenses (Payroll, Rent, Utilities, etc.)
    public decimal OperatingExpenses { get; set; }

    // Combined
    public decimal TotalExpenses { get; set; }
    public decimal TotalBudgetAmount { get; set; }
    public int TotalBudgets { get; set; }

    public List<AnalyticsDto> TopCustomers { get; set; } = new();
    public List<AnalyticsDto> TopVendors { get; set; } = new();
    public DateTime DateGenerated { get; set; }

    // ✅ Period filter information
    public string PeriodStart { get; set; } = string.Empty;
    public string PeriodEnd { get; set; } = string.Empty;
    public string PeriodType { get; set; } = "month";
    public string FiscalYear { get; set; } = string.Empty;

    // ✅ Financial Health
    public decimal NetIncome { get; set; }
    public decimal ProfitMargin { get; set; }
    public decimal CashBalance { get; set; }
    public decimal NetCashFlow { get; set; }
    public decimal CashInflow { get; set; }
    public decimal CashOutflow { get; set; }

    // ✅ Accounts
    public decimal ?AccountsReceivable { get; set; }
    public decimal ?AccountsPayable { get; set; }
    public decimal ?TotalAssets { get; set; }

    // ✅ Sales Breakdown
    public decimal PaidSalesAmount { get; set; }
    public decimal UnpaidSalesAmount { get; set; }
   public List<StatusSummaryDto> SalesByStatus { get; set; } = new();
    public List<ExpenseCategoryDto> ExpensesByCategory { get; set; } = new();

    // ✅ Budget vs Actual
    public BudgetVsActualDto? BudgetVsActual { get; set; }

    // ✅ Trends
    public List<RevenueTrendDto> RevenueTrend { get; set; } = new();
    public AgingReportDto? AgingReport { get; set; }


      public decimal BudgetUtilization { get; set; }
        public decimal BudgetRemaining { get; set; }
        public decimal BudgetVariance { get; set; }
        public decimal BudgetVariancePercentage { get; set; }
        public bool IsOverBudget { get; set; }
        public bool IsNearBudget { get; set; }

        // ✅ Cost Metrics (add these)
        public decimal PurchaseCost { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal ProfitPerUnit { get; set; }
        public decimal CostToRevenueRatio { get; set; }

        // ✅ General Ledger Metrics (add these)
        public int TotalJournalEntries { get; set; }
        public int PostedJournalCount { get; set; }
        public int UnpostedJournalCount { get; set; }
        public decimal TotalJournalDebit { get; set; }
        public decimal TotalJournalCredit { get; set; }
        public bool IsJournalBalanced { get; set; }
        public Dictionary<string, int> JournalEntriesByType { get; set; } = new();
        public List<JournalEntryDto> RecentJournalEntries { get; set; } = new();
        public Dictionary<string, int> AccountTypes { get; set; } = new();

        // ✅ Voucher Metrics (add these)
        public int PaymentVoucherCount { get; set; }
        public int ReceiptVoucherCount { get; set; }
        public int JournalVoucherCount { get; set; }
        public int TotalVouchers { get; set; }
        public int PendingPaymentVouchers { get; set; }
        public int PendingReceiptVouchers { get; set; }
        public int PendingJournalVouchers { get; set; }
        public int ProcessedVoucherTypes { get; set; }
        public decimal VoucherProcessedPercentage { get; set; }

        // ✅ Asset Metrics (add these)
        public decimal ?TotalAssetValue { get; set; }
        public decimal ?NetBookValue { get; set; }
        public decimal ?TotalDepreciation { get; set; }
        public int ActiveAssetCount { get; set; }
        public int MaintenanceAssetCount { get; set; }
        public int TotalAssetCount { get; set; }
        public Dictionary<string, int> AssetsByType { get; set; } = new();
        public List<AssetSummaryDto> TopAssets { get; set; } = new();

        // ✅ Cash & Bank Metrics (add these)
        public decimal CashAmount { get; set; }
        public decimal BankAmount { get; set; }
        public decimal AvgDailyCashFlow { get; set; }
        public List<decimal> WeeklyCashFlow { get; set; } = new();
        public List<TransactionDto> RecentTransactions { get; set; } = new();
        public int TotalBankAccounts { get; set; }
        public int TotalBankTransactions { get; set; }

        // ✅ Other Metrics (add these)
        public int OverdueInvoices { get; set; }
        public int PendingInvoices { get; set; }
        public decimal OverduePercentage { get; set; }
        public decimal CurrentPercentage { get; set; }
        public int SalesPaymentCount { get; set; }
        public int PurchasePaymentCount { get; set; }
        public int VendorCount { get; set; }
        public int CustomerCount { get; set; }
        public List<ExpenseCategoryDto> TopExpenseCategories { get; set; } = new();
        public List<MonthlyVarianceDataDto> MonthlyVarianceData { get; set; } = new();
        public Dictionary<string, decimal> RevenueByMonth { get; set; } = new();
        public Dictionary<string, decimal> ExpensesByMonth { get; set; } = new();
}
public class JournalEntryDto
{
    public string Reference { get; set; } = string.Empty;
    public bool IsPosted { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DateAdd { get; set; }
}

public class TransactionDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
}

public class AssetSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal AcquisitionCost { get; set; }
    public decimal CurrentValue { get; set; }
}

public class MonthlyVarianceDataDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal Budget { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercent { get; set; }
}
public class TrendDataDto
{
    public string Period { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Count { get; set; }
    public decimal Expense { get; set; }
    public decimal AverageInvoice { get; set; }
}
public class PurchaseStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Amount { get; set; }
}
// ✅ New DTOs for the full dashboard

public class BudgetVsActualDto
{
    public decimal TotalBudget { get; set; }
    public decimal ActualSpent { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercentage { get; set; }
}

public class ExpenseCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Count { get; set; }
}

public class RevenueTrendDto
{
    public string Month { get; set; } = string.Empty;
    public DateTime MonthStart { get; set; }
    public DateTime MonthEnd { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal Profit { get; set; }
}

// ✅ Aging Report DTO - Complete version
public class AgingReportDto
{
    // Current (not overdue)
    public decimal Current { get; set; }

    // Overdue buckets
    public decimal Days30_60 { get; set; }      // 1-30 days overdue
    public decimal Days60_90 { get; set; }      // 31-60 days overdue
    public decimal Days90Plus { get; set; }     // 90+ days overdue

    // Alternative naming (if you prefer)
    public decimal Overdue30 { get; set; }      // 1-30 days overdue
    public decimal Overdue60 { get; set; }      // 31-60 days overdue
    public decimal Overdue90 { get; set; }      // 61-90 days overdue
    public decimal Overdue90Plus { get; set; }  // 90+ days overdue
   public decimal UnknownDueDate { get; set; }
    // Totals
    public decimal Total { get; set; }
    public decimal TotalOutstanding { get; set; }

    // Metadata
    public DateTime AsOfDate { get; set; }
    public string Period { get; set; } = string.Empty;
    public string PeriodType { get; set; } = string.Empty;

    // Details
     public List<AgingDetailDto>? Details { get; set; }
    public List<AgingDetailDto>? AgingDetails { get; set; }
}
public class AgingDetailDto
{
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string InvoiceType { get; set; } = string.Empty;  // "Sales" or "Purchase"
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int DaysOverdue { get; set; }
    public string AgingBucket { get; set; } = string.Empty;  // "Current", "1-30", "31-60", "61-90", "90+", "Unknown"
    public string Status { get; set; } = string.Empty;       // ✅ Added Status property

    // Customer/Vendor info
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
}
// ✅ Aging Detail DTO - Complete version

// ✅ Asset DTO

public class StatusSummaryDto
{
    public string Status { get; set; } = "";
    public int Count { get; set; }
    public decimal Amount { get; set; }
}
// ✅ Full Dashboard Response
public class FullDashboardResponse
{
    public AnalyticsDashboardDto? Analytics { get; set; }
    public List<TrendDataDto> RevenueTrend { get; set; } = new();
    public AgingReportDto? AgingReport { get; set; }
    public PaymentAnalyticsDto? PaymentAnalytics { get; set; }
    public ExpenseAnalyticsDto? ExpenseAnalytics { get; set; }
    public BudgetAnalyticsDto? BudgetAnalytics { get; set; }
    public List<AssetDto> Assets { get; set; } = new();
    public DateTime DateGenerated { get; set; }



        // ✅ NEW: Reference Data (Static)
        public List<BankAccountAnDto> BankAccounts { get; set; } = new();
        public List<ChartOfAccountAnDto> ChartOfAccounts { get; set; } = new();
        public List<BudgetAnDto> Budgets { get; set; } = new();


}

// ✅ Existing DTOs remain unchanged

public class AnalyticsDto
{
    // For Customers (Warehouse - int key)
    public int CustomerKey { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    // For Vendors (Warehouse - int key)
    public int VendorKey { get; set; }
    public string VendorName { get; set; } = string.Empty;

    // For Main Context - Customer (Guid)
    public string  CustomerId { get; set; }

    // For Main Context - Vendor (Guid)
    public string  VendorId { get; set; }

    // Common properties
    public decimal TotalAmount { get; set; }
    public int Count { get; set; }
    public decimal AverageInvoice { get; set; }
}
public class RevenueAndCountDto
{
    public decimal Revenue { get; set; }
    public int Count { get; set; }
}
public class PaymentAnalyticsDto
{
    public int TotalPayments { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AveragePayment { get; set; }
    public decimal VendorPayments { get; set; }
    public decimal CustomerPayments { get; set; }
    public List<PaymentMethodSummaryDto> PaymentsByMethod { get; set; } = new();

    public string PeriodStart { get; set; } = string.Empty;
    public string PeriodEnd { get; set; } = string.Empty;
}

public class PaymentMethodSummaryDto
{
    public string Method { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

public class ExpenseAnalyticsDto
{
    public int TotalExpenses { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageExpense { get; set; }
    public int SupplierBillCount { get; set; }
    public decimal SupplierBillAmount { get; set; }

    public int OperatingExpenseCount { get; set; }
    public decimal OperatingExpenseAmount { get; set; }
    public List<ExpenseCategorySummaryDto> ExpensesByCategory { get; set; } = new();

    public string PeriodStart { get; set; } = string.Empty;
    public string PeriodEnd { get; set; } = string.Empty;
}

public class ExpenseCategorySummaryDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

public class BudgetAnalyticsDto
{
    public int TotalBudgets { get; set; }
    public decimal TotalBudgetAmount { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal OverallUtilization { get; set; }
    public DateTime DateGenerated { get; set; }

    public string PeriodStart { get; set; } = string.Empty;
    public string PeriodEnd { get; set; } = string.Empty;
}

public class InvoiceStatusSummaryDto
{
    public string? InvoiceType { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
    public double Percentage { get; set; }

    public string PeriodStart { get; set; } = string.Empty;
    public string PeriodEnd { get; set; } = string.Empty;
}

public class TopCustomersDto
{
    public List<CustomerSummaryDto> Customers { get; set; } = new();
    public int TotalCustomers { get; set; }
    public DateTime DateGenerated { get; set; }

    public string PeriodStart { get; set; } = string.Empty;
    public string PeriodEnd { get; set; } = string.Empty;
}

public class TopVendorsDto
{
    public List<VendorSummaryDto> Vendors { get; set; } = new();
    public int TotalVendors { get; set; }
    public DateTime DateGenerated { get; set; }

    public string PeriodStart { get; set; } = string.Empty;
    public string PeriodEnd { get; set; } = string.Empty;
}

public class SalesStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Amount { get; set; }
}

public class InvoiceSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalPurchases { get; set; }
    public int TotalCount { get; set; }
    public int PaidCount { get; set; }
    public int UnpaidCount { get; set; }
    public int OverdueCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageInvoice { get; set; }
}

public class MonthlyTrendDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Purchases { get; set; }
    public int Count { get; set; }
    public decimal AverageInvoice { get; set; }
}

public class RecentInvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string InvoiceType { get; set; } = string.Empty;
}

public class TopCustomerDto
{
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int Count { get; set; }
    public decimal AverageInvoice { get; set; }
}

public class BankAccountAnDto
{
    public Guid Id { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
}

public class ChartOfAccountAnDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string NormalBalance { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public decimal? OpeningBalance { get; set; }
    public bool IsActive { get; set; }
}

public class BudgetAnDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; } = string.Empty;
}
// Controllers/AnalyticsController.cs
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Helpers;
using Microsoft.Extensions.Caching.Memory;
using Cor.Finance.Services;
using Cor.Finance.Models.Analytics;
using Cor.Finance.Persistence;
using Shared.Helpers.Services;
using Microsoft.EntityFrameworkCore;
using InvoiceSummaryDto=Cor.Finance.Models.Analytics.InvoiceSummaryDto;
namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class AnalyticsController : BaseApiController
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;
 private readonly FinanceDbContext _context;
 private readonly RedisCacheService _cacheService;

    public AnalyticsController(IAnalyticsService analyticsService,   RedisCacheService cacheService,ILogger<AnalyticsController> logger,FinanceDbContext context)
        : base(null!, logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
         _context = context;
          _cacheService = cacheService;
    }

    /// <summary>
    /// Get the main analytics dashboard summary with period filtering
    /// </summary>
    [HttpGet("Dashboard")]
    [ProducesResponseType(typeof(AnalyticsDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] DateTime? periodStart = null,
        [FromQuery] DateTime? periodEnd = null,
        [FromQuery] string periodType = "month",
        [FromQuery] string? fiscalYear = null)
    {
        try
        {
            _logger.LogInformation($"📊 Fetching analytics dashboard for {periodType} starting {periodStart?.ToString("yyyy-MM-dd") ?? "default"}");
            var result = await _analyticsService.GetAnalyticsDashboardAsync(
                periodStart,
                periodEnd,
                periodType,
                fiscalYear);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error fetching analytics dashboard");
            return HandleException(ex, "GetDashboard");
        }
    }

    /// <summary>
    /// Get revenue trend data for the last N months
    /// </summary>
    [HttpGet("RevenueTrend")]
    [ProducesResponseType(typeof(List<TrendDataDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRevenueTrend(
        [FromQuery] int months = 12,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            _logger.LogInformation($"📈 Fetching revenue trend for {months} months");
            var result = await _analyticsService.GetRevenueTrendAsync(months, endDate);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error fetching revenue trend for {months} months");
            return HandleException(ex, "GetRevenueTrend");
        }
    }

    /// <summary>
    /// Get purchase trend data for the last N months
    /// </summary>
    [HttpGet("PurchaseTrend")]
    [ProducesResponseType(typeof(List<TrendDataDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPurchaseTrend(
        [FromQuery] int months = 12,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            _logger.LogInformation($"📈 Fetching purchase trend for {months} months");
            var result = await _analyticsService.GetPurchaseTrendAsync(months, endDate);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error fetching purchase trend for {months} months");
            return HandleException(ex, "GetPurchaseTrend");
        }
    }

    /// <summary>
    /// Get accounts receivable aging report
    /// </summary>
    [HttpGet("Aging")]
    [ProducesResponseType(typeof(Cor.Finance.Models.Analytics.AgingReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAgingReport(
        [FromQuery] DateTime? asOfDate = null)
    {
        try
        {
            _logger.LogInformation($"📋 Fetching aging report as of {asOfDate?.ToString("yyyy-MM-dd") ?? "today"}");
            var result = await _analyticsService.GetAgingReportAsync(asOfDate);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error fetching aging report");
            return HandleException(ex, "GetAgingReport");
        }
    }

    /// <summary>
    /// Get payment analytics summary
    /// </summary>
    [HttpGet("PaymentAnalytics")]
    [ProducesResponseType(typeof(PaymentAnalyticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaymentAnalytics(
        [FromQuery] DateTime? periodStart = null,
        [FromQuery] DateTime? periodEnd = null)
    {
        try
        {
            _logger.LogInformation($"💰 Fetching payment analytics from {periodStart?.ToString("yyyy-MM-dd") ?? "default"} to {periodEnd?.ToString("yyyy-MM-dd") ?? "default"}");
            var result = await _analyticsService.GetPaymentAnalyticsAsync(periodStart, periodEnd);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error fetching payment analytics");
            return HandleException(ex, "GetPaymentAnalytics");
        }
    }

    /// <summary>
    /// Get expense analytics summary
    /// </summary>
    [HttpGet("ExpenseAnalytics")]
    [ProducesResponseType(typeof(ExpenseAnalyticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExpenseAnalytics(
        [FromQuery] DateTime? periodStart = null,
        [FromQuery] DateTime? periodEnd = null)
    {
        try
        {
            _logger.LogInformation($"📊 Fetching expense analytics from {periodStart?.ToString("yyyy-MM-dd") ?? "default"} to {periodEnd?.ToString("yyyy-MM-dd") ?? "default"}");
            var result = await _analyticsService.GetExpenseAnalyticsAsync(periodStart, periodEnd);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error fetching expense analytics");
            return HandleException(ex, "GetExpenseAnalytics");
        }
    }

    /// <summary>
    /// Get budget analytics summary
    /// </summary>
    [HttpGet("BudgetAnalytics")]
    [ProducesResponseType(typeof(BudgetAnalyticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBudgetAnalytics(
        [FromQuery] DateTime? periodStart = null,
        [FromQuery] DateTime? periodEnd = null)
    {
        try
        {
            _logger.LogInformation($"📋 Fetching budget analytics from {periodStart?.ToString("yyyy-MM-dd") ?? "default"} to {periodEnd?.ToString("yyyy-MM-dd") ?? "default"}");
            var result = await _analyticsService.GetBudgetAnalyticsAsync(periodStart, periodEnd);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error fetching budget analytics");
            return HandleException(ex, "GetBudgetAnalytics");
        }
    }

    /// <summary>
    /// Get invoice status summary
    /// </summary>
    [HttpGet("InvoiceStatusSummary")]
    [ProducesResponseType(typeof(List<InvoiceStatusSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetInvoiceStatusSummary(
        [FromQuery] DateTime? periodStart = null,
        [FromQuery] DateTime? periodEnd = null)
    {
        try
        {
            _logger.LogInformation($"📄 Fetching invoice status summary from {periodStart?.ToString("yyyy-MM-dd") ?? "default"} to {periodEnd?.ToString("yyyy-MM-dd") ?? "default"}");
            var result = await _analyticsService.GetInvoiceStatusSummaryAsync(periodStart, periodEnd);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error fetching invoice status summary");
            return HandleException(ex, "GetInvoiceStatusSummary");
        }
    }

    /// <summary>
    /// Get sales invoice status summary
    /// </summary>
    [HttpGet("SalesInvoiceStatus")]
    [ProducesResponseType(typeof(List<InvoiceStatusSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSalesInvoiceStatus(
        [FromQuery] DateTime? periodStart = null,
        [FromQuery] DateTime? periodEnd = null)
    {
        try
        {
            _logger.LogInformation($"📄 Fetching sales invoice status summary");
            var result = await _analyticsService.GetSalesInvoiceStatusSummaryAsync(periodStart, periodEnd);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error fetching sales invoice status");
            return HandleException(ex, "GetSalesInvoiceStatus");
        }
    }

    /// <summary>
    /// Get purchase invoice status summary
    /// </summary>
    [HttpGet("PurchaseInvoiceStatus")]
    [ProducesResponseType(typeof(List<InvoiceStatusSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPurchaseInvoiceStatus(
        [FromQuery] DateTime? periodStart = null,
        [FromQuery] DateTime? periodEnd = null)
    {
        try
        {
            _logger.LogInformation($"📄 Fetching purchase invoice status summary");
            var result = await _analyticsService.GetPurchaseInvoiceStatusSummaryAsync(periodStart, periodEnd);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error fetching purchase invoice status");
            return HandleException(ex, "GetPurchaseInvoiceStatus");
        }
    }

    /// <summary>
    /// Get monthly revenue breakdown
    /// </summary>
    [HttpGet("MonthlyRevenue")]
    [ProducesResponseType(typeof(Dictionary<string, decimal>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMonthlyRevenue(
        [FromQuery] int months = 12,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            _logger.LogInformation($"📈 Fetching monthly revenue for {months} months");
            var result = await _analyticsService.GetMonthlyRevenueAsync(months, endDate);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error fetching monthly revenue for {months} months");
            return HandleException(ex, "GetMonthlyRevenue");
        }
    }

    /// <summary>
    /// Get monthly purchase expense breakdown
    /// </summary>
    [HttpGet("MonthlyPurchaseExpense")]
    [ProducesResponseType(typeof(Dictionary<string, decimal>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMonthlyPurchaseExpense(
        [FromQuery] int months = 12,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            _logger.LogInformation($"📈 Fetching monthly purchase expense for {months} months");
            var result = await _analyticsService.GetMonthlyPurchaseExpenseAsync(months, endDate);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error fetching monthly purchase expense for {months} months");
            return HandleException(ex, "GetMonthlyPurchaseExpense");
        }
    }

    /// <summary>
    /// Get top customers
    /// </summary>
    [HttpGet("TopCustomers")]
    [ProducesResponseType(typeof(TopCustomersDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTopCustomers(
        [FromQuery] int count = 10,
        [FromQuery] DateTime? periodStart = null,
        [FromQuery] DateTime? periodEnd = null)
    {
        try
        {
            _logger.LogInformation($"🏆 Fetching top {count} customers");
            var result = await _analyticsService.GetTopCustomersAsync(count, periodStart, periodEnd);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error fetching top {count} customers");
            return HandleException(ex, "GetTopCustomers");
        }
    }

    /// <summary>
    /// Get top vendors
    /// </summary>
    [HttpGet("TopVendors")]
    [ProducesResponseType(typeof(TopVendorsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTopVendors(
        [FromQuery] int count = 10,
        [FromQuery] DateTime? periodStart = null,
        [FromQuery] DateTime? periodEnd = null)
    {
        try
        {
            _logger.LogInformation($"🏆 Fetching top {count} vendors");
            var result = await _analyticsService.GetTopVendorsAsync(count, periodStart, periodEnd);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error fetching top {count} vendors");
            return HandleException(ex, "GetTopVendors");
        }
    }



    // Controllers/AnalyticsController.cs - Add new endpoint



   // Controllers/AnalyticsController.cs - Add aggregated endpoints

   [HttpGet("InvoiceSummary")]
   [ProducesResponseType(typeof(InvoiceSummaryDto), StatusCodes.Status200OK)]
   public async Task<IActionResult> GetInvoiceSummary(
       [FromQuery] DateTime? periodStart = null,
       [FromQuery] DateTime? periodEnd = null,
       CancellationToken ct = default)
   {
       var endDate = periodEnd ?? DateTime.UtcNow.Date;
       var startDate = periodStart ?? new DateTime(endDate.Year, endDate.Month, 1);

       var summary = await _context.Invoices
           .Where(i => !i.IsDeleted && i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
          .OrderBy(i => i.InvoiceDate) // Add OrderBy
              .GroupBy(i => 1)
              .Select(g => new InvoiceSummaryDto
           {
               TotalRevenue = g.Where(i => i.InvoiceType == "Sales").Sum(i => i.TotalAmount),
               TotalPurchases = g.Where(i => i.InvoiceType == "Purchase").Sum(i => i.TotalAmount),
               TotalCount = g.Count(),
               PaidCount = g.Count(i => i.Status == "Paid"),
               UnpaidCount = g.Count(i => i.Status != "Paid"),
               OverdueCount = g.Count(i => i.DueDate < DateTime.UtcNow && i.Status != "Paid"),
               TotalAmount = g.Sum(i => i.TotalAmount),
               AverageInvoice = g.Average(i => i.TotalAmount)
           })
           .FirstOrDefaultAsync(ct) ?? new InvoiceSummaryDto();

       return Ok(summary);
   }

   [HttpGet("InvoiceMonthlyTrend")]
   public async Task<IActionResult> GetInvoiceMonthlyTrend(
       [FromQuery] int months = 12,
       [FromQuery] DateTime? endDate = null,
       CancellationToken ct = default)
   {
       var end = endDate ?? DateTime.UtcNow.Date;
       var start = end.AddMonths(-months);

       var trend = await _context.Invoices
           .Where(i => !i.IsDeleted && i.InvoiceDate >= start && i.InvoiceDate <= end)
           .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
           .Select(g => new MonthlyTrendDto
           {
               Month = $"{g.Key.Year}-{g.Key.Month:D2}",
               Revenue = g.Where(i => i.InvoiceType == "Sales").Sum(i => i.TotalAmount),
               Purchases = g.Where(i => i.InvoiceType == "Purchase").Sum(i => i.TotalAmount),
               Count = g.Count(),
               AverageInvoice = g.Average(i => i.TotalAmount)
           })
           .OrderBy(x => x.Month)
           .ToListAsync(ct);

       return Ok(trend);
   }

   [HttpGet("RecentInvoices")]
   public async Task<IActionResult> GetRecentInvoices(
       [FromQuery] int count = 10,
       CancellationToken ct = default)
   {
       var recent = await _context.Invoices
           .Where(i => !i.IsDeleted)
           .OrderByDescending(i => i.InvoiceDate)
           .Take(count)
           .Select(i => new RecentInvoiceDto
           {
               Id = i.Id,
               InvoiceNumber = i.InvoiceNumber,
               InvoiceDate = i.InvoiceDate,
               TotalAmount = i.TotalAmount,
               Status = i.Status,
               InvoiceType = i.InvoiceType
           })
           .ToListAsync(ct);

       return Ok(recent);
   }

   [HttpGet("TopCustomers")]
   public async Task<IActionResult> GetTopCustomers(
       [FromQuery] int count = 10,
       [FromQuery] DateTime? periodStart = null,
       [FromQuery] DateTime? periodEnd = null,
       CancellationToken ct = default)
   {
       var endDate = periodEnd ?? DateTime.UtcNow.Date;
       var startDate = periodStart ?? new DateTime(endDate.Year, endDate.Month, 1);

       var topCustomers = await _context.Invoices
           .Where(i => !i.IsDeleted && i.InvoiceType == "Sales" && i.CustomerId != null)
           .GroupBy(i => i.CustomerId)
           .Select(g => new TopCustomerDto
           {
               CustomerId = g.Key,
               CustomerName = g.FirstOrDefault().Customer.Name,
               TotalAmount = g.Sum(i => i.TotalAmount),
               Count = g.Count(),
               AverageInvoice = g.Average(i => i.TotalAmount)
           })
           .OrderByDescending(x => x.TotalAmount)
           .Take(count)
           .ToListAsync(ct);

       return Ok(topCustomers);
   }
// Controllers/AnalyticsController.cs - Updated FullDashboard

// Controllers/AnalyticsController.cs - FULL COMPLETE VERSION

[HttpGet("FullDashboard")]
[ProducesResponseType(typeof(FullDashboardResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetFullDashboard(
    [FromQuery] DateTime? periodStart = null,
    [FromQuery] DateTime? periodEnd = null,
    [FromQuery] string periodType = "month",
    [FromQuery] string? fiscalYear = null,
    CancellationToken ct = default)
{
    try
    {
        var cacheKey = $"full_dashboard:{periodStart?.ToString("yyyy-MM-dd")}:{periodEnd?.ToString("yyyy-MM-dd")}:{periodType}:{fiscalYear ?? "none"}";

        // ✅ Check cache
        var cached = await _cacheService.GetAsync<FullDashboardResponse>(cacheKey, ct);
        if (cached is not null)
        {
            _logger.LogDebug("📦 Cache HIT: Full Dashboard");
            return Ok(cached);
        }

        _logger.LogDebug("📦 Cache MISS: Full Dashboard");

        // ✅ ============================================================
        // ✅ RUN SEQUENTIALLY - No concurrency issues
        // ✅ ============================================================

        // 1. Analytics
        var analytics = await _analyticsService.GetAnalyticsDashboardAsync(periodStart, periodEnd, periodType, fiscalYear, ct);
        var revenueTrend = await _analyticsService.GetRevenueTrendAsync(12, periodEnd, ct);
        var agingReport = await _analyticsService.GetAgingReportAsync(null, ct);
        var paymentAnalytics = await _analyticsService.GetPaymentAnalyticsAsync(periodStart, periodEnd, ct);
        var expenseAnalytics = await _analyticsService.GetExpenseAnalyticsAsync(periodStart, periodEnd, ct);
        var budgetAnalytics = await _analyticsService.GetBudgetAnalyticsAsync(periodStart, periodEnd, ct);

        // 2. Bank Accounts
        var bankAccounts = await _context.BankAccounts
            .Where(b => !b.IsDeleted && b.IsActive)
            .OrderBy(b => b.AccountName)
            .Select(b => new BankAccountAnDto
            {
                Id = b.Id,
                AccountName = b.AccountName,
                AccountNumber = b.AccountNumber,
                BankName = b.BankName,
                CurrentBalance = b.CurrentBalance,
                AvailableBalance = b.AvailableBalance,
                Currency = b.Currency,
                IsActive = b.IsActive,
                DateAdd = b.DateAdd
            })
            .ToListAsync(ct);

        // 3. Chart of Accounts
        var chartOfAccounts = await _context.ChartOfAccounts
            .Where(c => !c.IsDeleted && c.IsActive)
            .OrderBy(c => c.Code)
            .Select(c => new ChartOfAccountAnDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                AccountType = c.AccountType,
                NormalBalance = c.NormalBalance,
                CurrentBalance = c.CurrentBalance,
                OpeningBalance = c.OpeningBalance,
                IsActive = c.IsActive
            })
            .ToListAsync(ct);

        // 4. Budgets
        var budgets = await _context.Budgets
            .Where(b => !b.IsDeleted && b.Status == "Active")
            .OrderBy(b => b.Name)
            .Select(b => new BudgetAnDto
            {
                Id = b.Id,
                Name = b.Name,

                TotalAmount = b.TotalAmount,
                SpentAmount = b.SpentAmount,
                RemainingAmount = b.TotalAmount - b.SpentAmount,
                Status = b.Status,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Description = b.Description
            })
            .ToListAsync(ct);

        // 5. Assets
        var assets = await _context.Assets
            .Where(a => !a.IsDeleted && a.IsActive)
            .OrderBy(a => a.Name)
            .Select(a => new AssetDto
            {
                Id = a.Id,
                Name = a.Name,
                Code = a.Code,
                AssetType = a.AssetType,
                CurrentValue = a.CurrentValue,
                AcquisitionCost = a.AcquisitionCost,
                Status = a.Status,
                IsActive = a.IsActive,
                DateAdd = a.DateAdd
            })
            .ToListAsync(ct);

        // ✅ Build response
        var response = new FullDashboardResponse
        {
            Analytics = analytics,
            RevenueTrend = revenueTrend,
            AgingReport = agingReport,
            PaymentAnalytics = paymentAnalytics,
            ExpenseAnalytics = expenseAnalytics,
            BudgetAnalytics = budgetAnalytics,
            BankAccounts = bankAccounts,
            ChartOfAccounts = chartOfAccounts,
            Budgets = budgets,
            Assets = assets,
            DateGenerated = DateTime.UtcNow
        };

        // ✅ Cache for 30 seconds
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromSeconds(30), ct);

        _logger.LogInformation($"✅ Full Dashboard generated successfully");

        return Ok(response);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "❌ Error fetching full dashboard: {Message}", ex.Message);
        return HandleException(ex, "GetFullDashboard");
    }
}

    // Models/Analytics/FullDashboardResponse.cs

}
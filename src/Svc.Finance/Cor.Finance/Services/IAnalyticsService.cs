// Services/AnalyticsService.cs
using Cor.Finance.Models.Analytics;
using Cor.Finance.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using Cor.Finance.Models.Entities.Aggregates;

namespace Cor.Finance.Services;

public interface IAnalyticsService
{
    // Dashboard with period filtering
    Task<AnalyticsDashboardDto> GetAnalyticsDashboardAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        string periodType = "month",
        string? fiscalYear = null,
        CancellationToken ct = default);

    // Trends with date filtering
    Task<List<TrendDataDto>> GetRevenueTrendAsync(
        int months = 12,
        DateTime? endDate = null,
        CancellationToken ct = default);

    Task<List<TrendDataDto>> GetPurchaseTrendAsync(
        int months = 12,
        DateTime? endDate = null,
        CancellationToken ct = default);

    // Monthly Data with date filtering
    Task<Dictionary<string, decimal>> GetMonthlyRevenueAsync(
        int months = 12,
        DateTime? endDate = null,
        CancellationToken ct = default);

    Task<Dictionary<string, decimal>> GetMonthlyPurchaseExpenseAsync(
        int months = 12,
        DateTime? endDate = null,
        CancellationToken ct = default);

    // Invoice Status with optional date filtering
    Task<List<InvoiceStatusSummaryDto>> GetSalesInvoiceStatusSummaryAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default);

    Task<List<InvoiceStatusSummaryDto>> GetPurchaseInvoiceStatusSummaryAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default);

    Task<List<InvoiceStatusSummaryDto>> GetInvoiceStatusSummaryAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default);

    // Top Customers & Vendors with date filtering
    Task<TopCustomersDto> GetTopCustomersAsync(
        int count = 10,
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default);

    Task<TopVendorsDto> GetTopVendorsAsync(
        int count = 10,
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default);

    // Reports with date filtering
    Task<Cor.Finance.Models.Analytics.AgingReportDto> GetAgingReportAsync(
        DateTime? asOfDate = null,
        CancellationToken ct = default);

    Task<PaymentAnalyticsDto> GetPaymentAnalyticsAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default);

    Task<ExpenseAnalyticsDto> GetExpenseAnalyticsAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default);

    Task<BudgetAnalyticsDto> GetBudgetAnalyticsAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default);
}
using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.Procurement.Queries;

public class GetReportsDashboardQueryHandler
    : IRequestHandler<GetReportsDashboardQuery, ReportsDashboardDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetReportsDashboardQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetReportsDashboardQueryHandler(
        ProcurementDbContext context,
        ILogger<GetReportsDashboardQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ReportsDashboardDto> Handle(GetReportsDashboardQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"reports_dashboard_{request.Period}";
            var cached = await _cache.GetAsync<ReportsDashboardDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Reports dashboard");
                return cached;
            }

            _logger.LogInformation("📦 Cache MISS: Generating reports dashboard");

            var dashboard = new ReportsDashboardDto();
            var now = DateTime.UtcNow;
            var sixMonthsAgo = now.AddMonths(-6);

            // ============================================================
            // 1. STATS
            // ============================================================

            // For demo purposes, generate sample reports
            var reports = GenerateSampleReports();

            dashboard.Stats = new ReportsStats
            {
                TotalReports = reports.Count,
                ReadyReports = reports.Count(r => r.Status == "ready"),
                GeneratingReports = reports.Count(r => r.Status == "generating"),
                ScheduledReports = reports.Count(r => r.Status == "scheduled"),
                TotalDownloads = reports.Sum(r => r.Downloads),
                CategoriesCount = reports.Select(r => r.Category).Distinct().Count()
            };

            // ============================================================
            // 2. REPORTS LIST
            // ============================================================

            dashboard.Reports = reports
                .Take(request.RecentReportsCount)
                .ToList();

            // ============================================================
            // 3. SPEND ANALYSIS
            // ============================================================

            dashboard.SpendAnalysis = await GenerateSpendAnalysis(request, cancellationToken);

            // ============================================================
            // 4. VENDOR PERFORMANCE
            // ============================================================

            dashboard.VendorPerformance = await GenerateVendorPerformance(request, cancellationToken);

            // ============================================================
            // 5. LAST UPDATED
            // ============================================================

            dashboard.LastUpdated = DateTime.UtcNow;

            await _cache.SetAsync(cacheKey, dashboard, TimeSpan.FromMinutes(10), cancellationToken);

            return dashboard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating reports dashboard");
            throw;
        }
    }

    private List<ReportDto> GenerateSampleReports()
    {
        var reports = new List<ReportDto>
        {
            new ReportDto
            {
                Id = Guid.NewGuid(),
                Name = "Q3 Spend Analysis Report",
                Description = "Comprehensive spend analysis for Q3 2024 including category breakdown and trends",
                Category = "spend",
                Type = "detailed",
                GeneratedDate = DateTime.UtcNow.AddDays(-5),
                Period = "Q3 2024",
                Status = "ready",
                Format = "pdf",
                Size = "2.4 MB",
                Downloads = 45,
                LastViewed = DateTime.UtcNow.AddDays(-4),
                Tags = new List<string> { "spend", "analysis", "quarterly" }
            },
            new ReportDto
            {
                Id = Guid.NewGuid(),
                Name = "Vendor Performance Dashboard",
                Description = "Real-time vendor performance metrics and scoring",
                Category = "vendor",
                Type = "dashboard",
                GeneratedDate = DateTime.UtcNow.AddDays(-6),
                Period = "Q3 2024",
                Status = "ready",
                Format = "excel",
                Size = "1.8 MB",
                Downloads = 32,
                LastViewed = DateTime.UtcNow.AddDays(-5),
                Tags = new List<string> { "vendor", "performance", "dashboard" }
            },
            new ReportDto
            {
                Id = Guid.NewGuid(),
                Name = "Procurement Efficiency Report",
                Description = "Analysis of procurement cycle times and efficiency metrics",
                Category = "performance",
                Type = "summary",
                GeneratedDate = DateTime.UtcNow.AddDays(-7),
                Period = "Q3 2024",
                Status = "generating",
                Format = "pdf",
                Size = "--",
                Downloads = 0,
                LastViewed = null,
                Tags = new List<string> { "efficiency", "cycle time", "performance" }
            },
            new ReportDto
            {
                Id = Guid.NewGuid(),
                Name = "Inventory Status Report",
                Description = "Current inventory levels and reorder recommendations",
                Category = "inventory",
                Type = "detailed",
                GeneratedDate = DateTime.UtcNow.AddDays(-8),
                Period = "Q3 2024",
                Status = "scheduled",
                Format = "csv",
                Size = "--",
                Downloads = 0,
                LastViewed = null,
                Tags = new List<string> { "inventory", "stock", "reorder" }
            },
            new ReportDto
            {
                Id = Guid.NewGuid(),
                Name = "Compliance Audit Report",
                Description = "Vendor compliance and regulatory adherence report",
                Category = "compliance",
                Type = "detailed",
                GeneratedDate = DateTime.UtcNow.AddDays(-9),
                Period = "Q3 2024",
                Status = "ready",
                Format = "pdf",
                Size = "3.2 MB",
                Downloads = 18,
                LastViewed = DateTime.UtcNow.AddDays(-7),
                Tags = new List<string> { "compliance", "audit", "regulatory" }
            }
        };

        return reports;
    }

    private async Task<SpendAnalysisDto> GenerateSpendAnalysis(GetReportsDashboardQuery request, CancellationToken ct)
    {
        // Get invoices and vendors for spend analysis
        var invoices = await _context.Invoices
            .Where(i => !i.IsDeleted)
            .ToListAsync(ct);

        var vendors = await _context.Vendors
            .Where(v => !v.IsDeleted)
            .ToListAsync(ct);

        var totalSpend = invoices.Sum(i => i.TotalAmount);
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);

        // Monthly trend
        var monthlyTrend = Enumerable.Range(0, 6)
            .Select(i => new MonthlyTrendDto
            {
                Month = DateTime.UtcNow.AddMonths(-i).ToString("MMM"),
                Amount = invoices
                    .Where(inv => inv.InvoiceDate.Month == DateTime.UtcNow.AddMonths(-i).Month &&
                                  inv.InvoiceDate.Year == DateTime.UtcNow.AddMonths(-i).Year)
                    .Sum(inv => inv.TotalAmount)
            })
            .OrderBy(m => m.Month)
            .ToList();

        // Categories (using vendor types)
        var categoryData = invoices
            .Where(i => i.VendorId != Guid.Empty)
            .Join(vendors,
                i => i.VendorId,
                v => v.Id,
                (i, v) => new { Invoice = i, Vendor = v })
            .GroupBy(x => x.Vendor.VendorType ?? "Uncategorized")
            .Select(g => new SpendCategoryDto
            {
                Name = g.Key,
                Amount = g.Sum(x => x.Invoice.TotalAmount),
                Percentage = totalSpend > 0 ? (int)((g.Sum(x => x.Invoice.TotalAmount) / totalSpend) * 100) : 0,
                Trend = "stable"
            })
            .OrderByDescending(c => c.Amount)
            .Take(5)
            .ToList();

        // Top vendors
        var topVendors = invoices
            .Where(i => i.VendorId != Guid.Empty)
            .GroupBy(i => i.VendorId)
            .Select(g => new
            {
                VendorId = g.Key,
                Amount = g.Sum(i => i.TotalAmount)
            })
            .Join(vendors,
                g => g.VendorId,
                v => v.Id,
                (g, v) => new TopVendorDto
                {
                    Name = v.Name,
                    Amount = g.Amount,
                    Percentage = totalSpend > 0 ? (int)((g.Amount / totalSpend) * 100) : 0
                })
            .OrderByDescending(v => v.Amount)
            .Take(4)
            .ToList();

        // Budget utilization (sample)
        var budgetUtilization = new List<BudgetUtilizationDto>
        {
            new BudgetUtilizationDto
            {
                Category = "IT Equipment",
                Budgeted = 200000m,
                Actual = totalSpend * 0.33m,
                Variance = 200000m - (totalSpend * 0.33m),
                UtilizationPercentage = (totalSpend * 0.33m / 200000m) * 100
            },
            new BudgetUtilizationDto
            {
                Category = "Software Licenses",
                Budgeted = 150000m,
                Actual = totalSpend * 0.27m,
                Variance = 150000m - (totalSpend * 0.27m),
                UtilizationPercentage = (totalSpend * 0.27m / 150000m) * 100
            },
            new BudgetUtilizationDto
            {
                Category = "Logistics",
                Budgeted = 100000m,
                Actual = totalSpend * 0.20m,
                Variance = 100000m - (totalSpend * 0.20m),
                UtilizationPercentage = (totalSpend * 0.20m / 100000m) * 100
            }
        };

        return new SpendAnalysisDto
        {
            Period = "Q3 2024",
            TotalSpend = totalSpend,
            Categories = categoryData,
            TopVendors = topVendors,
            MonthlyTrend = monthlyTrend,
            BudgetUtilization = budgetUtilization,
            MonthlyChange = 12.5m,
            BudgetUtilizationPercentage = totalSpend > 0 ?
                (totalSpend / budgetUtilization.Sum(b => b.Budgeted)) * 100 : 0
        };
    }

    private async Task<List<VendorPerformanceDto>> GenerateVendorPerformance(GetReportsDashboardQuery request, CancellationToken ct)
    {
        var vendors = await _context.Vendors
            .Where(v => !v.IsDeleted && v.IsActive)
            .Take(request.TopVendorsCount)
            .ToListAsync(ct);

        var invoices = await _context.Invoices
            .Where(i => !i.IsDeleted)
            .ToListAsync(ct);

        var orders = await _context.PurchaseOrders
            .Where(p => !p.IsDeleted)
            .ToListAsync(ct);

        var result = new List<VendorPerformanceDto>();

        foreach (var vendor in vendors)
        {
            var vendorInvoices = invoices.Where(i => i.VendorId == vendor.Id).ToList();
            var vendorOrders = orders.Where(o => o.VendorId == vendor.Id).ToList();

            var totalOrders = vendorOrders.Count;
            var totalSpend = vendorInvoices.Sum(i => i.TotalAmount);
            var rating = vendor.Rating ?? 0;

            var score = (int)(rating * 20);
            var status = score >= 80 ? "Excellent" : score >= 60 ? "Good" : score >= 40 ? "Average" : "Poor";

            result.Add(new VendorPerformanceDto
            {
                Id = vendor.Id,
                VendorName = vendor.Name,
                VendorCode = vendor.Code,
                Category = vendor.VendorType,
                OverallScore = score,
                PerformanceMetrics = new VendorMetricsDto
                {
                    Delivery = 85 + (int)(rating * 5),
                    Quality = 80 + (int)(rating * 5),
                    Price = 75 + (int)(rating * 3),
                    Communication = 70 + (int)(rating * 4),
                    Compliance = 80 + (int)(rating * 5)
                },
                Trends = new VendorTrendsDto
                {
                    Delivery = "up",
                    Quality = "stable",
                    Price = "up"
                },
                TotalOrders = totalOrders,
                OnTimeDelivery = 80 + (int)(rating * 4),
                QualityRate = 75 + (int)(rating * 5),
                AverageResponseTime = 5.5m - (decimal)(rating / 10),
                Status = status,
                LastEvaluation = DateTime.UtcNow.AddDays(-30)
            });
        }

        return result.OrderByDescending(v => v.OverallScore).ToList();
    }
}
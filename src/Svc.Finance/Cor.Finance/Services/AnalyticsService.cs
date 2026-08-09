// Services/AnalyticsService.cs - Complete Fixed Version

using Cor.Finance.Models.Analytics;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Enums;
using Cor.Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Diagnostics;
using System.Collections.Concurrent;
using AgingReportDto = Cor.Finance.Models.Analytics.AgingReportDto;
using AgingDetailDto = Cor.Finance.Models.Analytics.AgingDetailDto;
using ExpenseCategoryDto = Cor.Finance.Models.Analytics.ExpenseCategoryDto;
using BudgetVsActualDto = Cor.Finance.Models.Analytics.BudgetVsActualDto;
using Npgsql;
namespace Cor.Finance.Services;

/// <summary>
/// Performance metrics for dashboard generation
/// </summary>
public class DashboardPerformanceMetrics
{
    public string CacheKey { get; set; }
    public string PeriodType { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime Timestamp { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public int QueryCount { get; set; }
    public int CacheHit { get; set; }
    public int DataSource { get; set; } // 0=Main, 1=Warehouse
    public int InvoiceCount { get; set; }
    public int ExpenseCount { get; set; }
    public long MemoryUsedBytes { get; set; }
    public double CpuUsagePercent { get; set; }
}

/// <summary>
/// Performance statistics aggregator
/// </summary>
public class PerformanceStatistics
{
    private readonly ConcurrentBag<DashboardPerformanceMetrics> _metrics = new();
    private readonly object _lock = new();

    public void AddMetric(DashboardPerformanceMetrics metric)
    {
        _metrics.Add(metric);

        // Keep only last 1000 metrics
        if (_metrics.Count > 1000)
        {
            lock (_lock)
            {
                while (_metrics.Count > 1000)
                {
                    _metrics.TryTake(out _);
                }
            }
        }
    }

    public PerformanceSummary GetSummary(TimeSpan? timeRange = null)
    {
        var cutoff = timeRange.HasValue ? DateTime.UtcNow - timeRange.Value : DateTime.MinValue;
        var relevant = _metrics.Where(m => m.Timestamp >= cutoff).ToList();

        if (!relevant.Any())
            return new PerformanceSummary();

        return new PerformanceSummary
        {
            TotalRequests = relevant.Count,
            AverageResponseTimeMs = relevant.Average(m => m.ElapsedMilliseconds),
            P95ResponseTimeMs = relevant.OrderBy(m => m.ElapsedMilliseconds)
                .Skip((int)(relevant.Count * 0.95))
                .FirstOrDefault()?.ElapsedMilliseconds ?? 0,
            P99ResponseTimeMs = relevant.OrderBy(m => m.ElapsedMilliseconds)
                .Skip((int)(relevant.Count * 0.99))
                .FirstOrDefault()?.ElapsedMilliseconds ?? 0,
            AverageQueryCount = relevant.Average(m => m.QueryCount),
            MaxQueryCount = relevant.Max(m => m.QueryCount),
            CacheHitRate = relevant.Count > 0 ? relevant.Count(m => m.CacheHit == 1) / (double)relevant.Count * 100 : 0,
            AverageMemoryUsedBytes = relevant.Average(m => m.MemoryUsedBytes),
            AverageCpuUsagePercent = relevant.Average(m => m.CpuUsagePercent),
            SlowestRequests = relevant.OrderByDescending(m => m.ElapsedMilliseconds).Take(10).ToList()
        };
    }
}

public class PerformanceSummary
{
    public int TotalRequests { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public double P95ResponseTimeMs { get; set; }
    public double P99ResponseTimeMs { get; set; }
    public double AverageQueryCount { get; set; }
    public int MaxQueryCount { get; set; }
    public double CacheHitRate { get; set; }
    public double AverageMemoryUsedBytes { get; set; }
    public double AverageCpuUsagePercent { get; set; }
    public List<DashboardPerformanceMetrics> SlowestRequests { get; set; } = new();
}

public class AnalyticsService : IAnalyticsService
{
    private readonly FinanceDbContext _context;
    private readonly FinanceWarehouseContext? _warehouseContext;
    private readonly ICacheService _cache;
    private readonly ILogger<AnalyticsService> _logger;
    private readonly bool _useWarehouse;
    private readonly PerformanceStatistics _performanceStats = new();

    // Performance thresholds
    private const long SLOW_THRESHOLD_MS = 5000; // 5 seconds
    private const long CRITICAL_THRESHOLD_MS = 10000; // 10 seconds
    private const int MAX_QUERY_COUNT = 50;
    private const int WARNING_QUERY_COUNT = 30;

    private static readonly TimeSpan CacheDurationShort = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CacheDurationMedium = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan CacheDurationLong = TimeSpan.FromMinutes(30);

    // ✅ ADD THIS - Cache prefix for pattern matching
    private const string CACHE_PREFIX = "analytics:dashboard:";

    // Query execution tracking
    private readonly ConcurrentDictionary<string, QueryExecutionStats> _queryStats = new();
    private static readonly ConcurrentDictionary<Guid, string> _categoryCache = new();
    private static DateTime _categoryCacheExpiry = DateTime.MinValue;
    private static readonly SemaphoreSlim _categoryCacheLock = new(1, 1);
  private async Task<Dictionary<Guid, string>> GetCachedExpenseCategoriesAsync(CancellationToken ct)
  {
      // ✅ Check if cache is valid
      if (_categoryCache.IsEmpty || DateTime.UtcNow > _categoryCacheExpiry)
      {
          await _categoryCacheLock.WaitAsync(ct);
          try
          {
              // Double-check after acquiring lock
              if (_categoryCache.IsEmpty || DateTime.UtcNow > _categoryCacheExpiry)
              {
                  var categories = await _context.ExpenseCategories
                      .Where(c => !c.IsDeleted)
                      .Select(c => new { c.Id, c.Name })
                      .ToDictionaryAsync(c => c.Id, c => c.Name, ct);

                  _categoryCache.Clear();
                  foreach (var cat in categories)
                  {
                      _categoryCache[cat.Key] = cat.Value;
                  }
                  _categoryCacheExpiry = DateTime.UtcNow.AddMinutes(30);

                  _logger.LogInformation("💾 Cached {Count} expense categories", _categoryCache.Count);
              }
          }
          finally
          {
              _categoryCacheLock.Release();
          }
      }

      return _categoryCache.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
  }



    public class QueryExecutionStats
    {
        public int ExecutionCount { get; set; }
        public long TotalMilliseconds { get; set; }
        public long MaxMilliseconds { get; set; }
        public DateTime LastExecuted { get; set; }
        public string QueryHash { get; set; }
    }

    public AnalyticsService(
        FinanceDbContext context,
        FinanceWarehouseContext? warehouseContext,
        ICacheService cache,
        ILogger<AnalyticsService> logger)
    {
        _context = context;
        _warehouseContext = warehouseContext;
        _cache = cache;
        _logger = logger;

        _useWarehouse = _warehouseContext != null && IsWarehousePopulated().GetAwaiter().GetResult();

        // Start background performance monitoring
        _ = StartPerformanceMonitoringAsync();
    }

    private async Task<bool> IsWarehousePopulated()
    {
        try
        {
            if (_warehouseContext == null) return false;
            return await _warehouseContext.FactInvoices.AnyAsync();
        }
        catch
        {
            return false;
        }
    }

  // In AnalyticsService.cs - StartPerformanceMonitoringAsync method

  private async Task StartPerformanceMonitoringAsync()
  {
      while (true)
      {
          try
          {
              await Task.Delay(TimeSpan.FromMinutes(10));

              var summary = _performanceStats.GetSummary(TimeSpan.FromHours(1));

              // ✅ ONLY LOG IF THERE WERE REQUESTS
              if (summary.TotalRequests > 0)
              {
                  _logger.LogInformation(
                      "📊 Performance Summary (last hour): " +
                      "Requests: {TotalRequests}, " +
                      "Avg Response: {AvgResponse:F0}ms, " +
                      "P95: {P95:F0}ms, " +
                      "P99: {P99:F0}ms, " +
                      "Cache Hit Rate: {CacheHitRate:F1}%, " +
                      "Avg Queries: {AvgQueries:F1}, " +
                      "Avg Memory: {AvgMemory:F0}KB",
                      summary.TotalRequests,
                      summary.AverageResponseTimeMs,
                      summary.P95ResponseTimeMs,
                      summary.P99ResponseTimeMs,
                      summary.CacheHitRate,
                      summary.AverageQueryCount,
                      summary.AverageMemoryUsedBytes / 1024
                  );

                  // Alert on performance degradation
                  if (summary.AverageResponseTimeMs > SLOW_THRESHOLD_MS)
                  {
                      _logger.LogWarning(
                          "⚠️ Performance degradation detected! " +
                          "Average response time: {AvgResponse:F0}ms exceeds threshold of {Threshold}ms",
                          summary.AverageResponseTimeMs,
                          SLOW_THRESHOLD_MS
                      );
                  }

                  if (summary.AverageQueryCount > WARNING_QUERY_COUNT)
                  {
                      _logger.LogWarning(
                          "⚠️ High query count detected! " +
                          "Average queries: {AvgQueries:F0} exceeds warning threshold of {Threshold}",
                          summary.AverageQueryCount,
                          WARNING_QUERY_COUNT
                      );
                  }

                  // Log slowest queries
                  if (summary.SlowestRequests.Any())
                  {
                      var slowest = summary.SlowestRequests.First();
                      _logger.LogWarning(
                          "🐌 Slowest request: {CacheKey} took {Elapsed}ms with {QueryCount} queries",
                          slowest.CacheKey,
                          slowest.ElapsedMilliseconds,
                          slowest.QueryCount
                      );
                  }
              }
              else
              {
                  // ✅ Log only once per hour if no requests
                  _logger.LogDebug("📊 No requests in the last hour");
              }
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "Error in performance monitoring background task");
          }
      }
  }

    // ============================================================
    // DASHBOARD WITH PERFORMANCE MONITORING
    // ============================================================

    public async Task<AnalyticsDashboardDto> GetAnalyticsDashboardAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        string periodType = "month",
        string? fiscalYear = null,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var memoryBefore = GC.GetTotalMemory(false);
        int queryCount = 0;

        try
        {
            // Build cache key with filter values
            var cacheKey = BuildAnalyticsCacheKey(periodStart, periodEnd, periodType, fiscalYear);

            // Check cache first
            var cached = await _cache.GetAsync<AnalyticsDashboardDto>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: Analytics Dashboard for {PeriodType} {FiscalYear}", periodType, fiscalYear ?? "current");

                // Record performance metrics
                RecordPerformanceMetrics(cacheKey, periodType, periodStart, periodEnd,
                    stopwatch.ElapsedMilliseconds, 0, true, 0, memoryBefore);

                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: Analytics Dashboard for {PeriodType} {FiscalYear}", periodType, fiscalYear ?? "current");

            // Generate dashboard data with query tracking
            var (dashboard, queries) = _useWarehouse
                ? await GetDashboardFromWarehouseWithMetricsAsync(periodStart, periodEnd, periodType, fiscalYear, ct)
                : await GetDashboardFromMainContextWithMetricsAsync(periodStart, periodEnd, periodType, fiscalYear, ct);

            queryCount = queries;

            // Add filter info to response
            dashboard.PeriodStart = periodStart?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd");
            dashboard.PeriodEnd = periodEnd?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd");
            dashboard.PeriodType = periodType;
            dashboard.FiscalYear = fiscalYear ?? DateTime.UtcNow.Year.ToString();
            dashboard.CacheDate = DateTime.UtcNow;
            dashboard.CacheKey = cacheKey;

            // Cache for configured duration
            var cacheDuration = GetCacheDuration(periodType);
            await _cache.SetAsync(cacheKey, dashboard, cacheDuration);
            _logger.LogDebug("💾 Cached analytics dashboard for {Duration} seconds", cacheDuration.TotalSeconds);

            // Record performance metrics
            RecordPerformanceMetrics(cacheKey, periodType, periodStart, periodEnd,
                stopwatch.ElapsedMilliseconds, queryCount, false, 0, memoryBefore);

            return dashboard;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "❌ Error generating analytics dashboard after {Elapsed}ms for {PeriodType} {FiscalYear}",
                stopwatch.ElapsedMilliseconds, periodType, fiscalYear ?? "current");
            throw;
        }
    }

    /// <summary>
    /// Records performance metrics for analysis
    /// </summary>
    private void RecordPerformanceMetrics(
        string cacheKey,
        string periodType,
        DateTime? periodStart,
        DateTime? periodEnd,
        long elapsedMs,
        int queryCount,
        bool cacheHit,
        int invoiceCount,
        long memoryBefore)
    {
        var memoryAfter = GC.GetTotalMemory(false);
        var memoryUsed = memoryAfter - memoryBefore;

        var metric = new DashboardPerformanceMetrics
        {
            CacheKey = cacheKey,
            PeriodType = periodType,
            PeriodStart = periodStart ?? DateTime.UtcNow,
            PeriodEnd = periodEnd ?? DateTime.UtcNow,
            Timestamp = DateTime.UtcNow,
            ElapsedMilliseconds = elapsedMs,
            QueryCount = queryCount,
            CacheHit = cacheHit ? 1 : 0,
            DataSource = _useWarehouse ? 1 : 0,
            InvoiceCount = invoiceCount,
            MemoryUsedBytes = memoryUsed,
            CpuUsagePercent = GetCurrentCpuUsage()
        };

        _performanceStats.AddMetric(metric);

        // Log if performance is slow
        if (elapsedMs > SLOW_THRESHOLD_MS)
        {
            _logger.LogWarning(
                "🐌 Slow dashboard generation: {CacheKey} took {Elapsed}ms with {QueryCount} queries. " +
                "Cache: {CacheHit}, Source: {DataSource}",
                cacheKey, elapsedMs, queryCount, cacheHit ? "Hit" : "Miss",
                _useWarehouse ? "Warehouse" : "Main"
            );
        }

        // Log if query count is high
        if (queryCount > WARNING_QUERY_COUNT)
        {
            _logger.LogWarning(
                "⚠️ High query count: {CacheKey} used {QueryCount} queries. " +
                "Consider optimizing or using warehouse. Elapsed: {Elapsed}ms",
                cacheKey, queryCount, elapsedMs
            );
        }

        // Track query statistics for optimization
        if (queryCount > 0)
        {
            UpdateQueryStats(cacheKey, queryCount, elapsedMs);
        }
    }

    /// <summary>
    /// Gets current CPU usage (approximate)
    /// </summary>
    private double GetCurrentCpuUsage()
    {
        try
        {
            using var process = Process.GetCurrentProcess();
            var totalTime = process.TotalProcessorTime;
            var startTime = DateTime.UtcNow;

            // Sample over 100ms
            Thread.Sleep(100);

            using var process2 = Process.GetCurrentProcess();
            var endTime = DateTime.UtcNow;
            var cpuUsedMs = (process2.TotalProcessorTime - totalTime).TotalMilliseconds;
            var totalMs = (endTime - startTime).TotalMilliseconds;

            return (cpuUsedMs / (totalMs * Environment.ProcessorCount)) * 100;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Updates query execution statistics
    /// </summary>
    private void UpdateQueryStats(string cacheKey, int queryCount, long elapsedMs)
    {
        var hash = cacheKey.GetHashCode().ToString();

        _queryStats.AddOrUpdate(hash,
            new QueryExecutionStats
            {
                ExecutionCount = 1,
                TotalMilliseconds = elapsedMs,
                MaxMilliseconds = elapsedMs,
                LastExecuted = DateTime.UtcNow,
                QueryHash = hash
            },
            (_, existing) =>
            {
                existing.ExecutionCount++;
                existing.TotalMilliseconds += elapsedMs;
                existing.MaxMilliseconds = Math.Max(existing.MaxMilliseconds, elapsedMs);
                existing.LastExecuted = DateTime.UtcNow;
                return existing;
            }
        );
    }

    // ============================================================
    // QUERY TRACKING HELPERS - FIXED (no ref parameter)
    // ============================================================

    // ✅ FIXED: Removed 'ref' parameter - just execute the query
    private async Task<T> TrackQueryWithCountAsync<T>(Func<Task<T>> query)
    {
        return await query();
    }

    // ✅ Helper method for growth rate calculation
    private double CalculateGrowthRate(decimal previous, decimal current)
    {
        if (previous == 0 && current == 0) return 0;
        if (previous == 0) return 100;
        return (double)((current - previous) / previous) * 100;
    }

    // ✅ Helper method to build cache key
    private string BuildAnalyticsCacheKey(
        DateTime? periodStart,
        DateTime? periodEnd,
        string periodType,
        string? fiscalYear)
    {
        var start = periodStart?.ToString("yyyy-MM-dd") ?? "none";
        var end = periodEnd?.ToString("yyyy-MM-dd") ?? "none";
        var year = fiscalYear ?? DateTime.UtcNow.Year.ToString();
        var type = periodType ?? "month";

        return $"analytics:dashboard:{type}:{year}:{start}:{end}";
    }

    // ✅ Helper method to determine cache duration based on period type
    private TimeSpan GetCacheDuration(string periodType)
    {
        return periodType?.ToLower() switch
        {
            "year" => TimeSpan.FromMinutes(10),
            "quarter" => TimeSpan.FromMinutes(5),
            "month" => TimeSpan.FromMinutes(2),
            "week" => TimeSpan.FromMinutes(1),
            "day" => TimeSpan.FromSeconds(30),
            _ => TimeSpan.FromSeconds(30)
        };
    }

    // ============================================================
    // DASHBOARD FROM WAREHOUSE WITH METRICS - FIXED
    // ============================================================

    private async Task<(AnalyticsDashboardDto Dashboard, int QueryCount)> GetDashboardFromWarehouseWithMetricsAsync(
        DateTime? periodStart,
        DateTime? periodEnd,
        string periodType,
        string? fiscalYear,
        CancellationToken ct)
    {
        var queryCount = 0;

        // Use provided dates or default to current month
        var endDate = periodEnd ?? DateTime.UtcNow.Date;
        var startDate = periodStart ?? new DateTime(endDate.Year, endDate.Month, 1);

        if (periodStart.HasValue && !periodEnd.HasValue)
        {
            endDate = startDate.AddMonths(1).AddDays(-1);
        }

        var fiscalYearStart = !string.IsNullOrEmpty(fiscalYear)
            ? new DateTime(int.Parse(fiscalYear), 1, 1)
            : new DateTime(DateTime.UtcNow.Year, 1, 1);

        var monthStart = new DateTime(startDate.Year, startDate.Month, 1);

        // Calculate previous period for growth
        DateTime previousStart, previousEnd;
        switch (periodType)
        {
            case "quarter":
                var quarterMonths = (startDate.Month - 1) / 3 * 3;
                previousStart = new DateTime(startDate.Year, quarterMonths + 1, 1).AddMonths(-3);
                previousEnd = new DateTime(startDate.Year, quarterMonths + 1, 1).AddDays(-1);
                break;
            case "year":
                previousStart = new DateTime(startDate.Year - 1, 1, 1);
                previousEnd = new DateTime(startDate.Year - 1, 12, 31);
                break;
            default: // month
                previousStart = monthStart.AddMonths(-1);
                previousEnd = monthStart.AddDays(-1);
                break;
        }

        // SALES REVENUE
        var salesQuery = _warehouseContext!.FactInvoices
            .Where(f => f.InvoiceType == InvoiceType.Sales.ToStringValue());

        // PURCHASE EXPENSES
        var purchaseQuery = _warehouseContext.FactInvoices
            .Where(f => f.InvoiceType == InvoiceType.Purchase.ToStringValue());

        // Current Period Sales
        var currentPeriodSales = await TrackQueryWithCountAsync(
            () => salesQuery
                .Where(f => f.InvoiceDate >= startDate && f.InvoiceDate <= endDate)
                .GroupBy(f => 1)
                .Select(g => new
                {
                    Revenue = g.Sum(x => x.TotalAmount),
                    Count = g.Count(),
                    Average = g.Average(x => x.TotalAmount)
                })
                .FirstOrDefaultAsync(ct)
        );
        queryCount++;

        // Sales Overdue
        var salesOverdue = await TrackQueryWithCountAsync(
            () => salesQuery
                .Where(f => f.DaysOverdue > 0 && f.Status != "Paid")
                .SumAsync(f => f.BalanceAmount, ct)
        );
        queryCount++;

        // Yearly Sales Revenue
        var yearlySalesRevenue = await TrackQueryWithCountAsync(
            () => salesQuery
                .Where(f => f.InvoiceDate >= fiscalYearStart && f.InvoiceDate <= endDate)
                .SumAsync(f => f.TotalAmount, ct)
        );
        queryCount++;

        // Previous Sales Revenue
        var previousSalesRevenue = await TrackQueryWithCountAsync(
            () => salesQuery
                .Where(f => f.InvoiceDate >= previousStart && f.InvoiceDate <= previousEnd)
                .SumAsync(f => f.TotalAmount, ct)
        );
        queryCount++;

        // Current Period Purchase
        var currentPeriodPurchase = await TrackQueryWithCountAsync(
            () => purchaseQuery
                .Where(f => f.InvoiceDate >= startDate && f.InvoiceDate <= endDate)
                .GroupBy(f => 1)
                .Select(g => new
                {
                    Expense = g.Sum(x => x.TotalAmount),
                    Count = g.Count(),
                    Average = g.Average(x => x.TotalAmount)
                })
                .FirstOrDefaultAsync(ct)
        );
        queryCount++;

        // Purchase Overdue
        var purchaseOverdue = await TrackQueryWithCountAsync(
            () => purchaseQuery
                .Where(f => f.DaysOverdue > 0 && f.Status != "Paid")
                .SumAsync(f => f.BalanceAmount, ct)
        );
        queryCount++;

        // Yearly Purchase Expense
        var yearlyPurchaseExpense = await TrackQueryWithCountAsync(
            () => purchaseQuery
                .Where(f => f.InvoiceDate >= fiscalYearStart && f.InvoiceDate <= endDate)
                .SumAsync(f => f.TotalAmount, ct)
        );
        queryCount++;

        // Previous Purchase Expense
        var previousPurchaseExpense = await TrackQueryWithCountAsync(
            () => purchaseQuery
                .Where(f => f.InvoiceDate >= previousStart && f.InvoiceDate <= previousEnd)
                .SumAsync(f => f.TotalAmount, ct)
        );
        queryCount++;

        // Top Customers & Vendors
        var topCustomersTask = GetTopCustomersFromWarehouseAsync(startDate, endDate, 10, ct);
        var topVendorsTask = GetTopVendorsFromWarehouseAsync(startDate, endDate, 10, ct);
        await Task.WhenAll(topCustomersTask, topVendorsTask);

        // Budget
        var budgetQuery = _context.Budgets
            .Where(b => !b.IsDeleted && b.Status == "Active");

        if (!string.IsNullOrEmpty(fiscalYear))
        {
            var fiscalYearStartBudget = new DateTime(int.Parse(fiscalYear), 1, 1);
            var fiscalYearEndBudget = new DateTime(int.Parse(fiscalYear), 12, 31);

            budgetQuery = budgetQuery.Where(b =>
                b.StartDate <= fiscalYearEndBudget && b.EndDate >= fiscalYearStartBudget);
        }
        else
        {
            budgetQuery = budgetQuery.Where(b =>
                b.StartDate <= endDate && b.EndDate >= startDate);
        }

        var totalBudgetAmount = await TrackQueryWithCountAsync(
            () => budgetQuery.SumAsync(b => b.TotalAmount, ct)
        );
        queryCount++;

        var totalBudgets = await TrackQueryWithCountAsync(
            () => budgetQuery.CountAsync(ct)
        );
        queryCount++;

        var operatingExpenses = await TrackQueryWithCountAsync(
            () => _context.Expenses
                .Where(e => !e.IsDeleted && e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
                .SumAsync(e => e.Amount, ct)
        );
        queryCount++;

        var cashBalance = await TrackQueryWithCountAsync(
            () => _context.BankAccounts
                .Where(b => !b.IsDeleted && b.DateAdd >= startDate && b.DateAdd <= endDate)
                .SumAsync(b => b.CurrentBalance, ct)
        );
        queryCount++;

        var monthlySalesRevenue = currentPeriodSales?.Revenue ?? 0;
        var monthlySalesCount = currentPeriodSales?.Count ?? 0;
        var avgSalesInvoice = currentPeriodSales?.Average ?? 0;
        var monthlyPurchaseExpense = currentPeriodPurchase?.Expense ?? 0;
        var monthlyPurchaseCount = currentPeriodPurchase?.Count ?? 0;
        var avgPurchaseInvoice = currentPeriodPurchase?.Average ?? 0;

        var dashboard = new AnalyticsDashboardDto
        {
            MonthlyRevenue = monthlySalesRevenue,
            MonthlyCount = monthlySalesCount,
            AverageInvoice = avgSalesInvoice,
            OverdueAmount = salesOverdue,
            YearlyRevenue = yearlySalesRevenue,
            MonthOverMonthGrowth = (double)CalculateGrowthRate(previousSalesRevenue, monthlySalesRevenue),
            MonthlyPurchaseExpense = monthlyPurchaseExpense,
            MonthlyPurchaseCount = monthlyPurchaseCount,
            AveragePurchaseInvoice = avgPurchaseInvoice,
            PurchaseOverdueAmount = purchaseOverdue,
            YearlyPurchaseExpense = yearlyPurchaseExpense,
            PurchaseMonthOverMonthGrowth = (double)CalculateGrowthRate(previousPurchaseExpense, monthlyPurchaseExpense),
            OperatingExpenses = operatingExpenses,
            CashBalance = cashBalance,
            TopCustomers = await topCustomersTask,
            TopVendors = await topVendorsTask,
            TotalBudgets = totalBudgets,
            TotalBudgetAmount = totalBudgetAmount,
            TotalExpenses = monthlyPurchaseExpense + operatingExpenses,
            DateGenerated = DateTime.UtcNow,
            PeriodStart = startDate.ToString("yyyy-MM-dd"),
            PeriodEnd = endDate.ToString("yyyy-MM-dd"),
            PeriodType = periodType,
            FiscalYear = fiscalYear ?? DateTime.UtcNow.Year.ToString()
        };

        return (dashboard, queryCount);
    }

    // ============================================================
    // DASHBOARD FROM MAIN CONTEXT WITH METRICS - FIXED
    // ============================================================

  private async Task<(AnalyticsDashboardDto Dashboard, int QueryCount)> GetDashboardFromMainContextWithMetricsAsync(
      DateTime? periodStart,
      DateTime? periodEnd,
      string periodType,
      string? fiscalYear,
      CancellationToken ct)
  {
      var stopwatch = Stopwatch.StartNew();
      var queryCount = 0;
      var tracker = new DashboardPerformanceTracker(_logger);

      // Declare variables outside using block
      DateTime startDate;
      DateTime endDate;
      DateTime fiscalYearStart;
      DateTime previousStart;
      DateTime previousEnd;

      try
      {
          _logger.LogInformation("📊 Starting dashboard generation for {PeriodType} {PeriodStart}",
              periodType, periodStart?.ToString("yyyy-MM-dd") ?? "default");

          // ============================================================
          // 1. DATE RANGE CALCULATIONS
          // ============================================================
          using (tracker.StartOperation("Date Range Calculations"))
          {
              endDate = periodEnd ?? DateTime.UtcNow.Date;
              startDate = periodStart ?? new DateTime(endDate.Year, endDate.Month, 1);

              if (periodStart.HasValue && !periodEnd.HasValue)
              {
                  endDate = startDate.AddMonths(1).AddDays(-1);
              }

              if (startDate > endDate)
              {
                  (startDate, endDate) = (endDate, startDate);
              }

              fiscalYearStart = !string.IsNullOrEmpty(fiscalYear)
                  ? new DateTime(int.Parse(fiscalYear), 1, 1)
                  : new DateTime(DateTime.UtcNow.Year, 1, 1);

              var monthStart = new DateTime(startDate.Year, startDate.Month, 1);

              // Previous period for growth calculations
              switch (periodType)
              {
                  case "quarter":
                      var quarterMonths = (startDate.Month - 1) / 3 * 3;
                      previousStart = new DateTime(startDate.Year, quarterMonths + 1, 1).AddMonths(-3);
                      previousEnd = new DateTime(startDate.Year, quarterMonths + 1, 1).AddDays(-1);
                      break;
                  case "year":
                      previousStart = new DateTime(startDate.Year - 1, 1, 1);
                      previousEnd = new DateTime(startDate.Year - 1, 12, 31);
                      break;
                  default:
                      previousStart = monthStart.AddMonths(-1);
                      previousEnd = monthStart.AddDays(-1);
                      break;
              }
          }

          // ============================================================
          // 2. SALES QUERY - OPTIMIZED
          // ============================================================
          var salesStopwatch = Stopwatch.StartNew();
          var salesQuery = _context.Invoices
              .Where(i => !i.IsDeleted && i.InvoiceType == InvoiceType.Sales.ToStringValue());



        // 1. Get Revenue and Count (Fast!)
     var revenueAndCount = await TrackQueryWithCountAsync(
         () => salesQuery
             .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
             .GroupBy(i => 1)
             .Select(g => new
             {
                 Revenue = g.Sum(i => i.TotalAmount),
                 Count = g.Count()
             })
             .OrderBy(x => x.Revenue)  // ✅ This silences the EF Core warning
             .FirstOrDefaultAsync(ct)
     );
     queryCount++;

        // 2. Get Overdue Amount (Fast with index)
        var overdueAmount = await TrackQueryWithCountAsync(
            () => salesQuery
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate
                    && i.Status != "Paid" && i.DueDate < endDate)
                .SumAsync(i => i.TotalAmount - i.PaidAmount, ct)
        );
        queryCount++;

        // 3. Get Status Breakdown (Fast with index)
        var byStatus = await TrackQueryWithCountAsync(
            () => salesQuery
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .GroupBy(i => i.Status)
                .Select(s => new StatusSummaryDto
                {
                    Status = s.Key ?? "Unknown",
                    Count = s.Count(),
                    Amount = s.Sum(i => i.TotalAmount)
                })
                .ToListAsync(ct)
        );
        queryCount++;

        // Combine results into the same structure as before
        var salesData = new
        {
            Revenue = revenueAndCount?.Revenue ?? 0,
            Count = revenueAndCount?.Count ?? 0,
            OverdueAmount = overdueAmount,
            ByStatus = byStatus
        };

        salesStopwatch.Stop();
        tracker.RecordTiming("Sales Data Query (Optimized)", salesStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 3. YEARLY SALES
          // ============================================================
          var yearlySalesStopwatch = Stopwatch.StartNew();
          var yearlySalesRevenue = await TrackQueryWithCountAsync(
              () => salesQuery
                  .Where(i => i.InvoiceDate >= fiscalYearStart && i.InvoiceDate <= endDate)
                  .SumAsync(i => i.TotalAmount, ct)
          );
          queryCount++;
          yearlySalesStopwatch.Stop();
          tracker.RecordTiming("Yearly Sales Query", yearlySalesStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 4. PREVIOUS SALES
          // ============================================================
          var previousSalesStopwatch = Stopwatch.StartNew();
          var previousSalesRevenue = await TrackQueryWithCountAsync(
              () => salesQuery
                  .Where(i => i.InvoiceDate >= previousStart && i.InvoiceDate <= previousEnd)
                  .SumAsync(i => i.TotalAmount, ct)
          );
          queryCount++;
          previousSalesStopwatch.Stop();
          tracker.RecordTiming("Previous Sales Query", previousSalesStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 5. PURCHASE QUERY
          // ============================================================
         // ============================================================
         // ✅ OPTIMIZE PURCHASE DATA QUERY TOO
         // ============================================================

         var purchaseStopwatch = Stopwatch.StartNew();
         var purchaseQuery = _context.Invoices
             .Where(i => !i.IsDeleted && i.InvoiceType == InvoiceType.Purchase.ToStringValue());

         // 1. Get Expense and Count
        var expenseAndCount = await TrackQueryWithCountAsync(
            () => purchaseQuery
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .GroupBy(i => 1)
                .Select(g => new
                {
                    Expense = g.Sum(i => i.TotalAmount),
                    Count = g.Count()
                })
                .OrderBy(x => x.Expense)  // ✅ Add OrderBy here too
                .FirstOrDefaultAsync(ct)
        );
        queryCount++;

         // 2. Get Purchase Overdue Amount
         var purchaseOverdueAmount = await TrackQueryWithCountAsync(
             () => purchaseQuery
                 .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate
                     && i.Status != "Paid" && i.DueDate < endDate)
                 .SumAsync(i => i.TotalAmount - i.PaidAmount, ct)
         );
         queryCount++;

         // 3. Get Purchase Status Breakdown
         var purchaseByStatus = await TrackQueryWithCountAsync(
             () => purchaseQuery
                 .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                 .GroupBy(i => i.Status)
                 .Select(s => new StatusSummaryDto
                 {
                     Status = s.Key ?? "Unknown",
                     Count = s.Count(),
                     Amount = s.Sum(i => i.TotalAmount)
                 })
                 .ToListAsync(ct)
         );
         queryCount++;

         // Combine results
         var purchaseData = new
         {
             Expense = expenseAndCount?.Expense ?? 0,
             Count = expenseAndCount?.Count ?? 0,
             OverdueAmount = purchaseOverdueAmount,
             ByStatus = purchaseByStatus
         };

         purchaseStopwatch.Stop();
         tracker.RecordTiming("Purchase Data Query (Optimized)", purchaseStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 6. YEARLY PURCHASES
          // ============================================================
          var yearlyPurchaseStopwatch = Stopwatch.StartNew();
          var yearlyPurchaseExpense = await TrackQueryWithCountAsync(
              () => purchaseQuery
                  .Where(i => i.InvoiceDate >= fiscalYearStart && i.InvoiceDate <= endDate)
                  .SumAsync(i => i.TotalAmount, ct)
          );
          queryCount++;
          yearlyPurchaseStopwatch.Stop();
          tracker.RecordTiming("Yearly Purchase Query", yearlyPurchaseStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 7. PREVIOUS PURCHASES
          // ============================================================
          var previousPurchaseStopwatch = Stopwatch.StartNew();
          var previousPurchaseExpense = await TrackQueryWithCountAsync(
              () => purchaseQuery
                  .Where(i => i.InvoiceDate >= previousStart && i.InvoiceDate <= previousEnd)
                  .SumAsync(i => i.TotalAmount, ct)
          );
          queryCount++;
          previousPurchaseStopwatch.Stop();
          tracker.RecordTiming("Previous Purchase Query", previousPurchaseStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 8. OPERATING EXPENSES
          // ============================================================
          var expensesStopwatch = Stopwatch.StartNew();
          var operatingExpenses = await TrackQueryWithCountAsync(
              () => _context.Expenses
                  .Where(e => !e.IsDeleted && e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
                  .SumAsync(e => e.Amount, ct)
          );
          queryCount++;
          expensesStopwatch.Stop();
          tracker.RecordTiming("Operating Expenses Query", expensesStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 9. EXPENSES BY CATEGORY - OPTIMIZED
          // ============================================================
          var categoryStopwatch = Stopwatch.StartNew();
          var expensesByCategory = await TrackQueryWithCountAsync(
              () => _context.Expenses
                  .Where(e => !e.IsDeleted && e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
                  .GroupBy(e => e.ExpenseCategoryId)
                  .Select(g => new
                  {
                      CategoryId = g.Key,
                      Amount = g.Sum(e => e.Amount),
                      Count = g.Count()
                  })
                  .OrderByDescending(x => x.Amount) // ✅ Add ordering
                  .ToListAsync(ct)
          );
          queryCount++;
          categoryStopwatch.Stop();
          tracker.RecordTiming("Expenses by Category Query", categoryStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 10. EXPENSE CATEGORIES NAMES - CACHED
          // ============================================================
          var categoryNamesStopwatch = Stopwatch.StartNew();

          // ✅ Use cached categories if available, otherwise query
          var expenseCategories = await GetCachedExpenseCategoriesAsync(ct);
          queryCount++; // Count as one query

          categoryNamesStopwatch.Stop();
          tracker.RecordTiming("Expense Categories Names Query", categoryNamesStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 11. BUDGET
          // ============================================================
          var budgetStopwatch = Stopwatch.StartNew();
          var budgetQuery = _context.Budgets
              .Where(b => !b.IsDeleted && b.Status == "Active");

          if (!string.IsNullOrEmpty(fiscalYear))
          {
              var fiscalYearStartBudget = new DateTime(int.Parse(fiscalYear), 1, 1);
              var fiscalYearEndBudget = new DateTime(int.Parse(fiscalYear), 12, 31);
              budgetQuery = budgetQuery.Where(b =>
                  b.StartDate <= fiscalYearEndBudget && b.EndDate >= fiscalYearStartBudget);
          }
          else
          {
              budgetQuery = budgetQuery.Where(b =>
                  b.StartDate <= endDate && b.EndDate >= startDate);
          }

          var totalBudgetAmount = await TrackQueryWithCountAsync(
              () => budgetQuery.SumAsync(b => b.TotalAmount, ct)
          );
          queryCount++;
          budgetStopwatch.Stop();
          tracker.RecordTiming("Budget Query", budgetStopwatch.ElapsedMilliseconds);

          var totalBudgetsStopwatch = Stopwatch.StartNew();
          var totalBudgets = await TrackQueryWithCountAsync(
              () => budgetQuery.CountAsync(ct)
          );
          queryCount++;
          totalBudgetsStopwatch.Stop();
          tracker.RecordTiming("Budget Count Query", totalBudgetsStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 12. CASH & BANK
          // ============================================================
          var cashStopwatch = Stopwatch.StartNew();
          var cashBalance = await TrackQueryWithCountAsync(
              () => _context.BankAccounts
                  .Where(b => !b.IsDeleted && b.DateAdd >= startDate && b.DateAdd <= endDate)
                  .SumAsync(b => b.CurrentBalance, ct)
          );
          queryCount++;
          cashStopwatch.Stop();
          tracker.RecordTiming("Cash Balance Query", cashStopwatch.ElapsedMilliseconds);

          var bankStopwatch = Stopwatch.StartNew();
          var bankAccounts = await TrackQueryWithCountAsync(
              () => _context.BankAccounts
                  .Where(b => !b.IsDeleted && b.DateAdd >= startDate && b.DateAdd <= endDate)
                  .Select(b => new { b.AccountType, b.CurrentBalance, b.DateAdd }) // ✅ Select only needed fields
                  .ToListAsync(ct)
          );
          queryCount++;
          bankStopwatch.Stop();
          tracker.RecordTiming("Bank Accounts Query", bankStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 13. ACCOUNTS RECEIVABLE & PAYABLE
          // ============================================================
          var arStopwatch = Stopwatch.StartNew();
          var accountsReceivable = await TrackQueryWithCountAsync(
              () => salesQuery
                  .Where(i => i.Status != "Paid" && i.Status != "Cancelled" && i.Status != "Completed")
                  .SumAsync(i => i.TotalAmount - i.PaidAmount, ct)
          );
          queryCount++;
          arStopwatch.Stop();
          tracker.RecordTiming("Accounts Receivable Query", arStopwatch.ElapsedMilliseconds);

          var apStopwatch = Stopwatch.StartNew();
          var accountsPayable = await TrackQueryWithCountAsync(
              () => purchaseQuery
                  .Where(i => i.Status != "Paid" && i.Status != "Cancelled" && i.Status != "Completed")
                  .SumAsync(i => i.TotalAmount - i.PaidAmount, ct)
          );
          queryCount++;
          apStopwatch.Stop();
          tracker.RecordTiming("Accounts Payable Query", apStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 14. ASSETS - OPTIMIZED WITH PROJECTION
          // ============================================================
          var assetsStopwatch = Stopwatch.StartNew();
          var assets = await TrackQueryWithCountAsync(
              () => _context.Assets
                  .Where(a => !a.IsDeleted && a.DateAdd >= startDate && a.DateAdd <= endDate)
                  .Select(a => new
                  {
                      a.CurrentValue,
                      a.AccumulatedDepreciation,
                      a.Status,
                      a.DateAdd
                  })
                  .ToListAsync(ct)
          );
          queryCount++;
          assetsStopwatch.Stop();
          tracker.RecordTiming("Assets Query", assetsStopwatch.ElapsedMilliseconds);

          // Calculate asset metrics in memory
          var totalAssetValue = assets.Sum(a => a.CurrentValue);
          var totalDepreciation = assets.Sum(a => a.AccumulatedDepreciation);
          var netBookValue = totalAssetValue - totalDepreciation;
          var activeAssetCount = assets.Count(a => a.Status == "Active");
          var maintenanceAssetCount = assets.Count(a => a.Status == "Maintenance");
          var totalAssetCount = assets.Count;

          // ============================================================
          // 15. JOURNAL ENTRIES - OPTIMIZED WITH PROJECTION
          // ============================================================
          var journalStopwatch = Stopwatch.StartNew();
          var journalEntries = await TrackQueryWithCountAsync(
              () => _context.JournalEntries
                  .Where(j => !j.IsDeleted && j.EntryDate >= startDate && j.EntryDate <= endDate)
                  .Select(j => new
                  {
                      j.IsPosted,
                      j.TotalDebit,
                      j.TotalCredit,
                      j.EntryDate,
                      j.Id
                  })
                  .ToListAsync(ct)
          );
          queryCount++;
          journalStopwatch.Stop();
          tracker.RecordTiming("Journal Entries Query", journalStopwatch.ElapsedMilliseconds);

          // Calculate journal metrics in memory
          var totalJournalEntries = journalEntries.Count;
          var postedJournalCount = journalEntries.Count(j => j.IsPosted);
          var unpostedJournalCount = journalEntries.Count(j => !j.IsPosted);
          var totalJournalDebit = journalEntries.Sum(j => j.TotalDebit);
          var totalJournalCredit = journalEntries.Sum(j => j.TotalCredit);
          var isJournalBalanced = Math.Abs(totalJournalDebit - totalJournalCredit) < 0.01m;

          // ============================================================
          // 16. PAYMENTS - OPTIMIZED
          // ============================================================
          var paymentsStopwatch = Stopwatch.StartNew();
          var payments = await TrackQueryWithCountAsync(
              () => _context.Payments
                  .Where(p => !p.IsDeleted && p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                  .Select(p => new
                  {
                      p.PaymentType,
                      p.Status,
                      p.Amount,
                      p.PaymentDate
                  })
                  .ToListAsync(ct)
          );
          queryCount++;
          paymentsStopwatch.Stop();
          tracker.RecordTiming("Payments Query", paymentsStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 17. INVOICES FOR VOUCHERS - OPTIMIZED
          // ============================================================
          var invoicesForVouchersStopwatch = Stopwatch.StartNew();
          var invoicesForVouchers = await TrackQueryWithCountAsync(
              () => _context.Invoices
                  .Where(i => !i.IsDeleted && i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                  .Select(i => new { i.Status, i.InvoiceDate })
                  .ToListAsync(ct)
          );
          queryCount++;
          invoicesForVouchersStopwatch.Stop();
          tracker.RecordTiming("Invoices for Vouchers Query", invoicesForVouchersStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 18. VENDORS AND CUSTOMERS - OPTIMIZED
          // ============================================================
          var vendorsStopwatch = Stopwatch.StartNew();
          var vendorsData = await TrackQueryWithCountAsync(
              () => _context.Vendors
                  .Where(v => !v.IsDeleted)
                  .Select(v => new { v.Id, v.Name })
                  .ToListAsync(ct)
          );
          queryCount++;
          vendorsStopwatch.Stop();
          tracker.RecordTiming("Vendors Query", vendorsStopwatch.ElapsedMilliseconds);

          var customersStopwatch = Stopwatch.StartNew();
          var customersData = await TrackQueryWithCountAsync(
              () => _context.Customers
                  .Where(c => !c.IsDeleted)
                  .Select(c => new { c.Id, c.Name })
                  .ToListAsync(ct)
          );
          queryCount++;
          customersStopwatch.Stop();
          tracker.RecordTiming("Customers Query", customersStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 19. TOP CUSTOMERS & VENDORS
          // ============================================================
          var topCustomersStopwatch = Stopwatch.StartNew();
          var topCustomers = await GetTopCustomersFromMainContextAsync(startDate, endDate, 10, ct);
          topCustomersStopwatch.Stop();
          tracker.RecordTiming("Top Customers Query", topCustomersStopwatch.ElapsedMilliseconds);

          var topVendorsStopwatch = Stopwatch.StartNew();
          var topVendors = await GetTopVendorsFromMainContextAsync(startDate, endDate, 10, ct);
          topVendorsStopwatch.Stop();
          tracker.RecordTiming("Top Vendors Query", topVendorsStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 20. REVENUE TREND
          // ============================================================
          var trendStopwatch = Stopwatch.StartNew();
          var revenueTrend = await GetRevenueTrendFromMainContextAsync(endDate, 12, ct);
          trendStopwatch.Stop();
          tracker.RecordTiming("Revenue Trend Query", trendStopwatch.ElapsedMilliseconds);

          // ============================================================
          // 21. AGING REPORT
          // ============================================================
          var agingStopwatch = Stopwatch.StartNew();
          var agingReport = await GetAgingReportFromMainContextAsync(endDate, ct);
          agingStopwatch.Stop();
          tracker.RecordTiming("Aging Report Query", agingStopwatch.ElapsedMilliseconds);

          // ============================================================
          // CALCULATE METRICS
          // ============================================================
          var calcStopwatch = Stopwatch.StartNew();

                var monthlySalesRevenue = salesData?.Revenue ?? 0;
                var monthlySalesCount = salesData?.Count ?? 0;
                var avgSalesInvoice = monthlySalesCount > 0 ? monthlySalesRevenue / monthlySalesCount : 0;
                var salesOverdueAmount = salesData?.OverdueAmount ?? 0;
                var salesByStatus = salesData?.ByStatus ?? new List<StatusSummaryDto>();
                var paidSalesAmount = salesByStatus.Where(s => s.Status == "Paid").Sum(s => s.Amount);
                var unpaidSalesAmount = salesByStatus.Where(s => s.Status != "Paid").Sum(s => s.Amount);

                var monthlyPurchaseExpense = purchaseData?.Expense ?? 0;
                var monthlyPurchaseCount = purchaseData?.Count ?? 0;
                var avgPurchaseInvoice = monthlyPurchaseCount > 0 ? monthlyPurchaseExpense / monthlyPurchaseCount : 0;
               // var purchaseOverdueAmount = purchaseData?.OverdueAmount ?? 0;
               // var purchaseByStatus = purchaseData?.ByStatus ?? new List<StatusSummaryDto>();
                var paidPurchaseAmount = purchaseByStatus.Where(s => s.Status == "Paid").Sum(s => s.Amount);
                var unpaidPurchaseAmount = purchaseByStatus.Where(s => s.Status != "Paid").Sum(s => s.Amount);

                // Expense Categories
                var expenseCategoryDtos = expensesByCategory
                    .Select(g => new ExpenseCategoryDto
                    {
                        Category = expenseCategories.TryGetValue(g.CategoryId, out var name) ? name : "Uncategorized",
                        Amount = g.Amount,
                        Count = g.Count
                    })
                    .OrderByDescending(e => e.Amount)
                    .ToList();

                // Budget Calculations
                var totalActualSpent = monthlyPurchaseExpense + operatingExpenses;
                var budgetUtilization = totalBudgetAmount > 0 ? (totalActualSpent / totalBudgetAmount) * 100 : 0;
                var budgetRemaining = totalBudgetAmount - totalActualSpent;
                var budgetVariance = totalBudgetAmount - totalActualSpent;
                var budgetVariancePercentage = totalBudgetAmount > 0 ? (budgetVariance / totalBudgetAmount) * 100 : 0;
                var isOverBudget = budgetUtilization > 100;
                var isNearBudget = budgetUtilization > 80 && budgetUtilization <= 100;

                var budgetVsActual = new BudgetVsActualDto
                {
                    TotalBudget = totalBudgetAmount,
                    ActualSpent = totalActualSpent,
                    Variance = totalBudgetAmount - totalActualSpent,
                    VariancePercentage = totalBudgetAmount > 0
                        ? ((totalBudgetAmount - totalActualSpent) / totalBudgetAmount) * 100
                        : 0
                };

                // Cash & Bank
                var cashInflow = monthlySalesRevenue + paidSalesAmount;
                var cashOutflow = monthlyPurchaseExpense + operatingExpenses + paidPurchaseAmount;
                var netCashFlow = cashInflow - cashOutflow;
                var cashAmount = bankAccounts.Where(b => b.AccountType == "Cash").Sum(b => b.CurrentBalance);
                var bankAmount = bankAccounts.Where(b => b.AccountType != "Cash").Sum(b => b.CurrentBalance);

                // Asset Metrics


                // Voucher Metrics
                var paymentVoucherCount = payments.Count(p => p.PaymentType == "Purchase" || p.PaymentType == "Payment");
                var receiptVoucherCount = invoicesForVouchers.Count(i => i.Status == "Paid");
                var journalVoucherCount = journalEntries.Count;
                var totalVouchers = paymentVoucherCount + receiptVoucherCount + journalVoucherCount;
                var pendingPaymentVouchers = payments.Count(p => p.Status == "Pending" || p.Status == "Draft");
                var pendingReceiptVouchers = invoicesForVouchers.Count(i => i.Status != "Paid");
                var pendingJournalVouchers = journalEntries.Count(j => !j.IsPosted);
                var processedVoucherTypes = (pendingPaymentVouchers == 0 ? 1 : 0) +
                                            (pendingReceiptVouchers == 0 ? 1 : 0) +
                                            (pendingJournalVouchers == 0 ? 1 : 0);
                var voucherProcessedPercentage = (processedVoucherTypes / 3m) * 100m;

                // Cost Metrics
                var totalRevenue = monthlySalesRevenue;
                var purchaseCost = monthlyPurchaseExpense;
                var costPerUnit = totalRevenue > 0 ? (purchaseCost / totalRevenue) * 100 : 0;
                var profitPerUnit = totalRevenue > 0 ? ((totalRevenue - purchaseCost) / totalRevenue) * 100 : 0;
                var costToRevenueRatio = totalRevenue > 0 ? (purchaseCost / totalRevenue) * 100 : 0;

                // Final Calculations
                var totalExpenses = monthlyPurchaseExpense + operatingExpenses;
                var netIncome = totalRevenue - totalExpenses;
                var profitMargin = totalRevenue > 0 ? (netIncome / totalRevenue) * 100 : 0;
                var growthRate = CalculateGrowthRate(previousSalesRevenue, monthlySalesRevenue);
                var purchaseGrowthRate = CalculateGrowthRate(previousPurchaseExpense, monthlyPurchaseExpense);

                // Overdue and pending metrics
                var overdueInvoices = salesByStatus.Where(s => s.Status == "Overdue").Sum(s => s.Count);
                var pendingInvoices = salesByStatus.Where(s => s.Status == "Pending").Sum(s => s.Count);
                var overduePercentage = accountsReceivable > 0 ? (purchaseOverdueAmount / accountsReceivable) * 100 : 0;
                var currentPercentage = accountsReceivable > 0 ? ((accountsReceivable - purchaseOverdueAmount) / accountsReceivable) * 100 : 0;

          calcStopwatch.Stop();
          tracker.RecordTiming("Calculations & DTO Building", calcStopwatch.ElapsedMilliseconds);

          stopwatch.Stop();

          // Log detailed performance summary
          tracker.LogSummary($"Dashboard Generation - {periodType} {startDate:yyyy-MM-dd}");

          // Log performance metrics
          _logger.LogInformation(
              "✅ Dashboard generated in {ElapsedMs}ms | DB Queries: {DbQueryCount} | Period: {PeriodType} {PeriodStart}",
              stopwatch.ElapsedMilliseconds,
              queryCount,
              periodType,
              startDate.ToString("yyyy-MM-dd")
          );

           // ============================================================
           // BUILD DASHBOARD DTO
           // ============================================================
           var dashboard = new AnalyticsDashboardDto
           {
               // Sales Metrics
               MonthlyRevenue = monthlySalesRevenue,
               MonthlyCount = monthlySalesCount,
               AverageInvoice = avgSalesInvoice,
               OverdueAmount = salesOverdueAmount,
               YearlyRevenue = yearlySalesRevenue,
               MonthOverMonthGrowth = (double)growthRate,

               // Purchase Metrics
               MonthlyPurchaseExpense = monthlyPurchaseExpense,
               MonthlyPurchaseCount = monthlyPurchaseCount,
               AveragePurchaseInvoice = avgPurchaseInvoice,
               PurchaseOverdueAmount = purchaseOverdueAmount,
               YearlyPurchaseExpense = yearlyPurchaseExpense,
               PurchaseMonthOverMonthGrowth = (double)purchaseGrowthRate,

               // Financial Health
               NetIncome = netIncome,
               ProfitMargin = (decimal)profitMargin,
               OperatingExpenses = operatingExpenses,
               CashBalance = cashBalance,
               NetCashFlow = netCashFlow,
               CashInflow = cashInflow,
               CashOutflow = cashOutflow,

               // Accounts
               AccountsReceivable = accountsReceivable,
               AccountsPayable = accountsPayable,
               TotalAssets = totalAssetValue,

               // Budget
               TotalBudgetAmount = totalBudgetAmount,
               TotalBudgets = totalBudgets,
               TotalExpenses = totalExpenses,
               BudgetVsActual = budgetVsActual,
               BudgetUtilization = budgetUtilization,
               BudgetRemaining = budgetRemaining,
               BudgetVariance = budgetVariance,
               BudgetVariancePercentage = budgetVariancePercentage,
               IsOverBudget = isOverBudget,
               IsNearBudget = isNearBudget,

               // Cost Metrics
               PurchaseCost = purchaseCost,
               CostPerUnit = costPerUnit,
               ProfitPerUnit = profitPerUnit,
               CostToRevenueRatio = costToRevenueRatio,

               // General Ledger Metrics
               TotalJournalEntries = totalJournalEntries,
               PostedJournalCount = postedJournalCount,
               UnpostedJournalCount = unpostedJournalCount,
               TotalJournalDebit = totalJournalDebit,
               TotalJournalCredit = totalJournalCredit,
               IsJournalBalanced = isJournalBalanced,

               // Voucher Metrics
               PaymentVoucherCount = paymentVoucherCount,
               ReceiptVoucherCount = receiptVoucherCount,
               JournalVoucherCount = journalVoucherCount,
               TotalVouchers = totalVouchers,
               PendingPaymentVouchers = pendingPaymentVouchers,
               PendingReceiptVouchers = pendingReceiptVouchers,
               PendingJournalVouchers = pendingJournalVouchers,
               ProcessedVoucherTypes = processedVoucherTypes,
               VoucherProcessedPercentage = voucherProcessedPercentage,

               // Asset Metrics
               TotalAssetValue = totalAssetValue,
               NetBookValue = netBookValue,
               TotalDepreciation = totalDepreciation,
               ActiveAssetCount = activeAssetCount,
               MaintenanceAssetCount = maintenanceAssetCount,
               TotalAssetCount = totalAssetCount,

               // Cash & Bank Metrics
               CashAmount = cashAmount,
               BankAmount = bankAmount,

               // Other Metrics
               OverdueInvoices = overdueInvoices,
               PendingInvoices = pendingInvoices,
               OverduePercentage = overduePercentage,
               CurrentPercentage = currentPercentage,
               VendorCount = vendorsData.Count,
               CustomerCount = customersData.Count,

               // Sales Breakdown
               PaidSalesAmount = paidSalesAmount,
               UnpaidSalesAmount = unpaidSalesAmount,
               SalesByStatus = salesByStatus,
               ExpensesByCategory = expenseCategoryDtos,

               // Top Lists
               TopCustomers = topCustomers,
               TopVendors = topVendors,

               // Trends
               RevenueTrend = revenueTrend,
               AgingReport = agingReport,

               // Metadata
               DateGenerated = DateTime.UtcNow,
               PeriodStart = startDate.ToString("yyyy-MM-dd"),
               PeriodEnd = endDate.ToString("yyyy-MM-dd"),
               PeriodType = periodType,
               FiscalYear = fiscalYear ?? DateTime.UtcNow.Year.ToString()
           };

           return (dashboard, queryCount);
       }
       catch (Exception ex)
       {
           stopwatch.Stop();
           tracker.LogSummary("❌ FAILED Dashboard Generation");
           _logger.LogError(ex,
               "❌ Dashboard generation failed after {ElapsedMs}ms | Period: {PeriodType} {PeriodStart}",
               stopwatch.ElapsedMilliseconds,
               periodType,
               periodStart?.ToString("yyyy-MM-dd") ?? "unknown"
           );
           throw;
       }
   }

    // ============================================================
    // PUBLIC PERFORMANCE API METHODS
    // ============================================================

    /// <summary>
    /// Gets performance statistics for monitoring
    /// </summary>
    public PerformanceSummary GetPerformanceSummary(TimeSpan? timeRange = null)
    {
        return _performanceStats.GetSummary(timeRange);
    }

    /// <summary>
    /// Gets query execution statistics for optimization
    /// </summary>
    public Dictionary<string, QueryExecutionStats> GetQueryStatistics()
    {
        return _queryStats.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value
        );
    }

    /// <summary>
    /// Gets slow queries for optimization
    /// </summary>
    public List<KeyValuePair<string, QueryExecutionStats>> GetSlowQueries(int thresholdMs = 1000)
    {
        return _queryStats
            .Where(kvp => kvp.Value.MaxMilliseconds > thresholdMs)
            .OrderByDescending(kvp => kvp.Value.MaxMilliseconds)
            .ToList();
    }

    // ============================================================
    // REVENUE TREND HELPERS
    // ============================================================

   private async Task<List<RevenueTrendDto>> GetRevenueTrendFromMainContextAsync(
       DateTime endDate,
       int months,
       CancellationToken ct)
   {
       // ✅ Cache key based on date range
       var cacheKey = $"revenue_trend_{endDate:yyyy-MM}_{months}";

       // ✅ Try get from cache
       var cached = await _cache.GetAsync<List<RevenueTrendDto>>(cacheKey);
       if (cached is not null)
       {
           _logger.LogDebug("📦 Revenue trend cache HIT");
           return cached;
       }

       _logger.LogDebug("📦 Revenue trend cache MISS - querying database");

       var startDate = endDate.AddMonths(-months + 1);
       var startOfMonth = new DateTime(startDate.Year, startDate.Month, 1);

       // ✅ Single query
       var query = await _context.Invoices
           .Where(i => !i.IsDeleted
               && i.InvoiceDate >= startOfMonth
               && i.InvoiceDate <= endDate
               && (i.InvoiceType == InvoiceType.Sales.ToStringValue()
                   || i.InvoiceType == InvoiceType.Purchase.ToStringValue()))
           .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month, i.InvoiceType })
           .Select(g => new
           {
               g.Key.Year,
               g.Key.Month,
               InvoiceType = g.Key.InvoiceType,
               Total = g.Sum(i => i.TotalAmount)
           })
           .ToListAsync(ct);

       // ✅ Build trend
       var trend = new List<RevenueTrendDto>();
       var currentDate = startOfMonth;

       for (int i = 0; i < months; i++)
       {
           var monthStart = currentDate;
           var monthEnd = monthStart.AddMonths(1).AddDays(-1);

           var monthData = query
               .Where(q => q.Year == monthStart.Year && q.Month == monthStart.Month)
               .ToList();

           var revenue = monthData
               .Where(q => q.InvoiceType == InvoiceType.Sales.ToStringValue())
               .Sum(q => q.Total);

           var expenses = monthData
               .Where(q => q.InvoiceType == InvoiceType.Purchase.ToStringValue())
               .Sum(q => q.Total);

           trend.Add(new RevenueTrendDto
           {
               Month = monthStart.ToString("MMM yyyy"),
               MonthStart = monthStart,
               MonthEnd = monthEnd,
               Revenue = revenue,
               Expenses = expenses,
               Profit = revenue - expenses
           });

           currentDate = currentDate.AddMonths(1);
       }

       // ✅ Cache for 30 minutes (trend data doesn't change often)
       await _cache.SetAsync(cacheKey, trend, TimeSpan.FromMinutes(30));

       return trend;
   }

    // ============================================================
    // AGING REPORT HELPERS
    // ============================================================

    private async Task<AgingReportDto> GetAgingReportFromMainContextAsync(
        DateTime asOfDate,
        CancellationToken ct)
    {
        var aging = new AgingReportDto
        {
            Details = new List<AgingDetailDto>()
        };

        var invoices = await _context.Invoices
            .Where(i => !i.IsDeleted && i.Status != "Paid" && i.Status != "Cancelled" && i.Status != "Completed")
            .Select(i => new
            {
                i.InvoiceNumber,
                i.TotalAmount,
                i.PaidAmount,
                i.InvoiceDate,
                i.DueDate,
                i.Status
            })
            .ToListAsync(ct);

        foreach (var invoice in invoices)
        {
            var amount = invoice.TotalAmount - invoice.PaidAmount;

            if (amount <= 0) continue;

            int daysOverdue;
            string agingBucket;

            if (!invoice.DueDate.HasValue)
            {
                aging.UnknownDueDate += amount;
                daysOverdue = 0;
                agingBucket = "No Due Date";
            }
            else
            {
                daysOverdue = (int)(asOfDate - invoice.DueDate.Value).TotalDays;

                if (daysOverdue <= 0)
                {
                    aging.Current += amount;
                    agingBucket = "Current";
                }
                else if (daysOverdue <= 30)
                {
                    aging.Overdue30 += amount;
                    agingBucket = "1-30 Days";
                }
                else if (daysOverdue <= 60)
                {
                    aging.Overdue60 += amount;
                    agingBucket = "31-60 Days";
                }
                else if (daysOverdue <= 90)
                {
                    aging.Overdue90 += amount;
                    agingBucket = "61-90 Days";
                }
                else
                {
                    aging.Overdue90Plus += amount;
                    agingBucket = "90+ Days";
                }
            }

            aging.Details.Add(new AgingDetailDto
            {
                InvoiceNumber = invoice.InvoiceNumber,
                Amount = amount,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                DaysOverdue = Math.Max(0, daysOverdue),
                AgingBucket = agingBucket,
                Status = invoice.Status
            });
        }

        aging.TotalOutstanding = aging.Current + aging.Overdue30 + aging.Overdue60 + aging.Overdue90 + aging.Overdue90Plus + aging.UnknownDueDate;
        aging.AsOfDate = asOfDate;

        return aging;
    }

    // ============================================================
    // TOP CUSTOMERS & VENDORS HELPERS
    // ============================================================

    private async Task<List<AnalyticsDto>> GetTopCustomersFromWarehouseAsync(
        DateTime startDate,
        DateTime endDate,
        int count,
        CancellationToken ct)
    {
        return await _warehouseContext!.FactInvoices
            .Where(f => f.InvoiceType == InvoiceType.Sales.ToStringValue()
                        && f.CustomerKey > 0
                        && f.InvoiceDate >= startDate
                        && f.InvoiceDate <= endDate)
            .Join(_warehouseContext.DimCustomers,
                  invoice => invoice.CustomerKey,
                  customer => customer.CustomerKey,
                  (invoice, customer) => new { invoice, customer })
            .GroupBy(x => new { x.customer.CustomerKey, x.customer.Name })
            .Select(g => new AnalyticsDto
            {
                CustomerKey = g.Key.CustomerKey,
                CustomerName = g.Key.Name,
                TotalAmount = g.Sum(x => x.invoice.TotalAmount),
                Count = g.Count(),
                AverageInvoice = g.Average(x => x.invoice.TotalAmount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .Take(count)
            .ToListAsync(ct);
    }

    private async Task<List<AnalyticsDto>> GetTopVendorsFromWarehouseAsync(
        DateTime startDate,
        DateTime endDate,
        int count,
        CancellationToken ct)
    {
        return await _warehouseContext!.FactInvoices
            .Where(f => f.InvoiceType == InvoiceType.Purchase.ToStringValue()
                        && f.VendorKey > 0
                        && f.InvoiceDate >= startDate
                        && f.InvoiceDate <= endDate)
            .Join(_warehouseContext.DimVendors,
                  invoice => invoice.VendorKey,
                  vendor => vendor.VendorKey,
                  (invoice, vendor) => new { invoice, vendor })
            .GroupBy(x => new { x.vendor.VendorKey, x.vendor.Name })
            .Select(g => new AnalyticsDto
            {
                VendorKey = g.Key.VendorKey,
                VendorName = g.Key.Name,
                TotalAmount = g.Sum(x => x.invoice.TotalAmount),
                Count = g.Count(),
                AverageInvoice = g.Average(x => x.invoice.TotalAmount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .Take(count)
            .ToListAsync(ct);
    }

private async Task<List<AnalyticsDto>> GetTopCustomersFromMainContextAsync(
    DateTime startDate,
    DateTime endDate,
    int count,
    CancellationToken ct)
{
    // ✅ Try cache first
    var cacheKey = $"top_customers_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}_{count}";
    var cached = await _cache.GetAsync<List<AnalyticsDto>>(cacheKey);
    if (cached is not null)
    {
        return cached;
    }

    // ✅ Use raw SQL with proper casting
    var sql = @"
        SELECT
            c.""Id""::text as CustomerId,
            c.""Name"" as CustomerName,
            COALESCE(c.""CustomerKey"", 0) as CustomerKey,
            '' as VendorId,
            '' as VendorName,
            0 as VendorKey,
            COALESCE(SUM(i.""TotalAmount""), 0) as TotalAmount,
            COUNT(i.""Id"") as Count,
            COALESCE(AVG(i.""TotalAmount""), 0) as AverageInvoice
        FROM ""Invoices"" i
        INNER JOIN ""Customers"" c ON i.""CustomerId"" = c.""Id""
        WHERE i.""InvoiceType"" = 'Sales'
            AND i.""IsDeleted"" = false
            AND c.""IsDeleted"" = false
            AND i.""InvoiceDate"" >= @startDate
            AND i.""InvoiceDate"" <= @endDate
        GROUP BY c.""Id"", c.""Name"", c.""CustomerKey""
        ORDER BY TotalAmount DESC
        LIMIT @count
    ";

    var result = await _context.Database
        .SqlQueryRaw<AnalyticsDto>(sql,
            new NpgsqlParameter("@startDate", startDate),
            new NpgsqlParameter("@endDate", endDate),
            new NpgsqlParameter("@count", count))
        .ToListAsync(ct);

    // ✅ Cache for 5 minutes
    await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

    return result;
}

private async Task<List<AnalyticsDto>> GetTopVendorsFromMainContextAsync(
    DateTime startDate,
    DateTime endDate,
    int count,
    CancellationToken ct)
{
    // ✅ Try cache first
    var cacheKey = $"top_vendors_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}_{count}";
    var cached = await _cache.GetAsync<List<AnalyticsDto>>(cacheKey);
    if (cached is not null)
    {
        return cached;
    }

    // ✅ Use raw SQL with proper casting
    var sql = @"
        SELECT
            '' as CustomerId,
            '' as CustomerName,
            0 as CustomerKey,
            v.""Id""::text as VendorId,
            v.""Name"" as VendorName,
            0 as VendorKey,
            COALESCE(SUM(i.""TotalAmount""), 0) as TotalAmount,
            COUNT(i.""Id"") as Count,
            COALESCE(AVG(i.""TotalAmount""), 0) as AverageInvoice
        FROM ""Invoices"" i
        INNER JOIN ""Vendors"" v ON i.""VendorId"" = v.""Id""
        WHERE i.""InvoiceType"" = 'Purchase'
            AND i.""IsDeleted"" = false
            AND v.""IsDeleted"" = false
            AND i.""InvoiceDate"" >= @startDate
            AND i.""InvoiceDate"" <= @endDate
        GROUP BY v.""Id"", v.""Name""
        ORDER BY TotalAmount DESC
        LIMIT @count
    ";

    var result = await _context.Database
        .SqlQueryRaw<AnalyticsDto>(sql,
            new NpgsqlParameter("@startDate", startDate),
            new NpgsqlParameter("@endDate", endDate),
            new NpgsqlParameter("@count", count))
        .ToListAsync(ct);

    // ✅ Cache for 5 minutes
    await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

    return result;
}

    // ============================================================
    // CACHE INVALIDATION & WARMUP
    // ============================================================

    public async Task InvalidateAnalyticsCacheAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        string? periodType = null,
        string? fiscalYear = null,
        CancellationToken ct = default)
    {
        try
        {
            if (periodStart.HasValue && periodEnd.HasValue && !string.IsNullOrEmpty(periodType))
            {
                var cacheKey = BuildAnalyticsCacheKey(periodStart, periodEnd, periodType, fiscalYear);
                await _cache.RemoveAsync(cacheKey);
                _logger.LogInformation("🗑️ Invalidated analytics cache: {CacheKey}", cacheKey);
            }
            else
            {
                await _cache.RemoveByPatternAsync(CACHE_PREFIX);
                _logger.LogInformation("🗑️ Invalidated ALL analytics caches");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error invalidating analytics cache");
        }
    }

    public async Task WarmupAnalyticsCacheAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("🔥 Warming up analytics cache...");

            var periods = new[]
            {
                ("day", DateTime.UtcNow.AddDays(-7), DateTime.UtcNow),
                ("week", DateTime.UtcNow.AddDays(-30), DateTime.UtcNow),
                ("month", DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow),
                ("quarter", DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow),
                ("year", DateTime.UtcNow.AddYears(-1), DateTime.UtcNow),
            };

            foreach (var (periodType, start, end) in periods)
            {
                if (!ct.IsCancellationRequested)
                {
                    _logger.LogDebug("🔥 Warming up {PeriodType} analytics...", periodType);
                    await GetAnalyticsDashboardAsync(start, end, periodType, null, ct);
                }
            }

            _logger.LogInformation("✅ Analytics cache warmed up successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error warming up analytics cache");
        }
    }

    // ============================================================
    // TREND DATA HELPERS
    // ============================================================

    public async Task<List<TrendDataDto>> GetRevenueTrendAsync(
        int months = 12,
        DateTime? endDate = null,
        CancellationToken ct = default)
    {
        var end = endDate ?? DateTime.UtcNow.Date;
        var start = end.AddMonths(-months);

        return await GetTrendDataAsync(
            InvoiceType.Sales,
            "analytics:revenue:trend",
            start,
            end,
            ct
        );
    }

    public async Task<List<TrendDataDto>> GetPurchaseTrendAsync(
        int months = 12,
        DateTime? endDate = null,
        CancellationToken ct = default)
    {
        var end = endDate ?? DateTime.UtcNow.Date;
        var start = end.AddMonths(-months);

        return await GetTrendDataAsync(
            InvoiceType.Purchase,
            "analytics:purchase:trend",
            start,
            end,
            ct
        );
    }

    private async Task<List<TrendDataDto>> GetTrendDataAsync(
        InvoiceType invoiceType,
        string baseCacheKey,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct)
    {
        try
        {
            var cacheKey = $"{baseCacheKey}:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";

            var cached = await _cache.GetAsync<List<TrendDataDto>>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {InvoiceType} Trend", invoiceType);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {InvoiceType} Trend", invoiceType);

            var rawData = await GetRawDataAsync(invoiceType, startDate, endDate, ct);

            var trendData = rawData
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Select(x => new TrendDataDto
                {
                    Period = $"{x.Year}-{x.Month:D2}",
                    Revenue = invoiceType == InvoiceType.Sales ? x.Amount : 0,
                    Expense = invoiceType == InvoiceType.Purchase ? x.Amount : 0,
                    Count = x.Count,
                    AverageInvoice = x.Count > 0 ? x.Amount / x.Count : 0
                })
                .ToList();

            await _cache.SetAsync(cacheKey, trendData, CacheDurationLong);

            return trendData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving {InvoiceType} trend", invoiceType);
            throw;
        }
    }

    private async Task<List<RawDataDto>> GetRawDataAsync(
        InvoiceType invoiceType,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct)
    {
        var typeString = invoiceType.ToStringValue();

        if (_useWarehouse && _warehouseContext != null)
        {
            return await _warehouseContext.FactInvoices
                .Where(f => f.InvoiceType == typeString && f.InvoiceDate >= startDate && f.InvoiceDate <= endDate)
                .GroupBy(f => new { f.InvoiceDate.Year, f.InvoiceDate.Month })
                .Select(g => new RawDataDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Amount = g.Sum(x => x.TotalAmount),
                    Count = g.Count()
                })
                .ToListAsync(ct);
        }

        return await _context.Invoices
            .Where(i => !i.IsDeleted && i.InvoiceType == typeString && i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
            .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
            .Select(g => new RawDataDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Amount = g.Sum(x => x.TotalAmount),
                Count = g.Count()
            })
            .ToListAsync(ct);
    }

    // ============================================================
    // MONTHLY DATA HELPERS
    // ============================================================

    public async Task<Dictionary<string, decimal>> GetMonthlyRevenueAsync(
        int months = 12,
        DateTime? endDate = null,
        CancellationToken ct = default)
    {
        var end = endDate ?? DateTime.UtcNow.Date;
        var start = end.AddMonths(-months);

        return await GetMonthlyDataAsync(
            InvoiceType.Sales,
            "analytics:revenue:monthly",
            start,
            end,
            ct
        );
    }

    public async Task<Dictionary<string, decimal>> GetMonthlyPurchaseExpenseAsync(
        int months = 12,
        DateTime? endDate = null,
        CancellationToken ct = default)
    {
        var end = endDate ?? DateTime.UtcNow.Date;
        var start = end.AddMonths(-months);

        return await GetMonthlyDataAsync(
            InvoiceType.Purchase,
            "analytics:purchase:monthly",
            start,
            end,
            ct
        );
    }

    private async Task<Dictionary<string, decimal>> GetMonthlyDataAsync(
        InvoiceType invoiceType,
        string baseCacheKey,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct)
    {
        try
        {
            var cacheKey = $"{baseCacheKey}:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";

            var cached = await _cache.GetAsync<Dictionary<string, decimal>>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {InvoiceType} Monthly Data", invoiceType);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {InvoiceType} Monthly Data", invoiceType);

            var typeString = invoiceType.ToStringValue();

            var data = await _context.Invoices
                .Where(i => !i.IsDeleted && i.InvoiceType == typeString && i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                .Select(g => new
                {
                    Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Total = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(g => g.Period)
                .ToDictionaryAsync(g => g.Period, g => g.Total, ct);

            await _cache.SetAsync(cacheKey, data, CacheDurationLong);

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving {InvoiceType} monthly data", invoiceType);
            throw;
        }
    }

    // ============================================================
    // INVOICE STATUS SUMMARY HELPERS
    // ============================================================

    public async Task<List<InvoiceStatusSummaryDto>> GetSalesInvoiceStatusSummaryAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default)
    {
        return await GetInvoiceStatusSummaryAsync(
            InvoiceType.Sales,
            "analytics:invoices:sales:status",
            periodStart,
            periodEnd,
            ct
        );
    }

    public async Task<List<InvoiceStatusSummaryDto>> GetPurchaseInvoiceStatusSummaryAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default)
    {
        return await GetInvoiceStatusSummaryAsync(
            InvoiceType.Purchase,
            "analytics:invoices:purchase:status",
            periodStart,
            periodEnd,
            ct
        );
    }

    public async Task<List<InvoiceStatusSummaryDto>> GetInvoiceStatusSummaryAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default)
    {
        try
        {
            var endDate = periodEnd ?? DateTime.UtcNow.Date;
            var startDate = periodStart ?? new DateTime(endDate.Year, endDate.Month, 1);

            var cacheKey = $"analytics:invoices:all:status:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";

            var cached = await _cache.GetAsync<List<InvoiceStatusSummaryDto>>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: All Invoice Status Summary");
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: All Invoice Status Summary");

            var totalInvoices = await _context.Invoices
                .Where(i => !i.IsDeleted && i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .CountAsync(ct);

            var summary = await _context.Invoices
                .Where(i => !i.IsDeleted && i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .GroupBy(i => new { i.Status, i.InvoiceType })
                .Select(g => new InvoiceStatusSummaryDto
                {
                    InvoiceType = g.Key.InvoiceType ?? "Unknown",
                    Status = g.Key.Status ?? "Unknown",
                    Count = g.Count(),
                    TotalAmount = g.Sum(x => x.TotalAmount),
                    Percentage = totalInvoices > 0 ? ((double)g.Count() / totalInvoices) * 100 : 0
                })
                .OrderByDescending(s => s.TotalAmount)
                .ToListAsync(ct);

            await _cache.SetAsync(cacheKey, summary, CacheDurationMedium);

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoice status summary");
            throw;
        }
    }

    private async Task<List<InvoiceStatusSummaryDto>> GetInvoiceStatusSummaryAsync(
        InvoiceType invoiceType,
        string baseCacheKey,
        DateTime? periodStart,
        DateTime? periodEnd,
        CancellationToken ct)
    {
        try
        {
            var endDate = periodEnd ?? DateTime.UtcNow.Date;
            var startDate = periodStart ?? new DateTime(endDate.Year, endDate.Month, 1);

            var cacheKey = $"{baseCacheKey}:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";

            var cached = await _cache.GetAsync<List<InvoiceStatusSummaryDto>>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {InvoiceType} Invoice Status Summary", invoiceType);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {InvoiceType} Invoice Status Summary", invoiceType);

            var typeString = invoiceType.ToStringValue();
            var invoicesQuery = _context.Invoices
                .Where(i => !i.IsDeleted
                            && i.InvoiceType == typeString
                            && i.InvoiceDate >= startDate
                            && i.InvoiceDate <= endDate);

            var totalCount = await invoicesQuery.CountAsync(ct);

            var summary = await invoicesQuery
                .GroupBy(i => i.Status)
                .Select(g => new InvoiceStatusSummaryDto
                {
                    InvoiceType = typeString,
                    Status = g.Key ?? "Unknown",
                    Count = g.Count(),
                    TotalAmount = g.Sum(x => x.TotalAmount),
                    Percentage = totalCount > 0 ? ((double)g.Count() / totalCount) * 100 : 0
                })
                .OrderByDescending(s => s.TotalAmount)
                .ToListAsync(ct);

            await _cache.SetAsync(cacheKey, summary, CacheDurationMedium);

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving {InvoiceType} invoice status summary", invoiceType);
            throw;
        }
    }

    // ============================================================
    // TOP CUSTOMERS & VENDORS (PUBLIC)
    // ============================================================

    public async Task<TopCustomersDto> GetTopCustomersAsync(
        int count = 10,
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default)
    {
        try
        {
            var endDate = periodEnd ?? DateTime.UtcNow.Date;
            var startDate = periodStart ?? new DateTime(endDate.Year, endDate.Month, 1);

            var cacheKey = $"analytics:customers:top:{count}:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";

            var cached = await _cache.GetAsync<TopCustomersDto>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: Top Customers");
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: Top Customers");

            var typeString = InvoiceType.Sales.ToStringValue();
            var topCustomers = await _context.Invoices
                .Where(i => !i.IsDeleted
                            && i.InvoiceType == typeString
                            && i.CustomerId.HasValue
                            && i.InvoiceDate >= startDate
                            && i.InvoiceDate <= endDate)
                .Join(_context.Customers,
                      invoice => invoice.CustomerId,
                      customer => customer.Id,
                      (invoice, customer) => new { invoice, customer })
                .GroupBy(x => new { x.customer.Id, x.customer.Name })
                .Select(g => new CustomerSummaryDto
                {
                    CustomerId = g.Key.Id,
                    CustomerName = g.Key.Name,
                    TotalAmount = g.Sum(x => x.invoice.TotalAmount),
                    Count = g.Count(),
                    AverageInvoice = g.Average(x => x.invoice.TotalAmount)
                })
                .OrderByDescending(c => c.TotalAmount)
                .Take(count)
                .ToListAsync(ct);

            var result = new TopCustomersDto
            {
                Customers = topCustomers,
                TotalCustomers = await _context.Invoices
                    .Where(i => !i.IsDeleted
                                && i.InvoiceType == typeString
                                && i.CustomerId.HasValue
                                && i.InvoiceDate >= startDate
                                && i.InvoiceDate <= endDate)
                    .Select(i => i.CustomerId!.Value)
                    .Distinct()
                    .CountAsync(ct),
                DateGenerated = DateTime.UtcNow
            };

            await _cache.SetAsync(cacheKey, result, CacheDurationLong);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving top customers");
            throw;
        }
    }

    public async Task<TopVendorsDto> GetTopVendorsAsync(
        int count = 10,
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default)
    {
        try
        {
            var endDate = periodEnd ?? DateTime.UtcNow.Date;
            var startDate = periodStart ?? new DateTime(endDate.Year, endDate.Month, 1);

            var cacheKey = $"analytics:vendors:top:{count}:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";

            var cached = await _cache.GetAsync<TopVendorsDto>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: Top Vendors");
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: Top Vendors");

            var typeString = InvoiceType.Purchase.ToStringValue();
            var topVendors = await _context.Invoices
                .Where(i => !i.IsDeleted
                            && i.InvoiceType == typeString
                            && i.VendorId.HasValue
                            && i.InvoiceDate >= startDate
                            && i.InvoiceDate <= endDate)
                .Join(_context.Vendors,
                      invoice => invoice.VendorId,
                      vendor => vendor.Id,
                      (invoice, vendor) => new { invoice, vendor })
                .GroupBy(x => new { x.vendor.Id, x.vendor.Name })
                .Select(g => new VendorSummaryDto
                {
                    VendorId = g.Key.Id,
                    VendorName = g.Key.Name,
                    TotalAmount = g.Sum(x => x.invoice.TotalAmount),
                    Count = g.Count(),
                    AverageInvoice = g.Average(x => x.invoice.TotalAmount)
                })
                .OrderByDescending(v => v.TotalAmount)
                .Take(count)
                .ToListAsync(ct);

            var result = new TopVendorsDto
            {
                Vendors = topVendors,
                TotalVendors = await _context.Invoices
                    .Where(i => !i.IsDeleted
                                && i.InvoiceType == typeString
                                && i.VendorId.HasValue
                                && i.InvoiceDate >= startDate
                                && i.InvoiceDate <= endDate)
                    .Select(i => i.VendorId!.Value)
                    .Distinct()
                    .CountAsync(ct),
                DateGenerated = DateTime.UtcNow
            };

            await _cache.SetAsync(cacheKey, result, CacheDurationLong);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving top vendors");
            throw;
        }
    }

    // ============================================================
    // PAYMENT & EXPENSE ANALYTICS
    // ============================================================

    public async Task<PaymentAnalyticsDto> GetPaymentAnalyticsAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default)
    {
        try
        {
            var endDate = periodEnd ?? DateTime.UtcNow.Date;
            var startDate = periodStart ?? new DateTime(endDate.Year, endDate.Month, 1);

            var cacheKey = $"analytics:payments:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";

            var cached = await _cache.GetAsync<PaymentAnalyticsDto>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: Payment Analytics");
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: Payment Analytics");

            var payments = await _context.Payments
                .Where(p => !p.IsDeleted && p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .ToListAsync(ct);

            var analytics = new PaymentAnalyticsDto
            {
                TotalPayments = payments.Count,
                TotalAmount = payments.Sum(p => p.Amount),
                AveragePayment = payments.Any() ? payments.Average(p => p.Amount) : 0,
                VendorPayments = payments.Where(p => p.PaymentType == InvoiceType.Purchase.ToStringValue()).Sum(p => p.Amount),
                CustomerPayments = payments.Where(p => p.PaymentType == InvoiceType.Sales.ToStringValue()).Sum(p => p.Amount),
                PaymentsByMethod = payments
                    .GroupBy(p => p.PaymentMethod)
                    .Select(g => new PaymentMethodSummaryDto
                    {
                        Method = g.Key ?? "Unknown",
                        Count = g.Count(),
                        TotalAmount = g.Sum(p => p.Amount)
                    }).ToList()
            };

            await _cache.SetAsync(cacheKey, analytics, CacheDurationMedium);

            return analytics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving payment analytics");
            throw;
        }
    }

    public async Task<ExpenseAnalyticsDto> GetExpenseAnalyticsAsync(
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        CancellationToken ct = default)
    {
        try
        {
            var endDate = periodEnd ?? DateTime.UtcNow.Date;
            var startDate = periodStart ?? new DateTime(endDate.Year, endDate.Month, 1);

            var cacheKey = $"analytics:expenses:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";

            var cached = await _cache.GetAsync<ExpenseAnalyticsDto>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: Expense Analytics");
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: Expense Analytics");

            // Supplier Bills (Purchase Invoices)
            var purchaseInvoices = await _context.Invoices
                .Where(i => !i.IsDeleted && i.InvoiceType == InvoiceType.Purchase.ToStringValue() && i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .Join(_context.Vendors,
                      invoice => invoice.VendorId,
                      vendor => vendor.Id,
                      (invoice, vendor) => new
                      {
                          invoice.Id,
                          invoice.InvoiceNumber,
                          invoice.InvoiceDate,
                          invoice.TotalAmount,
                          invoice.Description,
                          VendorName = vendor.Name
                      })
                .ToListAsync(ct);

            // Operating Expenses
            var operatingExpenses = await _context.Expenses
                .Where(e => !e.IsDeleted && e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
                .Include(e => e.ExpenseCategory)
                .ToListAsync(ct);

            var totalExpenseItems = purchaseInvoices.Count + operatingExpenses.Count;
            var totalExpenseAmount = purchaseInvoices.Sum(i => i.TotalAmount) + operatingExpenses.Sum(e => e.Amount);

            var categoryGroups = new Dictionary<string, decimal>();
            var categoryCounts = new Dictionary<string, int>();

            foreach (var invoice in purchaseInvoices)
            {
                var category = !string.IsNullOrEmpty(invoice.VendorName)
                    ? $"Supplier: {invoice.VendorName}"
                    : "Supplier Bills";

                if (categoryGroups.ContainsKey(category))
                    categoryGroups[category] += invoice.TotalAmount;
                else
                    categoryGroups[category] = invoice.TotalAmount;

                if (categoryCounts.ContainsKey(category))
                    categoryCounts[category]++;
                else
                    categoryCounts[category] = 1;
            }

            foreach (var expense in operatingExpenses)
            {
                var category = expense.ExpenseCategory != null
                    ? expense.ExpenseCategory.Name
                    : "Uncategorized";

                if (categoryGroups.ContainsKey(category))
                    categoryGroups[category] += expense.Amount;
                else
                    categoryGroups[category] = expense.Amount;

                if (categoryCounts.ContainsKey(category))
                    categoryCounts[category]++;
                else
                    categoryCounts[category] = 1;
            }

            var analytics = new ExpenseAnalyticsDto
            {
                TotalExpenses = totalExpenseItems,
                TotalAmount = totalExpenseAmount,
                AverageExpense = totalExpenseItems > 0 ? totalExpenseAmount / totalExpenseItems : 0,
                ExpensesByCategory = categoryGroups
                    .Select(g => new ExpenseCategorySummaryDto
                    {
                        Category = g.Key,
                        Count = categoryCounts.GetValueOrDefault(g.Key, 0),
                        TotalAmount = g.Value
                    })
                    .OrderByDescending(x => x.TotalAmount)
                    .ToList(),
                SupplierBillCount = purchaseInvoices.Count,
                SupplierBillAmount = purchaseInvoices.Sum(i => i.TotalAmount),
                OperatingExpenseCount = operatingExpenses.Count,
                OperatingExpenseAmount = operatingExpenses.Sum(e => e.Amount)
            };

            await _cache.SetAsync(cacheKey, analytics, CacheDurationMedium);

            return analytics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving expense analytics");
            throw;
        }
    }

    // ============================================================
    // BUDGET ANALYTICS
    // ============================================================

   public async Task<BudgetAnalyticsDto> GetBudgetAnalyticsAsync(
       DateTime? periodStart = null,
       DateTime? periodEnd = null,
       CancellationToken ct = default)
   {
       try
       {
           var endDate = periodEnd ?? DateTime.UtcNow.Date;
           var startDate = periodStart ?? new DateTime(endDate.Year, endDate.Month, 1);

           var cacheKey = $"analytics:budgets:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";

           var cached = await _cache.GetAsync<BudgetAnalyticsDto>(cacheKey);
           if (cached is not null)
           {
               _logger.LogDebug("📦 Cache HIT: Budget Analytics");
               return cached;
           }

           _logger.LogDebug("📦 Cache MISS: Budget Analytics");

           // ✅ Fix: Select only what you need, avoiding the null BudgetCodeId issue
           var budgetQuery = _context.Budgets
               .Where(b => !b.IsDeleted && b.Status == "Active");

           // ✅ Get basic budget data without loading related entities that might have null issues
           var budgets = await budgetQuery
               .Select(b => new
               {
                   b.Id,
                   b.TotalAmount,
                   b.Status,
                   b.Name,
                   Lines = b.Lines.Where(l => !l.IsDeleted).Select(l => new
                   {
                       l.Id,
                       l.SpentAmount,
                       l.PeriodId,
                       l.AllocatedAmount
                   }).ToList()
               })
               .ToListAsync(ct);

           // ✅ Calculate totals in memory
           var totalBudget = budgets.Sum(b => b.TotalAmount);

           var period = await _context.FinancialPeriods
               .FirstOrDefaultAsync(p => p.StartDate <= endDate && p.EndDate >= startDate, ct);

           decimal totalSpent;
           if (period != null)
           {
               totalSpent = budgets.Sum(b => b.Lines
                   .Where(l => l.PeriodId == period.Id)
                   .Sum(l => l.SpentAmount));
           }
           else
           {
               totalSpent = budgets.Sum(b => b.Lines.Sum(l => l.SpentAmount));
           }

           var analytics = new BudgetAnalyticsDto
           {
               TotalBudgets = budgets.Count,
               TotalBudgetAmount = totalBudget,
               TotalSpent = totalSpent,
               OverallUtilization = totalBudget > 0 ? (totalSpent / totalBudget) * 100 : 0,
               DateGenerated = DateTime.UtcNow,
               PeriodStart = startDate.ToString("yyyy-MM-dd"),
               PeriodEnd = endDate.ToString("yyyy-MM-dd")
           };

           await _cache.SetAsync(cacheKey, analytics, CacheDurationMedium);

           return analytics;
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "❌ Error retrieving budget analytics");
           throw;
       }
   }

    // ============================================================
    // AGING REPORT (PUBLIC)
    // ============================================================

    public async Task<Cor.Finance.Models.Analytics.AgingReportDto> GetAgingReportAsync(
        DateTime? asOfDate = null,
        CancellationToken ct = default)
    {
        try
        {
            var asOf = asOfDate ?? DateTime.UtcNow.Date;
            var cacheKey = $"analytics:aging:{asOf:yyyyMMdd}";

            var cached = await _cache.GetAsync<Cor.Finance.Models.Analytics.AgingReportDto>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: Aging Report");
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: Aging Report");

            Cor.Finance.Models.Analytics.AgingReportDto report;

            if (_useWarehouse)
            {
                report = await _warehouseContext!.FactInvoices
                    .Where(f => f.InvoiceType == InvoiceType.Sales.ToStringValue() && f.Status != "Paid" && f.BalanceAmount > 0)
                    .GroupBy(f => 1)
                    .Select(g => new Cor.Finance.Models.Analytics.AgingReportDto
                    {
                        Period = asOf.ToString("yyyy-MM-dd"),
                        Current = g.Where(x => x.DaysOverdue <= 0).Sum(x => x.BalanceAmount),
                        Days30_60 = g.Where(x => x.DaysOverdue > 0 && x.DaysOverdue <= 30).Sum(x => x.BalanceAmount),
                        Days60_90 = g.Where(x => x.DaysOverdue > 30 && x.DaysOverdue <= 60).Sum(x => x.BalanceAmount),
                        Days90Plus = g.Where(x => x.DaysOverdue > 60).Sum(x => x.BalanceAmount),
                        Total = g.Sum(x => x.BalanceAmount),
                        Details = g.Select(x => new Cor.Finance.Models.Analytics.AgingDetailDto
                        {
                            InvoiceId = Guid.NewGuid(),
                            InvoiceNumber = x.InvoiceNumber ?? "",
                            InvoiceDate = x.InvoiceDate,
                            DueDate = x.DueDate,
                            Amount = x.BalanceAmount,
                            DaysOverdue = x.DaysOverdue,
                            AgingBucket = GetAgingBucket(x.DaysOverdue)
                        }).ToList()
                    })
                    .FirstOrDefaultAsync(ct) ?? new Cor.Finance.Models.Analytics.AgingReportDto();
            }
            else
            {
                var invoices = await _context.Invoices
                    .Where(i => !i.IsDeleted && i.InvoiceType == InvoiceType.Sales.ToStringValue() && i.Status != "Paid" && (i.TotalAmount - i.PaidAmount) > 0)
                    .Select(i => new
                    {
                        i.Id,
                        i.InvoiceNumber,
                        i.InvoiceDate,
                        i.DueDate,
                        i.TotalAmount,
                        i.PaidAmount,
                        Balance = i.TotalAmount - i.PaidAmount,
                        DaysOverdue = i.DueDate.HasValue ? (asOf - i.DueDate.Value).Days : 0
                    })
                    .ToListAsync(ct);

                report = new Cor.Finance.Models.Analytics.AgingReportDto
                {
                    Period = asOf.ToString("yyyy-MM-dd"),
                    Details = new List<Cor.Finance.Models.Analytics.AgingDetailDto>()
                };

                foreach (var invoice in invoices)
                {
                    var daysOverdue = Math.Max(0, invoice.DaysOverdue);
                    var bucket = GetAgingBucket(daysOverdue);

                    report.Details.Add(new Cor.Finance.Models.Analytics.AgingDetailDto
                    {
                        InvoiceId = invoice.Id,
                        InvoiceNumber = invoice.InvoiceNumber,
                        InvoiceDate = invoice.InvoiceDate,
                        DueDate = invoice.DueDate ?? invoice.InvoiceDate,
                        Amount = invoice.Balance,
                        DaysOverdue = daysOverdue,
                        AgingBucket = bucket
                    });

                    switch (bucket)
                    {
                        case "Current": report.Current += invoice.Balance; break;
                        case "30-60": report.Days30_60 += invoice.Balance; break;
                        case "60-90": report.Days60_90 += invoice.Balance; break;
                        case "90+": report.Days90Plus += invoice.Balance; break;
                    }
                    report.Total += invoice.Balance;
                }
            }

            await _cache.SetAsync(cacheKey, report, CacheDurationLong);

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving aging report");
            throw;
        }
    }

    // ============================================================
    // PRIVATE HELPERS
    // ============================================================

    private string GetAgingBucket(int daysOverdue)
    {
        if (daysOverdue <= 0) return "Current";
        if (daysOverdue <= 30) return "30-60";
        if (daysOverdue <= 60) return "60-90";
        return "90+";
    }


    // Add this class to AnalyticsService.cs

    /// <summary>
    /// Performance timer for tracking execution time of operations
    /// </summary>
    public class OperationTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly ILogger _logger;
        private readonly string _operationName;
        private readonly long _thresholdMs;

        public OperationTimer(ILogger logger, string operationName, long thresholdMs = 100)
        {
            _logger = logger;
            _operationName = operationName;
            _thresholdMs = thresholdMs;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            var elapsed = _stopwatch.ElapsedMilliseconds;

            if (elapsed > _thresholdMs)
            {
                _logger.LogWarning($"⏱️ SLOW: {_operationName} took {elapsed}ms (threshold: {_thresholdMs}ms)");
            }
            else
            {
                _logger.LogDebug($"⏱️ {_operationName} took {elapsed}ms");
            }
        }
    }

    /// <summary>
    /// Detailed performance tracker for dashboard generation
    /// </summary>
    public class DashboardPerformanceTracker
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, long> _timings = new();
        private readonly Stopwatch _totalStopwatch;
        private string _currentOperation = "";

        public DashboardPerformanceTracker(ILogger logger)
        {
            _logger = logger;
            _totalStopwatch = Stopwatch.StartNew();
        }

        public IDisposable StartOperation(string operationName)
        {
            _currentOperation = operationName;
            return new OperationTimer(_logger, operationName, 50);
        }

        public void RecordTiming(string operationName, long elapsedMs)
        {
            _timings[operationName] = elapsedMs;
            if (elapsedMs > 100)
            {
                _logger.LogWarning($"⏱️ SLOW: {operationName} took {elapsedMs}ms");
            }
        }

        public void LogSummary(string context = "")
        {
            _totalStopwatch.Stop();
            var totalMs = _totalStopwatch.ElapsedMilliseconds;

            _logger.LogInformation($"📊 {context} Performance Summary - Total: {totalMs}ms");

            // Sort by time (slowest first)
            foreach (var timing in _timings.OrderByDescending(x => x.Value))
            {
                var percentage = totalMs > 0 ? (timing.Value / (double)totalMs) * 100 : 0;
                var bar = new string('█', (int)(percentage / 5));
                _logger.LogInformation($"  {timing.Key,-40} {timing.Value,6}ms  {percentage,5:F1}%  {bar}");
            }
        }
    }
}
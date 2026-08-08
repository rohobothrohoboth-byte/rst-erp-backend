// Cor.CRM/Queries/SalesForecastQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.CRM.Queries;

public class SalesForecastQry : IRequest<SalesForecastDto>
{
    public string? Period { get; set; } = "quarter";
}

public class SalesForecastHandler : IRequestHandler<SalesForecastQry, SalesForecastDto>
{
    private readonly IDapperHelper _dapper;
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public SalesForecastHandler(IDapperHelper dapper, IUnitOfWork uow, ILogService logger)
    {
        _dapper = dapper;
        _uow = uow;
        _logger = logger;
    }

public async Task<SalesForecastDto> Handle(SalesForecastQry request, CancellationToken ct)
{
    try
    {
        var forecast = new SalesForecastDto();

        // Determine date range based on period
        var (startDate, endDate) = GetDateRange(request.Period);

        // 1. Get opportunities by stage (pipeline)
        // ✅ FIX: Use integer values for stage comparison
        var stageSql = @"
            SELECT
                o.""Stage"" as Stage,
                COALESCE(SUM(o.""Amount""), 0) as Amount,
                COUNT(o.""Id"") as Count,
                COALESCE(AVG(o.""WinProbability""), 0) as Probability
            FROM ""Opportunities"" o
            WHERE o.""IsDeleted"" = false
                AND o.""Stage"" NOT IN (5, 6)  -- Exclude ClosedWon (5) and ClosedLost (6)
                AND o.""CreatedAt"" >= @StartDate
                AND o.""CreatedAt"" <= @EndDate
            GROUP BY o.""Stage""
            ORDER BY o.""Stage""
        ";

        var stageParams = new DynamicParameters();
        stageParams.Add("@StartDate", startDate);
        stageParams.Add("@EndDate", endDate);

        var stages = await _dapper.QueryAsync<ForecastByStageDto>(stageSql, stageParams, ct);
        forecast.ByStage = stages.ToList();

        // 2. Calculate total forecast
        forecast.TotalForecast = forecast.ByStage.Sum(s => s.Amount);

        // 3. Get monthly trend
        var trendSql = @"
            SELECT
                TO_CHAR(o.""CreatedAt"", 'Mon') as Month,
                COALESCE(SUM(o.""Amount""), 0) as Amount
            FROM ""Opportunities"" o
            WHERE o.""IsDeleted"" = false
                AND o.""CreatedAt"" >= @StartDate
                AND o.""CreatedAt"" <= @EndDate
            GROUP BY TO_CHAR(o.""CreatedAt"", 'Mon'), EXTRACT(MONTH FROM o.""CreatedAt"")
            ORDER BY EXTRACT(MONTH FROM o.""CreatedAt"")
        ";

        var trendParams = new DynamicParameters();
        trendParams.Add("@StartDate", startDate);
        trendParams.Add("@EndDate", endDate);

        var trends = await _dapper.QueryAsync<MonthlyTrendDto>(trendSql, trendParams, ct);
        forecast.MonthlyTrend = trends.ToList();

        // 4. Get rep performance
        // ✅ FIX: Use integer value 5 for ClosedWon
        var repSql = @"
            SELECT
                COALESCE(e.""FirstName"" || ' ' || e.""LastName"", 'Unassigned') as RepName,
                COALESCE(SUM(o.""Amount""), 0) as Revenue,
                COUNT(o.""Id"") as Deals,
                COALESCE(SUM(o.""Amount"") * 1.2, 0) as Target,
                CASE
                    WHEN COALESCE(SUM(o.""Amount"") * 1.2, 0) > 0
                    THEN ROUND((COALESCE(SUM(o.""Amount""), 0) / (COALESCE(SUM(o.""Amount"") * 1.2, 0))) * 100)
                    ELSE 0
                END as Achievement
            FROM ""Opportunities"" o
            LEFT JOIN ""LocalEmployees"" e ON o.""AssignedToUserId"" = e.""AppUserId""
            WHERE o.""IsDeleted"" = false
                AND o.""Stage"" = 5  -- ClosedWon
                AND o.""CreatedAt"" >= @StartDate
                AND o.""CreatedAt"" <= @EndDate
            GROUP BY e.""Id"", e.""FirstName"", e.""LastName""
            ORDER BY Revenue DESC
            LIMIT 10
        ";

        var repParams = new DynamicParameters();
        repParams.Add("@StartDate", startDate);
        repParams.Add("@EndDate", endDate);

        var reps = await _dapper.QueryAsync<RepPerformanceDto>(repSql, repParams, ct);
        forecast.ByRep = reps.ToList();

        // 5. Calculate statistics using Dapper (for consistency and performance)
        // ✅ FIX: Use integer value 5 for ClosedWon
        var statsSql = @"
            SELECT
                COUNT(*) as TotalOpportunities,
                COUNT(CASE WHEN ""Stage"" = 5 THEN 1 END) as WonOpportunities,
                COALESCE(AVG(CASE WHEN ""Stage"" = 5 THEN ""Amount"" END), 0) as AverageDealSize,
                COALESCE(AVG(EXTRACT(DAY FROM (""ActualCloseDate"" - ""CreatedAt""))), 0) as AverageDaysToClose
            FROM ""Opportunities""
            WHERE ""IsDeleted"" = false
                AND ""CreatedAt"" >= @StartDate
                AND ""CreatedAt"" <= @EndDate
        ";

        var statsParams = new DynamicParameters();
        statsParams.Add("@StartDate", startDate);
        statsParams.Add("@EndDate", endDate);

        var stats = await _dapper.QuerySingleOrDefaultAsync<dynamic>(statsSql, statsParams, ct);

        if (stats != null)
        {
            var totalOpportunities = stats.TotalOpportunities ?? 0;
            var wonOpportunities = stats.WonOpportunities ?? 0;

            // Conversion Rate
            forecast.ConversionRate = totalOpportunities > 0
                ? (int)Math.Round((double)wonOpportunities / totalOpportunities * 100)
                : 0;

            // Average Deal Size
            forecast.AverageDealSize = stats.AverageDealSize ?? 0;

            // Pipeline Velocity
           var avgDays = stats.AverageDaysToClose ?? 0;
           forecast.PipelineVelocity = (int)Math.Round((double)avgDays);
        }

       _logger.LogInformation("Sales forecast calculated successfully for period: {Period}", request.Period ?? "Unknown");

        return forecast;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to calculate sales forecast");
        throw;
    }
}




    private (DateTime startDate, DateTime endDate) GetDateRange(string? period)
    {
        var now = DateTime.UtcNow;
        var startDate = now;
        var endDate = now;

        switch (period?.ToLower())
        {
            case "month":
                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
                break;
            case "quarter":
                var currentQuarter = (now.Month - 1) / 3;
                var quarterStartMonth = currentQuarter * 3 + 1;
                startDate = new DateTime(now.Year, quarterStartMonth, 1);
                endDate = startDate.AddMonths(3).AddDays(-1);
                break;
            case "year":
                startDate = new DateTime(now.Year, 1, 1);
                endDate = new DateTime(now.Year, 12, 31);
                break;
            default:
                startDate = now.AddMonths(-3);
                endDate = now;
                break;
        }

        return (startDate, endDate);
    }
}
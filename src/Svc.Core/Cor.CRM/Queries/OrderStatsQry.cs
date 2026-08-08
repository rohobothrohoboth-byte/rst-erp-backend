// Cor.CRM/Queries/OrderStatsQry.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.CRM.Queries;

public class OrderStatsQry : IRequest<OrderStatsDto> { }

public class OrderStatsHandler : IRequestHandler<OrderStatsQry, OrderStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public OrderStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<OrderStatsDto> Handle(OrderStatsQry request, CancellationToken ct)
    {
        try
        {
            var stats = new OrderStatsDto();

            var sql = @"
                SELECT
                    COUNT(*) as Total,
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as Draft,
                    COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as Processing,
                    COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as Shipped,
                    COUNT(CASE WHEN ""Status"" = 4 THEN 1 END) as Delivered,
                    COUNT(CASE WHEN ""Status"" = 7 THEN 1 END) as Cancelled,
                    COUNT(CASE WHEN ""Status"" = 4 OR ""Status"" = 5 THEN 1 END) as Completed,
                    COALESCE(SUM(""TotalAmount""), 0) as TotalValue,
                    COALESCE(AVG(""TotalAmount""), 0) as AverageOrderValue
                FROM ""Invoices""
                WHERE ""IsDeleted"" = false AND ""Type"" = 1
            ";

            var result = await _dapper.QueryFirstOrDefaultAsync<dynamic>(sql, new { }, ct);
            if (result != null)
            {
                stats.Total = result.Total ?? 0;
                stats.Draft = result.Draft ?? 0;
                stats.Processing = result.Processing ?? 0;
                stats.Shipped = result.Shipped ?? 0;
                stats.Delivered = result.Delivered ?? 0;
                stats.Cancelled = result.Cancelled ?? 0;
                stats.Completed = result.Completed ?? 0;
                stats.TotalValue = result.TotalValue ?? 0;
                stats.AverageOrderValue = result.AverageOrderValue ?? 0;

                // Calculate pending (Draft + Processing)
                stats.Pending = stats.Draft + stats.Processing;
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get order stats");
            throw;
        }
    }
}
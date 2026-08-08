// Cor.CRM/Queries/OrderQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Dapper;
using Helpers;
using MediatR;
using Cor.CRM.Models.Entities;
namespace Cor.CRM.Queries;

public class OrderByIdQry : IRequest<OrderDto?>
{
    public Guid Id { get; set; }
}

public class OrderAllQry : IRequest<List<OrderDto>>
{
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? QuoteId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class OrderByIdHandler : IRequestHandler<OrderByIdQry, OrderDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public OrderByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(OrderByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    o.""Id"", o.""OrderNumber"", o.""CustomerId"",
                    c.""Name"" as CustomerName,
                    o.""OpportunityId"", opp.""Name"" as OpportunityName,
                    o.""QuoteId"", q.""QuoteNumber"",
                    o.""OrderDate"", o.""DueDate"",
                    o.""SubTotal"", o.""TaxAmount"", o.""DiscountAmount"",
                    o.""ShippingCost"", o.""TotalAmount"",
                    o.""ShippingAddress"", o.""BillingAddress"",
                    o.""Terms"", o.""Notes"", o.""Currency"", o.""Status"",
                    o.""CreatedAt"", o.""UpdatedAt""
                FROM ""SalesOrders"" o
                LEFT JOIN ""Customers"" c ON o.""CustomerId"" = c.""Id""
                LEFT JOIN ""Opportunities"" opp ON o.""OpportunityId"" = opp.""Id""
                LEFT JOIN ""Quotes"" q ON o.""QuoteId"" = q.""Id""
                WHERE o.""Id"" = @Id AND o.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var order = await _dapper.QueryFirstOrDefaultAsync<OrderDto>(sql, parameters, ct);

            if (order != null)
            {
                // Get order lines
                var lineSql = @"
                    SELECT
                        ol.""Id"", ol.""Description"", ol.""Quantity"",
                        ol.""UnitPrice"", ol.""Discount"", ol.""TaxRate"",
                        ol.""TotalPrice"", ol.""SortOrder"",
                        ol.""ProductId"", p.""Name"" as ProductName,
                        ol.""Notes""
                    FROM ""OrderLines"" ol
                    LEFT JOIN ""Products"" p ON ol.""ProductId"" = p.""Id""
                    WHERE ol.""OrderId"" = @OrderId AND ol.""IsDeleted"" = false
                    ORDER BY ol.""SortOrder""
                ";

                var lineParams = new DynamicParameters();
                lineParams.Add("@OrderId", request.Id);

                var lines = await _dapper.QueryAsync<OrderLineDto>(lineSql, lineParams, ct);
                order.OrderLines = lines.ToList();
            }

            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get order by ID: {OrderId}", request.Id);
            throw;
        }
    }
}

public class OrderAllHandler : IRequestHandler<OrderAllQry, List<OrderDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public OrderAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<OrderDto>> Handle(OrderAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    o.""Id"", o.""OrderNumber"", o.""CustomerId"",
                    c.""Name"" as CustomerName,
                    o.""OpportunityId"", opp.""Name"" as OpportunityName,
                    o.""QuoteId"", q.""QuoteNumber"",
                    o.""OrderDate"", o.""DueDate"",
                    o.""SubTotal"", o.""TaxAmount"", o.""DiscountAmount"",
                    o.""ShippingCost"", o.""TotalAmount"",
                    o.""ShippingAddress"", o.""BillingAddress"",
                    o.""Terms"", o.""Notes"", o.""Currency"", o.""Status"",
                    o.""CreatedAt"", o.""UpdatedAt""
                FROM ""SalesOrders"" o
                LEFT JOIN ""Customers"" c ON o.""CustomerId"" = c.""Id""
                LEFT JOIN ""Opportunities"" opp ON o.""OpportunityId"" = opp.""Id""
                LEFT JOIN ""Quotes"" q ON o.""QuoteId"" = q.""Id""
                WHERE o.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (request.CustomerId.HasValue)
            {
                sql += " AND o.\"CustomerId\" = @CustomerId";
                parameters.Add("@CustomerId", request.CustomerId.Value);
            }

            if (request.OpportunityId.HasValue)
            {
                sql += " AND o.\"OpportunityId\" = @OpportunityId";
                parameters.Add("@OpportunityId", request.OpportunityId.Value);
            }

            if (request.QuoteId.HasValue)
            {
                sql += " AND o.\"QuoteId\" = @QuoteId";
                parameters.Add("@QuoteId", request.QuoteId.Value);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                sql += " AND o.\"Status\" = @Status";
                parameters.Add("@Status", Enum.Parse<OrderStatus>(request.Status));
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND o.\"OrderDate\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND o.\"OrderDate\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            sql += " ORDER BY o.\"OrderDate\" DESC";
            sql += $" OFFSET {(request.Page - 1) * request.PageSize} LIMIT {request.PageSize}";

            var orders = await _dapper.QueryAsync<OrderDto>(sql, parameters, ct);
            var orderList = orders.ToList();

            // Get order lines for each order
            if (orderList.Any())
            {
                foreach (var order in orderList)
                {
                    var lineSql = @"
                        SELECT
                            ol.""Id"", ol.""Description"", ol.""Quantity"",
                            ol.""UnitPrice"", ol.""Discount"", ol.""TaxRate"",
                            ol.""TotalPrice"", ol.""SortOrder"",
                            ol.""ProductId"", p.""Name"" as ProductName,
                            ol.""Notes""
                        FROM ""OrderLines"" ol
                        LEFT JOIN ""Products"" p ON ol.""ProductId"" = p.""Id""
                        WHERE ol.""OrderId"" = @OrderId AND ol.""IsDeleted"" = false
                        ORDER BY ol.""SortOrder""
                    ";

                    var lineParams = new DynamicParameters();
                    lineParams.Add("@OrderId", order.Id);

                    var lines = await _dapper.QueryAsync<OrderLineDto>(lineSql, lineParams, ct);
                    order.OrderLines = lines.ToList();
                }
            }

            return orderList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all orders");
            throw;
        }
    }
}
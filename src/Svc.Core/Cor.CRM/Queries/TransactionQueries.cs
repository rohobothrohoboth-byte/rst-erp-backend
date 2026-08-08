// Cor.CRM/Queries/TransactionQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.CRM.Queries;

public class TransactionByIdQry : IRequest<RealEstateTransactionDto?>
{
    public Guid Id { get; set; }
}

public class TransactionAllQry : IRequest<List<RealEstateTransactionDto>>
{
    public Guid? PropertyId { get; set; }
    public Guid? BuyerId { get; set; }
    public Guid? SellerId { get; set; }
    public int? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class TransactionStatsQry : IRequest<TransactionStatsDto>
{
    public Guid? PropertyId { get; set; }
    public Guid? AgentId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class TransactionByIdHandler : IRequestHandler<TransactionByIdQry, RealEstateTransactionDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public TransactionByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<RealEstateTransactionDto?> Handle(TransactionByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    t.*,
                    p.""Title"" as PropertyTitle,
                    p.""Address"" as PropertyAddress,
                    b.""Name"" as BuyerName,
                    s.""Name"" as SellerName,
                    ba.""FirstName"" || ' ' || ba.""LastName"" as BuyerAgentName,
                    sa.""FirstName"" || ' ' || sa.""LastName"" as SellerAgentName,
                    q.""QuoteNumber"",
                    o.""OrderNumber"",
                    c.""ContractNumber""
                FROM ""RealEstateTransactions"" t
                LEFT JOIN ""Property"" p ON t.""PropertyId"" = p.""Id""
                LEFT JOIN ""Customers"" b ON t.""BuyerId"" = b.""Id""
                LEFT JOIN ""Customers"" s ON t.""SellerId"" = s.""Id""
                LEFT JOIN ""LocalEmployees"" ba ON t.""BuyerAgentId"" = ba.""Id""
                LEFT JOIN ""LocalEmployees"" sa ON t.""SellerAgentId"" = sa.""Id""
                LEFT JOIN ""Quotes"" q ON t.""QuoteId"" = q.""Id""
                LEFT JOIN ""SalesOrders"" o ON t.""OrderId"" = o.""Id""
                LEFT JOIN ""Contracts"" c ON t.""ContractId"" = c.""Id""
                WHERE t.""Id"" = @Id AND t.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            return await _dapper.QueryFirstOrDefaultAsync<RealEstateTransactionDto>(sql, parameters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get transaction by ID: {TransactionId}", request.Id);
            throw;
        }
    }
}

public class TransactionAllHandler : IRequestHandler<TransactionAllQry, List<RealEstateTransactionDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public TransactionAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<RealEstateTransactionDto>> Handle(TransactionAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    t.*,
                    p.""Title"" as PropertyTitle,
                    p.""Address"" as PropertyAddress,
                    b.""Name"" as BuyerName,
                    s.""Name"" as SellerName,
                    ba.""FirstName"" || ' ' || ba.""LastName"" as BuyerAgentName,
                    sa.""FirstName"" || ' ' || sa.""LastName"" as SellerAgentName,
                    q.""QuoteNumber"",
                    o.""OrderNumber"",
                    c.""ContractNumber""
                FROM ""RealEstateTransactions"" t
                LEFT JOIN ""Property"" p ON t.""PropertyId"" = p.""Id""
                LEFT JOIN ""Customers"" b ON t.""BuyerId"" = b.""Id""
                LEFT JOIN ""Customers"" s ON t.""SellerId"" = s.""Id""
                LEFT JOIN ""LocalEmployees"" ba ON t.""BuyerAgentId"" = ba.""Id""
                LEFT JOIN ""LocalEmployees"" sa ON t.""SellerAgentId"" = sa.""Id""
                LEFT JOIN ""Quotes"" q ON t.""QuoteId"" = q.""Id""
                LEFT JOIN ""SalesOrders"" o ON t.""OrderId"" = o.""Id""
                LEFT JOIN ""Contracts"" c ON t.""ContractId"" = c.""Id""
                WHERE t.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (request.PropertyId.HasValue)
            {
                sql += " AND t.\"PropertyId\" = @PropertyId";
                parameters.Add("@PropertyId", request.PropertyId.Value);
            }

            if (request.BuyerId.HasValue)
            {
                sql += " AND t.\"BuyerId\" = @BuyerId";
                parameters.Add("@BuyerId", request.BuyerId.Value);
            }

            if (request.SellerId.HasValue)
            {
                sql += " AND t.\"SellerId\" = @SellerId";
                parameters.Add("@SellerId", request.SellerId.Value);
            }

            if (request.Status.HasValue)
            {
                sql += " AND t.\"Status\" = @Status";
                parameters.Add("@Status", request.Status.Value);
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND t.\"CreatedAt\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND t.\"CreatedAt\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            sql += " ORDER BY t.\"CreatedAt\" DESC";
            sql += $" OFFSET {(request.Page - 1) * request.PageSize} LIMIT {request.PageSize}";

            var transactions = await _dapper.QueryAsync<RealEstateTransactionDto>(sql, parameters, ct);
            return transactions.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all transactions");
            throw;
        }
    }
}



// Add the handler
public class TransactionStatsHandler : IRequestHandler<TransactionStatsQry, TransactionStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public TransactionStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<TransactionStatsDto> Handle(TransactionStatsQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    COUNT(*) as TotalTransactions,
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as Negotiation,
                    COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as Accepted,
                    COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as PendingInspection,
                    COUNT(CASE WHEN ""Status"" = 4 THEN 1 END) as PendingFinancing,
                    COUNT(CASE WHEN ""Status"" = 5 THEN 1 END) as PendingAppraisal,
                    COUNT(CASE WHEN ""Status"" = 6 THEN 1 END) as Closing,
                    COUNT(CASE WHEN ""Status"" = 7 THEN 1 END) as Completed,
                    COUNT(CASE WHEN ""Status"" = 8 THEN 1 END) as Cancelled,
                    COALESCE(SUM(CASE WHEN ""Status"" = 7 THEN ""SalePrice"" ELSE 0 END), 0) as TotalSalesValue,
                    COALESCE(AVG(CASE WHEN ""Status"" = 7 THEN ""SalePrice"" END), 0) as AverageSalePrice,
                    COALESCE(SUM(""CommissionAmount""), 0) as TotalCommission
                FROM ""RealEstateTransactions""
                WHERE ""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (request.PropertyId.HasValue)
            {
                sql += " AND \"PropertyId\" = @PropertyId";
                parameters.Add("@PropertyId", request.PropertyId.Value);
            }

            if (request.AgentId.HasValue)
            {
                sql += " AND (\"BuyerAgentId\" = @AgentId OR \"SellerAgentId\" = @AgentId)";
                parameters.Add("@AgentId", request.AgentId.Value);
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND \"CreatedAt\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND \"CreatedAt\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            var stats = await _dapper.QueryFirstOrDefaultAsync<TransactionStatsDto>(sql, parameters, ct);
            return stats ?? new TransactionStatsDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get transaction stats");
            throw;
        }
    }
}
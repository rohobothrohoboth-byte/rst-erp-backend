// Cor.CRM/Queries/InvoiceQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.CRM.Queries;

// ============================================================
// QUERIES
// ============================================================

public class InvoiceAllQry : IRequest<List<InvoiceDto>>
{
    public string? Status { get; set; }
    public string? Type { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class InvoiceByIdQry : IRequest<InvoiceDto?>
{
    public Guid Id { get; set; }
}

public class InvoiceStatsQry : IRequest<InvoiceStatsDto>
{
    public Guid? CustomerId { get; set; }
}

public class InvoiceByCustomerQry : IRequest<List<InvoiceDto>>
{
    public Guid CustomerId { get; set; }
}

public class InvoiceByStatusQry : IRequest<List<InvoiceDto>>
{
    public string Status { get; set; } = string.Empty;
}

// ============================================================
// HANDLERS
// ============================================================

public class InvoiceAllHandler : IRequestHandler<InvoiceAllQry, List<InvoiceDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public InvoiceAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<InvoiceDto>> Handle(InvoiceAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    i.""Id"", i.""InvoiceNumber"", i.""InvoiceDate"", i.""DueDate"",
                    i.""PaidDate"", i.""SubTotal"", i.""TaxAmount"", i.""DiscountAmount"",
                    i.""TotalAmount"", i.""AmountPaid"", i.""BalanceDue"", i.""Status"",
                    i.""Type"", i.""Terms"", i.""Notes"", i.""Currency"",
                    i.""CreatedAt"", i.""UpdatedAt"",
                    i.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
                    i.""CustomerId"", c.""Name"" as CustomerName,
                    i.""OpportunityId"", o.""Name"" as OpportunityName,
                    i.""QuoteId"", q.""QuoteNumber""
                FROM ""Invoices"" i
                LEFT JOIN ""Leads"" l ON i.""LeadId"" = l.""Id""
                LEFT JOIN ""Customers"" c ON i.""CustomerId"" = c.""Id""
                LEFT JOIN ""Opportunities"" o ON i.""OpportunityId"" = o.""Id""
                LEFT JOIN ""Quotes"" q ON i.""QuoteId"" = q.""Id""
                WHERE i.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Status))
            {
                sql += " AND i.\"Status\" = @Status";
                parameters.Add("@Status", Enum.Parse<InvoiceStatus>(request.Status));
            }

            if (!string.IsNullOrEmpty(request.Type))
            {
                sql += " AND i.\"Type\" = @Type";
                parameters.Add("@Type", Enum.Parse<InvoiceType>(request.Type));
            }

            if (request.CustomerId.HasValue)
            {
                sql += " AND i.\"CustomerId\" = CAST(@CustomerId AS UUID)";
                parameters.Add("@CustomerId", request.CustomerId.Value.ToString());
            }

            if (request.LeadId.HasValue)
            {
                sql += " AND i.\"LeadId\" = CAST(@LeadId AS UUID)";
                parameters.Add("@LeadId", request.LeadId.Value.ToString());
            }

            if (request.OpportunityId.HasValue)
            {
                sql += " AND i.\"OpportunityId\" = CAST(@OpportunityId AS UUID)";
                parameters.Add("@OpportunityId", request.OpportunityId.Value.ToString());
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND i.\"InvoiceDate\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND i.\"InvoiceDate\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            if (request.MinAmount.HasValue)
            {
                sql += " AND i.\"TotalAmount\" >= @MinAmount";
                parameters.Add("@MinAmount", request.MinAmount.Value);
            }

            if (request.MaxAmount.HasValue)
            {
                sql += " AND i.\"TotalAmount\" <= @MaxAmount";
                parameters.Add("@MaxAmount", request.MaxAmount.Value);
            }

            var sortBy = request.SortBy ?? "CreatedAt";
            var sortOrder = request.SortDescending ? "DESC" : "ASC";
            sql += $" ORDER BY i.\"{sortBy}\" {sortOrder}";

            var offset = (request.Page - 1) * request.PageSize;
            sql += $" OFFSET {offset} LIMIT {request.PageSize}";

            var data = await _dapper.QueryAsync<InvoiceDto>(sql, parameters, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all invoices");
            throw;
        }
    }
}

public class InvoiceByIdHandler : IRequestHandler<InvoiceByIdQry, InvoiceDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public InvoiceByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<InvoiceDto?> Handle(InvoiceByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    i.""Id"", i.""InvoiceNumber"", i.""InvoiceDate"", i.""DueDate"",
                    i.""PaidDate"", i.""SubTotal"", i.""TaxAmount"", i.""DiscountAmount"",
                    i.""TotalAmount"", i.""AmountPaid"", i.""BalanceDue"", i.""Status"",
                    i.""Type"", i.""Terms"", i.""Notes"", i.""Currency"",
                    i.""CreatedAt"", i.""UpdatedAt"",
                    i.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
                    i.""CustomerId"", c.""Name"" as CustomerName,
                    i.""OpportunityId"", o.""Name"" as OpportunityName,
                    i.""QuoteId"", q.""QuoteNumber""
                FROM ""Invoices"" i
                LEFT JOIN ""Leads"" l ON i.""LeadId"" = l.""Id""
                LEFT JOIN ""Customers"" c ON i.""CustomerId"" = c.""Id""
                LEFT JOIN ""Opportunities"" o ON i.""OpportunityId"" = o.""Id""
                LEFT JOIN ""Quotes"" q ON i.""QuoteId"" = q.""Id""
                WHERE i.""Id"" = CAST(@Id AS UUID) AND i.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id.ToString());

            var data = await _dapper.QueryFirstOrDefaultAsync<InvoiceDto>(sql, parameters, ct);

            if (data != null)
            {
                // Get invoice lines
                var lineSql = @"
                    SELECT
                        il.""Id"", il.""Description"", il.""Quantity"",
                        il.""UnitPrice"", il.""Discount"", il.""TaxRate"",
                        il.""TotalPrice"", il.""Notes"",
                        il.""ProductId"", p.""Name"" as ProductName
                    FROM ""InvoiceLines"" il
                    LEFT JOIN ""Products"" p ON il.""ProductId"" = p.""Id""
                    WHERE il.""InvoiceId"" = CAST(@InvoiceId AS UUID) AND il.""IsDeleted"" = false
                    ORDER BY il.""SortOrder""
                ";

                var lineParams = new DynamicParameters();
                lineParams.Add("@InvoiceId", request.Id.ToString());

                var lines = await _dapper.QueryAsync<InvoiceLineDto>(lineSql, lineParams, ct);
                data.InvoiceLines = lines.ToList();

                // Get payments
                var paymentSql = @"
                    SELECT
                        p.""Id"", p.""PaymentNumber"", p.""PaymentDate"", p.""Amount"",
                        p.""Status"", p.""Method"", p.""ReferenceNumber"", p.""Notes"",
                        p.""ProcessedDate"", p.""IsReconciled"",
                        p.""CreatedAt"", p.""UpdatedAt"",
                        p.""InvoiceId""
                    FROM ""Payments"" p
                    WHERE p.""InvoiceId"" = CAST(@InvoiceId AS UUID) AND p.""IsDeleted"" = false
                    ORDER BY p.""PaymentDate"" DESC
                ";

                var payments = await _dapper.QueryAsync<PaymentDto>(paymentSql, lineParams, ct);
                data.Payments = payments.ToList();
            }

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get invoice by ID: {InvoiceId}", request.Id);
            throw;
        }
    }
}

public class InvoiceStatsHandler : IRequestHandler<InvoiceStatsQry, InvoiceStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public InvoiceStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<InvoiceStatsDto> Handle(InvoiceStatsQry request, CancellationToken ct)
    {
        try
        {
            var stats = new InvoiceStatsDto();

            var sql = @"
                SELECT
                    COUNT(*) as Total,
                    COUNT(CASE WHEN ""Status"" = 4 THEN 1 END) as Paid,
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as Draft,
                    COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as Sent,
                    COUNT(CASE WHEN ""Status"" = 6 THEN 1 END) as Overdue,
                    COUNT(CASE WHEN ""Status"" = 5 THEN 1 END) as Partial,
                    COALESCE(SUM(CASE WHEN ""Status"" = 4 THEN ""TotalAmount"" ELSE 0 END), 0) as TotalPaidAmount,
                    COALESCE(SUM(""TotalAmount""), 0) as TotalAmount,
                    COALESCE(AVG(""TotalAmount""), 0) as AverageAmount
                FROM ""Invoices""
                WHERE ""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (request.CustomerId.HasValue)
            {
                sql += " AND \"CustomerId\" = CAST(@CustomerId AS UUID)";
                parameters.Add("@CustomerId", request.CustomerId.Value.ToString());
            }

            var result = await _dapper.QueryFirstOrDefaultAsync<dynamic>(sql, parameters, ct);
            if (result != null)
            {
                stats.TotalInvoices = result.Total ?? 0;
                stats.Paid = result.Paid ?? 0;
                stats.Draft = result.Draft ?? 0;
                stats.Sent = result.Sent ?? 0;
                stats.Overdue = result.Overdue ?? 0;
                stats.Partial = result.Partial ?? 0;
                stats.TotalPaidAmount = result.TotalPaidAmount ?? 0;
                stats.TotalAmount = result.TotalAmount ?? 0;
                stats.AverageAmount = result.AverageAmount ?? 0;
                stats.PaymentRate = stats.TotalInvoices > 0
                    ? (stats.Paid / (double)stats.TotalInvoices) * 100
                    : 0;
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get invoice stats");
            throw;
        }
    }
}

public class InvoiceByCustomerHandler : IRequestHandler<InvoiceByCustomerQry, List<InvoiceDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public InvoiceByCustomerHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<InvoiceDto>> Handle(InvoiceByCustomerQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    i.""Id"", i.""InvoiceNumber"", i.""InvoiceDate"", i.""DueDate"",
                    i.""PaidDate"", i.""SubTotal"", i.""TaxAmount"", i.""DiscountAmount"",
                    i.""TotalAmount"", i.""AmountPaid"", i.""BalanceDue"", i.""Status"",
                    i.""Type"", i.""Terms"", i.""Notes"", i.""Currency"",
                    i.""CreatedAt"", i.""UpdatedAt"",
                    i.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
                    i.""CustomerId"", c.""Name"" as CustomerName,
                    i.""OpportunityId"", o.""Name"" as OpportunityName,
                    i.""QuoteId"", q.""QuoteNumber""
                FROM ""Invoices"" i
                LEFT JOIN ""Leads"" l ON i.""LeadId"" = l.""Id""
                LEFT JOIN ""Customers"" c ON i.""CustomerId"" = c.""Id""
                LEFT JOIN ""Opportunities"" o ON i.""OpportunityId"" = o.""Id""
                LEFT JOIN ""Quotes"" q ON i.""QuoteId"" = q.""Id""
                WHERE i.""CustomerId"" = CAST(@CustomerId AS UUID) AND i.""IsDeleted"" = false
                ORDER BY i.""CreatedAt"" DESC
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@CustomerId", request.CustomerId.ToString());

            var data = await _dapper.QueryAsync<InvoiceDto>(sql, parameters, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get invoices by customer: {CustomerId}", request.CustomerId);
            throw;
        }
    }
}

public class InvoiceByStatusHandler : IRequestHandler<InvoiceByStatusQry, List<InvoiceDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public InvoiceByStatusHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<InvoiceDto>> Handle(InvoiceByStatusQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    i.""Id"", i.""InvoiceNumber"", i.""InvoiceDate"", i.""DueDate"",
                    i.""PaidDate"", i.""SubTotal"", i.""TaxAmount"", i.""DiscountAmount"",
                    i.""TotalAmount"", i.""AmountPaid"", i.""BalanceDue"", i.""Status"",
                    i.""Type"", i.""Terms"", i.""Notes"", i.""Currency"",
                    i.""CreatedAt"", i.""UpdatedAt"",
                    i.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
                    i.""CustomerId"", c.""Name"" as CustomerName,
                    i.""OpportunityId"", o.""Name"" as OpportunityName,
                    i.""QuoteId"", q.""QuoteNumber""
                FROM ""Invoices"" i
                LEFT JOIN ""Leads"" l ON i.""LeadId"" = l.""Id""
                LEFT JOIN ""Customers"" c ON i.""CustomerId"" = c.""Id""
                LEFT JOIN ""Opportunities"" o ON i.""OpportunityId"" = o.""Id""
                LEFT JOIN ""Quotes"" q ON i.""QuoteId"" = q.""Id""
                WHERE i.""Status"" = @Status AND i.""IsDeleted"" = false
                ORDER BY i.""CreatedAt"" DESC
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Status", Enum.Parse<InvoiceStatus>(request.Status));

            var data = await _dapper.QueryAsync<InvoiceDto>(sql, parameters, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get invoices by status: {Status}", request.Status);
            throw;
        }
    }
}
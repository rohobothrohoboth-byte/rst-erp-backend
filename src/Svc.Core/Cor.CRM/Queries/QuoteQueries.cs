// Cor.CRM/Queries/QuoteQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Dapper;
using Helpers;
using MediatR;
using Cor.CRM.Models.Entities;
using System.Text.Json;

namespace Cor.CRM.Queries;

public class QuoteAllQry : IRequest<List<QuoteDto>>
{
    public string? Status { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class QuoteByIdQry : IRequest<QuoteDto?>
{
    public Guid Id { get; set; }
}

public class QuoteStatsQry : IRequest<QuoteStatsDto> { }

public class QuoteAllHandler : IRequestHandler<QuoteAllQry, List<QuoteDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public QuoteAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

   // Cor.CRM/Queries/QuoteQueries.cs - Updated QuoteAllHandler

   public async Task<List<QuoteDto>> Handle(QuoteAllQry request, CancellationToken ct)
   {
       try
       {
           // First, get the quotes
           var sql = @"
               SELECT
                   q.""Id"", q.""QuoteNumber"", q.""SubTotal"", q.""TaxAmount"",
                   q.""DiscountAmount"", q.""TotalAmount"", q.""ShippingCost"",
                   q.""ValidUntil"", q.""Status"", q.""TermsAndConditions"",
                   q.""Notes"", q.""ViewCount"", q.""SentDate"", q.""AcceptedDate"",
                   q.""CreatedAt"", q.""UpdatedAt"",
                   q.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
                   q.""CustomerId"", c.""Name"" as CustomerName,
                   c.""Email"" as CustomerEmail,
                   c.""Phone"" as CustomerPhone,
                   q.""OpportunityId"", o.""Name"" as OpportunityName
               FROM ""Quotes"" q
               LEFT JOIN ""Leads"" l ON q.""LeadId"" = l.""Id""
               LEFT JOIN ""Customers"" c ON q.""CustomerId"" = c.""Id""
               LEFT JOIN ""Opportunities"" o ON q.""OpportunityId"" = o.""Id""
               WHERE q.""IsDeleted"" = false
           ";

           var parameters = new DynamicParameters();

           if (!string.IsNullOrEmpty(request.Status))
           {
               sql += " AND q.\"Status\" = @Status";
               parameters.Add("@Status", Enum.Parse<QuoteStatus>(request.Status));
           }

           if (request.CustomerId.HasValue)
           {
               sql += " AND q.\"CustomerId\" = @CustomerId";
               parameters.Add("@CustomerId", request.CustomerId.Value);
           }

           if (request.LeadId.HasValue)
           {
               sql += " AND q.\"LeadId\" = @LeadId";
               parameters.Add("@LeadId", request.LeadId.Value);
           }

           if (request.OpportunityId.HasValue)
           {
               sql += " AND q.\"OpportunityId\" = @OpportunityId";
               parameters.Add("@OpportunityId", request.OpportunityId.Value);
           }

           if (request.FromDate.HasValue)
           {
               sql += " AND q.\"CreatedAt\" >= @FromDate";
               parameters.Add("@FromDate", request.FromDate.Value);
           }

           if (request.ToDate.HasValue)
           {
               sql += " AND q.\"CreatedAt\" <= @ToDate";
               parameters.Add("@ToDate", request.ToDate.Value);
           }

           var sortBy = request.SortBy ?? "CreatedAt";
           var sortOrder = request.SortDescending ? "DESC" : "ASC";
           sql += $" ORDER BY q.\"{sortBy}\" {sortOrder}";

           var offset = (request.Page - 1) * request.PageSize;
           sql += $" OFFSET {offset} LIMIT {request.PageSize}";

           var quotes = await _dapper.QueryAsync<QuoteDto>(sql, parameters, ct);
           var quoteList = quotes.ToList();

           // Get quote lines for each quote
           if (quoteList.Any())
           {
               var quoteIds = quoteList.Select(q => q.Id).ToList();

               // Use proper UUID array casting for PostgreSQL
               var lineSql = @"
                   SELECT
                       ql.""Id"", ql.""QuoteId"", ql.""Description"", ql.""Quantity"",
                       ql.""UnitPrice"", ql.""Discount"", ql.""TaxRate"",
                       ql.""TotalPrice"", ql.""Notes"", ql.""SortOrder"",
                       ql.""ProductId"", p.""Name"" as ProductName
                   FROM ""QuoteLines"" ql
                   LEFT JOIN ""Products"" p ON ql.""ProductId"" = p.""Id""
                   WHERE ql.""QuoteId"" = ANY(@QuoteIds::uuid[]) AND ql.""IsDeleted"" = false
                   ORDER BY ql.""SortOrder""
               ";

               // Convert Guids to string array and cast to UUID in PostgreSQL
               var quoteIdStrings = quoteIds.Select(id => id.ToString()).ToArray();

               var lineParams = new DynamicParameters();
               lineParams.Add("@QuoteIds", quoteIdStrings);

               var lines = await _dapper.QueryAsync<dynamic>(lineSql, lineParams, ct);

               // Group lines by QuoteId manually
               var linesByQuoteId = new Dictionary<Guid, List<QuoteLineDto>>();

               foreach (var line in lines)
               {
                   var quoteId = (Guid)line.QuoteId;
                   var lineDto = new QuoteLineDto
                   {
                       Id = line.Id,
                       QuoteId = quoteId,
                       Description = line.Description,
                       Quantity = line.Quantity,
                       UnitPrice = line.UnitPrice,
                       Discount = line.Discount ?? 0,
                       TaxRate = line.TaxRate ?? 0,
                       TotalPrice = line.TotalPrice,
                       ProductId = line.ProductId,
                       ProductName = line.ProductName,
                       Notes = line.Notes,
                       SortOrder = line.SortOrder ?? 0
                   };

                   if (!linesByQuoteId.ContainsKey(quoteId))
                   {
                       linesByQuoteId[quoteId] = new List<QuoteLineDto>();
                   }
                   linesByQuoteId[quoteId].Add(lineDto);
               }

               // Assign lines to each quote
               foreach (var quote in quoteList)
               {
                   if (linesByQuoteId.TryGetValue(quote.Id, out var quoteLines))
                   {
                       quote.QuoteLines = quoteLines;
                   }
                   else
                   {
                       quote.QuoteLines = new List<QuoteLineDto>();
                   }
               }
           }

           return quoteList;
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Failed to get all quotes");
           throw;
       }
   }
}

public class QuoteByIdHandler : IRequestHandler<QuoteByIdQry, QuoteDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public QuoteByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

   // Cor.CRM/Queries/QuoteQueries.cs - QuoteByIdHandler

   public async Task<QuoteDto?> Handle(QuoteByIdQry request, CancellationToken ct)
   {
       try
       {
           var sql = @"
               SELECT
                   q.""Id"", q.""QuoteNumber"", q.""SubTotal"", q.""TaxAmount"",
                   q.""DiscountAmount"", q.""TotalAmount"", q.""ShippingCost"",
                   q.""ValidUntil"", q.""Status"", q.""TermsAndConditions"",
                   q.""Notes"", q.""ViewCount"", q.""SentDate"", q.""AcceptedDate"",
                   q.""CreatedAt"", q.""UpdatedAt"",
                   q.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
                   q.""CustomerId"", c.""Name"" as CustomerName,
                   c.""Email"" as CustomerEmail,
                   c.""Phone"" as CustomerPhone,
                   q.""OpportunityId"", o.""Name"" as OpportunityName
               FROM ""Quotes"" q
               LEFT JOIN ""Leads"" l ON q.""LeadId"" = l.""Id""
               LEFT JOIN ""Customers"" c ON q.""CustomerId"" = c.""Id""
               LEFT JOIN ""Opportunities"" o ON q.""OpportunityId"" = o.""Id""
               WHERE q.""Id"" = CAST(@Id AS UUID) AND q.""IsDeleted"" = false
           ";

           var parameters = new DynamicParameters();
           parameters.Add("@Id", request.Id.ToString());

           var data = await _dapper.QueryFirstOrDefaultAsync<QuoteDto>(sql, parameters, ct);

           if (data != null)
           {
               // ✅ Make sure this query is returning the lines
               var lineSql = @"
                   SELECT
                       ql.""Id"", ql.""QuoteId"", ql.""Description"", ql.""Quantity"",
                       ql.""UnitPrice"", ql.""Discount"", ql.""TaxRate"",
                       ql.""TotalPrice"", ql.""Notes"", ql.""SortOrder"",
                       ql.""ProductId"", p.""Name"" as ProductName
                   FROM ""QuoteLines"" ql
                   LEFT JOIN ""Products"" p ON ql.""ProductId"" = p.""Id""
                   WHERE ql.""QuoteId"" = CAST(@QuoteId AS UUID) AND ql.""IsDeleted"" = false
                   ORDER BY ql.""SortOrder""
               ";

               var lineParams = new DynamicParameters();
               lineParams.Add("@QuoteId", request.Id.ToString());

               var lines = await _dapper.QueryAsync<QuoteLineDto>(lineSql, lineParams, ct);
               data.QuoteLines = lines.ToList(); // ✅ Set the QuoteLines property

               Console.WriteLine($"Loaded {lines.Count()} quote lines for quote {data.QuoteNumber}");
           }

           return data;
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Failed to get quote by ID: {QuoteId}", request.Id);
           throw;
       }
   }
}

public class QuoteStatsHandler : IRequestHandler<QuoteStatsQry, QuoteStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public QuoteStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<QuoteStatsDto> Handle(QuoteStatsQry request, CancellationToken ct)
    {
        try
        {
            var stats = new QuoteStatsDto();

            var sql = @"
                SELECT
                    COUNT(*) as Total,
                    COUNT(CASE WHEN ""Status"" = 5 THEN 1 END) as Accepted,
                    COUNT(CASE WHEN ""Status"" = 6 THEN 1 END) as Rejected,
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as Draft,
                    COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as Sent,
                    COALESCE(SUM(CASE WHEN ""Status"" = 5 THEN ""TotalAmount"" ELSE 0 END), 0) as AcceptedValue
                FROM ""Quotes""
                WHERE ""IsDeleted"" = false
            ";

            var result = await _dapper.QueryFirstOrDefaultAsync<dynamic>(sql, new { }, ct);
            if (result != null)
            {
                stats.TotalQuotes = result.Total ?? 0;
                stats.Accepted = result.Accepted ?? 0;
                stats.Rejected = result.Rejected ?? 0;
                stats.Draft = result.Draft ?? 0;
                stats.Sent = result.Sent ?? 0;
                stats.AcceptedValue = result.AcceptedValue ?? 0;
                stats.ConversionRate = stats.TotalQuotes > 0
                    ? (stats.Accepted / (double)stats.TotalQuotes) * 100
                    : 0;
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get quote stats");
            throw;
        }
    }
}
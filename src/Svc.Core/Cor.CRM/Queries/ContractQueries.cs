// Cor.CRM/Queries/ContractQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Dapper;
using Helpers;
using MediatR;
using Cor.CRM.Models.Entities;
using ContractStatus = Cor.CRM.Models.Entities.ContractStatus;
namespace Cor.CRM.Queries;

public class ContractByIdQry : IRequest<ContractDto?>
{
    public Guid Id { get; set; }
}

public class ContractAllQry : IRequest<List<ContractDto>>
{
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? QuoteId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
}

public class ContractStatsQry : IRequest<ContractStatsDto> { }

public class ContractByIdHandler : IRequestHandler<ContractByIdQry, ContractDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public ContractByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<ContractDto?> Handle(ContractByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    c.""Id"", c.""ContractNumber"", c.""Title"", c.""Description"",
                    c.""CustomerId"", cust.""Name"" as CustomerName,
                    c.""OpportunityId"", opp.""Name"" as OpportunityName,
                    c.""QuoteId"", q.""QuoteNumber"",
                    c.""TotalValue"", c.""Status"",
                    c.""StartDate"", c.""EndDate"", c.""SignedDate"",
                    c.""TermsAndConditions"", c.""Notes"",
                    c.""CreatedAt"", c.""UpdatedAt""
                FROM ""Contracts"" c
                LEFT JOIN ""Customers"" cust ON c.""CustomerId"" = cust.""Id""
                LEFT JOIN ""Opportunities"" opp ON c.""OpportunityId"" = opp.""Id""
                LEFT JOIN ""Quotes"" q ON c.""QuoteId"" = q.""Id""
                WHERE c.""Id"" = @Id AND c.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var contract = await _dapper.QueryFirstOrDefaultAsync<ContractDto>(sql, parameters, ct);

            if (contract != null)
            {
                // Get contract lines
                var lineSql = @"
                    SELECT
                        cl.""Id"", cl.""ContractId"", cl.""ProductId"",
                        p.""Name"" as ProductName,
                        cl.""Description"", cl.""Quantity"", cl.""UnitPrice"",
                        cl.""TotalPrice"", cl.""SortOrder"", cl.""Notes""
                    FROM ""ContractLines"" cl
                    LEFT JOIN ""Products"" p ON cl.""ProductId"" = p.""Id""
                    WHERE cl.""ContractId"" = @ContractId AND cl.""IsDeleted"" = false
                    ORDER BY cl.""SortOrder""
                ";

                var lineParams = new DynamicParameters();
                lineParams.Add("@ContractId", request.Id);

                var lines = await _dapper.QueryAsync<ContractLineDto>(lineSql, lineParams, ct);
                contract.ContractLines = lines.ToList();
            }

            return contract;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get contract by ID: {ContractId}", request.Id);
            throw;
        }
    }
}

public class ContractAllHandler : IRequestHandler<ContractAllQry, List<ContractDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public ContractAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<ContractDto>> Handle(ContractAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    c.""Id"", c.""ContractNumber"", c.""Title"", c.""Description"",
                    c.""CustomerId"", cust.""Name"" as CustomerName,
                    c.""OpportunityId"", opp.""Name"" as OpportunityName,
                    c.""QuoteId"", q.""QuoteNumber"",
                    c.""TotalValue"", c.""Status"",
                    c.""StartDate"", c.""EndDate"", c.""SignedDate"",
                    c.""TermsAndConditions"", c.""Notes"",
                    c.""CreatedAt"", c.""UpdatedAt""
                FROM ""Contracts"" c
                LEFT JOIN ""Customers"" cust ON c.""CustomerId"" = cust.""Id""
                LEFT JOIN ""Opportunities"" opp ON c.""OpportunityId"" = opp.""Id""
                LEFT JOIN ""Quotes"" q ON c.""QuoteId"" = q.""Id""
                WHERE c.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (request.CustomerId.HasValue)
            {
                sql += " AND c.\"CustomerId\" = @CustomerId";
                parameters.Add("@CustomerId", request.CustomerId.Value);
            }

            if (request.OpportunityId.HasValue)
            {
                sql += " AND c.\"OpportunityId\" = @OpportunityId";
                parameters.Add("@OpportunityId", request.OpportunityId.Value);
            }

            if (request.QuoteId.HasValue)
            {
                sql += " AND c.\"QuoteId\" = @QuoteId";
                parameters.Add("@QuoteId", request.QuoteId.Value);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                sql += " AND c.\"Status\" = @Status";
                parameters.Add("@Status", Enum.Parse<ContractStatus>(request.Status));
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND c.\"CreatedAt\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND c.\"CreatedAt\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            var sortBy = request.SortBy ?? "CreatedAt";
            var sortOrder = request.SortDescending ? "DESC" : "ASC";
            sql += $" ORDER BY c.\"{sortBy}\" {sortOrder}";
            sql += $" OFFSET {(request.Page - 1) * request.PageSize} LIMIT {request.PageSize}";

            var contracts = await _dapper.QueryAsync<ContractDto>(sql, parameters, ct);
            var contractList = contracts.ToList();

            // Get contract lines for each contract
            if (contractList.Any())
            {
                foreach (var contract in contractList)
                {
                    var lineSql = @"
                        SELECT
                            cl.""Id"", cl.""ContractId"", cl.""ProductId"",
                            p.""Name"" as ProductName,
                            cl.""Description"", cl.""Quantity"", cl.""UnitPrice"",
                            cl.""TotalPrice"", cl.""SortOrder"", cl.""Notes""
                        FROM ""ContractLines"" cl
                        LEFT JOIN ""Products"" p ON cl.""ProductId"" = p.""Id""
                        WHERE cl.""ContractId"" = @ContractId AND cl.""IsDeleted"" = false
                        ORDER BY cl.""SortOrder""
                    ";

                    var lineParams = new DynamicParameters();
                    lineParams.Add("@ContractId", contract.Id);

                    var lines = await _dapper.QueryAsync<ContractLineDto>(lineSql, lineParams, ct);
                    contract.ContractLines = lines.ToList();
                }
            }

            return contractList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all contracts");
            throw;
        }
    }
}

public class ContractStatsHandler : IRequestHandler<ContractStatsQry, ContractStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public ContractStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<ContractStatsDto> Handle(ContractStatsQry request, CancellationToken ct)
    {
        try
        {
            var stats = new ContractStatsDto();

            var sql = @"
                SELECT
                    COUNT(*) as TotalContracts,
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as Draft,
                    COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as Pending,
                    COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as Active,
                    COUNT(CASE WHEN ""Status"" = 4 THEN 1 END) as Signed,
                    COUNT(CASE WHEN ""Status"" = 5 THEN 1 END) as Expired,
                    COUNT(CASE WHEN ""Status"" = 6 THEN 1 END) as Terminated,
                    COALESCE(SUM(""TotalValue""), 0) as TotalValue,
                    COALESCE(AVG(""TotalValue""), 0) as AverageContractValue
                FROM ""Contracts""
                WHERE ""IsDeleted"" = false
            ";

            var result = await _dapper.QueryFirstOrDefaultAsync<dynamic>(sql, new { }, ct);
            if (result != null)
            {
                stats.TotalContracts = result.TotalContracts ?? 0;
                stats.Draft = result.Draft ?? 0;
                stats.Pending = result.Pending ?? 0;
                stats.Active = result.Active ?? 0;
                stats.Signed = result.Signed ?? 0;
                stats.Expired = result.Expired ?? 0;
                stats.Terminated = result.Terminated ?? 0;
                stats.TotalValue = result.TotalValue ?? 0;
                stats.AverageContractValue = result.AverageContractValue ?? 0;
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get contract stats");
            throw;
        }
    }
}
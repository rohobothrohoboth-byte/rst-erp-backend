// Cor.CRM/Queries/CompanyQueries.cs

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

public class CompanyAllQry : IRequest<List<CompanyDto>>
{
    public string? Search { get; set; }
    public string? Industry { get; set; }
    public string? Status { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

public class CompanyByIdQry : IRequest<CompanyDto?>
{
    public Guid Id { get; set; }
}

public class CompanyStatsQry : IRequest<CompanyStatsDto> { }

// ============================================================
// HANDLERS
// ============================================================

public class CompanyAllHandler : IRequestHandler<CompanyAllQry, List<CompanyDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CompanyAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<CompanyDto>> Handle(CompanyAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    c.""Id"", c.""Name"", c.""LegalName"", c.""Email"", c.""Phone"",
                    c.""Website"", c.""Industry"", c.""Size"", c.""Status"",
                    c.""Address"", c.""City"", c.""State"", c.""Country"", c.""PostalCode"",
                    c.""Description"", c.""FoundedYear"", c.""Revenue"", c.""EmployeeCount"",
                    c.""TaxId"", c.""RegistrationNumber"", c.""ContactCount"", c.""LeadCount"",
                    c.""IsActive"", c.""CreatedAt"", c.""UpdatedAt""
                FROM ""Company"" c
                WHERE c.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Search))
            {
                sql += @" AND (c.""Name"" ILIKE @Search
                        OR c.""LegalName"" ILIKE @Search
                        OR c.""Email"" ILIKE @Search
                        OR c.""Industry"" ILIKE @Search)";
                parameters.Add("@Search", $"%{request.Search}%");
            }

            if (!string.IsNullOrEmpty(request.Industry))
            {
                sql += " AND c.\"Industry\" = @Industry";
                parameters.Add("@Industry", request.Industry);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                sql += " AND c.\"Status\" = @Status";
                parameters.Add("@Status", request.Status);
            }

            var sortBy = request.SortBy ?? "CreatedAt";
            var sortOrder = request.SortOrder?.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY c.\"{sortBy}\" {sortOrder}";

            if (request.Page.HasValue && request.PageSize.HasValue)
            {
                var offset = (request.Page.Value - 1) * request.PageSize.Value;
                sql += $" OFFSET {offset} LIMIT {request.PageSize.Value}";
            }

            var data = await _dapper.QueryAsync<CompanyDto>(sql, parameters, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all companies");
            throw;
        }
    }
}

public class CompanyByIdHandler : IRequestHandler<CompanyByIdQry, CompanyDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CompanyByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<CompanyDto?> Handle(CompanyByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    c.""Id"", c.""Name"", c.""LegalName"", c.""Email"", c.""Phone"",
                    c.""Website"", c.""Industry"", c.""Size"", c.""Status"",
                    c.""Address"", c.""City"", c.""State"", c.""Country"", c.""PostalCode"",
                    c.""Description"", c.""FoundedYear"", c.""Revenue"", c.""EmployeeCount"",
                    c.""TaxId"", c.""RegistrationNumber"", c.""ContactCount"", c.""LeadCount"",
                    c.""IsActive"", c.""CreatedAt"", c.""UpdatedAt""
                FROM ""Company"" c
                WHERE c.""Id"" = @Id AND c.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var data = await _dapper.QueryFirstOrDefaultAsync<CompanyDto>(sql, parameters, ct);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get company by ID: {CompanyId}", request.Id);
            throw;
        }
    }
}

public class CompanyStatsHandler : IRequestHandler<CompanyStatsQry, CompanyStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CompanyStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<CompanyStatsDto> Handle(CompanyStatsQry request, CancellationToken ct)
    {
        try
        {
            var stats = new CompanyStatsDto();

            var countSql = @"
                SELECT
                    COUNT(*) as Total,
                    COUNT(CASE WHEN ""IsActive"" = true THEN 1 END) as Active,
                    COUNT(CASE WHEN ""IsActive"" = false THEN 1 END) as Inactive
                FROM ""Company""
                WHERE ""IsDeleted"" = false
            ";


            var counts = await _dapper.QueryFirstOrDefaultAsync<dynamic>(countSql, new { }, ct);
            if (counts != null)
            {
                stats.Total = counts.Total ?? 0;
                stats.Active = counts.Active ?? 0;
                stats.Inactive = counts.Inactive ?? 0;
            }

            var industrySql = @"
                SELECT
                    ""Industry"" as Key,
                    COUNT(*) as Value
                FROM ""Company""
                WHERE ""IsDeleted"" = false AND ""Industry"" IS NOT NULL
                GROUP BY ""Industry""
            ";

            var industryData = await _dapper.QueryAsync<KeyValuePair<string, int>>(industrySql, new { }, ct);
            stats.ByIndustry = industryData.ToDictionary(x => x.Key, x => x.Value);

            var statusSql = @"
                SELECT
                    ""Status"" as Key,
                    COUNT(*) as Value
                FROM ""Company""
                WHERE ""IsDeleted"" = false AND ""Status"" IS NOT NULL
                GROUP BY ""Status""
            ";

            var statusData = await _dapper.QueryAsync<KeyValuePair<string, int>>(statusSql, new { }, ct);
            stats.ByStatus = statusData.ToDictionary(x => x.Key, x => x.Value);

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get company stats");
            throw;
        }
    }
}
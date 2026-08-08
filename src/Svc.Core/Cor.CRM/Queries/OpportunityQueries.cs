// Cor.CRM/Queries/OpportunityQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;
using System.Data;
using NpgsqlTypes;

namespace Cor.CRM.Queries;

// ============================================================
// QUERIES
// ============================================================

public class OpportunityAllQry : IRequest<List<OpportunityDto>>
{
    public string? SearchTerm { get; set; }
    public string? Stage { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class OpportunityByIdQry : IRequest<OpportunityDto?>
{
    public Guid Id { get; set; }
}

public class OpportunityStatsQry : IRequest<OpportunityStatsDto>
{
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
}

public class OpportunityPipelineQry : IRequest<OpportunityPipelineDto>
{
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
}

// ============================================================
// HANDLERS
// ============================================================

public class OpportunityAllHandler : IRequestHandler<OpportunityAllQry, List<OpportunityDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public OpportunityAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<OpportunityDto>> Handle(OpportunityAllQry request, CancellationToken ct)
    {
        try
        {

var sql = @"
    SELECT
        o.""Id"", o.""Name"", o.""Description"", o.""Amount"",
        o.""Stage"", o.""WinProbability"", o.""ExpectedCloseDate"",
        o.""ActualCloseDate"", o.""IsActive"", o.""CreatedAt"", o.""UpdatedAt"",
        o.""CustomerId"", c.""Name"" as CustomerName,
        o.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
        o.""AssignedToUserId"",
        e.""FirstName"" || ' ' || e.""LastName"" as AssignedToUserName,
        o.""ActivityCount""
    FROM ""Opportunities"" o
    LEFT JOIN ""Customers"" c ON o.""CustomerId"" = c.""Id""
    LEFT JOIN ""Leads"" l ON o.""LeadId"" = l.""Id""
    LEFT JOIN ""LocalEmployees"" e ON o.""AssignedToUserId"" = e.""Id""
    WHERE o.""IsDeleted"" = false
";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                sql += @" AND (o.""Name"" ILIKE @SearchTerm
                        OR o.""Description"" ILIKE @SearchTerm)";
                parameters.Add("@SearchTerm", $"%{request.SearchTerm}%");
            }

            // ✅ FIX: Convert stage string to enum integer value
            if (!string.IsNullOrEmpty(request.Stage))
            {
                if (Enum.TryParse<OpportunityStage>(request.Stage, true, out var stageEnum))
                {
                    sql += " AND o.\"Stage\" = @Stage";
                    parameters.Add("@Stage", (int)stageEnum);
                }
                else
                {
                    _logger.LogWarning("Invalid stage value: {Stage}", request.Stage);
                }
            }

            // ✅ FIX: Use explicit CAST with string values for all GUIDs
            if (request.CustomerId.HasValue)
            {
                sql += " AND o.\"CustomerId\" = CAST(@CustomerId AS UUID)";
                parameters.Add("@CustomerId", request.CustomerId.Value.ToString());
            }

            if (request.LeadId.HasValue)
            {
                sql += " AND o.\"LeadId\" = CAST(@LeadId AS UUID)";
                parameters.Add("@LeadId", request.LeadId.Value.ToString());
            }

            if (request.AssignedToUserId.HasValue)
            {
                sql += " AND o.\"AssignedToUserId\" = CAST(@AssignedToUserId AS UUID)";
                parameters.Add("@AssignedToUserId", request.AssignedToUserId.Value.ToString());
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND o.\"ExpectedCloseDate\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND o.\"ExpectedCloseDate\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            if (request.MinAmount.HasValue)
            {
                sql += " AND o.\"Amount\" >= @MinAmount";
                parameters.Add("@MinAmount", request.MinAmount.Value);
            }

            if (request.MaxAmount.HasValue)
            {
                sql += " AND o.\"Amount\" <= @MaxAmount";
                parameters.Add("@MaxAmount", request.MaxAmount.Value);
            }

            var sortBy = request.SortBy ?? "CreatedAt";
            var sortOrder = request.SortDescending ? "DESC" : "ASC";
            sql += $" ORDER BY o.\"{sortBy}\" {sortOrder}";

            var offset = (request.Page - 1) * request.PageSize;
            sql += $" OFFSET {offset} LIMIT {request.PageSize}";

            _logger.LogInformation("SQL Query: {Sql}", sql);
            _logger.LogInformation("Parameters: {@Parameters}", parameters);

            var data = await _dapper.QueryAsync<OpportunityDto>(sql, parameters, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all opportunities");
            throw;
        }
    }
}

public class OpportunityByIdHandler : IRequestHandler<OpportunityByIdQry, OpportunityDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public OpportunityByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<OpportunityDto?> Handle(OpportunityByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    o.""Id"", o.""Name"", o.""Description"", o.""Amount"",
                    o.""Stage"", o.""WinProbability"", o.""ExpectedCloseDate"",
                    o.""ActualCloseDate"", o.""IsActive"", o.""CreatedAt"", o.""UpdatedAt"",
                    o.""CustomerId"", c.""Name"" as CustomerName,
                    o.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
                    o.""AssignedToUserId"",
                    (e.""FirstName"" || ' ' || e.""LastName"") as AssignedToUserName,
                    o.""ActivityCount""
                FROM ""Opportunities"" o
                LEFT JOIN ""Customers"" c ON o.""CustomerId"" = c.""Id""
                LEFT JOIN ""Leads"" l ON o.""LeadId"" = l.""Id""
                LEFT JOIN ""LocalEmployees"" e ON o.""AssignedToUserId"" = e.""AppUserId""
                WHERE o.""Id"" = CAST(@Id AS UUID) AND o.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id.ToString());

            var data = await _dapper.QueryFirstOrDefaultAsync<OpportunityDto>(sql, parameters, ct);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get opportunity by ID: {OpportunityId}", request.Id);
            throw;
        }
    }
}

public class OpportunityStatsHandler : IRequestHandler<OpportunityStatsQry, OpportunityStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public OpportunityStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<OpportunityStatsDto> Handle(OpportunityStatsQry request, CancellationToken ct)
    {
        try
        {
            var stats = new OpportunityStatsDto();

            // ✅ FIX: Use integer values for stage comparisons
            var sql = @"
                SELECT
                    COUNT(*) as Total,
                    COUNT(CASE WHEN ""Stage"" = 5 THEN 1 END) as ClosedWon,
                    COUNT(CASE WHEN ""Stage"" = 6 THEN 1 END) as ClosedLost,
                    COUNT(CASE WHEN ""Stage"" IN (1,2,3,4) THEN 1 END) as Active,
                    COALESCE(SUM(CASE WHEN ""Stage"" = 5 THEN ""Amount"" ELSE 0 END), 0) as TotalWonAmount,
                    COALESCE(AVG(""WinProbability""), 0) as AvgWinProbability
                FROM ""Opportunities""
                WHERE ""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (request.CustomerId.HasValue)
            {
                sql += " AND \"CustomerId\" = CAST(@CustomerId AS UUID)";
                parameters.Add("@CustomerId", request.CustomerId.Value.ToString());
            }

            if (request.LeadId.HasValue)
            {
                sql += " AND \"LeadId\" = CAST(@LeadId AS UUID)";
                parameters.Add("@LeadId", request.LeadId.Value.ToString());
            }

            var result = await _dapper.QueryFirstOrDefaultAsync<dynamic>(sql, parameters, ct);
            if (result != null)
            {
                stats.TotalOpportunities = result.Total ?? 0;
                stats.ClosedWon = result.ClosedWon ?? 0;
                stats.ClosedLost = result.ClosedLost ?? 0;
                stats.Active = result.Active ?? 0;
                stats.TotalWonAmount = result.TotalWonAmount ?? 0;
                stats.AvgWinProbability = result.AvgWinProbability ?? 0;
                stats.ConversionRate = stats.TotalOpportunities > 0
                    ? (stats.ClosedWon / (double)stats.TotalOpportunities) * 100
                    : 0;
            }

            // Get stage distribution
            var stageSql = @"
                SELECT
                    CAST(""Stage"" AS TEXT) as Key,
                    COUNT(*) as Value,
                    COALESCE(SUM(""Amount""), 0) as TotalAmount
                FROM ""Opportunities""
                WHERE ""IsDeleted"" = false
            ";

            if (request.CustomerId.HasValue)
            {
                stageSql += " AND \"CustomerId\" = CAST(@CustomerId AS UUID)";
            }

            if (request.LeadId.HasValue)
            {
                stageSql += " AND \"LeadId\" = CAST(@LeadId AS UUID)";
            }

            stageSql += " GROUP BY \"Stage\"";

            var stageData = await _dapper.QueryAsync<StageDistributionDto>(stageSql, parameters, ct);

            // Convert integer stage values to enum names for display
            foreach (var item in stageData)
            {
                if (int.TryParse(item.Key, out int stageValue))
                {
                    var stageName = Enum.GetName(typeof(OpportunityStage), stageValue) ?? item.Key;
                    item.Key = stageName;
                }
            }

            stats.StageDistribution = stageData.ToList();

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get opportunity stats");
            throw;
        }
    }
}

public class OpportunityPipelineHandler : IRequestHandler<OpportunityPipelineQry, OpportunityPipelineDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public OpportunityPipelineHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<OpportunityPipelineDto> Handle(OpportunityPipelineQry request, CancellationToken ct)
    {
        try
        {
            var pipeline = new OpportunityPipelineDto();

            // ✅ FIX: Use integer values for stage comparisons
            var sql = @"
                SELECT
                    ""Stage"" as StageValue,
                    COUNT(*) as Count,
                    COALESCE(SUM(""Amount""), 0) as TotalValue,
                    COALESCE(AVG(""WinProbability""), 0) as AvgProbability
                FROM ""Opportunities""
                WHERE ""IsDeleted"" = false
                    AND ""Stage"" IN (1,2,3,4)
            ";

            var parameters = new DynamicParameters();

            if (request.CustomerId.HasValue)
            {
                sql += " AND \"CustomerId\" = CAST(@CustomerId AS UUID)";
                parameters.Add("@CustomerId", request.CustomerId.Value.ToString());
            }

            if (request.LeadId.HasValue)
            {
                sql += " AND \"LeadId\" = CAST(@LeadId AS UUID)";
                parameters.Add("@LeadId", request.LeadId.Value.ToString());
            }

            sql += " GROUP BY \"Stage\" ORDER BY \"Stage\"";

            var stages = await _dapper.QueryAsync<dynamic>(sql, parameters, ct);

            // Convert integer stage values to enum names
            foreach (var stage in stages)
            {
                int stageValue = stage.StageValue;
                var stageName = Enum.GetName(typeof(OpportunityStage), stageValue) ?? stageValue.ToString();

                pipeline.Stages.Add(new PipelineStageDto
                {
                    StageName = stageName,
                    Count = stage.Count,
                    TotalValue = stage.TotalValue,
                    AvgProbability = stage.AvgProbability
                });
            }

            // Calculate total pipeline value
            pipeline.TotalPipelineValue = pipeline.Stages.Sum(s => s.TotalValue);
            pipeline.TotalOpportunities = pipeline.Stages.Sum(s => s.Count);

            return pipeline;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get opportunity pipeline");
            throw;
        }
    }
}
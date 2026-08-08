// Cor.CRM/Queries/RoutingQry.cs

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

public class RoutingRulesQry : IRequest<List<RoutingRuleDto>> { }

public class RoutingRuleByIdQry : IRequest<RoutingRuleDto?>
{
    public Guid Id { get; set; }
}

public class RoutingStatsQry : IRequest<RoutingStatsDto> { }

// ============================================================
// GET ALL ROUTING RULES
// ============================================================

// Cor.CRM/Queries/RoutingQry.cs

public class RoutingRulesHandler : IRequestHandler<RoutingRulesQry, List<RoutingRuleDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public RoutingRulesHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<RoutingRuleDto>> Handle(RoutingRulesQry request, CancellationToken ct)
    {
        try
        {
            // ✅ FIXED: Remove AppUser join - use LocalEmployee or just skip user name
            var sql = @"
                SELECT
                    r.""Id"",
                    r.""Name"",
                    r.""Description"",
                    r.""Type"",
                    r.""Conditions"",
                    r.""IsActive"",
                    r.""Priority"",
                    r.""AssignedToUserId"",
                    r.""AssignedToTeamId"",
                    r.""FallbackRule"",
                    r.""MaxLeadsPerDay"",
                    r.""CreatedAt"",
                    r.""UpdatedAt"",
                    0 as ""MatchesCount"",
                    '' as ""AssignedToUserName""
                FROM ""LeadRoutingRules"" r
                WHERE r.""IsDeleted"" = false
                ORDER BY r.""Priority"" ASC, r.""CreatedAt"" DESC
            ";

            var data = await _dapper.QueryAsync<RoutingRuleDto>(sql, new { }, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get routing rules");
            throw;
        }
    }
}

// ============================================================
// GET ROUTING RULE BY ID
// ============================================================

// Cor.CRM/Queries/RoutingQry.cs

public class RoutingRuleByIdHandler : IRequestHandler<RoutingRuleByIdQry, RoutingRuleDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public RoutingRuleByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<RoutingRuleDto?> Handle(RoutingRuleByIdQry request, CancellationToken ct)
    {
        try
        {
            // ✅ FIXED: Remove AppUser join
            var sql = @"
                SELECT
                    r.""Id"",
                    r.""Name"",
                    r.""Description"",
                    r.""Type"",
                    r.""Conditions"",
                    r.""IsActive"",
                    r.""Priority"",
                    r.""AssignedToUserId"",
                    r.""AssignedToTeamId"",
                    r.""FallbackRule"",
                    r.""MaxLeadsPerDay"",
                    r.""CreatedAt"",
                    r.""UpdatedAt"",
                    '' as ""AssignedToUserName""
                FROM ""LeadRoutingRules"" r
                WHERE r.""Id"" = @Id AND r.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var data = await _dapper.QueryFirstOrDefaultAsync<RoutingRuleDto>(sql, parameters, ct);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get routing rule by ID: {RuleId}", request.Id);
            throw;
        }
    }
}

// ============================================================
// GET ROUTING STATS
// ============================================================

public class RoutingStatsHandler : IRequestHandler<RoutingStatsQry, RoutingStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public RoutingStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<RoutingStatsDto> Handle(RoutingStatsQry request, CancellationToken ct)
    {
        try
        {
            var stats = new RoutingStatsDto();

            // Get rule counts
            var ruleSql = @"
                SELECT
                    COUNT(*) as TotalRules,
                    COUNT(CASE WHEN ""IsActive"" = true THEN 1 END) as ActiveRules
                FROM ""LeadRoutingRules""
                WHERE ""IsDeleted"" = false
            ";

            var ruleStats = await _dapper.QueryFirstOrDefaultAsync<dynamic>(ruleSql, new { }, ct);
            if (ruleStats != null)
            {
                stats.TotalRules = ruleStats.TotalRules ?? 0;
                stats.ActiveRules = ruleStats.ActiveRules ?? 0;
            }

            // Get routing stats
            var routingSql = @"
                SELECT
                    COUNT(*) as TotalRouted,
                    COUNT(CASE WHEN ""AssignedToUserId"" IS NULL THEN 1 END) as PendingRouting,
                    COALESCE(AVG(EXTRACT(EPOCH FROM (""UpdatedAt"" - ""CreatedAt""))), 0) as AvgResponseTime
                FROM ""Leads""
                WHERE ""IsDeleted"" = false
            ";

            var routingStats = await _dapper.QueryFirstOrDefaultAsync<dynamic>(routingSql, new { }, ct);
            if (routingStats != null)
            {
                // ✅ FIXED: Handle null values with COALESCE or null checks
                stats.TotalRouted = routingStats.TotalRouted != null ? (int)routingStats.TotalRouted : 0;
                stats.PendingRouting = routingStats.PendingRouting != null ? (int)routingStats.PendingRouting : 0;
                stats.AvgResponseTime = routingStats.AvgResponseTime != null ? (double)routingStats.AvgResponseTime : 0;
            }

            // Get rules by type
            var typeSql = @"
                SELECT
                    ""Type""::text as Key,
                    COUNT(*) as Value
                FROM ""LeadRoutingRules""
                WHERE ""IsDeleted"" = false
                GROUP BY ""Type""
            ";

            var typeData = await _dapper.QueryAsync<KeyValuePair<string, int>>(typeSql, new { }, ct);
            stats.RulesByType = typeData.ToDictionary(x => x.Key, x => x.Value);

            // Get rules by status
            var statusSql = @"
                SELECT
                    CASE WHEN ""IsActive"" = true THEN 'Active' ELSE 'Inactive' END as Key,
                    COUNT(*) as Value
                FROM ""LeadRoutingRules""
                WHERE ""IsDeleted"" = false
                GROUP BY ""IsActive""
            ";

            var statusData = await _dapper.QueryAsync<KeyValuePair<string, int>>(statusSql, new { }, ct);
            stats.RulesByStatus = statusData.ToDictionary(x => x.Key, x => x.Value);

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get routing stats");
            throw;
        }
    }
}
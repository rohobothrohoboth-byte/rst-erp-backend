// Cor.CRM/Queries/ScoringQry.cs

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

public class ScoreRulesQry : IRequest<List<ScoreRuleDto>> { }

public class ScoreRuleByIdQry : IRequest<ScoreRuleDto?>
{
    public Guid Id { get; set; }
}

// ============================================================
// GET ALL SCORE RULES
// ============================================================

public class ScoreRulesHandler : IRequestHandler<ScoreRulesQry, List<ScoreRuleDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public ScoreRulesHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<ScoreRuleDto>> Handle(ScoreRulesQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    ""Id"",
                    ""Name"",
                    ""Description"",
                    ""Type"",
                    ""Field"",
                    ""Operator"",
                    ""Value"",
                    ""Score"",
                    ""IsActive"",
                    ""Priority"",
                    ""Category"",
                    ""CreatedAt"",
                    ""UpdatedAt""
                FROM ""LeadScoreRules""
                WHERE ""IsDeleted"" = false
                ORDER BY ""Priority"" ASC, ""CreatedAt"" ASC
            ";

            var data = await _dapper.QueryAsync<ScoreRuleDto>(sql, new { }, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get score rules");
            throw;
        }
    }
}

// ============================================================
// GET SCORE RULE BY ID
// ============================================================

public class ScoreRuleByIdHandler : IRequestHandler<ScoreRuleByIdQry, ScoreRuleDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public ScoreRuleByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<ScoreRuleDto?> Handle(ScoreRuleByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    ""Id"",
                    ""Name"",
                    ""Description"",
                    ""Type"",
                    ""Field"",
                    ""Operator"",
                    ""Value"",
                    ""Score"",
                    ""IsActive"",
                    ""Priority"",
                    ""Category"",
                    ""CreatedAt"",
                    ""UpdatedAt""
                FROM ""LeadScoreRules""
                WHERE ""Id"" = @Id AND ""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var data = await _dapper.QueryFirstOrDefaultAsync<ScoreRuleDto>(sql, parameters, ct);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get score rule by ID: {RuleId}", request.Id);
            throw;
        }
    }
}
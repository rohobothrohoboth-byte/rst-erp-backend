// Cor.CRM/Commands/ScoringCmd.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
namespace Cor.CRM.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class ScoreRuleAddCmd : IRequest<ScoreRuleDto>
{
    public CreateScoreRuleDto Dto { get; set; } = default!;
}

public class ScoreRuleModCmd : IRequest<ScoreRuleDto>
{
    public Guid Id { get; set; }
    public UpdateScoreRuleDto Dto { get; set; } = default!;
}

public class ScoreRuleDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class CalculateScoreCmd : IRequest<ScoreResultDto>
{
    public Guid LeadId { get; set; }
}

// ============================================================
// ADD SCORE RULE
// ============================================================

public class ScoreRuleAddHandler : IRequestHandler<ScoreRuleAddCmd, ScoreRuleDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ScoreRuleAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ScoreRuleDto> Handle(ScoreRuleAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var rule = new LeadScoreRule
            {
                Id = Guid.CreateVersion7(),
                Name = request.Dto.Name,
                Description = request.Dto.Description,
                Type = Enum.Parse<ScoreRuleType>(request.Dto.Type),
                Field = request.Dto.Field,
                Operator = request.Dto.Operator,
                Value = request.Dto.Value,
                Score = request.Dto.Score,
                IsActive = request.Dto.IsActive,
                Priority = request.Dto.Priority,
                Category = request.Dto.Category,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(rule, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Score rule created: {RuleId} - {RuleName}", rule.Id, rule.Name);

            return new ScoreRuleDto
            {
                Id = rule.Id,
                Name = rule.Name,
                Description = rule.Description,
                Type = rule.Type.ToString(),
                Field = rule.Field,
                Operator = rule.Operator,
                Value = rule.Value,
                Score = rule.Score,
                IsActive = rule.IsActive,
                Priority = rule.Priority,
                Category = rule.Category,
                CreatedAt = rule.CreatedAt
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// UPDATE SCORE RULE
// ============================================================

public class ScoreRuleModHandler : IRequestHandler<ScoreRuleModCmd, ScoreRuleDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ScoreRuleModHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ScoreRuleDto> Handle(ScoreRuleModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var rule = await _uow.Set<LeadScoreRule>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (rule == null)
            {
                throw new DomainException($"Score rule with id [{request.Id}] NOT FOUND.");
            }

            if (request.Dto.Name != null)
                rule.Name = request.Dto.Name;
            if (request.Dto.Description != null)
                rule.Description = request.Dto.Description;
            if (request.Dto.Type != null)
                rule.Type = Enum.Parse<ScoreRuleType>(request.Dto.Type);
            if (request.Dto.Field != null)
                rule.Field = request.Dto.Field;
            if (request.Dto.Operator != null)
                rule.Operator = request.Dto.Operator;
            if (request.Dto.Value != null)
                rule.Value = request.Dto.Value;
            if (request.Dto.Score.HasValue)
                rule.Score = request.Dto.Score.Value;
            if (request.Dto.IsActive.HasValue)
                rule.IsActive = request.Dto.IsActive.Value;
            if (request.Dto.Priority.HasValue)
                rule.Priority = request.Dto.Priority.Value;
            if (request.Dto.Category != null)
                rule.Category = request.Dto.Category;

            rule.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(rule);
            await _uow.Commit(ct);

            _logger.LogInformation("Score rule updated: {RuleId} - {RuleName}", rule.Id, rule.Name);

            return new ScoreRuleDto
            {
                Id = rule.Id,
                Name = rule.Name,
                Description = rule.Description,
                Type = rule.Type.ToString(),
                Field = rule.Field,
                Operator = rule.Operator,
                Value = rule.Value,
                Score = rule.Score,
                IsActive = rule.IsActive,
                Priority = rule.Priority,
                Category = rule.Category,
                UpdatedAt = rule.UpdatedAt,
                CreatedAt = rule.CreatedAt
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// DELETE SCORE RULE
// ============================================================

public class ScoreRuleDelHandler : IRequestHandler<ScoreRuleDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ScoreRuleDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(ScoreRuleDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var rule = await _uow.Set<LeadScoreRule>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (rule == null)
            {
                throw new DomainException($"Score rule with id [{request.Id}] NOT FOUND.");
            }

            await _uow.Delete(rule);
            await _uow.Commit(ct);

            _logger.LogInformation("Score rule deleted: {RuleId} - {RuleName}", rule.Id, rule.Name);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// CALCULATE SCORE - SINGLE DEFINITION
// ============================================================
// Cor.CRM/Commands/ScoringCmd.cs

// ============================================================
// CALCULATE SCORE - FIXED VERSION
// ============================================================

public class CalculateScoreHandler : IRequestHandler<CalculateScoreCmd, ScoreResultDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CalculateScoreHandler(
        IUnitOfWork uow,
        IDapperHelper dapper,
        ILogService logger)
    {
        _uow = uow;
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<ScoreResultDto> Handle(CalculateScoreCmd request, CancellationToken ct)
    {
        try
        {
            // Get the lead
            var leadSql = @"
                SELECT
                    ""Id"", ""FirstName"", ""LastName"", ""Email"", ""CompanyName"",
                    ""Status"", ""Source"", ""Priority"", ""Industry"", ""Title"",
                    ""Budget"", ""EstimatedValue"", ""Score"", ""EngagementScore"",
                    ""Tags"", ""Country"", ""City"", ""State""
                FROM ""Leads""
                WHERE ""Id"" = @LeadId AND ""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@LeadId", request.LeadId);

            var lead = await _dapper.QueryFirstOrDefaultAsync<dynamic>(leadSql, parameters, ct);
            if (lead == null)
            {
                throw new DomainException($"Lead with id [{request.LeadId}] NOT FOUND.");
            }

            // Get all active score rules
            var rulesSql = @"
                SELECT
                    ""Id"", ""Name"", ""Description"", ""Type"", ""Field"",
                    ""Operator"", ""Value"", ""Score"", ""Priority"", ""Category""
                FROM ""LeadScoreRules""
                WHERE ""IsActive"" = true AND ""IsDeleted"" = false
                ORDER BY ""Priority"" ASC, ""CreatedAt"" ASC
            ";

            var rules = await _dapper.QueryAsync<LeadScoreRule>(rulesSql, new { }, ct);

            var breakdown = new List<ScoreBreakdownDto>();
            var totalScore = 0;

            // Apply each rule
            foreach (var rule in rules)
            {
                var matched = EvaluateRule(rule, lead);
                if (matched)
                {
                    totalScore += rule.Score;
                    breakdown.Add(new ScoreBreakdownDto
                    {
                        RuleName = rule.Name,
                        Category = rule.Category ?? "General",
                        Score = rule.Score,
                        Matched = true,
                        Details = $"Matched: {rule.Field} {rule.Operator} {rule.Value}"
                    });
                }
                else
                {
                    breakdown.Add(new ScoreBreakdownDto
                    {
                        RuleName = rule.Name,
                        Category = rule.Category ?? "General",
                        Score = 0,
                        Matched = false,
                        Details = $"Did not match: {rule.Field} {rule.Operator} {rule.Value}"
                    });
                }
            }

            // Update the lead score
            var updateSql = @"
                UPDATE ""Leads""
                SET ""Score"" = @Score, ""UpdatedAt"" = @UpdatedAt
                WHERE ""Id"" = @LeadId
            ";

            var updateParams = new DynamicParameters();
            updateParams.Add("@Score", totalScore);
            updateParams.Add("@UpdatedAt", DateTime.UtcNow);
            updateParams.Add("@LeadId", request.LeadId);

            await _dapper.ExecuteAsync(updateSql, updateParams, ct);

            _logger.LogInformation("Score calculated for lead {LeadId}: {Score}", request.LeadId, totalScore);

            return new ScoreResultDto
            {
                LeadId = request.LeadId,
                TotalScore = totalScore,
                Breakdown = breakdown
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate score for lead: {LeadId}", request.LeadId);
            throw;
        }
    }

    private bool EvaluateRule(LeadScoreRule rule, dynamic lead)
    {
        try
        {
            // Get the field value from the lead
            var fieldValue = GetFieldValue(lead, rule.Field);
            if (fieldValue == null) return false;

            // Evaluate based on operator
            var operatorLower = rule.Operator?.ToLower() ?? "";

            switch (operatorLower)
            {
                case "equals":
                    return fieldValue.ToString()?.Equals(rule.Value, StringComparison.OrdinalIgnoreCase) ?? false;

                case "not_equals":
                    return !(fieldValue.ToString()?.Equals(rule.Value, StringComparison.OrdinalIgnoreCase) ?? false);

                case "contains":
                    return fieldValue.ToString()?.Contains(rule.Value, StringComparison.OrdinalIgnoreCase) ?? false;

                case "starts_with":
                    return fieldValue.ToString()?.StartsWith(rule.Value, StringComparison.OrdinalIgnoreCase) ?? false;

                case "ends_with":
                    return fieldValue.ToString()?.EndsWith(rule.Value, StringComparison.OrdinalIgnoreCase) ?? false;

                case "greater_than":
                    return CompareValues(fieldValue, rule.Value) > 0;

                case "less_than":
                    return CompareValues(fieldValue, rule.Value) < 0;

                case "greater_or_equal":
                    return CompareValues(fieldValue, rule.Value) >= 0;

                case "less_or_equal":
                    return CompareValues(fieldValue, rule.Value) <= 0;

                case "between":
                    var values = rule.Value.Split(',');
                    if (values.Length == 2)
                    {
                        var min = values[0].Trim();
                        var max = values[1].Trim();
                        return CompareValues(fieldValue, min) >= 0 && CompareValues(fieldValue, max) <= 0;
                    }
                    return false;

                case "in":
                    var inValues = rule.Value.Split(',').Select(v => v.Trim());
                    return inValues.Any(v => fieldValue.ToString()?.Equals(v, StringComparison.OrdinalIgnoreCase) ?? false);

                default:
                    return false;
            }
        }
        catch (Exception ex)
        {
            // ✅ FIXED: Log the exception message without passing the exception object
            _logger.LogWarning($"Error evaluating rule {rule.Name}: {ex.Message}");
            return false;
        }
    }

    private object? GetFieldValue(dynamic lead, string field)
    {
        if (lead == null) return null;

        try
        {
            // Cast to object first to avoid dynamic lambda issues
            var leadObj = lead as object;
            if (leadObj == null) return null;

            // Try to get the property by name (case sensitive first)
            var type = leadObj.GetType();
            var property = type.GetProperty(field);
            if (property != null)
            {
                return property.GetValue(leadObj);
            }

            // Try case-insensitive
            var properties = type.GetProperties();
            var prop = properties.FirstOrDefault(p =>
                p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));

            return prop?.GetValue(leadObj);
        }
        catch (Exception ex)
        {
            // ✅ FIXED: Log the exception message without passing the exception object
            _logger.LogWarning($"Error getting field value for {field}: {ex.Message}");
            return null;
        }
    }

    private int CompareValues(object fieldValue, string compareValue)
    {
        if (fieldValue == null) return -1;

        // Try numeric comparison
        if (decimal.TryParse(fieldValue.ToString(), out var num1) &&
            decimal.TryParse(compareValue, out var num2))
        {
            return num1.CompareTo(num2);
        }

        // Try date comparison
        if (DateTime.TryParse(fieldValue.ToString(), out var date1) &&
            DateTime.TryParse(compareValue, out var date2))
        {
            return date1.CompareTo(date2);
        }

        // Default: string comparison
        return string.Compare(fieldValue.ToString(), compareValue, StringComparison.OrdinalIgnoreCase);
    }
}
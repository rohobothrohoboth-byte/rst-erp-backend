// Cor.CRM/Models/DTOs/ScoringDto.cs

namespace Cor.CRM.Models.DTOs;

public class ScoreRuleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int Score { get; set; }
    public bool IsActive { get; set; }
    public int Priority { get; set; }
    public string? Category { get; set; }
    public int MatchesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateScoreRuleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = "Demographic";
    public string Field { get; set; } = string.Empty;
    public string Operator { get; set; } = "equals";
    public string Value { get; set; } = string.Empty;
    public int Score { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 0;
    public string? Category { get; set; }
}

public class UpdateScoreRuleDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Field { get; set; }
    public string? Operator { get; set; }
    public string? Value { get; set; }
    public int? Score { get; set; }
    public bool? IsActive { get; set; }
    public int? Priority { get; set; }
    public string? Category { get; set; }
}

public class ScoreResultDto
{
    public Guid LeadId { get; set; }
    public int TotalScore { get; set; }
    public List<ScoreBreakdownDto> Breakdown { get; set; } = new();
}

public class ScoreBreakdownDto
{
    public string RuleName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Score { get; set; }
    public bool Matched { get; set; }
    public string? Details { get; set; }
}
// LeadScoreRule.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum ScoreRuleType
{
    Demographic = 1,
    Behavioral = 2,
    Engagement = 3,
    Firmographic = 4,
    Custom = 5
}

public class LeadScoreRule : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public ScoreRuleType Type { get; set; }

    [Required]
    [MaxLength(100)]
    public string Field { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Operator { get; set; } = string.Empty; // equals, contains, greater_than, etc.

    [Required]
    [MaxLength(500)]
    public string Value { get; set; } = string.Empty;

    public int Score { get; set; }

    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 0;

    [MaxLength(50)]
    public string? Category { get; set; }
}
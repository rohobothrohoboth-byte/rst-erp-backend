// Cor.CRM/Models/DTOs/RoutingDto.cs

using System.Text.Json.Serialization;

namespace Cor.CRM.Models.DTOs;

public class RoutingRuleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Conditions { get; set; }
    public bool IsActive { get; set; }
    public int Priority { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public Guid? AssignedToTeamId { get; set; }
    public string? AssignedToTeamName { get; set; }
    public string? FallbackRule { get; set; }
    public int? MaxLeadsPerDay { get; set; }
    public int MatchesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateRoutingRuleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = "RoundRobin";
    public string? Conditions { get; set; }
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 0;
    public Guid? AssignedToUserId { get; set; }
    public Guid? AssignedToTeamId { get; set; }
    public string? FallbackRule { get; set; }
    public int? MaxLeadsPerDay { get; set; }
}

public class UpdateRoutingRuleDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Conditions { get; set; }
    public bool? IsActive { get; set; }
    public int? Priority { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public Guid? AssignedToTeamId { get; set; }
    public string? FallbackRule { get; set; }
    public int? MaxLeadsPerDay { get; set; }
}

public class RoutingStatsDto
{
    public int TotalRules { get; set; }
    public int ActiveRules { get; set; }
    public int TotalRouted { get; set; }
    public int PendingRouting { get; set; }
    public double AvgResponseTime { get; set; }
    public Dictionary<string, int> RulesByType { get; set; } = new();
    public Dictionary<string, int> RulesByStatus { get; set; } = new();
}
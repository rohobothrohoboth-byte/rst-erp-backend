// LeadRoutingRule.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Cor.CRM.Models.Entities;

public enum RoutingType
{
    RoundRobin = 1,
    LoadBalance = 2,
    SkillBased = 3,
    PriorityBased = 4,
    LocationBased = 5,
    Custom = 6
}

public class LeadRoutingRule : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public RoutingType Type { get; set; }

    [MaxLength(500)]
    public string? Conditions { get; set; } // JSON or expression

    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 0;

    public Guid? AssignedToUserId { get; set; }
    public Guid? AssignedToTeamId { get; set; }

    [MaxLength(50)]
    public string? FallbackRule { get; set; }

    public int? MaxLeadsPerDay { get; set; }
}
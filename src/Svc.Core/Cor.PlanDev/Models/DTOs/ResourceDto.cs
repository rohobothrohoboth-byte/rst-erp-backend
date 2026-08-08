// ResourceDto.cs
namespace Cor.PlanDev.Models.DTOs;

public class ResourceDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid ResourceUserId { get; set; }
    public string? ResourceUserName { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Allocation { get; set; }
    public string Status { get; set; } = "Active";
    public string? ResourceType { get; set; }
    public decimal? HourlyRate { get; set; }
    public int HoursWorked { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class ResourceAllocationDto
{
    public Guid ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public decimal Allocation { get; set; }
    public int HoursWorked { get; set; }
    public decimal TotalCost { get; set; }
}

public class ResourceUtilizationDto
{
    public int TotalResources { get; set; }
    public int ActiveResources { get; set; }
    public int InactiveResources { get; set; }
    public decimal AverageAllocation { get; set; }
    public decimal TotalHoursWorked { get; set; }
    public decimal TotalCost { get; set; }
    public List<ResourceAllocationDto> Resources { get; set; } = new();
}

public class CreateResourceDto
{
    public Guid ProjectId { get; set; }
    public Guid ResourceUserId { get; set; }
    public string? ResourceUserName { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Allocation { get; set; }
    public string? ResourceType { get; set; }
    public decimal? HourlyRate { get; set; }
}

public class UpdateResourceDto
{
    public Guid Id { get; set; }
    public string? Role { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Allocation { get; set; }
    public string? Status { get; set; }
    public decimal? HourlyRate { get; set; }
}
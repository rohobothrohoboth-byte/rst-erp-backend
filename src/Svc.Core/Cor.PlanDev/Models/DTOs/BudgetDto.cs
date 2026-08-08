namespace Cor.PlanDev.Models.DTOs;

public class BudgetDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PlannedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal Variance { get; set; }
    public int PlannedQuantity { get; set; }
    public int ActualQuantity { get; set; }
    public string? Unit { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BudgetType { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateBudgetDto
{
    public Guid ProjectId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PlannedAmount { get; set; }
    public int PlannedQuantity { get; set; }
    public string? Unit { get; set; }
    public string? BudgetType { get; set; }
}

public class UpdateBudgetDto
{
    public Guid Id { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public decimal? PlannedAmount { get; set; }
    public decimal? ActualAmount { get; set; }
    public int? PlannedQuantity { get; set; }
    public int? ActualQuantity { get; set; }
    public string? Unit { get; set; }
    public string? Status { get; set; }
    public string? RowVersion { get; set; }
}
public class BudgetSummaryDto
{
    public Guid ProjectId { get; set; }
    public decimal TotalPlanned { get; set; }
    public decimal TotalActual { get; set; }
    public decimal TotalVariance { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public List<BudgetDto> BudgetItems { get; set; } = new();
}
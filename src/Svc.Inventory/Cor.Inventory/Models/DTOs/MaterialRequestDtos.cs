namespace Cor.Inventory.Models.DTOs;

public class MaterialRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "Pending";
    public Guid? DecidedByUserId { get; set; }
    public string? DecidedByName { get; set; }
    public string? DecisionNote { get; set; }
    public DateTime? DecisionDate { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateMaterialRequestDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string? Reason { get; set; }
}

public class MaterialRequestDecisionDto
{
    public string? Note { get; set; }
}

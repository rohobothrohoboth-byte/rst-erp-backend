namespace Cor.Inventory.Models.DTOs;

public class ReorderRuleDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid? WarehouseId { get; set; }
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateReorderRuleDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid? WarehouseId { get; set; }
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateReorderRuleDto
{
    public Guid Id { get; set; }
    public string? ProductName { get; set; }
    public Guid? WarehouseId { get; set; }
    public int? MinLevel { get; set; }
    public int? MaxLevel { get; set; }
    public int? ReorderQuantity { get; set; }
    public bool? IsActive { get; set; }
    public string? RowVersion { get; set; }
}

public class ReorderAlertDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid WarehouseId { get; set; }
    public int QuantityOnHand { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
}

public class ReorderRequestDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid? WarehouseId { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Reason { get; set; }
    public Guid? DecidedByUserId { get; set; }
    public string? DecidedByName { get; set; }
    public string? DecisionNote { get; set; }
    public DateTime? DecisionDate { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateReorderRequestDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid? WarehouseId { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
}

public class ReorderDecisionDto
{
    public Guid? DecidedByUserId { get; set; }
    public string? DecidedByName { get; set; }
    public string? DecisionNote { get; set; }
}

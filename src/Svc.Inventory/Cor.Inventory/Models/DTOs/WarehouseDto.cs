namespace Cor.Inventory.Models.DTOs;

public class WarehouseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? WarehouseType { get; set; }
    public string? Status { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}
public class StockLevelDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int QuantityOnHand { get; set; }
    public int QuantityReserved { get; set; }
    public int QuantityAvailable { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public DateTime? LastReceivedDate { get; set; }
    public DateTime? LastIssuedDate { get; set; }
    public decimal? AverageCost { get; set; }
    public decimal? LastUnitCost { get; set; }
    public string? BinLocation { get; set; }
    public int? ShelfNumber { get; set; }
    public int? RackNumber { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}
public class CreateWarehouseDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? WarehouseType { get; set; }
    public string? Status { get; set; } = "Active";
    public bool IsActive { get; set; } = true;
}

public class UpdateWarehouseDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Location { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? WarehouseType { get; set; }
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
    public string? RowVersion { get; set; }
}
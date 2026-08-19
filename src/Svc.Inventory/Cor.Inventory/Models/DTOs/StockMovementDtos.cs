namespace Cor.Inventory.Models.DTOs;

public class StockMovementDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? ToWarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Reference { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Posted";
    public DateTime MovementDate { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class InboundStockDto
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Reference { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class OutboundStockDto
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Reference { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class TransferStockDto
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Reference { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class AdjustmentStockDto
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }

    // When true, Quantity is treated as a delta applied to the current on-hand.
    // When false (default), Quantity is the new absolute on-hand value.
    public bool IsDelta { get; set; }

    public int Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Reference { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

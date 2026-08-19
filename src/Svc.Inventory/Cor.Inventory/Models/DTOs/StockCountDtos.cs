namespace Cor.Inventory.Models.DTOs;

public class StockCountDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string Status { get; set; } = "Open";
    public DateTime? ScheduledDate { get; set; }
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public List<StockCountLineDto> Lines { get; set; } = new();
}

public class StockCountLineDto
{
    public Guid Id { get; set; }
    public Guid StockCountId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public int SystemQuantity { get; set; }
    public int CountedQuantity { get; set; }
    public int Variance { get; set; }
}

public class CreateStockCountDto
{
    public Guid WarehouseId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? Notes { get; set; }
}

public class RecordStockCountLineDto
{
    public Guid ProductId { get; set; }
    public int CountedQuantity { get; set; }
}

public class RecordStockCountDto
{
    public List<RecordStockCountLineDto> Lines { get; set; } = new();
}

namespace Cor.Inventory.Models.DTOs;

public class DashboardStatsDto
{
    public int ProductCount { get; set; }
    public int WarehouseCount { get; set; }
    public decimal TotalStockValue { get; set; }
    public int LowStockCount { get; set; }
    public List<StockMovementDto> RecentMovements { get; set; } = new();
}

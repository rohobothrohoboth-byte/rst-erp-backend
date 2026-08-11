namespace Cor.Inventory.Models.DTOs;

public class StockSummaryDto
{
    public int ProductCount { get; set; }
    public int TotalOnHand { get; set; }
    public decimal TotalValue { get; set; }
    public int LowStockCount { get; set; }
}

public class MovementAnalysisItemDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public int TotalQuantity { get; set; }
    public int MovementCount { get; set; }
}

public class MovementAnalysisDto
{
    public List<MovementAnalysisItemDto> FastMovers { get; set; } = new();
    public List<MovementAnalysisItemDto> SlowMovers { get; set; } = new();
}

public class ForecastItemDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal AverageMonthlyOutbound { get; set; }
    public decimal ProjectedNextMonthOutbound { get; set; }
    public int CurrentOnHand { get; set; }
    public decimal ProjectedEndOfMonthOnHand { get; set; }
}

public class ForecastDto
{
    public int MonthsAnalyzed { get; set; }
    public List<ForecastItemDto> Items { get; set; } = new();
}

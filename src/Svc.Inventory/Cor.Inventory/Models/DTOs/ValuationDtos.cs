namespace Cor.Inventory.Models.DTOs;

public class ValuationMethodDto
{
    public string Method { get; set; } = "AVG"; // "AVG" | "FIFO" | "LIFO"
}

public class ValuationReportLineDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid WarehouseId { get; set; }
    public int QuantityOnHand { get; set; }
    public decimal AverageCost { get; set; }
    public decimal Value { get; set; }
}

public class ValuationReportDto
{
    public string Method { get; set; } = "AVG";
    public Guid? WarehouseId { get; set; }
    public List<ValuationReportLineDto> Lines { get; set; } = new();
    public int TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
}

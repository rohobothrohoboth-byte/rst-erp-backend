namespace Cor.Inventory.Models.DTOs;

public class BarcodeProductDto
{
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
}

public class GenerateBarcodeDto
{
    public string? Barcode { get; set; }
}

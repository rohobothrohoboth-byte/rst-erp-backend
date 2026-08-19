namespace Cor.Inventory.Models.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateProductDto
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateProductDto
{
    public Guid Id { get; set; }
    public string? Sku { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? UnitId { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public bool? IsActive { get; set; }
    public string? RowVersion { get; set; }
}

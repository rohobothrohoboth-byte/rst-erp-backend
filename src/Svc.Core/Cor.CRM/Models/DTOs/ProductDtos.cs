using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs
{
    public class ProductDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SKU { get; set; }
        public decimal Price { get; set; }
        public decimal? Cost { get; set; }
        public decimal? WholesalePrice { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? UnitOfMeasure { get; set; }
        public int? StockQuantity { get; set; }
        public int? ReorderLevel { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SKU { get; set; }
        public decimal Price { get; set; }
        public decimal? Cost { get; set; }
        public string? Type { get; set; }
        public string? Brand { get; set; }
        public string? UnitOfMeasure { get; set; }
        public int? StockQuantity { get; set; }
        public int? ReorderLevel { get; set; }
        public bool IsTaxable { get; set; } = true;
    }

    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Cost { get; set; }
        public string? Status { get; set; }
        public int? StockQuantity { get; set; }
        public int? ReorderLevel { get; set; }
    }
}

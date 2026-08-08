// Product.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum ProductStatus
{
    Active = 1,
    Inactive = 2,
    Discontinued = 3,
    ComingSoon = 4
}

public enum ProductType
{
    Product = 1,
    Service = 2,
    Bundle = 3
}

public class Product : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? SKU { get; set; }

    [MaxLength(50)]
    public string? Barcode { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Cost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? WholesalePrice { get; set; }

    public ProductType Type { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Active;

    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }

    [MaxLength(200)]
    public string? Brand { get; set; }

    [MaxLength(100)]
    public string? UnitOfMeasure { get; set; }

    public int? StockQuantity { get; set; }
    public int? ReorderLevel { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public string? SpecificationsJson { get; set; }
    public string? FeaturesJson { get; set; }

    public bool IsTaxable { get; set; } = true;
    public decimal? TaxRate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<OpportunityProduct> OpportunityProducts { get; set; } = new List<OpportunityProduct>();
    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
    public virtual ICollection<QuoteLine> QuoteLines { get; set; } = new List<QuoteLine>();
}
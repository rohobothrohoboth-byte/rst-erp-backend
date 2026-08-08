// Models/Entities/Product.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class Product : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameAm { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? CostPrice { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }

    [MaxLength(50)]
    public string? SubCategory { get; set; }

    [MaxLength(50)]
    public string? UnitOfMeasure { get; set; } = "EA"; // EA, KG, M, L, etc.

    [MaxLength(20)]
    public string? TaxRate { get; set; }

    public int? StockQuantity { get; set; }

    public int? ReorderLevel { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; } = "Active"; // Active, Inactive, Discontinued

    public bool IsActive { get; set; } = true;

    public Guid? CategoryId { get; set; }

    [Column(TypeName = "jsonb")]
    public string? Attributes { get; set; } // JSON for additional attributes

    // ✅ PeriodId is inherited from BaseEntity

    // Navigation
    public virtual ICollection<ProductPrice> PriceHistory { get; set; } = new List<ProductPrice>();
    public virtual ICollection<ProductInventory> InventoryTransactions { get; set; } = new List<ProductInventory>();
}

public class ProductPrice : BaseEntity
{
    [Required]
    public Guid ProductId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [MaxLength(10)]
    public string? Currency { get; set; } = "USD";

    public DateTime EffectiveDate { get; set; }

    public DateTime? EndDate { get; set; }

    [MaxLength(50)]
    public string? PriceType { get; set; } // Regular, Promotional, Wholesale, Retail

    [ForeignKey(nameof(ProductId))]
    public virtual Product? Product { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
}

public class ProductInventory : BaseEntity
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [MaxLength(20)]
    public string TransactionType { get; set; } = string.Empty; // StockIn, StockOut, Adjustment, Return

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public Guid? ReferenceId { get; set; } // PurchaseOrderId, SalesOrderId, etc.

    [MaxLength(50)]
    public string? ReferenceType { get; set; } // PurchaseOrder, SalesOrder, Adjustment

    [ForeignKey(nameof(ProductId))]
    public virtual Product? Product { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
}
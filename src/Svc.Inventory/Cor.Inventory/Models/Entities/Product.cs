using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Inventory.Models.Entities;

public class Product : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    public Guid UnitId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ReorderLevel { get; set; }

    [MaxLength(100)]
    public string? Barcode { get; set; }

    public bool IsActive { get; set; } = true;
}

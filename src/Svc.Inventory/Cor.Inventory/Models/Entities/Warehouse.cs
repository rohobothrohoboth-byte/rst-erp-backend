using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Inventory.Models.Entities;

public class Warehouse : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Location { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? State { get; set; }

    [MaxLength(50)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? ZipCode { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? WarehouseType { get; set; } // Main, Sub, Distribution, Store

    [MaxLength(50)]
    public string? Status { get; set; } = "Active";

    public bool IsActive { get; set; } = true;

    [Column(TypeName = "jsonb")]
    public string? Metadata { get; set; } // JSON for additional fields

    // Navigation
    public virtual ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
}
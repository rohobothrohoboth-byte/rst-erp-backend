using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Inventory.Models.Entities;

public class StockLevel : BaseEntity
{
    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [MaxLength(50)]
    public string? ProductCode { get; set; }

    [MaxLength(200)]
    public string? ProductName { get; set; }

    [Required]
    public int QuantityOnHand { get; set; }

    public int QuantityReserved { get; set; }

    // ✅ Computed property - NOT mapped to database
    [NotMapped]
    public int QuantityAvailable => QuantityOnHand - QuantityReserved;

    public int ReorderLevel { get; set; }

    public int ReorderQuantity { get; set; }

    public DateTime? LastReceivedDate { get; set; }

    public DateTime? LastIssuedDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AverageCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? LastUnitCost { get; set; }

    [MaxLength(50)]
    public string? BinLocation { get; set; }

    public int? ShelfNumber { get; set; }

    public int? RackNumber { get; set; }

    // Navigation
    [ForeignKey(nameof(WarehouseId))]
    public virtual Warehouse? Warehouse { get; set; }
}
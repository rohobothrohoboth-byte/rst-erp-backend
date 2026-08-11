using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Inventory.Models.Entities;

public class StockMovement : BaseEntity
{
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = "Inbound"; // "Inbound" | "Outbound" | "Transfer" | "Adjustment"

    [Required]
    public Guid ProductId { get; set; }

    [MaxLength(200)]
    public string? ProductName { get; set; }

    [Required]
    public Guid WarehouseId { get; set; }

    public Guid? ToWarehouseId { get; set; } // used for Transfer

    [Required]
    public int Quantity { get; set; } // positive; direction implied by Type

    [Column(TypeName = "decimal(18,2)")]
    public decimal? UnitCost { get; set; }

    [MaxLength(100)]
    public string? Reference { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Posted"; // "Draft" | "Posted"

    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
}

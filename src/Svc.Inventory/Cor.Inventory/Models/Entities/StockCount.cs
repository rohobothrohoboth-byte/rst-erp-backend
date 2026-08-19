using System.ComponentModel.DataAnnotations;

namespace Cor.Inventory.Models.Entities;

public class StockCount : BaseEntity
{
    [Required]
    public Guid WarehouseId { get; set; }

    [MaxLength(200)]
    public string? WarehouseName { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Open"; // "Open" | "Counted" | "Reconciled"

    public DateTime? ScheduledDate { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

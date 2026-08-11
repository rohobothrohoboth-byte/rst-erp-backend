using System.ComponentModel.DataAnnotations;

namespace Cor.Inventory.Models.Entities;

public class StockCountLine : BaseEntity
{
    [Required]
    public Guid StockCountId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [MaxLength(200)]
    public string? ProductName { get; set; }

    public int SystemQuantity { get; set; }

    public int CountedQuantity { get; set; }

    public int Variance { get; set; } // Variance = Counted - System
}

using System.ComponentModel.DataAnnotations;

namespace Cor.Inventory.Models.Entities;

public class ReorderRule : BaseEntity
{
    [Required]
    public Guid ProductId { get; set; }

    [MaxLength(200)]
    public string? ProductName { get; set; }

    public Guid? WarehouseId { get; set; }

    public int MinLevel { get; set; }

    public int MaxLevel { get; set; }

    public int ReorderQuantity { get; set; }

    public bool IsActive { get; set; } = true;
}

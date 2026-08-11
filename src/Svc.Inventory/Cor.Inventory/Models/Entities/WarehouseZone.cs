using System.ComponentModel.DataAnnotations;

namespace Cor.Inventory.Models.Entities;

public class WarehouseZone : BaseEntity
{
    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Code { get; set; }

    [MaxLength(50)]
    public string? ZoneType { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}

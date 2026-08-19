using System.ComponentModel.DataAnnotations;

namespace Cor.Inventory.Models.Entities;

public class Bin : BaseEntity
{
    [Required]
    public Guid ZoneId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? Capacity { get; set; }

    public bool IsActive { get; set; } = true;
}

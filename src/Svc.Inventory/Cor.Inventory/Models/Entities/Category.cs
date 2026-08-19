using System.ComponentModel.DataAnnotations;

namespace Cor.Inventory.Models.Entities;

public class Category : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Code { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public Guid? ParentId { get; set; }

    public bool IsActive { get; set; } = true;
}

using System.ComponentModel.DataAnnotations;

namespace Cor.Inventory.Models.Entities;

public class ValuationSetting : BaseEntity
{
    [Required]
    [MaxLength(10)]
    public string Method { get; set; } = "AVG"; // "AVG" | "FIFO" | "LIFO"
}

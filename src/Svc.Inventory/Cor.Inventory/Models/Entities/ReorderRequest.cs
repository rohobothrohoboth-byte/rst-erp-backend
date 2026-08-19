using System.ComponentModel.DataAnnotations;

namespace Cor.Inventory.Models.Entities;

public class ReorderRequest : BaseEntity
{
    [Required]
    public Guid ProductId { get; set; }

    [MaxLength(200)]
    public string? ProductName { get; set; }

    public Guid? WarehouseId { get; set; }

    public int Quantity { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // "Pending" | "Approved" | "Rejected" | "Converted"

    [MaxLength(500)]
    public string? Reason { get; set; }

    public Guid? DecidedByUserId { get; set; }

    [MaxLength(200)]
    public string? DecidedByName { get; set; }

    [MaxLength(1000)]
    public string? DecisionNote { get; set; }

    public DateTime? DecisionDate { get; set; }
}

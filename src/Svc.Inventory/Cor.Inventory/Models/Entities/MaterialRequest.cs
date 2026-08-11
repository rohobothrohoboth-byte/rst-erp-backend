using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Inventory.Models.Entities;

public class MaterialRequest : BaseEntity
{
    [Required]
    public Guid EmployeeId { get; set; }

    [MaxLength(200)]
    public string? EmployeeName { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; }

    [MaxLength(1000)]
    public string? Reason { get; set; }

    // Pending | Approved | Rejected | Issued
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    public Guid? DecidedByUserId { get; set; }

    [MaxLength(200)]
    public string? DecidedByName { get; set; }

    [MaxLength(1000)]
    public string? DecisionNote { get; set; }

    public DateTime? DecisionDate { get; set; }
}

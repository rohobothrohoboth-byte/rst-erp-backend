using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Inventory.Models.Entities;

public class MaterialAssignment : BaseEntity
{
    [Required]
    public Guid EmployeeId { get; set; }

    [MaxLength(200)]
    public string? EmployeeName { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; }

    public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

    // Issued | Returned
    [MaxLength(50)]
    public string Status { get; set; } = "Issued";

    public DateTime? ReturnedDate { get; set; }

    public Guid? RequestId { get; set; }

    [MaxLength(1000)]
    public string? Note { get; set; }
}

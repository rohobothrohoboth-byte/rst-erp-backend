// Models/Entities/SalesOrder.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class SalesOrder : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    public DateTime OrderDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public Guid? CustomerId { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Draft"; // Draft, Confirmed, Processing, Shipped, Delivered, Cancelled

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public Guid? BranchId { get; set; }
    public Guid? SalesRepId { get; set; }

    // ✅ PeriodId inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    public virtual ICollection<SalesOrderLine> Lines { get; set; } = new List<SalesOrderLine>();
}

public class SalesOrderLine : BaseEntity
{
    [Required]
    public Guid SalesOrderId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [ForeignKey(nameof(SalesOrderId))]
    public virtual SalesOrder? SalesOrder { get; set; }
}
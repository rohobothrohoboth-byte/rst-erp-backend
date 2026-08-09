// Models/Entities/Invoice.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Finance.Models.Entities.Local;
namespace Cor.Finance.Models.Entities;

public class Invoice : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    public DateTime InvoiceDate { get; set; }

    public DateTime? DueDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PaidAmount { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Draft";

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string InvoiceType { get; set; } = "Purchase"; // "Purchase" (AP) or "Sales" (AR)

    // AP (Purchase Invoices)
    public Guid? VendorId { get; set; }

    // AR (Sales Invoices)
    public Guid? CustomerId { get; set; }

    // ✅ Branch, Department, Employee
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }

    [MaxLength(100)]
    public string? SalesRep { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public Guid? PurchaseOrderId { get; set; }
    public DateTime? ReceivedDate { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // ============================================================
    // NAVIGATION PROPERTIES
    // ============================================================

    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    // ✅ ADD THESE NAVIGATION PROPERTIES
    [ForeignKey(nameof(BranchId))]
    public virtual LocalBranch? Branch { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    public virtual LocalDepartment? Department { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public virtual LocalEmployee? Employee { get; set; }

    // ✅ Period navigation (if not already in BaseEntity)
    [ForeignKey(nameof(PeriodId))]
    public new virtual FinancialPeriod? Period { get; set; }

    // Collections
    public virtual ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
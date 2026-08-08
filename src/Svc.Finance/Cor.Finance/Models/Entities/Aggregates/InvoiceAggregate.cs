// Models/Entities/Aggregates/InvoiceAggregate.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace Cor.Finance.Models.Entities.Aggregates;

[Table("InvoiceAggregates")]
[Index(nameof(AggregateDate))]
[Index(nameof(Period))]
[Index(nameof(AggregateType))]
public class InvoiceAggregate
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime AggregateDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Period { get; set; } = string.Empty; // "2024-01", "2024-Q1", "2024"

    [Required]
    [MaxLength(50)]
    public string AggregateType { get; set; } = string.Empty; // "Daily", "Monthly", "Quarterly", "Yearly"

    // Invoice metrics
    public int TotalInvoices { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPaid { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalBalance { get; set; }

    public int PaidCount { get; set; }
    public int OverdueCount { get; set; }
    public int DraftCount { get; set; }

    // Averages
    [Column(TypeName = "decimal(18,2)")]
    public decimal AverageInvoiceAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AveragePaymentDays { get; set; }

    // By type
    public int SalesCount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SalesAmount { get; set; }

    public int PurchaseCount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchaseAmount { get; set; }

    // Timestamps
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
}


// Models/Entities/PaymentSchedule.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class PaymentSchedule : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public Guid? VendorId { get; set; }

    public Guid? CustomerId { get; set; }

    [MaxLength(50)]
    public string? PaymentType { get; set; } // AP_Payment, AR_Receipt

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(10)]
    public string? Currency { get; set; } = "USD";

    [MaxLength(50)]
    public string Frequency { get; set; } = "Monthly"; // Daily, Weekly, Monthly, Quarterly, SemiAnnually, Annually

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime NextPaymentDate { get; set; }

    public int? OccurrenceCount { get; set; } // Number of occurrences (if limited)
    public int? CurrentOccurrence { get; set; } // Current occurrence number

    [MaxLength(50)]
    public string Status { get; set; } = "Active"; // Active, Paused, Completed, Cancelled

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? PaymentMethod { get; set; } // BankTransfer, CreditCard, Check, Cash

    [MaxLength(100)]
    public string? AccountNumber { get; set; }

    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }

    public Guid? BankAccountId { get; set; }

    // ✅ PeriodId is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    [ForeignKey(nameof(BankAccountId))]
    public virtual BankAccount? BankAccount { get; set; }

    public virtual ICollection<PaymentScheduleHistory> History { get; set; } = new List<PaymentScheduleHistory>();
}

public class PaymentScheduleHistory : BaseEntity
{
    [Required]
    public Guid PaymentScheduleId { get; set; }

    public DateTime ScheduledDate { get; set; }
    public DateTime? ProcessedDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Processed, Failed, Skipped

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public Guid? PaymentId { get; set; } // Link to actual payment

    // ✅ PeriodId is inherited from BaseEntity

    [ForeignKey(nameof(PaymentScheduleId))]
    public virtual PaymentSchedule? PaymentSchedule { get; set; }
}
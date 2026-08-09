// src/Svc.Procurement/Cor.Procurement/Models/Entities/FinancialPeriod.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Procurement.Models.Enums;

namespace Cor.Procurement.Models.Entities.Local;

public class FinancialPeriod : LocalBaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Code { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime EndDate { get; set; }

    [Required]
    public PeriodType PeriodType { get; set; } = PeriodType.MONTHLY;

    [Required]
    public PeriodStatus Status { get; set; } = PeriodStatus.OPEN;

    public bool IsClosed { get; set; } = false;

    public DateTime? ClosedDate { get; set; }

    public Guid? ClosedBy { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public string? FiscalYear { get; set; }

    public bool IsActive { get; set; } = true;

    // Local copy tracking
    public new DateTime? SyncedAt { get; set; }
    public Guid? SourceId { get; set; } // ID from Finance module

    public void ValidateDates()
    {
        if (StartDate > EndDate)
        {
            throw new InvalidOperationException("Start date must be before end date");
        }
    }
}
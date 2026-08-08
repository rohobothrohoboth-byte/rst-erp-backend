// Models/Entities/FinancialPeriod.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Finance.Models.Enums;

namespace Cor.Finance.Models.Entities
{
    public class FinancialPeriod : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

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

        [Column(TypeName = "jsonb")]
        public string? ClosingMetadataJson { get; set; }

        [Column(TypeName = "jsonb")]
        public string? MetadataJson { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        // ? PeriodId is NOT needed here (this IS the period)
        // This entity does NOT inherit PeriodId from BaseEntity - it IS the period

        // Navigation Properties
        public virtual ICollection<AuditLog>? AuditLogs { get; set; }

        // Non-mapped properties (calculated)
        [NotMapped]
        public int TotalEntries { get; set; }

        [NotMapped]
        public int PostedEntries { get; set; }

        [NotMapped]
        public int UnpostedEntries { get; set; }

        // Custom validation
        public void ValidateDates()
        {
            if (StartDate > EndDate)
            {
                throw new InvalidOperationException("Start date must be before end date");
            }
        }

        public void GenerateNameIfNotProvided()
        {
            if (string.IsNullOrWhiteSpace(Name) && StartDate != default)
            {
                Name = $"{StartDate:MMMM yyyy}";
            }
        }
    }
}
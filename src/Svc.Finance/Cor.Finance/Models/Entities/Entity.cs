// Models/Entities/Entity.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class Entity : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string LegalName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Type { get; set; } // 'Parent' | 'Subsidiary' | 'Associate' | 'JointVenture'

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(10)]
    public string? Currency { get; set; }

    public DateTime? FiscalYearStart { get; set; }
    public DateTime? FiscalYearEnd { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? OwnershipPercentage { get; set; }

    [MaxLength(50)]
    public string? ConsolidationMethod { get; set; } // 'Full' | 'Equity' | 'Proportionate' | 'None'

    [MaxLength(50)]
    public string? RegistrationNumber { get; set; }

    [MaxLength(50)]
    public string? TaxId { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(200)]
    public string? Website { get; set; }

    public Guid? ParentEntityId { get; set; }

    // ✅ This field is expected by the frontend
    public bool IsConsolidated { get; set; } = false;

    public bool IsActive { get; set; } = true;

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(ParentEntityId))]
    public virtual Entity? ParentEntity { get; set; }

    public virtual ICollection<Entity> Subsidiaries { get; set; } = new List<Entity>();
}
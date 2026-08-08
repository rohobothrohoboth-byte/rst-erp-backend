// Models/Entities/ProfitCenter.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class ProfitCenter : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(200)]
    public string? Manager { get; set; }

    [MaxLength(100)]
    public string? Region { get; set; }

    public Guid? ParentId { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(ParentId))]
    public virtual ProfitCenter? Parent { get; set; }

    public virtual ICollection<ProfitCenter> Children { get; set; } = new List<ProfitCenter>();
}
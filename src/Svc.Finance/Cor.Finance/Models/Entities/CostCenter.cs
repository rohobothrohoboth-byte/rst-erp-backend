// Models/Entities/CostCenter.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class CostCenter : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameAm { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? DepartmentId { get; set; }

    [MaxLength(200)]
    public string? BudgetHolder { get; set; }

    public Guid? ParentId { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(ParentId))]
    public virtual CostCenter? Parent { get; set; }

    public virtual ICollection<CostCenter> Children { get; set; } = new List<CostCenter>();
}
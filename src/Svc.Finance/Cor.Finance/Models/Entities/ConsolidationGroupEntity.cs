// Models/Entities/ConsolidationGroupEntity.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class ConsolidationGroupEntity : BaseEntity  // ✅ Inherit from BaseEntity
{
    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public Guid EntityId { get; set; }

    // ✅ PeriodId is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(GroupId))]
    public virtual ConsolidationGroup? Group { get; set; }

    [ForeignKey(nameof(EntityId))]
    public virtual Entity? Entity { get; set; }
}
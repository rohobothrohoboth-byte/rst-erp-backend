// Models/Entities/Category.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Procurement.Models.Entities;

[Table("Categories")]
public class Category : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? NameAm { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public Guid? ParentId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public DateTime DateAdd { get; set; }

    public DateTime? DateMod { get; set; }

    [ForeignKey(nameof(ParentId))]
    public virtual Category? Parent { get; set; }

    public virtual ICollection<Category> Children { get; set; } = new List<Category>();
}
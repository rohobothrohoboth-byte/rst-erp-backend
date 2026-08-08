// Department.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public class Department : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public Guid? ManagerId { get; set; }

    [MaxLength(50)]
    public string? Code { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
   // [ForeignKey("ManagerId")]
   // public virtual User? Manager { get; set; }

   // public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
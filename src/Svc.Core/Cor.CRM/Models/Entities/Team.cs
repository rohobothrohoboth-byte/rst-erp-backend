// Team.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public class Team : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public Guid? DepartmentId { get; set; }
    public Guid? TeamLeadId { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(50)]
    public string? Code { get; set; }

    // Navigation Properties
    [ForeignKey("DepartmentId")]
    public virtual Department? Department { get; set; }

   // [ForeignKey("TeamLeadId")]
  //  public virtual User? TeamLead { get; set; }

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
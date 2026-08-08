// TeamMember.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum TeamRole
{
    Member = 1,
    Lead = 2,
    Manager = 3
}

public class TeamMember : BaseEntity
{
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }

    public TeamRole Role { get; set; } = TeamRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; set; }
    public bool IsActive { get; set; } = true;

    [ForeignKey("TeamId")]
    public virtual Team Team { get; set; } = null!;

   // [ForeignKey("UserId")]
   // public virtual User User { get; set; } = null!;
}
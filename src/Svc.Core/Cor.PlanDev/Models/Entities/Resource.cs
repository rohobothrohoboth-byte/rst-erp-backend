using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.PlanDev.Models.Entities;

public class Resource : BaseEntity
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid ResourceUserId { get; set; }
    public string? ResourceUserName { get; set; }

    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal Allocation { get; set; } // Percentage

    [MaxLength(50)]
    public string Status { get; set; } = "Active"; // Active, Inactive, Completed

    [MaxLength(50)]
    public string? ResourceType { get; set; } // FullTime, PartTime, Contractor, Consultant

    [Column(TypeName = "decimal(18,2)")]
    public decimal? HourlyRate { get; set; }

    public int HoursWorked { get; set; }

    // Navigation
    [ForeignKey(nameof(ProjectId))]
    public virtual Project? Project { get; set; }
}
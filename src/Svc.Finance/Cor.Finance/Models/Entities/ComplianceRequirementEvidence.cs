// Models/Entities/ComplianceRequirementEvidence.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class ComplianceRequirementEvidence : BaseEntity  // ✅ Inherit from BaseEntity
{
    [Required]
    public Guid ComplianceRequirementId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? FilePath { get; set; }

    [MaxLength(100)]
    public string? FileType { get; set; }

    public long? FileSize { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(200)]
    public string? UploadedBy { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(ComplianceRequirementId))]
    public virtual ComplianceRequirement ComplianceRequirement { get; set; } = null!;
}
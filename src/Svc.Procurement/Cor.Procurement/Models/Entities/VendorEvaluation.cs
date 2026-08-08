using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Procurement.Models.Entities.Local;
namespace Cor.Procurement.Models.Entities;

public class VendorEvaluation : BaseEntity
{
    [Required]
    public Guid VendorId { get; set; }

    [MaxLength(100)]
    public string? VendorName { get; set; }

    [MaxLength(50)]
    public string? VendorCode { get; set; }

    [Required]
    public int OverallScore { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    [Required]
    public DateTime EvaluationDate { get; set; }

    [MaxLength(100)]
    public string? Evaluator { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Good"; // Excellent, Good, Average, Poor

    [Column(TypeName = "jsonb")]
    public string? CriteriaJson { get; set; }

    [Column(TypeName = "jsonb")]
    public string? StrengthsJson { get; set; }

    [Column(TypeName = "jsonb")]
    public string? WeaknessesJson { get; set; }

    [Column(TypeName = "jsonb")]
    public string? RecommendationsJson { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation
    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }
}
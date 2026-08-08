using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Procurement.Models.Entities;

public class Inspection : BaseEntity
{
    [Required]
    public Guid GoodsReceiptNoteId { get; set; }

    [Required]
    [MaxLength(50)]
    public string InspectionNumber { get; set; } = string.Empty;

    [Required]
    public DateTime InspectionDate { get; set; }

    [MaxLength(100)]
    public string? InspectorId { get; set; }

    [MaxLength(200)]
    public string? InspectorName { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "InProgress"; // Pending, InProgress, Completed, Failed

    [MaxLength(50)]
    public string? Department { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal QualityScore { get; set; }

    public DateTime? CompletedDate { get; set; }

    [Column(TypeName = "jsonb")]
    public string? TeamMembersJson { get; set; } // JSON array of team members

    public int TotalItems { get; set; }
    public int ItemsPassed { get; set; }
    public int ItemsFailed { get; set; }

    // Navigation
    [ForeignKey(nameof(GoodsReceiptNoteId))]
    public virtual GoodsReceiptNote? GoodsReceiptNote { get; set; }

    public virtual ICollection<InspectionItem> Items { get; set; } = new List<InspectionItem>();
}
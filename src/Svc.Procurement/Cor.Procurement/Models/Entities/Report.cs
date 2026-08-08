using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Procurement.Models.Entities;

public class Report : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // spend, vendor, performance, inventory, compliance

    [MaxLength(50)]
    public string Type { get; set; } = string.Empty; // dashboard, detailed, summary

    [Required]
    public DateTime GeneratedDate { get; set; }

    [MaxLength(50)]
    public string Period { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Status { get; set; } = "ready"; // ready, generating, scheduled

    [MaxLength(10)]
    public string Format { get; set; } = "pdf"; // pdf, excel, csv

    [MaxLength(50)]
    public string? Size { get; set; }

    public int Downloads { get; set; }

    public DateTime? LastViewed { get; set; }

    [Column(TypeName = "jsonb")]
    public string? TagsJson { get; set; }

    [MaxLength(500)]
    public string? ReportUrl { get; set; }

    [Column(TypeName = "jsonb")]
    public string? DataJson { get; set; } // Store report data as JSON
}
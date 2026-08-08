// Document.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public class Document : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Title { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public string? FileHash { get; set; }

    public Guid? EntityId { get; set; }
    public string? EntityType { get; set; } // Lead, Customer, Opportunity, etc.

    [MaxLength(50)]
    public string? Category { get; set; }

    [MaxLength(500)]
    public string? Tags { get; set; }

    public bool IsPublic { get; set; } = false;
    public int DownloadCount { get; set; } = 0;
    public DateTime? LastDownloadDate { get; set; }

    [MaxLength(50)]
    public string? Version { get; set; }

    public Guid? ParentDocumentId { get; set; }

    public string? MetadataJson { get; set; }
}
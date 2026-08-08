// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\Entities\FileDocument.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.FileManagement.Models.Entities;

public class FileDocument
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; }

    [Required]
    [MaxLength(500)]
    public string OriginalFileName { get; set; }

    [Required]
    public long FileSize { get; set; }

    [Required]
    [MaxLength(100)]
    public string FileType { get; set; }

    [Required]
    [MaxLength(100)]
    public string FileExtension { get; set; }

    [Required]
    [MaxLength(1000)]
    public string FilePath { get; set; }
    [MaxLength(500)]
    public string? ThumbnailPath { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    [MaxLength(100)]
    public string StorageProvider { get; set; } = "Local";

    [Required]
    [MaxLength(50)]
    public string Module { get; set; } // Invoice, PurchaseOrder, Employee, etc.

    public Guid? ReferenceId { get; set; } // ID of the related entity

    [MaxLength(100)]
    public string Category { get; set; } // InvoiceAttachment, Receipt, Contract, etc.

    [MaxLength(100)]
    public string DocumentType { get; set; } // PDF, Image, Word, Excel, etc.

    public bool IsPublic { get; set; } = false;

    public bool IsShared { get; set; } = false;

    [MaxLength(100)]
    public string SharingLevel { get; set; } = "Private"; // Private, Company, Department, Public

    public bool IsArchived { get; set; } = false;

    public DateTime? ArchivedAt { get; set; }

    public Guid? ArchivedBy { get; set; }

    public int Version { get; set; } = 1;

    public Guid? ParentId { get; set; } // For folder structure

    public Guid? FolderId { get; set; } // For folder organization

    [Required]
    public Guid UploadedBy { get; set; }





      [MaxLength(255)]
        public string? UploadedByName { get; set; }  // ✅ Nullable


    public DateTime UploadedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    public Guid? DeletedBy { get; set; }

    public DateTime DateAdd { get; set; }

    public DateTime DateMod { get; set; }

    public long RowVersion { get; set; } = 1;

    [MaxLength(50)]
    public string? Hash { get; set; } // File checksum for deduplication

    // Navigation Properties
    [ForeignKey("ParentId")]
    public virtual FileDocument Parent { get; set; }

    [ForeignKey("FolderId")]
    public virtual FileFolder Folder { get; set; }

    public virtual ICollection<FileDocument> Children { get; set; }

    public virtual ICollection<FileShare> Shares { get; set; }

    public virtual ICollection<FileAccessLog> AccessLogs { get; set; }
}
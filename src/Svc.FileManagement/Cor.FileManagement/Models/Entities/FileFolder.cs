// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\Entities\FileFolder.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.FileManagement.Models.Entities;

public class FileFolder
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    [MaxLength(50)]
    public string FolderType { get; set; } // Company, Department, Personal, Shared, Archive
    public string? Category { get; set; }
    public Guid? ParentId { get; set; } // For nested folders

    public Guid? OwnerId { get; set; } // User who owns the folder

    public bool IsPublic { get; set; } = false;

    public bool IsShared { get; set; } = false;

    [MaxLength(100)]
    public string SharingLevel { get; set; } = "Private"; // Private, Company, Department, Public

    public bool IsArchived { get; set; } = false;

    public int Order { get; set; } = 0;

    [Required]
    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    public Guid? DeletedBy { get; set; }

    public DateTime DateAdd { get; set; }

    public DateTime DateMod { get; set; }

    public long RowVersion { get; set; } = 1;

    // Navigation Properties
    [ForeignKey("ParentId")]
    public virtual FileFolder Parent { get; set; }

    public virtual ICollection<FileFolder> Children { get; set; }

    public virtual ICollection<FileDocument> Documents { get; set; }

    public virtual ICollection<FileShare> Shares { get; set; }
}
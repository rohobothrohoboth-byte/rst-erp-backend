// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\Entities\FileShare.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.FileManagement.Models.Entities;

public class FileShare
{
    [Key]
    public Guid Id { get; set; }

    public Guid? DocumentId { get; set; }

    public Guid? FolderId { get; set; }

    [Required]
    public Guid SharedWithId { get; set; } // User ID

    [MaxLength(50)]
    public string SharedWithType { get; set; } // User, Department, Role

    [MaxLength(20)]
    public string Permission { get; set; } = "Read"; // Read, Write, FullControl

    public bool CanDownload { get; set; } = true;

    public bool CanDelete { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime? ExpiresAt { get; set; }

    [Required]
    public Guid SharedBy { get; set; }

    public DateTime SharedAt { get; set; }

    public DateTime DateAdd { get; set; }

    public DateTime DateMod { get; set; }

    public long RowVersion { get; set; } = 1;

    [ForeignKey("DocumentId")]
    public virtual FileDocument Document { get; set; }

    [ForeignKey("FolderId")]
    public virtual FileFolder Folder { get; set; }
}
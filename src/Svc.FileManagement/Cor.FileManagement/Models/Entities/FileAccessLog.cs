// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\Entities\FileAccessLog.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.FileManagement.Models.Entities;

public class FileAccessLog
{
    [Key]
    public Guid Id { get; set; }

    public Guid? DocumentId { get; set; }

    public Guid? FolderId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [MaxLength(50)]
    public string Action { get; set; } // View, Download, Upload, Delete, Share, Archive

    [MaxLength(50)]
    public string ActionType { get; set; } // Read, Write, Delete

    [MaxLength(100)]
    public string IPAddress { get; set; }

    [MaxLength(255)]
    public string UserAgent { get; set; }

    public DateTime AccessedAt { get; set; }

    public DateTime DateAdd { get; set; }

    public long RowVersion { get; set; } = 1;

    [ForeignKey("DocumentId")]
    public virtual FileDocument Document { get; set; }

    [ForeignKey("FolderId")]
    public virtual FileFolder Folder { get; set; }
}
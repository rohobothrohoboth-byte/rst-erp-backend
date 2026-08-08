// Models/Entities/RequisitionAttachment.cs
using System;

namespace Cor.Procurement.Models.Entities;

public class RequisitionAttachment : BaseEntity
{
    public Guid RequisitionId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? UploadedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }

    // Navigation
    public virtual Requisition Requisition { get; set; } = null!;
}
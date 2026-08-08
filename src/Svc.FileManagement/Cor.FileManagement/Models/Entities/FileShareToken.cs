// Models/Entities/FileShareToken.cs

using System;

namespace Cor.FileManagement.Models.Entities;

public class FileShareToken
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Guid CreatedBy { get; set; }
     public string? CreatedByName { get; set; }
    public bool IsActive { get; set; }
    public int? DownloadCount { get; set; }
    public DateTime? LastDownloadedAt { get; set; }

    // Navigation property
    public FileDocument Document { get; set; } = null!;
}
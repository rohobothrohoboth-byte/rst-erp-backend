// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\DTOs\FolderShareDto.cs

namespace Cor.FileManagement.Models.DTOs;

 public class ShareFolderDto
 {
     public string SharedWithId { get; set; } = string.Empty;
     public string? SharedWithType { get; set; }
     public string? Permission { get; set; }
     public bool CanDownload { get; set; }
     public bool CanDelete { get; set; }
     public DateTime? ExpiresAt { get; set; }
 }
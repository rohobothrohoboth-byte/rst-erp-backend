// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\DTOs\CreateFolderDto.cs

namespace Cor.FileManagement.Models.DTOs;

public class CreateFolderDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FolderType { get; set; }
    public string? Category { get; set; } // ✅ Add this
    public Guid? ParentId { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsShared { get; set; }
    public string? SharingLevel { get; set; }
    public int? Order { get; set; }
}
// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\DTOs\DownloadDto.cs

namespace Cor.FileManagement.Models.DTOs;

public class DownloadDto
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public byte[]? FileContent { get; set; }
}
// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\DTOs\DownloadResultDto.cs

namespace Cor.FileManagement.Models.DTOs;

public class DownloadResultDto
{
    public byte[] FileBytes { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}
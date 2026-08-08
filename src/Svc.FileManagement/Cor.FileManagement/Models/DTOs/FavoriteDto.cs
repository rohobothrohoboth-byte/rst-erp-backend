// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\DTOs\FavoriteDto.cs

namespace Cor.FileManagement.Models.DTOs;

public class FavoriteDto
{
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
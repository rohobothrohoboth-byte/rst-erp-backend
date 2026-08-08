// Models/Entities/FileFavorite.cs
namespace Cor.FileManagement.Models.Entities;
public class FileFavorite
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DateAdd { get; set; }
    public FileDocument Document { get; set; } = null!;
}


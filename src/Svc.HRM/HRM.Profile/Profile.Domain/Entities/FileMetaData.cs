namespace Profile.Domain.Entities;

public class FileMetaData: BaseEntity
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; } = default!;
}
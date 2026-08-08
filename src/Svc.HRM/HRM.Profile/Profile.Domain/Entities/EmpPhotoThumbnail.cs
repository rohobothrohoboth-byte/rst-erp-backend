namespace Profile.Domain.Entities;

public class EmpPhotoThumbnail : BaseEntity
{
    public byte[] Data { get; set; } = default!; // For BYTEA only
    public Guid FileMetaDataId { get; set; } = default!; //FileMetaData

    //******************************************//

    public FileMetaData FileMetaData { get; set; } = null!;
}
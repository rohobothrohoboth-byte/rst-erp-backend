namespace Profile.Domain.Entities;

public class EmpStampBlob : BaseEntity
{
    public byte[] Data { get; set; } = default!; // For BYTEA only
    public Guid FileMetaDataId { get; set; } = default!; //FileMetaData

    //******************************************//

    public FileMetaData FileMetaData { get; set; } = null!;
}
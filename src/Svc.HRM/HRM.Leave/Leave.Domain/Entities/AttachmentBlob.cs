namespace Leave.Domain.Entities;

public class AttachmentBlob : BaseEntity
{
    public byte[] Data { get; set; } = default!; // For BYTEA only
    public Guid AttachmentId { get; set; } = default!; //Attachment

    //******************************************//

    public Attachment Attachment { get; set; } = null!;
}
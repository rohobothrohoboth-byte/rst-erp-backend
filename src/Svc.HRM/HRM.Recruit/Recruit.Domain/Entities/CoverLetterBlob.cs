namespace Recruit.Domain.Entities;

public class CoverLetterBlob : BaseEntity
{
    public byte[] Data { get; set; } = default!; // For BYTEA only
    public Guid CoverLetterId { get; set; } = default!; //CoverLetter

    //******************************************//

    public CoverLetter CoverLetter { get; set; } = null!;
}
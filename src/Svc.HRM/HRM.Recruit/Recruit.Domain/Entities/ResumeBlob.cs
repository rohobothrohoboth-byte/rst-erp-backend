namespace Recruit.Domain.Entities;

public class ResumeBlob : BaseEntity
{
    public byte[] Data { get; set; } = default!; // For BYTEA only
    public Guid ResumeId { get; set; } = default!; //Resume

    //******************************************//

    public virtual Resume Resume { get; set; } = null!;
}
namespace Recruit.Domain.Entities;

public class Resume : BaseEntity
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; } = default!;
    public Guid JobAppId { get; set; } // JobApplication

    //******************************************//

    public virtual JobApplication JobApp { get; set; } = null!;
}
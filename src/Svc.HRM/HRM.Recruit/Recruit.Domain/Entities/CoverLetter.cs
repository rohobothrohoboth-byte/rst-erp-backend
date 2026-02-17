namespace Recruit.Domain.Entities;

public class CoverLetter : BaseEntity
{
    public string Content { get; set; } = default!;
    public Guid JobAppId { get; set; } // JobApplication

    //******************************************//

    public JobApplication JobApp { get; set; } = null!;
}
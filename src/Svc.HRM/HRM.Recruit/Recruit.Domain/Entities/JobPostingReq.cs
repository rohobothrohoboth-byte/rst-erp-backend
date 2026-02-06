namespace Recruit.Domain.Entities;

public class JobPostingReq : BaseEntity
{
    public string PostingNumber { get; set; } = default!;
    public string JobTitle { get; set; } = default!;
    public string JobDescription { get; set; } = default!;
    public string RequiredQualifications { get; set; } = default!;
    public string PreferredQualifications { get; set; } = default!;
}
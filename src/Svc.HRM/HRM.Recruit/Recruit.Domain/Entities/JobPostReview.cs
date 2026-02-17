namespace Recruit.Domain.Entities;

public class JobPostReview : BaseEntity
{
    public string Comment { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.PostingStatus(0/1)
    public Guid ReviewById { get; set; } // HRM.Profile.Employee
    public Guid JobPostingId { get; set; } // JobPosting

    //******************************************//

    public JobPosting JobPosting { get; set; } = null!;
}
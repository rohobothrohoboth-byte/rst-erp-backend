namespace Recruit.Domain.Entities;

public class JobPosting : BaseEntity
{
    public string PostNumber { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.PostingStatus(0/1)
    public string PostType { get; set; } = default!; // enum.JobPostingType(0/1) 
    public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
    public DateTime DeadlineDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public Guid JobReqId { get; set; } // JobRequisition

    //******************************************//

    public JobRequisition JobReq { get; set; } = null!;
    public List<JobApplication> Applications { get; set; } = new();
}
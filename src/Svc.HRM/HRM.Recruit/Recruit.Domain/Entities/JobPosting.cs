namespace Recruit.Domain.Entities;

public class JobPosting : BaseEntity
{
    public string Status { get; set; } = default!; // enum.PostingStatus(0/1)
    public string PostingType { get; set; } = default!; // enum.JobPostingType(0/1) 
    public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
    public DateTime DeadlineDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public Guid JobRequisitionId { get; set; } // JobRequisition
    public Guid JJobPostingReqId { get; set; } // JobPostingReq

    //******************************************//

    public JobRequisition JobRequisition { get; set; } = null!;
    public JobPostingReq JobPostingReq { get; set; } = null!;
    public List<CandidateApplication> Applications { get; set; } = new();
}
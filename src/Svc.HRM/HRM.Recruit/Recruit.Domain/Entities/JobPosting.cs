

namespace Recruit.Domain.Entities;

public class JobPosting : BaseEntity
{
    public string PostNumber { get; set; } = default!;
    public string Status { get; set; } = default!; // Stores enum name as string
    public string PostType { get; set; } = default!;
    public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
    public DateTime DeadlineDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public Guid JobReqId { get; set; }

    public virtual JobRequisition JobReq { get; set; } = null!;
    public List<JobApplication> Applications { get; set; } = new();
}
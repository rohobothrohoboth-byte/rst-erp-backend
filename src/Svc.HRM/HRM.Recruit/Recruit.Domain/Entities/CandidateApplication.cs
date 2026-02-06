namespace Recruit.Domain.Entities;

public class CandidateApplication : BaseEntity
{
    public string Status { get; set; } = default!; // enum.ApplicationStatus(0/1)
    public DateTime AppliedDate { get; set; }
    public int ScreeningScore { get; set; }
    public string? ScreeningComments { get; set; }
    public string? UpdatedBy { get; set; }
    public Guid CandidateId { get; set; } // Candidate
    public Guid JobPostingId { get; set; } // JobPosting
    public Guid CoverLetterId { get; set; } // CoverLetter

    //******************************************//

    public Candidate Candidate { get; set; } = null!;
    public JobPosting JobPosting { get; set; } = null!;
    public CoverLetter CoverLetter { get; set; } = null!;
    public List<Interview> Interviews { get; set; } = new();
}

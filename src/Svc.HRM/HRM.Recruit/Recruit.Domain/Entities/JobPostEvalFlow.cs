namespace Recruit.Domain.Entities;

public class JobPostEvalFlow : BaseEntity
{
    public Guid EvaluationFlowId { get; set; } // EvaluationFlow
    public Guid JobPostingId { get; set; } // JobPosting

    //******************************************

    public virtual EvaluationFlow EvaluationFlow { get; set; } = null!;
    public virtual JobPosting JobPosting { get; set; } = null!;
}
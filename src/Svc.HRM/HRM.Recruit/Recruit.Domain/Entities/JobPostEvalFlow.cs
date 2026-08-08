namespace Recruit.Domain.Entities;

public class JobPostEvalFlow : BaseEntity
{
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Guid EvaluationFlowId { get; set; } // EvaluationFlow
    public Guid JobPostingId { get; set; } // JobPosting

    //******************************************

    public virtual EvaluationFlow EvaluationFlow { get; set; } = null!;
    public virtual JobPosting JobPosting { get; set; } = null!;
}
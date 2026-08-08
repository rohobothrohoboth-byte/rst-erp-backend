namespace Recruit.Domain.Entities;

public class JobAppEvalProgress : BaseEntity
{
    public bool IsCompleted { get; set; }
    public Guid JobAppId { get; set; } // JobApplication
    public Guid CurrentStepId { get; set; } // EvaluationStep

    //******************************************//

    public virtual JobApplication JobApp { get; set; } = null!;
    public virtual EvaluationStep CurrentStep { get; set; } = null!;
}
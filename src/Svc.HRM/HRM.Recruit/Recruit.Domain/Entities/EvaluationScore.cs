namespace Recruit.Domain.Entities;

public class EvaluationScore : BaseEntity
{
    public double Score { get; set; }
    public bool IsCurrent { get; set; } = true;
    public string Feedback { get; set; } = default!;
    public Guid EvaluatorId { get; set; } // HRM.Profile.Employee
    public Guid EvaluationStepId { get; set; } // EvaluationStep
    public Guid JobAppId { get; set; } // JobApplication

    //******************************************//

    public virtual EvaluationStep EvaluationStep { get; set; } = null!;
    public virtual JobApplication JobApp { get; set; } = null!;
}
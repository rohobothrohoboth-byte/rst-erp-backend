namespace Recruit.Domain.Entities;

public class EvaluationScore : BaseEntity
{
    public double Score { get; set; }
    public bool IsCurrent { get; set; } = true;
    public string Feedback { get; set; } = default!;
    public Guid EvaluatorId { get; set; }
    public Guid EvalTypeId { get; set; } // EvaluationType
    public Guid EvaluationStepId { get; set; } // EvaluationStep

    //******************************************//

    public virtual EvaluationStep EvaluationStep { get; set; } = null!;
    public virtual EvaluationType EvalType { get; set; } = null!;
}
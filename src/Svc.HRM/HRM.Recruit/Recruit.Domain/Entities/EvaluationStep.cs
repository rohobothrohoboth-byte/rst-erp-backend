namespace Recruit.Domain.Entities;

public class EvaluationStep : BaseEntity
{
    public string StepName { get; set; } = default!;
    public int StepOrder { get; set; }    // 1, 2, 3 ...
    public bool IsFinal { get; set; } = false;
    public Guid EvalTypeId { get; set; } // EvaluationType
    public Guid EvaluationFlowId { get; set; } // EvaluationFlow

    //******************************************//

    public virtual EvaluationType EvalType { get; set; } = null!;
    public virtual EvaluationFlow EvaluationFlow { get; set; } = null!;
}
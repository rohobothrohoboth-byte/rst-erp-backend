namespace Recruit.Domain.Entities;

public class EvaluationFlow : BaseEntity
{
    public string Name { get; set; } = default!;
    public bool IsGlobal { get; set; } = false;
    public bool IsActive { get; set; } = true;

    //******************************************//

    public List<EvaluationStep> Steps { get; set; } = [];
}
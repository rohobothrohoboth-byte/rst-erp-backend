namespace Recruit.Domain.Entities;

public class ApprovalWorkflow : BaseEntity
{
    public string WorkflowCode { get; set; } = default!;
    public string WorkflowName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string EntityType { get; set; } = default!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; set; }

    //******************************************//

    public List<ApprovalStep> ApprovalSteps { get; set; } = new();
}
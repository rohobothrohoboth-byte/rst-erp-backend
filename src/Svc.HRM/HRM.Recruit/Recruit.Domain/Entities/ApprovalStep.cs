namespace Recruit.Domain.Entities;

public class ApprovalStep : BaseEntity
{
    public int StepOrder { get; set; }
    public string StepName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ApproverRole { get; set; } = default!;
    public bool AllowMultipleApprovers { get; set; }
    public bool CanRejectAndReturn { get; set; }
    public int? TimeoutDays { get; set; }
    public Guid WorkflowId { get; set; } // ApprovalWorkflow

    //******************************************//

    public ApprovalWorkflow Workflow { get; set; } = null!;
    public List<ApprovalInstance> ApprovalInstances { get; set; } = new();
}
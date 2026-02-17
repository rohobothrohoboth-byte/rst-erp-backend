namespace Recruit.Domain.DTOs;

public class ApprovalStepListDto : BaseDto
{
    public int StepOrder { get; set; }
    public string StepName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ApproverRole { get; set; } = default!;
    public bool AllowMultipleApprovers { get; set; }
    public bool CanRejectAndReturn { get; set; }
    public int? TimeoutDays { get; set; }
    public Guid WorkflowId { get; set; } // ApprovalWorkflow
}

public class ApprovalStepAddDto
{
    public int StepOrder { get; set; }
    public string StepName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ApproverRole { get; set; } = default!;
    public bool AllowMultipleApprovers { get; set; }
    public bool CanRejectAndReturn { get; set; }
    public int? TimeoutDays { get; set; }
    public Guid WorkflowId { get; set; } // ApprovalWorkflow
}

public class ApprovalStepModDto
{
    public Guid Id { get; set; }
    public int StepOrder { get; set; }
    public string StepName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ApproverRole { get; set; } = default!;
    public bool AllowMultipleApprovers { get; set; }
    public bool CanRejectAndReturn { get; set; }
    public int? TimeoutDays { get; set; }
    public Guid WorkflowId { get; set; } // ApprovalWorkflow
    public string RowVersion { get; set; } = default!;
}
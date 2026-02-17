namespace Recruit.Domain.DTOs;

public class AppWorkflowListDto : BaseDto
{
    public string WorkflowCode { get; set; } = default!;
    public string WorkflowName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string EntityType { get; set; } = default!;
    public bool IsActive { get; private set; }
}

public class AppWorkflowAddDto
{
    public string WorkflowCode { get; set; } = default!;
    public string WorkflowName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string EntityType { get; set; } = default!;
    public bool IsActive { get; private set; }
}

public class AppWorkflowModDto
{
    public Guid Id { get; set; }
    public string WorkflowCode { get; set; } = default!;
    public string WorkflowName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string EntityType { get; set; } = default!;
    public bool IsActive { get; private set; }
    public string RowVersion { get; set; } = default!;
}
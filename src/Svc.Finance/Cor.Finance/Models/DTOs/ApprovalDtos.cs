namespace Cor.Finance.Models.DTOs;

public class ApprovalWorkflowDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string EntityType { get; set; } = default!; // Invoice, Expense, Payment
    public List<ApprovalStepDto> Steps { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}



public class ApprovalRequestDto
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = default!;
    public string Status { get; set; } = default!; // Pending, Approved, Rejected
    public string? Comments { get; set; }
    public Guid? CurrentApproverId { get; set; }
    public string? CurrentApproverName { get; set; }
    public List<ApprovalHistoryDto> History { get; set; } = new();
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class ApprovalHistoryDto
{
    public Guid ApproverId { get; set; }
    public string ApproverName { get; set; } = default!;
    public string Action { get; set; } = default!; // Approved, Rejected
    public string? Comments { get; set; }
    public DateTime ActionDate { get; set; }
}

public class ApproveRejectDto
{
    public Guid RequestId { get; set; }
    public bool IsApproved { get; set; }
    public string? Comments { get; set; }
    public string RowVersion { get; set; } = default!;
}
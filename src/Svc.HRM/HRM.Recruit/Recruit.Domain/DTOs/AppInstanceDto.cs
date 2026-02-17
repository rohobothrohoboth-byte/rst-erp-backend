namespace Recruit.Domain.DTOs;

public class AppInstanceListDto : BaseDto
{
    public string EntityType { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.ApprovalStatus(0/1)
    public DateTime? AssignedDate { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string? Comments { get; set; } = default!;
    public int SequenceNumber { get; set; }
    public Guid ApprovalStepId { get; set; } // ApprovalStep
    public Guid EntityId { get; set; }
    public Guid? ApproverId { get; set; } // HRM.Profile.Employee

}

public class AppInstanceAddDto
{
    public string EntityType { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.ApprovalStatus(0/1)
    public DateTime? AssignedDate { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string? Comments { get; set; } = default!;
    public int SequenceNumber { get; set; }
    public Guid ApprovalStepId { get; set; } // ApprovalStep
    public Guid EntityId { get; set; }
    public Guid? ApproverId { get; set; } // HRM.Profile.Employee
}

public class AppInstanceModDto
{
    public Guid Id { get; set; }
    public string EntityType { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.ApprovalStatus(0/1)
    public DateTime? AssignedDate { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string? Comments { get; set; } = default!;
    public int SequenceNumber { get; set; }
    public Guid ApprovalStepId { get; set; } // ApprovalStep
    public Guid EntityId { get; set; }
    public Guid? ApproverId { get; set; } // HRM.Profile.Employee

    public string RowVersion { get; set; } = default!;
}
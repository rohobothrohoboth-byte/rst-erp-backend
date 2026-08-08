using System.ComponentModel.DataAnnotations;

namespace Profile.Domain.DTOs;

// ---------- Contract ----------
public class EmpContractListDto
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string StatusName { get; set; } = default!;
    public string ContractType { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? SignedDate { get; set; }
    public DateTime? TerminatedDate { get; set; }
    public string? TerminationReason { get; set; }
    public string? DocumentRef { get; set; }
    public string? Notes { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? RenewedFromId { get; set; }
    public string RowVersion { get; set; } = default!;
}

public class EmpContractAddDto
{
    [Required] public Guid EmployeeId { get; set; }
    [Required] public string ContractType { get; set; } = default!;
    [Required] public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? SignedDate { get; set; }
    public string? DocumentRef { get; set; }
    public string? Notes { get; set; }
    public bool ActivateImmediately { get; set; } = true;
}

public class EmpContractModDto
{
    [Required] public Guid Id { get; set; }
    [Required] public string ContractType { get; set; } = default!;
    [Required] public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? SignedDate { get; set; }
    public string? DocumentRef { get; set; }
    public string? Notes { get; set; }
    [Required] public string RowVersion { get; set; } = default!;
}

public class EmpContractTerminateDto
{
    [Required] public Guid Id { get; set; }
    [Required] public string Reason { get; set; } = default!;
    public DateTime? TerminatedDate { get; set; }
    [Required] public string RowVersion { get; set; } = default!;
}

public class EmpContractRenewDto
{
    [Required] public Guid Id { get; set; }
    [Required] public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? DocumentRef { get; set; }
    public string? Notes { get; set; }
    [Required] public string RowVersion { get; set; } = default!;
}

// ---------- Promotion ----------
public class EmpPromotionListDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!;
    public string StatusName { get; set; } = default!;
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string? Comments { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? AppliedDate { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid FromJobGradeId { get; set; }
    public Guid FromPositionId { get; set; }
    public Guid FromDepartmentId { get; set; }
    public Guid? FromJgStepId { get; set; }
    public Guid ToJobGradeId { get; set; }
    public Guid ToPositionId { get; set; }
    public Guid ToDepartmentId { get; set; }
    public Guid? ToJgStepId { get; set; }
    public string RowVersion { get; set; } = default!;
}

public class EmpPromotionAddDto
{
    [Required] public Guid EmployeeId { get; set; }
    [Required] public DateTime EffectiveDate { get; set; }
    [Required] public Guid ToJobGradeId { get; set; }
    [Required] public Guid ToPositionId { get; set; }
    [Required] public Guid ToDepartmentId { get; set; }
    public Guid? ToJgStepId { get; set; }
    public string? Reason { get; set; }
}

public class EmpPromotionDecisionDto
{
    [Required] public Guid Id { get; set; }
    public Guid? ApprovedById { get; set; }
    public string? Comments { get; set; }
    [Required] public string RowVersion { get; set; } = default!;
}

// ---------- Transfer ----------
public class EmpTransferListDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!;
    public string StatusName { get; set; } = default!;
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string? Comments { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? AppliedDate { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid FromDepartmentId { get; set; }
    public Guid FromPositionId { get; set; }
    public Guid? FromJobGradeId { get; set; }
    public Guid ToDepartmentId { get; set; }
    public Guid ToPositionId { get; set; }
    public Guid? ToJobGradeId { get; set; }
    public string RowVersion { get; set; } = default!;
}

public class EmpTransferAddDto
{
    [Required] public Guid EmployeeId { get; set; }
    [Required] public DateTime EffectiveDate { get; set; }
    [Required] public Guid ToDepartmentId { get; set; }
    [Required] public Guid ToPositionId { get; set; }
    public Guid? ToJobGradeId { get; set; }
    public string? Reason { get; set; }
}

public class EmpTransferDecisionDto
{
    [Required] public Guid Id { get; set; }
    public Guid? ApprovedById { get; set; }
    public string? Comments { get; set; }
    [Required] public string RowVersion { get; set; } = default!;
}

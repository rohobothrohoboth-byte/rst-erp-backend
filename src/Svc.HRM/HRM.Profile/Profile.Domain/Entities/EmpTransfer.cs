namespace Profile.Domain.Entities;

public class EmpTransfer : BaseEntity
{
    public string Status { get; set; } = default!; // HrChangeStatus
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

    public Employee Employee { get; set; } = null!;
}

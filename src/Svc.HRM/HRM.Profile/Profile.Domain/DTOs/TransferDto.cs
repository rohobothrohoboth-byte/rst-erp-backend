namespace Profile.Domain.DTOs;

public sealed class TransferListDto : BaseDto
{
    public Guid EmployeeId { get; set; }
    public string? FromBranch { get; set; }
    public string? ToBranch { get; set; }
    public string? FromDepartment { get; set; }
    public string? ToDepartment { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = default!;
}

public sealed class TransferAddDto
{
    public Guid EmployeeId { get; set; }
    public string? FromBranch { get; set; }
    public string? ToBranch { get; set; }
    public string? FromDepartment { get; set; }
    public string? ToDepartment { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "Pending";
}

public sealed class TransferModDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? FromBranch { get; set; }
    public string? ToBranch { get; set; }
    public string? FromDepartment { get; set; }
    public string? ToDepartment { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "Pending";
}

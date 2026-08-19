namespace Profile.Domain.Entities;

public class EmployeeTransfer : BaseEntity
{
    public Guid EmployeeId { get; set; } = default!;
    public string? FromBranch { get; set; }
    public string? ToBranch { get; set; }
    public string? FromDepartment { get; set; }
    public string? ToDepartment { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "Pending";

    //******************************************//

    public Employee Employee { get; set; } = null!;
}

namespace Profile.Domain.Entities;

public class EmployeeContract : BaseEntity
{
    public Guid EmployeeId { get; set; } = default!;
    public string ContractType { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Salary { get; set; }
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }

    //******************************************//

    public Employee Employee { get; set; } = null!;
}

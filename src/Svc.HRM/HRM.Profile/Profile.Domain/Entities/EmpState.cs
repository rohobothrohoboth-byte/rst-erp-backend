namespace Profile.Domain.Entities;

public class EmpState: BaseEntity
{
    public string IsTerminated { get; set; } = default!; //enum.YesNo
    public string IsApproved { get; set; } = default!; //enum.YesNo
    public string IsStandBy { get; set; } = default!; //enum.YesNo
    public string IsRetired { get; set; } = default!; //enum.YesNo
    public string IsUnderProbation { get; set; } = default!; //enum.YesNo
    public Guid EmployeeId { get; set; } = default!; //Employee

    //******************************************//

    public Employee Employee { get; set; } = null!;
}
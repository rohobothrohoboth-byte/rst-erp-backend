namespace Profile.Domain.DTOs;

public class EmpStateListDto : BaseDto
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string IsTerminated { get; set; } = default!; //enum.YesNo (0/1)
    public string IsApproved { get; set; } = default!; //enum.YesNo (0/1)
    public string IsStandBy { get; set; } = default!; //enum.YesNo (0/1)
    public string IsRetired { get; set; } = default!; //enum.YesNo (0/1)
    public string IsUnderProbation { get; set; } = default!; //enum.YesNo (0/1)
    public string IsTerminatedStr { get; set; } = default!;
    public string IsApprovedStr { get; set; } = default!;
    public string IsStandByStr { get; set; } = default!;
    public string IsRetiredStr { get; set; } = default!;
    public string IsUnderProbationStr { get; set; } = default!;
    public string EmpFullName { get; set; } = default!;
    public string EmpFullNameAm { get; set; } = default!;
}

public class EmpStateAddDto
{
    public string IsTerminated { get; set; } = default!; //enum.YesNo (0/1)
    public string IsApproved { get; set; } = default!; //enum.YesNo (0/1)
    public string IsStandBy { get; set; } = default!; //enum.YesNo (0/1)
    public string IsRetired { get; set; } = default!; //enum.YesNo (0/1)
    public string IsUnderProbation { get; set; } = default!; //enum.YesNo (0/1)
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmpStateModDto
{
    public Guid Id { get; set; }
    public string IsTerminated { get; set; } = default!; //enum.YesNo (0/1)
    public string IsApproved { get; set; } = default!; //enum.YesNo (0/1)
    public string IsStandBy { get; set; } = default!; //enum.YesNo (0/1)
    public string IsRetired { get; set; } = default!; //enum.YesNo (0/1)
    public string IsUnderProbation { get; set; } = default!; //enum.YesNo (0/1)
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string RowVersion { get; set; } = default!;
}
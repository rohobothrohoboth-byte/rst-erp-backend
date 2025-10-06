using EthiopianCalendar;

namespace Profile.Domain.DTOs;

public class EmpBioListDto : BaseDto
{
    public Guid MaritalStatusId { get; set; } = default!; //lup.MaritalStatus
    public Guid EmployeeId { get; set; } = default!; //Employee
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCert { get; set; } = default!; // enum.YesNo (0/1)
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo (0/1)
    public string HasBirthCertStr { get; set; } = default!;
    public string HasMarriageCertStr { get; set; } = default!;
    public string MaritalStatus { get; set; } = default!;
    public string EmpFullName { get; set; } = default!;
    public string EmpFullNameAm { get; set; } = default!;
    public string BirthDateStr => $"{BirthDate:MMMM dd, yyyy}";
    public string BirthDateStrAm => BirthDate.ToEthiopianDateString("MMMM dd, yyyy");
}

public class EmpBioAddDto
{
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCert { get; set; } = default!; // enum.YesNo (0/1)
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo (0/1)
    public Guid MaritalStatusId { get; set; } = default!; //lup.MaritalStatus
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmpBioModDto
{
    public Guid Id { get; set; }
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCert { get; set; } = default!; // enum.YesNo (0/1)
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo (0/1)
    public Guid MaritalStatusId { get; set; } = default!; //lup.MaritalStatus
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string RowVersion { get; set; } = default!;
}
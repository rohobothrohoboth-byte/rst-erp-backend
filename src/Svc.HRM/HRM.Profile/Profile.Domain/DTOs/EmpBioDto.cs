using EthiopianCalendar;
using System.Text.Json.Serialization;

namespace Profile.Domain.DTOs;

public class EmpBioListDto : BaseDto
{
    [JsonIgnore]
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCert { get; set; } = default!; // enum.YesNo
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo
    public string MaritalStatus { get; set; } = default!; //lup.MaritalStatus
    public string Address { get; set; } = default!; //Employee
    public string EmpFullName { get; set; } = default!; //Employee
    public string DateOfBirth => $"{BirthDate:MMMM dd, yyyy}";
    public string DateOfBirthAm => BirthDate.ToEthiopianDateString("MMMM dd, yyyy");
}

public class EmpBioAddDto
{
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCert { get; set; } = default!; // enum.YesNo
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo
    public Guid MaritalStatusId { get; set; } = default!; //lup.MaritalStatus
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmpBioModDto
{
    public Guid Id { get; set; }
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCert { get; set; } = default!; // enum.YesNo
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo
    public Guid MaritalStatusId { get; set; } = default!; //lup.MaritalStatus
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string RowVersion { get; set; } = default!;
}
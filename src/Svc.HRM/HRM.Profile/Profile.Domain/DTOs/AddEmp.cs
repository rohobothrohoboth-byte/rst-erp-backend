using Microsoft.AspNetCore.Http;

namespace Profile.Domain.DTOs;

public class EmpAddRes
{
    public Guid Id { get; set; } = default!; //Employee Id
}

public class Step1Dto
{
    public string FirstName { get; set; } = default!;
    public string FirstNameAm { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameAm { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameAm { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender (0/1)
    public string Nationality { get; set; } = default!;
    public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;
    public Guid JobGradeId { get; set; } = default!; //Cor.HRMM.JobGrade
    public Guid PositionId { get; set; } = default!; //Cor.HRMM.Position
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department
    public string EmploymentType { get; set; } = default!; //enum.EmpType (0/1)
    public string EmploymentNature { get; set; } = default!; //enum.EmpNature (0/1)
    public IFormFile File { get; set; } = default!;
}

public class Step2Dto
{
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCert { get; set; } = default!; // enum.YesNo (0/1)
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo (0/1)
    public string MaritalStatus { get; set; } = default!; //eum.MaritalStatus (0/1)
    public string Tin { get; set; } = default!;
    public string BankAccountNo { get; set; } = default!;
    public string PensionNumber { get; set; } = default!;
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string Country { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string Subcity { get; set; } = default!;
    public string Zone { get; set; } = default!;
    public string Woreda { get; set; } = default!;
    public string Kebele { get; set; } = default!;
    public string HouseNo { get; set; } = default!;
    public string Telephone { get; set; } = default!;
    public string PoBox { get; set; } = default!;
    public string Fax { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Website { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class Step3Dto
{
    public string FirstName { get; set; } = default!;
    public string FirstNameAm { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameAm { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameAm { get; set; } = default!;
    public string Nationality { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string Country { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string Subcity { get; set; } = default!;
    public string Zone { get; set; } = default!;
    public string Woreda { get; set; } = default!;
    public string Kebele { get; set; } = default!;
    public string HouseNo { get; set; } = default!;
    public string Telephone { get; set; } = default!;
    public string PoBox { get; set; } = default!;
    public string Fax { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Website { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class Step4Dto
{
    public string FirstName { get; set; } = default!;
    public string FirstNameAm { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameAm { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameAm { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender (0/1)
    public string Nationality { get; set; } = default!;
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string Country { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string Subcity { get; set; } = default!;
    public string Zone { get; set; } = default!;
    public string Woreda { get; set; } = default!;
    public string Kebele { get; set; } = default!;
    public string HouseNo { get; set; } = default!;
    public string Telephone { get; set; } = default!;
    public string PoBox { get; set; } = default!;
    public string Fax { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Website { get; set; } = default!;
    public IFormFile File { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
}
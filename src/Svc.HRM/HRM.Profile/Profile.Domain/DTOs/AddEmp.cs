using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Profile.Domain.DTOs;

public class EmpAddRes
{
    public Guid Id { get; set; } = default!; //Employee Id
}

public class Step1Dto
{
    [JsonIgnore]
    public Guid BranchId { get; set; } = default!; //Dummy property for Branch
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
    public Guid JgStepId { get; set; } = default!; //Cor.HRMM.JgStep
    public Guid PositionId { get; set; } = default!; //Cor.HRMM.Position
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department
    public Guid? ReportsToId { get; set; } //HRM.Profile.Employee (direct manager / boss)
    public string EmploymentType { get; set; } = default!; //enum.EmpType (0/1)
    public string EmploymentNature { get; set; } = default!; //enum.EmpNature (0/1)
    public string WorkArrangement { get; set; } = default!; //enum.WorkArrangement (0/1)
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
    public string MaritalStatus { get; set; } = default!; //eum.MaritalStatus (0/1)


    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string? Country { get; set; }
    public string Region { get; set; } = default!;
    public string? Subcity { get; set; }
    public string? Zone { get; set; }
    public string? Woreda { get; set; }
    public string? Kebele { get; set; }
    public string? HouseNo { get; set; }
    public string Telephone { get; set; } = default!;
    public string? PoBox { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public IFormFile? File { get; set; } = default!;
}

public class Step2Dto
{
    public string FirstName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender (0/1)
    public string Nationality { get; set; } = default!;
    public string Relation { get; set; } = default!; //enum.Relation
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string? Country { get; set; }
    public string Region { get; set; } = default!;
    public string? Subcity { get; set; }
    public string? Zone { get; set; }
    public string? Woreda { get; set; }
    public string? Kebele { get; set; }
    public string? HouseNo { get; set; }
    public string Telephone { get; set; } = default!;
    public string? PoBox { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public IFormFile? File { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmpAddPrintDto
{
    // Basic Info
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string Photo { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string FullNameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Nationality { get; set; } = default!;
    public string EmploymentDate { get; set; } = default!;
    public string EmploymentDateAm { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string EmploymentType { get; set; } = default!;
    public string EmploymentNature { get; set; } = default!;
    public string WorkArr { get; set; } = default!;
    public string BirthDate { get; set; } = default!;
    public string BirthDateAm { get; set; } = default!;
    public string MaritalStatus { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Telephone { get; set; } = default!;

    // Guarantor
    public string GuaFullName { get; set; } = default!;
    public string GuaNationality { get; set; } = default!;
    public string GuaGender { get; set; } = default!;
    public string GuaRelation { get; set; } = default!;
    public string GuaAddress { get; set; } = default!;
    public string GuaTelephone { get; set; } = default!;
    public string GuaFileName { get; set; } = default!;
    public string GuaFileSize { get; set; } = default!;
    public string GuaFileType { get; set; } = default!;
}

public class BasicInfoDto
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string Photo { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string FullNameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Nationality { get; set; } = default!;
    public string EmploymentDate { get; set; } = default!;
    public string EmploymentDateAm { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string EmploymentType { get; set; } = default!;
    public string EmploymentNature { get; set; } = default!;
    public string WorkArrangement { get; set; } = default!;
}
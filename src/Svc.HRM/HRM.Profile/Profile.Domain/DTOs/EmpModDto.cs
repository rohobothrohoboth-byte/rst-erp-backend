using Microsoft.AspNetCore.Http;

namespace Profile.Domain.DTOs;

public class EmpModBasicDto : RawBaseDto
{
    public Guid BranchId { get; set; } = default!; //Dummy property for Branch
    public Guid JobGradeId { get; set; } = default!; //Cor.HRMM.JobGrade
    public Guid JgStepId { get; set; } = default!; //Cor.HRMM.JgStep
    public Guid PositionId { get; set; } = default!; //Cor.HRMM.Position
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department

    public string FirstName { get; set; } = default!;
    public string FirstNameAm { get; set; } = "";
    public string MiddleName { get; set; } = default!;
    public string MiddleNameAm { get; set; } = "";
    public string LastName { get; set; } = default!;
    public string LastNameAm { get; set; } = "";
    public string Gender { get; set; } = default!; // enum.Gender
    public string Nationality { get; set; } = default!;
    public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;
    public string EmploymentType { get; set; } = default!; //enum.EmpType (0/1)
    public string EmploymentNature { get; set; } = default!; //enum.EmpNature (0/1)
    public string WorkArrangement { get; set; } = default!; //enum.WorkArrangement (0/1)
    public IFormFile? File { get; set; } = default!;
}

public class EmpModBioDto : RawBaseDto
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public bool HasData { get; set; }

    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string MaritalStatus { get; set; } = default!; //eum.MaritalStatus (0/1)
    public string Tin { get; set; } = default!;
    public string BankAccountNo { get; set; } = default!;
    public string PensionNumber { get; set; } = default!;

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
}

public class EmpModGuarDto : RawBaseDto
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public bool HasData { get; set; }

    public string FirstName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
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
}

public class EmpModStaSignDto : BaseDto
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public IFormFile File { get; set; } = default!;
}

public class ModSalaryDto
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid JgStepId { get; set; } = default!;
    public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;
}

public class ModFileDto
{
    public Guid Id { get; set; } = default!;
    public IFormFile File { get; set; } = default!;
}
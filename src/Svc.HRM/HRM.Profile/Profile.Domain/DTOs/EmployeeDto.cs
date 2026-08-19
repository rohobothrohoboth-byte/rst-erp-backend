namespace Profile.Domain.DTOs;

public sealed class EmpJoinRow
{
    public Guid Id { get; init; }
    public string Code { get; init; } = default!;
    public string EmpState { get; init; } = default!;
    public string EmploymentType { get; init; } = default!;
    public string EmploymentNature { get; init; } = default!;
    public string WorkArrangement { get; init; } = default!;
    public DateTime EmploymentDate { get; init; }
    public Guid DepartmentId { get; init; }
    public Guid JobGradeId { get; init; }
    public Guid PositionId { get; init; }
     public Guid? BranchId { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime DateAdd { get; init; }
    public DateTime? DateMod { get; init; }
    public string xmin { get; init; } = default!;
    public Guid PersonId { get; init; }
    public string FirstName { get; init; } = default!;
    public string MiddleName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string FirstNameAm { get; init; } = default!;
    public string MiddleNameAm { get; init; } = default!;
    public string LastNameAm { get; init; } = default!;
    public string Gender { get; init; } = default!;
    public string Nationality { get; init; } = default!;
    public byte[]? PhotoThumbnail { get; init; }
    public string? Email { get; init; }  // ? ADD THIS
    public string? Phone { get; init; }  // ? ADD THIS
}
public class EmployeeJoinRow
{
    // Employee fields
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string EmpState { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public string EmploymentNature { get; set; } = string.Empty;
    public string WorkArrangement { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Guid JobGradeId { get; set; }
    public Guid PositionId { get; set; }
    public Guid? ReportsToId { get; set; }
    public Guid PersonId { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public uint xmin { get; set; }

    // Person fields
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstNameAm { get; set; } = string.Empty;
    public string MiddleNameAm { get; set; } = string.Empty;
    public string LastNameAm { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;

    // Department fields
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentNameAm { get; set; } = string.Empty;

    // Branch fields
    public string BranchName { get; set; } = string.Empty;
    public string BranchNameAm { get; set; } = string.Empty;

    // JobGrade fields
    public string JobGradeName { get; set; } = string.Empty;

    // Position fields
    public string PositionName { get; set; } = string.Empty;
}
public sealed class EmpBioJoin
{
    public DateTime? BirthDate { get; init; }
    public string? MaritalStatus { get; init; }
    public string? AddressType { get; init; }
    public string? Region { get; init; }
    public string? Subcity { get; init; }
    public string? Zone { get; init; }
    public string? Woreda { get; init; }
    public string? Kebele { get; init; }
    public string? Telephone { get; init; }
}

public sealed class EmpGuaJoin
{
    public string FirstName { get; init; } = default!;
    public string MiddleName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Gender { get; init; } = default!;
    public string Nationality { get; init; } = default!;
    public string Relation { get; init; } = default!;
    public string AddressType { get; init; } = default!; // enum.AddressType (0/1)
    public string Region { get; init; } = default!;
    public string Subcity { get; init; } = default!;
    public string Zone { get; init; } = default!;
    public string Woreda { get; init; } = default!;
    public string Kebele { get; init; } = default!;
    public string Telephone { get; init; } = default!;
    public string FileName { get; init; } = default!;
    public long FileSize { get; init; } = default!;
    public string ContentType { get; init; } = default!;
}
// Profile.Domain.DTOs/EmployeeListDto.cs
public class EmployeeListDto : BaseDto
{
    public string EmpFullName { get; set; } = default!;
    public string EmpFullNameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string EmpState { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
    public string EmpType { get; set; } = default!;
    public string EmpNature { get; set; } = default!;
    public string WorkArr { get; set; } = default!;
    public string Photo { get; set; } = default!;

    // IDs
    public Guid DepartmentId { get; set; }
    public Guid JobGradeId { get; set; }
    public Guid PositionId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? ReportsToId { get; set; }  // direct manager / boss (HRM.Profile.Employee)
    public Guid PersonId { get; set; }  // ? ADD THIS

    // ? ADD ALL PERSON FIELDS
    public string FirstName { get; set; } = string.Empty;
    public string FirstNameAm { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string MiddleNameAm { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string LastNameAm { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

public class EmployeeModDto
{
    public Guid Id { get; set; }
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
    public Guid? ReportsToId { get; set; } //HRM.Profile.Employee (direct manager / boss)
    public string EmploymentType { get; set; } = default!; //enum.EmpType (0/1)
    public string EmploymentNature { get; set; } = default!; //enum.EmpNature (0/1)
    public string RowVersion { get; set; } = default!;
}

public sealed class EmpCodeJoin
{
    public string? Code { get; set; }
}
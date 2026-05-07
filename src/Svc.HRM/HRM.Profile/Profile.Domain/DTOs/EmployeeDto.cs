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

    public bool IsDeleted { get; init; }
    public DateTime DateAdd { get; init; }
    public DateTime? DateMod { get; init; }
    public string xmin { get; init; } = default!;

    public string FirstName { get; init; } = default!;
    public string MiddleName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string FirstNameAm { get; init; } = default!;
    public string MiddleNameAm { get; init; } = default!;
    public string LastNameAm { get; init; } = default!;
    public string Gender { get; init; } = default!;
    public string Nationality { get; init; } = default!;
    public byte[]? PhotoThumbnail { get; init; }
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
    public string EmploymentType { get; set; } = default!; //enum.EmpType (0/1)
    public string EmploymentNature { get; set; } = default!; //enum.EmpNature (0/1)
    public string RowVersion { get; set; } = default!;
}

public sealed class EmpCodeJoin
{
    public string? Code { get; set; }
}
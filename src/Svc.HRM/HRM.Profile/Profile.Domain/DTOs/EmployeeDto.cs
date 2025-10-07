using EthiopianCalendar;

namespace Profile.Domain.DTOs;

public class EmployeeListDto : BaseDto
{
    public Guid PersonId { get; set; } = default!; //Person
    public Guid JobGradeId { get; set; } = default!; //Cor.HRMM.JobGrade
    public Guid PositionId { get; set; } = default!; //Cor.HRMM.Position
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department
    public Guid EmploymentTypeId { get; set; } = default!; //Lup.EmploymentType
    public Guid EmploymentNatureId { get; set; } = default!; //Lup.EmploymentNature
    public string Gender { get; set; } = default!; // enum.Gender (0/1)
    public string Nationality { get; set; } = default!;
    public string Code { get; set; } = default!;
    public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;
    public string JobGrade { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Department{ get; set; } = default!;
    public string EmploymentType{ get; set; } = default!;
    public string EmploymentNature { get; set; } = default!;
    public string GenderStr { get; set; } = default!;
    public string EmpFullName { get; set; } = default!;
    public string EmpFullNameAm { get; set; } = default!;
    public string EmploymentDateStr => $"{EmploymentDate:MMMM dd, yyyy}";
    public string EmploymentDateStrAm => EmploymentDate.ToEthiopianDateString("MMMM dd, yyyy");
}

public class EmployeeAddDto
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
    public Guid EmploymentTypeId { get; set; } = default!; //Lup.EmploymentType
    public Guid EmploymentNatureId { get; set; } = default!; //Lup.EmploymentNature
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
    public Guid PersonId { get; set; } = default!; //Person
    public Guid JobGradeId { get; set; } = default!; //Cor.HRMM.JobGrade
    public Guid PositionId { get; set; } = default!; //Cor.HRMM.Position
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department
    public Guid EmploymentTypeId { get; set; } = default!; //Lup.EmploymentType
    public Guid EmploymentNatureId { get; set; } = default!; //Lup.EmploymentNature
    public string RowVersion { get; set; } = default!;
}
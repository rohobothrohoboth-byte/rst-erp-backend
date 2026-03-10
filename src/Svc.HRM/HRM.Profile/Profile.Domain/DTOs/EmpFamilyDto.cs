namespace Profile.Domain.DTOs;

public class EmpFamilyListDto : BaseDto
{
    public Guid PersonId { get; set; } = default!; //Person
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string Gender { get; set; } = default!; // enum.Gender (0/1)
    public string Nationality { get; set; } = default!;
    public string Relation { get; set; } = default!;
    public string GenderStr { get; set; } = default!;
    public string FamilyName { get; set; } = default!;
    public string FamilyNameAm { get; set; } = default!;
}

public class EmpFamilyAddDto
{
    public string FirstName { get; set; } = default!;
    public string FirstNameAm { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameAm { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameAm { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
    public string Nationality { get; set; } = default!;
    public string Relation { get; set; } = default!; //enum.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmpFamilyModDto
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
    public string Relation { get; set; } = default!; //enum.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid PersonId { get; set; } = default!; //Person
    public string RowVersion { get; set; } = default!;
}
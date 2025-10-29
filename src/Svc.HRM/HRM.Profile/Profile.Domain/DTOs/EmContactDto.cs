namespace Profile.Domain.DTOs;

public class EmContactListDto : BaseDto
{
    public Guid PersonId { get; set; } = default!; //Person
    public Guid AddressId { get; set; } = default!; //Address
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string Gender { get; set; } = default!; // enum.Gender (0/1)
    public string Nationality { get; set; } = default!;
    public string ContactName { get; set; } = default!;
    public string ContactNameAm { get; set; } = default!;
    public string GenderStr { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Relation { get; set; } = default!;
}

public class EmContactAddDto
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
    public Guid AddressId { get; set; } = default!; //Address
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmContactModDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string FirstNameAm { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameAm { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameAm { get; set; } = default!;
    public string Nationality { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid AddressId { get; set; } = default!; //Address
    public Guid PersonId { get; set; } = default!; //Person
}
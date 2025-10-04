namespace Profile.Domain.DTOs;

public class EmergencyContactListDto : BaseDto
{
    public Guid PersonId { get; set; } = default!; //Person
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string ContactFullName { get; set; } = default!; //Person
    public string Gender { get; set; } = default!; // enum.Gender
    public string Nationality { get; set; } = default!;
    public string Address { get; set; } = default!; //Cor.HRMM.Address
    public string Relation { get; set; } = default!; //lup.Relation
    public string EmpFullName { get; set; } = default!; //Employee
}

public class EmergencyContactAddDto
{
    public string FirstName { get; set; } = default!;
    public string FirstNameA { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameA { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameA { get; set; } = default!;
    public string Nationality { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmergencyContactModDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string FirstNameA { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameA { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameA { get; set; } = default!;
    public string Nationality { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string RowVersion { get; set; } = default!;
}
namespace Profile.Domain.DTOs;

public class EmpGuarantorListDto : BaseDto
{
    public Guid PersonId { get; set; } = default!; //Person
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string Gender { get; set; } = default!;  // enum.Gender (0/1)
    public string Nationality { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Relation { get; set; } = default!;
    public string GenderStr { get; set; } = default!;
    public string GuarantorName { get; set; } = default!;
    public string GuarantorNameAm { get; set; } = default!;
    public string EmpFullName { get; set; } = default!;
    public string EmpFullNameAm { get; set; } = default!;
}

public class EmpGuarantorAddDto
{
    public string FirstName { get; set; } = default!;
    public string FirstNameA { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameA { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameA { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender (0/1)
    public string Nationality { get; set; } = default!;
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmpGuarantorModDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string FirstNameA { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameA { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameA { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender (0/1)
    public string Nationality { get; set; } = default!;
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string RowVersion { get; set; } = default!;
}
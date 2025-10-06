namespace Profile.Domain.Entities;

public class Person : BaseEntity
{
    public string FirstName { get; set; } = default!;
    public string FirstNameAm { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameAm { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameAm { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
    public string Nationality { get; set; } = default!;

    //******************************************//

    public string FullName => $"{FirstName} {MiddleName} {LastName}";
    public string FullNameAm => $"{FirstNameAm} {MiddleNameAm} {LastNameAm}";
}
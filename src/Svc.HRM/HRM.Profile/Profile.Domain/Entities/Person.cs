namespace Profile.Domain.Entities;

public class Person : BaseEntity
{
    public string FirstName { get; set; } = default!;
    public string FirstNameAm { get; set; } = "";
    public string MiddleName { get; set; } = default!;
    public string MiddleNameAm { get; set; } = "";
    public string LastName { get; set; } = default!;
    public string LastNameAm { get; set; } = "";
    public string Gender { get; set; } = default!; // enum.Gender
    public string Nationality { get; set; } = default!;
}
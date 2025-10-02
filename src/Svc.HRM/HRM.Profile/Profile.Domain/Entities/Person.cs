namespace Profile.Domain.Entities;

public class Person : BaseEntity
{
    public string FirstName { get; set; } = default!;
    public string FirstNameA { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameA { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameA { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
    public string Nationality { get; set; } = default!;
}

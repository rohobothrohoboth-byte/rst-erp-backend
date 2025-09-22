namespace Cor.HRMM.Models.Entities;

public class Person : BaseEntity
{
    public string FirstName { get; set; } = default!;
    public string FirstNameA { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameA { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameA { get; set; } = default!;
    public Gender Gender { get; set; } = default!;
}

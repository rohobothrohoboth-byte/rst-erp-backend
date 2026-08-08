namespace Profile.Domain.DTOs;

public class NameList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class NameAmList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
}

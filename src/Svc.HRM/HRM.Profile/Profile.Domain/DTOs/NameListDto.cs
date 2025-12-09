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

public class EmpSearchRes
{
    public Guid Id { get; set; }
    public string Photo { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string FullNameAm { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Dept { get; set; } = default!;
    public string Position { get; set; } = default!;
}


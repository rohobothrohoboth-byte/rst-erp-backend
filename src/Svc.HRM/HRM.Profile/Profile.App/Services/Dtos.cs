namespace Profile.App.Services;

public class LupListDto
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

public class NameList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class NameAmListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
}
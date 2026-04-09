namespace Cor.HRMM.Models.DTOs;

public class NameList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class DoubleList
{
    public double Name { get; set; } = 0;
}

public class NameAmList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
}


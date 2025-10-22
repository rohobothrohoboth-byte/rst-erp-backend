namespace Cor.Module.Models.DTOs;

public class NameListDto
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
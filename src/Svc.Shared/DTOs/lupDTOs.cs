namespace Svc.Shared.DTOs;

public class LupCodeListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
}

public class LupListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}
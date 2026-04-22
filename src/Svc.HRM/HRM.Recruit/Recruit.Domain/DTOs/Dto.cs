namespace Recruit.Domain.DTOs;

public class StatChangeDto
{
    public Guid Id { get; set; }
    public bool Stat { get; set; } = true;
    public string RowVersion { get; set; } = default!;
}

public class IdListDto
{
    public Guid Id { get; set; }
}
namespace Cor.Domain.DTOs;

public class CompListDto : BaseDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public int BranchCount { get; set; }
}

public class AddCompDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
}

public class EditCompDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}

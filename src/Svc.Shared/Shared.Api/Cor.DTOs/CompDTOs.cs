namespace Shared.Api.Cor.DTOs;
public class CompListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public int BranchCount { get; set; } = 0;
    public string CreatedAt { get; set; } = default!;
    public string CreatedAtAm { get; set; } = default!;
    public string ModifiedAt { get; set; } = default!;
    public string ModifiedAtAm { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}

public class AddCompDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
}

public class EditCompDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}

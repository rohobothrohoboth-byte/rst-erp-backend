namespace Shared.Api.Cor.DTOs;
public class BranchListDto
{
    public Guid Id { get; set; }
    public Guid CompId { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string DateOpened { get; set; } = default!;
    public string DateOpenedAm { get; set; } = default!;
    public string Comp { get; set; } = default!;
    public string CompAm { get; set; } = default!;
    public string CreatedAt { get; set; } = default!;
    public string CreatedAtAm { get; set; } = default!;
    public string ModifiedAt { get; set; } = default!;
    public string ModifiedAtAm { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}

public class AddBranchDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public BranchType Type { get; set; } = BranchType.HeadOff;
    public BranchStat Status { get; set; } = BranchStat.Active;
    public DateTime DateOpened { get; set; } = DateTime.UtcNow;
    public Guid CompId { get; set; }
}

public class EditBranchDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public BranchType Type { get; set; } = BranchType.HeadOff;
    public BranchStat Status { get; set; } = BranchStat.Active;
    public DateTime DateOpened { get; set; } = DateTime.UtcNow;
    public Guid CompId { get; set; }
    public string RowVersion { get; set; } = default!;
}

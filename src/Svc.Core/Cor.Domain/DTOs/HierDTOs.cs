namespace Cor.Domain.DTOs;
public class HierListDto: BaseDto
{
    public string Parent { get; set; } = default!;
    public string Child { get; set; } = default!;
    public string ParentAm { get; set; } = default!;
    public string ChildAm { get; set; } = default!;
}

public class AddHierDto
{
    public Guid ParentId { get; set; }
    public Guid ChildId { get; set; }
}

public class EditHierDto
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public Guid ChildId { get; set; }
    public string RowVersion { get; set; } = default!;
}

namespace Module.Domain.Entities;

public class Hierarchy : BaseEntity
{
    public Guid ParentId { get; set; }
    public Guid ChildId { get; set; }

    //******************************************//

    public Company Parent { get; set; } = null!;
    public Company Child { get; set; } = null!;
}
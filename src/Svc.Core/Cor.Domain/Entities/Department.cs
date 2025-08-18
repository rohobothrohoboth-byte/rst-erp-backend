namespace Cor.Domain.Entities;
public class Department : BaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public DeptStat Status { get; set; } = DeptStat.Active;
    public Guid BranchId { get; set; }

    //******************************************//

    public Branch Branch { get; set; } = default!;
}
namespace Cor.Module.Models.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string DeptStat { get; set; } = default!;
    public Guid BranchId { get; set; }

    //******************************************//

    public Branch Branch { get; set; } = null!;
}
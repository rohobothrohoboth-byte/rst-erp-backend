namespace Cor.Module.Models.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string DeptStat { get; set; } = default!;
    public Guid BranchId { get; set; }

    // Detail captured at registration
    public string? ManagerName { get; set; }
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    //******************************************//

    public Branch Branch { get; set; } = null!;
}
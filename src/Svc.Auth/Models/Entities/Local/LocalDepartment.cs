// Svc.Auth/Models/Entities/Local/LocalDepartment.cs
namespace Svc.Auth.Models.Entities.Local;

public class LocalDepartment : LocalBaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string DeptStat { get; set; } = default!;
    public Guid BranchId { get; set; }

    public LocalBranch Branch { get; set; } = null!;
}
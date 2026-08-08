// Svc.Auth/Models/Entities/Local/LocalBranch.cs
namespace Svc.Auth.Models.Entities.Local;

public class LocalBranch : LocalBaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime OpenDate { get; set; } = DateTime.UtcNow;
    public string BranchType { get; set; } = default!;
    public string BranchStat { get; set; } = default!;
    public Guid CompId { get; set; }

    public LocalCompany Company { get; set; } = null!;
    public ICollection<LocalDepartment> Departments { get; set; } = new List<LocalDepartment>();
}
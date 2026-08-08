using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leave.Domain.Entities.Local;


public class LocalDepartment
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string DeptStat { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

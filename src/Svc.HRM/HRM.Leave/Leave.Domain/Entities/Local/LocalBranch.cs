using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Leave.Domain.Entities.Local;



public class LocalBranch
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime OpenDate { get; set; }
    public string BranchType { get; set; } = string.Empty;
    public string BranchStat { get; set; } = string.Empty;
    public Guid CompId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}



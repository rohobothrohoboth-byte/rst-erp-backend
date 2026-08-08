using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities.Local;


public class LocalBranch : LocalBaseEntity
{
    [MaxLength(200)]
    public string Name { get; set; } = default!;

    [MaxLength(200)]
    public string NameAm { get; set; } = default!;

    [MaxLength(20)]
    public string Code { get; set; } = default!;

    [MaxLength(500)]
    public string? Location { get; set; }

    public Guid? CompId { get; set; }

   // public bool IsDeleted { get; set; } = false;
   // public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

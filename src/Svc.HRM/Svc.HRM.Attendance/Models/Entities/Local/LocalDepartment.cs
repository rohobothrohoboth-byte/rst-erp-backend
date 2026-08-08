using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Svc.HRM.Attendance.Models.Entities.Local;


public class LocalDepartment : LocalBaseEntity
{
    [MaxLength(200)]
    public string Name { get; set; } = default!;

    [MaxLength(200)]
    public string NameAm { get; set; } = default!;

    public Guid? BranchId { get; set; }

   // public bool IsDeleted { get; set; } = false;
   // public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

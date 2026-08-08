// Svc.Auth/Models/Entities/Local/LocalBaseEntity.cs
namespace Svc.HRM.Payroll.Models.Entities.Local;


public abstract class LocalBaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}
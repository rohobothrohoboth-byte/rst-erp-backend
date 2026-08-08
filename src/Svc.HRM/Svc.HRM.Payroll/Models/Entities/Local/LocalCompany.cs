using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Svc.HRM.Payroll.Models.Entities.Local;


public class LocalCompany : LocalBaseEntity
{
    [MaxLength(200)]
    public string Name { get; set; } = default!;

    [MaxLength(200)]
    public string NameAm { get; set; } = default!;

    [MaxLength(20)]
    public string? TaxId { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

   // public bool IsDeleted { get; set; } = false;
    //public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

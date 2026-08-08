// Svc.Auth/Models/Entities/Local/LocalCompany.cs
namespace Svc.Auth.Models.Entities.Local;

public class LocalCompany : LocalBaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string? TaxId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }

    public ICollection<LocalBranch> Branches { get; set; } = new List<LocalBranch>();
}
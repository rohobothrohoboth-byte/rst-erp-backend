// Svc.Auth/Models/Entities/Local/LocalJobGrade.cs
namespace Cor.Finance.Models.Entities.Local;

public class LocalJobGrade : LocalBaseEntity
{
    public string Name { get; set; } = default!;
    public double StartSalary { get; set; }
    public double MaxSalary { get; set; }
}
using System.Text.Json.Serialization;

namespace Module.Domain.Entities;

public class FiscalYear : BaseEntity
{
    public string Name { get; set; } = default!;
    [JsonIgnore]
    public DateTime DateStart { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public DateTime DateEnd { get; set; } = DateTime.UtcNow.AddDays(365);
    public string IsActive { get; set; } = default!;
}
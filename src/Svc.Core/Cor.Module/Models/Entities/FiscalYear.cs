using System.Text.Json.Serialization;

namespace Cor.Module.Models.Entities;

public class FiscalYear : BaseEntity
{
    public string Name { get; set; } = default!;
    [JsonIgnore]
    public DateTime DateStart { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public DateTime DateEnd { get; set; } = DateTime.UtcNow.AddDays(365);
    public string IsActive { get; set; } = default!;

        public ICollection<Period> Periods { get; set; } = new List<Period>();
        public ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
}
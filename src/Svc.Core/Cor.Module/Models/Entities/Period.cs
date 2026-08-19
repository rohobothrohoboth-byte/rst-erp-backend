using System.Text.Json.Serialization;

namespace Cor.Module.Models.Entities;

public class Period : BaseEntity
{
    public string Name { get; set; } = default!;
    [JsonIgnore]
    public DateTime DateStart { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public DateTime DateEnd { get; set; } = DateTime.UtcNow;
    public string IsActive { get; set; } = default!; // Enum.YesNo
    public string Quarter { get; set; } = default!; // Enum.Quarter
    public string? PeriodType { get; set; } // "Weekly"|"Monthly"|"Quarterly"|"SemiAnnual"|"EightMonth"|"Yearly"|"Custom"
    public Guid FiscalYearId { get; set; }

    //******************************************//

    public FiscalYear FiscalYear { get; set; } = null!;
}

using Cor.Domain.Enums;
using System.Text.Json.Serialization;

namespace Cor.Domain.Entities;
public class FiscalYear : BaseEntity
{
    public string Name { get; set; } = default!;
    [JsonIgnore]
    public DateTime DateStart { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public DateTime DateEnd { get; set; } = DateTime.UtcNow.AddDays(365);
    public YesNo IsActive { get; set; } = YesNo.Yes;
    //public string StartDate => $"{DateStart:MMMM dd, yyyy}";
    //public string EndDate => $"{DateEnd:MMMM dd, yyyy}";

    //public string StartDateAm => DateStart.ToEthiopianDateString("MMMM dd, yyyy");
    //public string EndDateAm => DateEnd.ToEthiopianDateString("MMMM dd, yyyy");
}
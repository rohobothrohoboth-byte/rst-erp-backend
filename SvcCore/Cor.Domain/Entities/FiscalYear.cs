using System.ComponentModel;
using System.Text.Json.Serialization;
using EthiopianCalendar;

namespace Cor.Domain.Entities
{
    public class FiscalYear : BaseEntity
    {
        public required string Name { get; set; }
        [JsonIgnore]
        public DateTime DateStart { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public DateTime DateEnd { get; set; } = DateTime.UtcNow.AddDays(365);
        public bool IsActive { get; set; }
        public string StartDate => $"{DateStart:MMMM dd, yyyy}";
        public string EndDate => $"{DateEnd:MMMM dd, yyyy}";

        public string StartDateAm => DateStart.ToEthiopianDateString("MMMM dd, yyyy");
        public string EndDateAm => DateEnd.ToEthiopianDateString("MMMM dd, yyyy");
    }
}

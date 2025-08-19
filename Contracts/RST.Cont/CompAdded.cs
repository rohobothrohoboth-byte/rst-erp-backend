using System.Text.Json.Serialization;
using EthiopianCalendar;

namespace RST.Cont;

public class CompAdded
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NameAm { get; set; }
    public int BranchCount { get; set; }
    [JsonIgnore]
    public DateTime DateAdd { get; set; }
    [JsonIgnore]
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
    public string CreatedAt => $"{DateAdd:MMMM dd, yyyy}";
    public string CreatedAtAm => DateAdd.ToEthiopianDateString("MMMM dd, yyyy");
    public string ModifiedAt => DateMod.HasValue ? $"{DateMod:MMMM dd, yyyy}" : "";
    public string ModifiedAtAm => DateMod.HasValue ? DateMod.Value.ToEthiopianDateString("MMMM dd, yyyy") : "";
}

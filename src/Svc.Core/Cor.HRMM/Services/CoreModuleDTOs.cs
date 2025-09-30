using System.Text.Json.Serialization;
using EthiopianCalendar;

namespace Cor.HRMM.Services;

public abstract class BaseDto
{
    public Guid Id { get; set; }
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

public class DeptListDto : BaseDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string DeptStat { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string BranchAm { get; set; } = default!;
}
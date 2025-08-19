using System.Text.Json.Serialization;
using EthiopianCalendar;
using Shared.Api.Cor.DTOs;

namespace Cor.Domain.DTOs;
public class BranchListDto : BaseDto
{
    public Guid CompId { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Comp { get; set; } = default!;
    public string CompAm { get; set; } = default!;
    [JsonIgnore]
    public DateTime OpenDate { get; set; }
    public string DateOpenedAm => OpenDate.ToEthiopianDateString("MMMM dd, yyyy");
    public string DateOpened => $"{OpenDate:MMMM dd, yyyy}";
}

public class AddBranchDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public BranchType Type { get; set; } = BranchType.HeadOff;
    public BranchStat Status { get; set; } = BranchStat.Active;
    public DateTime DateOpened { get; set; } = DateTime.UtcNow;
    public Guid CompId { get; set; }
}

public class EditBranchDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public BranchType Type { get; set; } = BranchType.HeadOff;
    public BranchStat Status { get; set; } = BranchStat.Active;
    public DateTime DateOpened { get; set; } = DateTime.UtcNow;
    public Guid CompId { get; set; }
    public string RowVersion { get; set; } = default!;
}

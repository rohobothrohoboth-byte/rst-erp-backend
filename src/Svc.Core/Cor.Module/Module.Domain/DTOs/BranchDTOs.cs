using System.Text.Json.Serialization;
using EthiopianCalendar;

namespace Module.Domain.DTOs;

public class BranchListDto : BaseDTO
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string BranchType { get; set; } = default!;
    public string BranchStat { get; set; } = default!;
    public string Comp { get; set; } = default!;
    public string CompAm { get; set; } = default!;
    [JsonIgnore]
    public DateTime OpenDate { get; set; }
    public string DateOpenedAm => OpenDate.ToEthiopianDateString("MMMM dd, yyyy");
    public string DateOpened => $"{OpenDate:MMMM dd, yyyy}";
}

public class BranchCompListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
}

public class AddBranchDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime DateOpened { get; set; } = DateTime.UtcNow;
    public string BranchType { get; set; } = default!;
    public Guid CompId { get; set; }
}

public class EditBranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime DateOpened { get; set; } = DateTime.UtcNow;
    public string BranchType { get; set; } = default!;
    public string BranchStat { get; set; } = default!;
    public Guid CompId { get; set; }
    public string RowVersion { get; set; } = default!;
}

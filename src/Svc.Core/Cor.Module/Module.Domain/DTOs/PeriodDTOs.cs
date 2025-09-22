using EthiopianCalendar;
using System.Text.Json.Serialization;

namespace Module.Domain.DTOs;

public class PeriodListDto : BaseDTO
{
    public string Name { get; set; } = default!;
    public string Quarter { get; set; } = default!;
    public string FiscYear { get; set; } = default!;
    public string IsActive { get; set; } = default!;
    [JsonIgnore]
    public DateTime DateStart { get; set; }
    [JsonIgnore]
    public DateTime DateEnd { get; set; }

    public string StartDate => $"{DateStart:MMMM dd, yyyy}";
    public string StartDateAm => DateStart.ToEthiopianDateString("MMMM dd, yyyy");
    public string EndDate => $"{DateEnd:MMMM dd, yyyy}";
    public string EndDateAm => DateEnd.ToEthiopianDateString("MMMM dd, yyyy");
}

public class AddPeriodDto
{
    public required string Name { get; set; }
    public DateTime DateStart { get; set; } = DateTime.UtcNow;
    public DateTime DateEnd { get; set; } = DateTime.UtcNow;
    public string IsActive { get; set; } = default!;
    public Guid QuarterId { get; set; }
    public Guid FiscalYearId { get; set; }
}

public class EditPeriodDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public string IsActive { get; set; } = default!;
    public Guid QuarterId { get; set; }
    public Guid FiscalYearId { get; set; }
    public string RowVersion { get; set; } = default!;
}

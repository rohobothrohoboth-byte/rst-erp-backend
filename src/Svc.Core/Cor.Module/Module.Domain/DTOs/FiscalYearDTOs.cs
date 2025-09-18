using EthiopianCalendar;
using System.Text.Json.Serialization;

namespace Module.Domain.DTOs;

public class FiscYearListDto : BaseDTO
{
    public string Name { get; set; } = default!;
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

public class AddFiscYearDto
{
    public required string Name { get; set; }
    public DateTime DateStart { get; set; } = DateTime.UtcNow;
    public DateTime DateEnd { get; set; } = DateTime.UtcNow.AddDays(365);
    public string IsActive { get; set; } = default!;
}

public class EditFiscYearDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public string IsActive { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}
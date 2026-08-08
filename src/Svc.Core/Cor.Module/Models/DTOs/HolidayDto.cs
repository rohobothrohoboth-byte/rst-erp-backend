using EthiopianCalendar;

namespace Cor.Module.Models.DTOs;

public class HolidayListDto : BaseDTO
{
    public Guid FiscalYearId { get; set; }  // FiscalYear
    public string Name { get; set; } = default!;
    public DateTime Date { get; set; }
    public bool IsPublic { get; set; }  // bool (true/false)
    public string IsPublicStr { get; set; } = default!;
    public string FiscYear { get; set; } = default!;
    public string DateStr => $"{Date:MMMM dd, yyyy}";
    public string DateStrAm => Date.ToEthiopianDateString("MMMM dd, yyyy");
}

public class AddHolidayDto
{
    public string Name { get; set; } = default!;
    public DateTime Date { get; set; } = default!;
    public bool IsPublic { get; set; }  // bool (true/false)
    public Guid FiscalYearId { get; set; }
}

public class EditHolidayDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime Date { get; set; } = default!;
    public bool IsPublic { get; set; }  // bool (true/false)
    public string RowVersion { get; set; } = default!;
}
using EthiopianCalendar;

namespace Cor.Module.Models.DTOs;

public class FiscYearListDto : BaseDTO
{
    public string Name { get; set; } = default!;
    public string IsActive { get; set; } = default!;  // enum.YesNo (0/1)
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }

    public string IsActiveStr { get; set; } = default!;
    public string DateStartStr => $"{DateStart:MMMM dd, yyyy}";
    public string DateStartStrAm => DateStart.ToEthiopianDateString("MMMM dd, yyyy");
    public string DateEndStr => $"{DateEnd:MMMM dd, yyyy}";
    public string DateEndStrAm => DateEnd.ToEthiopianDateString("MMMM dd, yyyy");
}

public class AddFiscYearDto
{
    public required string Name { get; set; }
    public DateTime DateStart { get; set; } = DateTime.UtcNow;
    public DateTime DateEnd { get; set; } = DateTime.UtcNow.AddDays(365);
}

public class EditFiscYearDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public string IsActive { get; set; } = default!;// Enum.YesNo
    public string RowVersion { get; set; } = default!;
}
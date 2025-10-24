using EthiopianCalendar;

namespace Cor.Module.Models.DTOs;

public class PeriodListDto : BaseDTO
{
    public string Quarter { get; set; }  // enum.Quarter (0/1)
    public Guid FiscalYearId { get; set; }  // FiscalYear
    public string Name { get; set; } = default!;
    public string QuarterStr { get; set; } = default!;
    public string FiscYear { get; set; } = default!;
    public string IsActive { get; set; } = default!;  // enum.YesNo (0/1)
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public string IsActiveStr { get; set; } = default!;
    public string DateStartStr => $"{DateStart:MMMM dd, yyyy}";
    public string DateStartStrAm => DateStart.ToEthiopianDateString("MMMM dd, yyyy");
    public string DateEndStr => $"{DateEnd:MMMM dd, yyyy}";
    public string DateEndStrAm => DateEnd.ToEthiopianDateString("MMMM dd, yyyy");
}

public class AddPeriodDto
{
    public string Name { get; set; }
    public DateTime DateStart { get; set; } = DateTime.UtcNow;
    public DateTime DateEnd { get; set; } = DateTime.UtcNow;
    public string IsActive { get; set; } = default!;  // enum.YesNo (0/1)
    public string Quarter { get; set; }  // enum.Quarter (0/1)
    public Guid FiscalYearId { get; set; }  // FiscalYear
}

public class EditPeriodDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public string IsActive { get; set; } = default!;  // enum.YesNo (0/1)
    public string Quarter { get; set; }  // enum.Quarter (0/1)
    public Guid FiscalYearId { get; set; }  // FiscalYear
    public string RowVersion { get; set; } = default!;
}
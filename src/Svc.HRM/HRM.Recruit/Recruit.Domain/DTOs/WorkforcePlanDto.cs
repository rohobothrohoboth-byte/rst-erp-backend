namespace Recruit.Domain.DTOs;

public class WorkforcePlanListDto : BaseDto
{
    public string PlanCode { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPositions { get; set; }
    public int AppPositions { get; set; } = 0;
    public string Status { get; set; } = default!; // enum.ReqStatus(0/1)
    public string StatusStr { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Period { get; set; } = default!;
    public string RequistionBy { get; set; } = default!;
    public Guid? PeriodId { get; set; } // Cor.Module.Period (or FiscalYear)
}

public class WorkforcePlanAddDto
{
    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPositions { get; set; }
    public Guid? PeriodId { get; set; } // Cor.Module.Period (or FiscalYear)
    public Guid RequistionById { get; set; } // HRM.Profile.Employee
}

public class WorkforcePlanModDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPositions { get; set; }
    public string RowVersion { get; set; } = default!;
}
using EthiopianCalendar;

namespace Leave.Domain.DTOs;

public class LeaveBalanceListDto : BaseDto
{
    public Guid EmployeeId { get; set; } //HRM.Profile.Employee
    public Guid FiscalYearId { get; set; } //Cor.Module.FiscalYear
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public double Balance { get; set; }
    public double Carried { get; set; }
    public DateTime? CarriedExpireDate { get; set; }
    public string CarriedExpireDateStr => CarriedExpireDate.HasValue ? $"{CarriedExpireDate:MMMM dd, yyyy}" : "";
    public string CarriedExpireDateStrAm => CarriedExpireDate.HasValue ? CarriedExpireDate.Value.ToEthiopianDateString("MMMM dd, yyyy") : "";
    public string Comments { get; set; } = default!;
    public string LeavePolicy { get; set; } = default!;
    public string FiscalYear { get; set; } = default!;
    public string EmployeeName { get; set; } = default!;
}

public class LeaveBalanceAddDto
{
    public Guid EmployeeId { get; set; } //HRM.Profile.Employee
    public Guid FiscalYearId { get; set; } //Cor.Module.FiscalYear
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public double Balance { get; set; }
    public double Carried { get; set; }
    public DateTime? CarriedExpiresAt { get; set; }
}

public class LeaveBalanceModDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; } //HRM.Profile.Employee
    public Guid FiscalYearId { get; set; } //Cor.Module.FiscalYear
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public double Balance { get; set; }
    public double Carried { get; set; }
    public DateTime? CarriedExpiresAt { get; set; }
    public string RowVersion { get; set; } = default!;
}
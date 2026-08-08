// Leave.Domain/DTOs/YearEndProcessingDto.cs

namespace Leave.Domain.DTOs;


public class FiscalYearDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateStart { get; set; }  // Changed from StartDate
    public DateTime DateEnd { get; set; }    // Changed from EndDate
    public string IsActive { get; set; } = "0";
    public DateTime? DateAdd { get; set; }   // Optional: include if needed
    public DateTime? DateMod { get; set; }   // Optional: include if needed
    public bool IsDeleted { get; set; }      // Optional: include if needed
}
public class YearEndProcessResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int EmployeesProcessed { get; set; }
    public int CarryoverRecordsCreated { get; set; }
    public int EncashmentRecordsCreated { get; set; }
    public List<YearEndProcessError> Errors { get; set; } = new();
    public DateTime ProcessedAt { get; set; }
}

public class YearEndProcessError
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public Guid LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

public class CarryoverCalculationDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public Guid LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public decimal RemainingBalance { get; set; }
    public decimal MaxCarryoverDays { get; set; }
    public decimal CarryoverAmount { get; set; }
    public decimal EncashmentAmount { get; set; }
    public decimal LostAmount { get; set; }
     public string FiscalYearName { get; set; } = string.Empty;
}

public class CarryoverHistoryDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public Guid LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public decimal CarryoverAmount { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string ProcessedBy { get; set; } = string.Empty;
}

public class YearEndProcessRequestDto
{
    public Guid FiscalYearId { get; set; }  // Use FiscalYearId instead of int
    public DateTime ProcessingDate { get; set; } = DateTime.UtcNow;
    public bool ProcessCarryover { get; set; } = true;
    public bool ProcessEncashment { get; set; } = false;
    public Guid? ProcessedBy { get; set; }
}

public class FiscalYearInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
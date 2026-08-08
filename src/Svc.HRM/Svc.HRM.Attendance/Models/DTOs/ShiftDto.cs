namespace Svc.HRM.Attendance.Models.DTOs;

public class ShiftDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string? Description { get; set; }
    public string StartTime { get; set; } = default!;
    public string EndTime { get; set; } = default!;
    public string BreakStartTime { get; set; } = default!;
    public string BreakEndTime { get; set; } = default!;
    public double BreakDurationHours { get; set; }
    public double TotalHours { get; set; }
    public bool IsActive { get; set; }
    public string? ColorCode { get; set; }
}

public class ShiftCreateDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string? Description { get; set; }
    public string StartTime { get; set; } = default!;
    public string EndTime { get; set; } = default!;
     public string BreakEndTime { get; set; } = default!;
    public string BreakStartTime { get; set; } = default!;
    public double BreakDurationHours { get; set; }
    public string? ColorCode { get; set; }
}

public class ShiftAssignmentDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public Guid ShiftId { get; set; }
    public string ShiftName { get; set; } = default!;
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}

public class ShiftAssignmentCreateDto
{
    public Guid EmployeeId { get; set; }
    public Guid ShiftId { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}
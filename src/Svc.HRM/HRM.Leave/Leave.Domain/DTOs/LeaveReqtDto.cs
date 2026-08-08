using System.Text.Json.Serialization;

namespace Leave.Domain.DTOs;

public sealed class LvRqstAddDto
{
    [JsonIgnore]
    public Guid EmpId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public bool IsHalfDay { get; set; } = false;
    public string Comments { get; set; } = default!;
}

public sealed class LvRqstModDto
{
    [JsonIgnore]
    public Guid EmpId { get; set; } // HRM.Profile.Employee
    public Guid Id { get; set; }
    public Guid LeaveTypeId { get; set; } // LeaveType
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public bool IsHalfDay { get; set; } = default!;
    public string Comments { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}

public sealed class LvRqstRevDto
{
    [JsonIgnore]
    public Guid AppId { get; set; } // HRM.Profile.Employee

    public Guid Id { get; set; }
    public bool Decision { get; set; } = true; // True = Accept, False = Deny
    public string? Comment { get; set; }
}

public sealed class AppStepQryDto
{
    public Guid Id { get; set; } // LeaveRequest
    public Guid EmpId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public int Cur { get; set; }
}


public sealed class MyPendLvList : RawBaseDto
{
    [JsonIgnore]
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    [JsonIgnore]
    public Guid LeaveTypeId { get; set; } // LeaveType
    [JsonIgnore]
    public double DaysRequested { get; set; } = default!;
    [JsonIgnore]
    public DateTime StartDate { get; set; } = default!;
    [JsonIgnore]
    public DateTime EndDate { get; set; } = default!;
    [JsonIgnore]
    public DateTime DateRequested { get; set; } = default!;
    [JsonIgnore]
    public bool IsHalfDay { get; set; } = false;
    [JsonIgnore]
    public int CurrentAppStep { get; set; }

    public string DaysRequestedStr { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.Status (0/1)
    public string StartDateStr => StartDate.ToString("MMMM dd, yyyy");
    public string EndDateStr => EndDate.ToString("MMMM dd, yyyy");
    public string DateRequestedStr => DateRequested.ToString("MMMM dd, yyyy");
    public string Comments { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
    public double PerApp { get; set; } = 0;
    public bool DelMod { get; set; } = false;
    public List<MyPendLvApp> AppStep { get; set; } = [];
}

public sealed class MyPendLvApp
{
    [JsonIgnore]
    public DateTime? DateApp { get; set; } = default!;

    public string Step { get; set; } = default!;
    public string AppBy { get; set; } = default!;
    public string Decision { get; set; } = default!;
    public string DateAppStr => DateApp.HasValue ? $"{DateApp:MMMM dd, yyyy}" : "";
    public bool IsFinal { get; set; } = false;
    public bool IsCurrent { get; set; } = false;
    public string? Comment { get; set; }
}

public sealed class PendLvReqList : RawBaseDto
{
    [JsonIgnore]
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    [JsonIgnore]
    public double DaysRequested { get; set; } = default!;
    [JsonIgnore]
    public bool IsHalfDay { get; set; } = false;
    [JsonIgnore]
    public DateTime StartDate { get; set; } = default!;
    [JsonIgnore]
    public DateTime EndDate { get; set; } = default!;
    [JsonIgnore]
    public DateTime DateRequested { get; set; } = default!;

    public string DaysRequestedStr { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.Status (0/1)
    public string StartDateStr => StartDate.ToString("MMMM dd, yyyy");
    public string EndDateStr => EndDate.ToString("MMMM dd, yyyy");
    public string DateRequestedStr => DateRequested.ToString("MMMM dd, yyyy");
    public string EmpName { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
    public double PerApp { get; set; } = 0;
}

public sealed class MyHistLvList
{
    [JsonIgnore]
    public double DaysRequested { get; set; } = default!;
    [JsonIgnore]
    public DateTime StartDate { get; set; } = default!;
    [JsonIgnore]
    public DateTime EndDate { get; set; } = default!;
    [JsonIgnore]
    public DateTime DateRequested { get; set; } = default!;
    [JsonIgnore]
    public bool IsHalfDay { get; set; } = false;

    public Guid Id { get; set; }
    public string DaysRequestedStr { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.Status (0/1)
    public string StartDateStr => StartDate.ToString("MMMM dd, yyyy");
    public string EndDateStr => EndDate.ToString("MMMM dd, yyyy");
    public string DateRequestedStr => DateRequested.ToString("MMMM dd, yyyy");
    public string LeaveType { get; set; } = default!;
    public double PerApp { get; set; } = 0;
}

public sealed class HistLvReqList
{
    [JsonIgnore]
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    [JsonIgnore]
    public double DaysRequested { get; set; } = default!;
    [JsonIgnore]
    public bool IsHalfDay { get; set; } = false;
    [JsonIgnore]
    public DateTime StartDate { get; set; } = default!;
    [JsonIgnore]
    public DateTime EndDate { get; set; } = default!;
    [JsonIgnore]
    public DateTime DateRequested { get; set; } = default!;

    public Guid Id { get; set; }
    public string DaysRequestedStr { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.Status (0/1)
    public string StartDateStr => StartDate.ToString("MMMM dd, yyyy");
    public string EndDateStr => EndDate.ToString("MMMM dd, yyyy");
    public string DateRequestedStr => DateRequested.ToString("MMMM dd, yyyy");
    public string EmpName { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
    public double PerApp { get; set; } = 0;
}

public sealed class ViewLvReqJoin
{
    public Guid Id { get; set; } // LeaveRequest
    public DateTime DateAdd { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DaysRequested { get; set; }
    public bool IsHalfDay { get; set; }
    public string Status { get; set; } = default!; // enum.Status (0/1)
    public string Comments { get; set; } = default!;
    public int CurrentAppStep { get; set; }
    public double PerApp { get; set; }
    public DateTime? DateApp { get; set; }
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid BranchId { get; set; } // Cor.Module.Branch
    public Guid DeptId { get; set; } // Cor.Module.Department
    public Guid LeaveTypeId { get; set; } // LeaveType
    public string LeaveType { get; set; } = default!;
}

public sealed class ViewLvReqDto
{
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public string TotalDaysReq { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string StartDate { get; set; } = default!;
    public string EndDate { get; set; } = default!;
    public string DateRequested { get; set; } = default!;
    public string DateApp { get; set; } = default!;
    public string EmpName { get; set; } = default!;
    public string EmpNameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string Dept { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
    public double PerApp { get; set; } = 0;
    public string Comments { get; set; } = default!;
    public List<MyPendLvApp> AppStep { get; set; } = [];
}
using EthiopianCalendar;
using System.Text.Json.Serialization;

namespace Leave.Domain.DTOs;

public class LeaveRequestListDto : BaseDto
{
    [JsonIgnore]
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    [JsonIgnore]
    public Guid? ApprovedById { get; set; } // HRM.Profile.Employee
    [JsonIgnore]
    public double DaysRequested { get; set; } = default!;
    [JsonIgnore]
    public bool IsHalfDay { get; set; } = false;
    [JsonIgnore]
    public string Status { get; set; } = default!; // enum.Status (0/1)
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public DateTime DateRequested { get; set; } = default!;
    public string DaysRequestedStr { get; set; } = default!;
    public string IsHalfDayStr { get; set; } = default!;
    public string StatusStr { get; set; } = default!;
    public string StartDateStr => $"{StartDate:MMMM dd, yyyy}";
    public string StartDateStrAm => StartDate.ToEthiopianDateString("MMMM dd, yyyy");
    public string EndDateStr => $"{EndDate:MMMM dd, yyyy}";
    public string EndDateStrAm => EndDate.ToEthiopianDateString("MMMM dd, yyyy");
    public string DateRequestedStr => $"{DateRequested:MMMM dd, yyyy}";
    public string DateRequestedStrAm => DateRequested.ToEthiopianDateString("MMMM dd, yyyy");
    public string Employee { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
}

public class LeaveRequestAddDto
{
    public Guid LeaveTypeId { get; set; } // LeaveType
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public bool IsHalfDay { get; set; } = false;
    public string Comments { get; set; } = default!;
}

public class LeaveRequestModDto
{
    public Guid Id { get; set; }
    public Guid LeaveTypeId { get; set; } // LeaveType
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public bool IsHalfDay { get; set; } = default!;
    public string Comments { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}

public class LeaveRequestDetailDto : BaseDto
{
    public Guid LeaveTypeId { get; set; } // LeaveType
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public DateTime DateRequested { get; set; } = default!;
    public DateTime? DateApproved { get; set; } = default!;
    public string Comments { get; set; } = default!;
    public string DaysRequestedStr { get; set; } = default!;
    public string IsHalfDayStr { get; set; } = default!;
    public string StatusStr { get; set; } = default!;
    public string StartDateStr => $"{StartDate:MMMM dd, yyyy}";
    public string StartDateStrAm => StartDate.ToEthiopianDateString("MMMM dd, yyyy");
    public string EndDateStr => $"{EndDate:MMMM dd, yyyy}";
    public string EndDateStrAm => EndDate.ToEthiopianDateString("MMMM dd, yyyy");
    public string DateApprovedStr => DateApproved.HasValue ? $"{DateApproved:MMMM dd, yyyy}" : "";
    public string DateApprovedStrAm => DateApproved.HasValue ? DateApproved.Value.ToEthiopianDateString("MMMM dd, yyyy") : "";
    public string DateRequestedStr => $"{DateRequested:MMMM dd, yyyy}";
    public string DateRequestedStrAm => DateRequested.ToEthiopianDateString("MMMM dd, yyyy");
    public string ApprovedBy { get; set; } = default!;
    public string Employee { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
}

public class LeaveReqTestDto
{
    public Guid EmpId { get; set; } // LeaveType
    public Guid LeaveTypeId { get; set; } // LeaveType
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public bool IsHalfDay { get; set; } = false;
    public bool IsValid { get; set; } = false;
    public double Days { get; set; } = default!;
    public string Comments { get; set; } = default!;
}
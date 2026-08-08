// Leave.Domain/DTOs/LeaveReqOnLeaveDto.cs

using System.Text.Json.Serialization;

namespace Leave.Domain.DTOs;

public class LeaveReqOnLeaveDto
{
    public Guid RequestId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeNameAm { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentNameAm { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string PositionNameAm { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string LeaveTypeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DaysRequested { get; set; }
    public bool IsHalfDay { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }

    [JsonIgnore]
    public Guid LeaveTypeId { get; set; }
}
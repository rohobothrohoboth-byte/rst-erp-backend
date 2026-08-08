using System.Text.Json.Serialization;

namespace Leave.Domain.DTOs;

public class LeaveReqDbList
{
    [JsonIgnore]
    public double DaysRequested { get; set; } = default!;
    [JsonIgnore]
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee

    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
    public string ReqDay => $"{DaysRequested:#,##0.##} days";
}
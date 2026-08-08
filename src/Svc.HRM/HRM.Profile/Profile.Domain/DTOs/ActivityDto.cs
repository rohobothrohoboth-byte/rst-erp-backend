// Profile.Domain/DTOs/ActivityDto.cs

namespace Profile.Domain.DTOs;

public class ActivityDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeNameAm { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; }
    public string ActivityType { get; set; } = string.Empty;
}
public class EventDto
{
    public Guid EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
namespace Shared.Helpers.Events;

public class EntityEvent<T>
{
    public string EventType { get; set; } = string.Empty; // "CREATED", "UPDATED", "DELETED"
    public string EntityName { get; set; } = string.Empty; // "Branch", "Department", "Position", "Employee", "JobGrade"
    public T Data { get; set; } = default!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}



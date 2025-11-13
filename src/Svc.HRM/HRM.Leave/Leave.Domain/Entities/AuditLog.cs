namespace Leave.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid EntityId { get; set; } = default!;
    public string EntityName { get; set; } = default!;
    public string Operation { get; set; } = default!; // Create/Update/Delete
    public string ChangedBy { get; set; } = default!; // user id
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string Payload { get; set; } = default!; // JSON diff
}
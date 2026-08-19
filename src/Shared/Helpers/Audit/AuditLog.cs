using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Helpers.Audit;

public enum AuditStatus { Success, Error, Unauthorized, Warning }

/// <summary>
/// Shared audit record, reusable by every module. A module maps it in its DbContext
/// (DbSet&lt;AuditLog&gt; -> "AuditLogs") and enables the pipeline via AddSharedAudit /
/// UseSharedAudit. Self-contained (own Id/timestamps) so it doesn't depend on any
/// module's BaseEntity.
/// </summary>
public class AuditLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? UserId { get; set; }
    [MaxLength(255)] public string? UserEmail { get; set; }
    [MaxLength(200)] public string? UserName { get; set; }
    [MaxLength(50)] public string? UserRole { get; set; }

    [Required, MaxLength(200)] public string Action { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string EntityType { get; set; } = string.Empty;
    [MaxLength(100)] public string? EntityId { get; set; }

    [Column(TypeName = "jsonb")] public string? OldValues { get; set; }
    [Column(TypeName = "jsonb")] public string? NewValues { get; set; }
    [Column(TypeName = "jsonb")] public string? MetadataJson { get; set; }
    [Column(TypeName = "jsonb")] public string? RequestHeaders { get; set; }

    [MaxLength(45)] public string? IpAddress { get; set; }
    [MaxLength(500)] public string? UserAgent { get; set; }
    [MaxLength(500)] public string? QueryString { get; set; }
    public string? RequestId { get; set; }

    public AuditStatus Status { get; set; } = AuditStatus.Success;
    [MaxLength(1000)] public string? ErrorMessage { get; set; }
    public long? DurationMs { get; set; }

    public DateTime ActionDate { get; set; } = DateTime.UtcNow;
}

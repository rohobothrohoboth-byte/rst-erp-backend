// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Models\Entities\ApiKeyLog.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.HRMM.Models.Entities;

/// <summary>
/// Logs all API Key usage for audit purposes
/// </summary>
[Table("ApiKeyLogs")]
public class ApiKeyLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public Guid ApiKeyId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Endpoint { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string Method { get; set; } = string.Empty;

    public bool Success { get; set; }

    public int? StatusCode { get; set; }

    [MaxLength(50)]
    public string? ClientIp { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey(nameof(ApiKeyId))]
    public virtual ApiKey ApiKey { get; set; } = null!;
}
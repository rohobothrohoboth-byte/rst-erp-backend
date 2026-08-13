using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Helpers.ExternalAccess;

/// <summary>
/// Registered external system that may call this API read-only via an API key.
/// Shared across all modules (single definition) instead of duplicating the entity
/// in every service. Each module's DbContext maps it to its "ExternalSystems" table.
/// </summary>
public class ExternalSystem
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ApiKey { get; set; } = string.Empty;

    [MaxLength(500)]
    public string BaseUrl { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public string? AllowedEndpointsJson { get; set; }

    [NotMapped]
    public string[] AllowedEndpoints
    {
        get => string.IsNullOrEmpty(AllowedEndpointsJson)
            ? Array.Empty<string>()
            : System.Text.Json.JsonSerializer.Deserialize<string[]>(AllowedEndpointsJson) ?? Array.Empty<string>();
        set => AllowedEndpointsJson = System.Text.Json.JsonSerializer.Serialize(value ?? Array.Empty<string>());
    }

    public int RateLimitPerMinute { get; set; } = 60;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public long RequestCount { get; set; } = 0;
    public DateTime? RevokedAt { get; set; }
}

public class ApiKeyInfo
{
    public string ClientName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public int RequestCount { get; set; }
}

public class ApiKeySettings
{
    public bool EnableAuditLogging { get; set; } = true;
    public int CacheDurationMinutes { get; set; } = 5;
    public bool EnableRateLimiting { get; set; } = true;
    public int RateLimitPerMinute { get; set; } = 60;
}

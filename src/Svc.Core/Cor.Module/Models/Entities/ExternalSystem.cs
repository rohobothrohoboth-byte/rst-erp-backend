// E:\untitled46\RST_ERP\src\Svc.Core\Cor.Module\Models\Entities\ExternalSystem.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Module.Models.Entities;

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

    // ✅ አዲስ: ፈቃዶችን ለማከማቸት
    [Column(TypeName = "jsonb")]
    public string? PermissionsJson { get; set; }

    [NotMapped]
    public string[] Permissions
    {
        get => string.IsNullOrEmpty(PermissionsJson)
            ? Array.Empty<string>()
            : System.Text.Json.JsonSerializer.Deserialize<string[]>(PermissionsJson) ?? Array.Empty<string>();
        set => PermissionsJson = System.Text.Json.JsonSerializer.Serialize(value ?? Array.Empty<string>());
    }

    public int RateLimitPerMinute { get; set; } = 60;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public long RequestCount { get; set; } = 0;

    public DateTime? RevokedAt { get; set; }
}
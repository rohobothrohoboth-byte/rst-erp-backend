// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Models\Entities\ApiKey.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Cor.HRMM.Models.Entities;

[Table("ApiKeys")]
public class ApiKey
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string ClientName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ApiKeyValue { get; set; } = string.Empty; // ✅ Changed from ApiKey to ApiKeyValue

    [Required]
    [MaxLength(50)]
    public string ClientType { get; set; } = "External";

    [MaxLength(500)]
    public string? BaseUrl { get; set; }

    [Column(TypeName = "jsonb")]
    public string AllowedEndpointsJson { get; set; } = "[]";

    [NotMapped]
    public string[] AllowedEndpoints
    {
        get => string.IsNullOrEmpty(AllowedEndpointsJson)
            ? Array.Empty<string>()
            : JsonSerializer.Deserialize<string[]>(AllowedEndpointsJson) ?? Array.Empty<string>();
        set => AllowedEndpointsJson = JsonSerializer.Serialize(value ?? Array.Empty<string>());
    }

    public int RateLimitPerMinute { get; set; } = 60;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public long RequestCount { get; set; } = 0;

    public DateTime? RevokedAt { get; set; }

    // Navigation property
    public ICollection<ApiKeyLog> ApiKeyLogs { get; set; } = new List<ApiKeyLog>();
}
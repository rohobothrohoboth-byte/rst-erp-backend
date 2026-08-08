// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Models\DTOs\ApiKeyDtos.cs
namespace Cor.Inventory.Models.DTOs;

public class ApiKeyDto
{
    public Guid Id { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ClientType { get; set; } = string.Empty;
    public string? BaseUrl { get; set; }
    public string[] AllowedEndpoints { get; set; } = Array.Empty<string>();
    public int RateLimitPerMinute { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public long RequestCount { get; set; }
}

public class CreateApiKeyDto
{
    public string ClientName { get; set; } = string.Empty;
    public string ClientType { get; set; } = "External";
    public string? BaseUrl { get; set; }
    public string[]? AllowedEndpoints { get; set; }
    public int? RateLimitPerMinute { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class UpdateApiKeyDto
{
    public string? ClientName { get; set; }
    public string? BaseUrl { get; set; }
    public string[]? AllowedEndpoints { get; set; }
    public int? RateLimitPerMinute { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class ApiKeyLogDto
{
    public Guid Id { get; set; }
    public Guid ApiKeyId { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public bool Success { get; set; }
    public int? StatusCode { get; set; }
    public string? ClientIp { get; set; }
    public DateTime Timestamp { get; set; }
}
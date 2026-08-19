// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Services\ApiKeyInfo.cs
namespace Cor.Module.Services;

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
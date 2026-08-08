// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Services\ApiKeySettings.cs
namespace Cor.Finance.Services;

public class ApiKeySettings
{
    public bool EnableAuditLogging { get; set; } = true;
    public int CacheDurationMinutes { get; set; } = 5;
    public bool EnableRateLimiting { get; set; } = true;
    public int RateLimitPerMinute { get; set; } = 60;
}
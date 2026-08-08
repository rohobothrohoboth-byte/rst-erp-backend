// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Services\IApiKeyService.cs
namespace Cor.Finance.Services;

public interface IApiKeyService
{
    Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken ct = default);
    Task<string?> GetClientNameAsync(string apiKey, CancellationToken ct = default);
    Task LogApiKeyUsageAsync(string apiKey, string endpoint, string method, bool success, CancellationToken ct = default);
    Task<IEnumerable<ApiKeyInfo>> GetAllApiKeysAsync(CancellationToken ct = default);
    Task RevokeApiKeyAsync(string apiKey, CancellationToken ct = default);
}


// E:\untitled46\RST_ERP\src\Svc.Core\Cor.Module\Services\IExternalSystemService.cs
using Cor.Module.Models.Entities;

namespace Cor.Module.Services;

public interface IExternalSystemService
{
    Task<ExternalSystem> RegisterSystemAsync(string systemName, string[]? allowedEndpoints = null, string[]? permissions = null);
    Task RevokeAccessAsync(string apiKey);
    Task<bool> IsSystemAllowedAsync(string apiKey, string endpoint);
    Task<IEnumerable<ExternalSystem>> GetAllSystemsAsync();
    Task<ExternalSystem?> GetSystemByApiKeyAsync(string apiKey);

    // ✅ አዲስ ሜቶዶች
    Task<bool> HasPermissionAsync(string apiKey, string permission);
    Task<string[]> GetPermissionsAsync(string apiKey);
    Task UpdatePermissionsAsync(string apiKey, string[] permissions);
}
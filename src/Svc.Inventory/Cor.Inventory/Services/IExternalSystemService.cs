// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Services\IExternalSystemService.cs
using Cor.Inventory.Models.Entities;

namespace Cor.Inventory.Services;

public interface IExternalSystemService
{
    Task<ExternalSystem> RegisterSystemAsync(string systemName, string[]? allowedEndpoints = null);
    Task RevokeAccessAsync(string apiKey);
    Task<bool> IsSystemAllowedAsync(string apiKey, string endpoint);
    Task<IEnumerable<ExternalSystem>> GetAllSystemsAsync();
    Task<ExternalSystem?> GetSystemByApiKeyAsync(string apiKey);
}
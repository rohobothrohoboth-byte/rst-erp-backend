using System.Net.Http.Json;
using Cor.HRMM.Models.Entities.Local;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Helpers;
namespace Cor.HRMM.Services;

public class CoreModuleApiService : ICoreModuleApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CoreModuleApiService> _logger;

    public CoreModuleApiService(HttpClient httpClient, ILogger<CoreModuleApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<LocalCompany>> GetAllCompaniesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/core/module/v1/Company/AllCompany", ct);
            if (!response.IsSuccessStatusCode) return new List<LocalCompany>();

            var result = await response.Content.ReadFromJsonAsync<CoreApiResponse<List<LocalCompany>>>(cancellationToken: ct);
            return result?.Data ?? new List<LocalCompany>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all companies from Core Module");
            return new List<LocalCompany>();
        }
    }

    public async Task<List<LocalBranch>> GetAllBranchesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/core/module/v1/Branch/AllBranch", ct);
            if (!response.IsSuccessStatusCode) return new List<LocalBranch>();

            var result = await response.Content.ReadFromJsonAsync<CoreApiResponse<List<LocalBranch>>>(cancellationToken: ct);
            return result?.Data ?? new List<LocalBranch>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all branches from Core Module");
            return new List<LocalBranch>();
        }
    }

    public async Task<List<LocalDepartment>> GetAllDepartmentsAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/core/module/v1/Department/AllDept", ct);
            if (!response.IsSuccessStatusCode) return new List<LocalDepartment>();

            var result = await response.Content.ReadFromJsonAsync<CoreApiResponse<List<LocalDepartment>>>(cancellationToken: ct);
            return result?.Data ?? new List<LocalDepartment>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all departments from Core Module");
            return new List<LocalDepartment>();
        }
    }
}

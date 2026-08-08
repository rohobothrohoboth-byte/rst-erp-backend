using System.Net.Http.Json;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Services;
namespace Svc.Auth.Services;

public class CoreModuleApiService : ICoreModuleApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CoreModuleApiService> _logger;

    public CoreModuleApiService(HttpClient httpClient, ILogger<CoreModuleApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<CompanyDto>> GetAllCompaniesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/core/module/v1/Company/AllCompany", ct);
            if (!response.IsSuccessStatusCode) return new List<CompanyDto>();

            var result = await response.Content.ReadFromJsonAsync<CoreApiResponse<List<CompanyDto>>>(cancellationToken: ct);
            return result?.Data ?? new List<CompanyDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all companies from Core Module");
            return new List<CompanyDto>();
        }
    }

    public async Task<List<BranchDto>> GetAllBranchesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/core/module/v1/Branch/AllBranch", ct);
            if (!response.IsSuccessStatusCode) return new List<BranchDto>();

            var result = await response.Content.ReadFromJsonAsync<CoreApiResponse<List<BranchDto>>>(cancellationToken: ct);
            return result?.Data ?? new List<BranchDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all branches from Core Module");
            return new List<BranchDto>();
        }
    }

    public async Task<List<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken ct = default)
    {
        try
        {
             var response = await _httpClient.GetAsync("/api/core/module/v1/Department/AllDept", ct);
            if (!response.IsSuccessStatusCode) return new List<DepartmentDto>();

            var result = await response.Content.ReadFromJsonAsync<CoreApiResponse<List<DepartmentDto>>>(cancellationToken: ct);
            return result?.Data ?? new List<DepartmentDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all departments from Core Module");
            return new List<DepartmentDto>();
        }
    }

    public async Task<BranchDto?> GetBranchAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
           var response = await _httpClient.GetAsync($"/api/core/module/v1/Branch/GetBranch/{id}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<CoreApiResponse<BranchDto>>(cancellationToken: ct);
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting branch {BranchId} from Core Module", id);
            return null;
        }
    }

    public async Task<DepartmentDto?> GetDepartmentAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/core/module/v1/Department/GetDept/{id}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<CoreApiResponse<DepartmentDto>>(cancellationToken: ct);
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department {DepartmentId} from Core Module", id);
            return null;
        }
    }

     public async Task<CompanyDto?> GetCompanyAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                // ✅ FIXED: Correct endpoint
                var response = await _httpClient.GetAsync($"/api/core/module/v1/Company/GetCompany/{id}", ct);
                if (!response.IsSuccessStatusCode) return null;

                var result = await response.Content.ReadFromJsonAsync<CoreApiResponse<CompanyDto>>(cancellationToken: ct);
                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company {CompanyId} from Core Module", id);
                return null;
            }
        }
}
using System.Net.Http.Json;
using Cor.CRM.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace Cor.CRM.Services;

public class CoreModuleApiService : ICoreModuleApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CoreModuleApiService> _logger;

    public CoreModuleApiService(HttpClient httpClient, ILogger<CoreModuleApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

   public async Task<List<LocalCompanyDto>> GetAllCompaniesAsync(CancellationToken ct = default)
       {
           try
           {
               var response = await _httpClient.GetAsync("/api/core/module/v1/Company/AllCompany", ct);
               if (!response.IsSuccessStatusCode) return new List<LocalCompanyDto>();

               var result = await response.Content.ReadFromJsonAsync<CoreApiResponse<List<LocalCompanyDto>>>(cancellationToken: ct);
               return result?.Data ?? new List<LocalCompanyDto>();
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Error getting companies from Core Module");
               return new List<LocalCompanyDto>();
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
              _logger.LogError(ex, "Error getting branches from Core Module");
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
               _logger.LogError(ex, "Error getting departments from Core Module");
               return new List<DepartmentDto>();
           }
       }

}

public class CoreApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
}

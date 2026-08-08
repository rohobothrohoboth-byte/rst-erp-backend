using System.Net.Http.Json;
using Cor.CRM.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace Cor.CRM.Services;

public class HrmProApiService : IHrmProApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HrmProApiService> _logger;

    public HrmProApiService(HttpClient httpClient, ILogger<HrmProApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<EmployeeDto>> GetAllEmployeesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/hrm/profile/v1/Employee/AllEmployee", ct);
            if (!response.IsSuccessStatusCode) return new List<EmployeeDto>();

            var result = await response.Content.ReadFromJsonAsync<HrmProApiResponse<List<EmployeeDto>>>(cancellationToken: ct);
            return result?.Data ?? new List<EmployeeDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all employees from HRM Pro");
            return new List<EmployeeDto>();
        }
    }

    public async Task<EmployeeDto?> GetEmployeeAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/hrm/profile/v1/Employee/GetEmployee/{id}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<HrmProApiResponse<EmployeeDto>>(cancellationToken: ct);
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employee {EmployeeId} from HRM Pro", id);
            return null;
        }
    }

    public async Task<bool> UpdateEmployeeAppUserIdAsync(Guid employeeId, string appUserId, CancellationToken ct = default)
    {
        try
        {
            var payload = new { AppUserId = appUserId };
            var response = await _httpClient.PutAsJsonAsync($"/api/hrm/profile/v1/Employee/ModEmployee/{employeeId}", payload, ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee AppUserId for {EmployeeId}", employeeId);
            return false;
        }
    }
}
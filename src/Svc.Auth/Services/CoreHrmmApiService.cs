using System.Net.Http.Json;
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Services;

public class CoreHrmmApiService : ICoreHrmmApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CoreHrmmApiService> _logger;

    public CoreHrmmApiService(HttpClient httpClient, ILogger<CoreHrmmApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<PositionDto>> GetAllPositionsAsync(CancellationToken ct = default)
    {
        try
        {
           var response = await _httpClient.GetAsync("/api/core/hrmm/v1/Position/AllPosition", ct);
            if (!response.IsSuccessStatusCode) return new List<PositionDto>();

            var result = await response.Content.ReadFromJsonAsync<HrmmApiResponse<List<PositionDto>>>(cancellationToken: ct);
            return result?.Data ?? new List<PositionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all positions from Core HRMM");
            return new List<PositionDto>();
        }
    }

    public async Task<List<JobGradeDto>> GetAllJobGradesAsync(CancellationToken ct = default)
    {
        try
        {
              var response = await _httpClient.GetAsync("/api/core/hrmm/v1/JobGrade/AllJobGrade", ct);
            if (!response.IsSuccessStatusCode) return new List<JobGradeDto>();

            var result = await response.Content.ReadFromJsonAsync<HrmmApiResponse<List<JobGradeDto>>>(cancellationToken: ct);
            return result?.Data ?? new List<JobGradeDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all job grades from Core HRMM");
            return new List<JobGradeDto>();
        }
    }

    public async Task<PositionDto?> GetPositionAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
           var response = await _httpClient.GetAsync($"/api/core/hrmm/v1/Position/GetPosition/{id}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<HrmmApiResponse<PositionDto>>(cancellationToken: ct);
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting position {PositionId} from Core HRMM", id);
            return null;
        }
    }

    public async Task<JobGradeDto?> GetJobGradeAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/core/hrmm/v1/JobGrade/GetJobGrade/{id}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<HrmmApiResponse<JobGradeDto>>(cancellationToken: ct);
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job grade {JobGradeId} from Core HRMM", id);
            return null;
        }
    }
}
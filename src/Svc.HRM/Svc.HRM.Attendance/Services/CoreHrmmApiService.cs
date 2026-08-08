using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities.Local;
namespace Svc.HRM.Attendance.Services;

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

                // Try to deserialize as PositionDto directly
                var result = await response.Content.ReadFromJsonAsync<HrmmApiResponse<List<PositionDto>>>(cancellationToken: ct);

                if (result?.Data == null)
                {
                    _logger.LogWarning("No position data received from HRMM API");
                    return new List<PositionDto>();
                }

                // Filter out positions with null or empty names
                var validPositions = result.Data
                    .Where(p => !string.IsNullOrEmpty(p.Name) && !string.IsNullOrEmpty(p.NameAm))
                    .ToList();

                _logger.LogInformation("Retrieved {Count} valid positions from HRMM API (filtered out {Filtered} invalid ones)",
                    validPositions.Count, result.Data.Count - validPositions.Count);

                return validPositions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting positions from Core HRMM");
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

                if (result?.Data == null)
                {
                    _logger.LogWarning("No job grade data received from HRMM API");
                    return new List<JobGradeDto>();
                }

                // Filter out job grades with null or empty names
                var validJobGrades = result.Data
                    .Where(j => !string.IsNullOrEmpty(j.Name))
                    .ToList();

                _logger.LogInformation("Retrieved {Count} valid job grades from HRMM API (filtered out {Filtered} invalid ones)",
                    validJobGrades.Count, result.Data.Count - validJobGrades.Count);

                return validJobGrades;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting job grades from Core HRMM");
                return new List<JobGradeDto>();
            }
        }


    public async Task<LocalPosition?> GetPositionAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/core/hrmm/v1/Position/GetPosition/{id}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var dataElement))
            {
                var dataJson = dataElement.GetRawText();
                return JsonSerializer.Deserialize<LocalPosition>(dataJson);
            }

            return JsonSerializer.Deserialize<LocalPosition>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting position {PositionId} from Core HRMM", id);
            return null;
        }
    }

    public async Task<LocalJobGrade?> GetJobGradeAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/core/hrmm/v1/JobGrade/GetJobGrade/{id}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var dataElement))
            {
                var dataJson = dataElement.GetRawText();
                return JsonSerializer.Deserialize<LocalJobGrade>(dataJson);
            }

            return JsonSerializer.Deserialize<LocalJobGrade>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job grade {JobGradeId} from Core HRMM", id);
            return null;
        }
    }
}
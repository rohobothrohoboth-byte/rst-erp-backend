using System.Net.Http.Json;
using Leave.Domain.Entities.Local;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Helpers;
using System.Text.Json;
using Leave.Domain.DTOs;
namespace Leave.App.Services;

public class CoreHrmmApiService : ICoreHrmmApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CoreHrmmApiService> _logger;

    public CoreHrmmApiService(HttpClient httpClient, ILogger<CoreHrmmApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<LocalPosition>> GetAllPositionsAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/core/hrmm/v1/Position/AllPosition", ct);
            if (!response.IsSuccessStatusCode) return new List<LocalPosition>();

            var result = await response.Content.ReadFromJsonAsync<HrmmApiResponse<List<LocalPosition>>>(cancellationToken: ct);
            return result?.Data ?? new List<LocalPosition>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all positions from Core HRMM");
            return new List<LocalPosition>();
        }
    }

    public async Task<List<LocalJobGrade>> GetAllJobGradesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/core/hrmm/v1/JobGrade/AllJobGrade", ct);
            if (!response.IsSuccessStatusCode) return new List<LocalJobGrade>();

            var result = await response.Content.ReadFromJsonAsync<HrmmApiResponse<List<LocalJobGrade>>>(cancellationToken: ct);
            return result?.Data ?? new List<LocalJobGrade>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all job grades from Core HRMM");
            return new List<LocalJobGrade>();
        }
    }


  public async Task<List<PositionReqDto>> GetAllPositionRequirementsAsync(CancellationToken ct = default)
  {
      try
      {
          // ✅ Log the full URL being called
          var endpoint  = "api/core/hrmm/v1/PositionReq";
          var fullUrl = $"{_httpClient.BaseAddress}{endpoint}";
          _logger.LogInformation("📡 Calling URL: {FullUrl}", fullUrl);

          _logger.LogInformation("📡 Calling URL: {FullUrl}", fullUrl);

                 var response = await _httpClient.GetAsync(endpoint, ct);
                 _logger.LogInformation("📡 Response Status Code: {StatusCode}", response.StatusCode);

                 if (!response.IsSuccessStatusCode)
                 {
                     _logger.LogWarning("⚠️ Failed to get position requirements. Status: {StatusCode}", response.StatusCode);
                     return new List<PositionReqDto>();
                 }

                 // ✅ Use the same HrmmApiResponse pattern as GetAllPositionsAsync
                 var result = await response.Content.ReadFromJsonAsync<HrmmApiResponse<List<PositionReqDto>>>(cancellationToken: ct);

                 if (result == null)
                 {
                     _logger.LogError("❌ Failed to deserialize position requirements response");
                     return new List<PositionReqDto>();
                 }

                 if (!result.Success)
                 {
                     _logger.LogError("❌ API returned error: {Message}", result.Message);
                     return new List<PositionReqDto>();
                 }

                 _logger.LogInformation("✅ Successfully deserialized {Count} position requirements", result.Data?.Count ?? 0);
                 return result.Data ?? new List<PositionReqDto>();
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "❌ Failed to get position requirements");
                 return new List<PositionReqDto>();
             }
         }
   public async Task<List<LocalJgStep>> GetAllJgStepsAsync(CancellationToken ct = default)
   {
       try
       {
           _logger.LogInformation("📡 Fetching JgSteps from Core HRMM API...");

         //  var response = await _httpClient.GetAsync("/api/core/hrmm/v1/JgStep/AllJgStep", ct);
           var response = await _httpClient.GetAsync("/api/core/hrmm/v1/JgStep/AllJgSteps", ct);
           _logger.LogInformation("📡 Response status: {StatusCode}", response.StatusCode);

           if (!response.IsSuccessStatusCode)
           {
               _logger.LogWarning("⚠️ Failed to fetch JgSteps. Status: {StatusCode}", response.StatusCode);
               return new List<LocalJgStep>();
           }

           var json = await response.Content.ReadAsStringAsync(ct);
           _logger.LogInformation("📡 Raw JSON response: {Json}", json); // Log raw JSON

           using var doc = JsonDocument.Parse(json);
           var root = doc.RootElement;

           // Log the structure
           _logger.LogInformation("📡 JSON root has properties: {Properties}", string.Join(", ", root.EnumerateObject().Select(p => p.Name)));

           if (root.TryGetProperty("data", out var dataElement))
           {
               var dataJson = dataElement.GetRawText();
               _logger.LogInformation("📡 Data JSON: {DataJson}", dataJson);

               var result = JsonSerializer.Deserialize<List<LocalJgStep>>(dataJson);
               _logger.LogInformation("✅ Deserialized {Count} JgSteps", result?.Count ?? 0);
               return result ?? new List<LocalJgStep>();
           }

           // Try direct deserialization if no "data" property
           var directResult = JsonSerializer.Deserialize<List<LocalJgStep>>(json);
           _logger.LogInformation("✅ Direct deserialized {Count} JgSteps", directResult?.Count ?? 0);
           return directResult ?? new List<LocalJgStep>();
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "❌ Error getting all job grade steps from Core HRMM");
           return new List<LocalJgStep>();
       }
   }
}
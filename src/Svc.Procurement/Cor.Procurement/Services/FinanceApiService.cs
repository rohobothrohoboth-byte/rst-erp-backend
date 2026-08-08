using System.Net.Http.Json;
using Cor.Procurement.Models.DTOs;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Cor.Procurement.Services;

public class FinanceApiService : IFinanceApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FinanceApiService> _logger;

    public FinanceApiService(HttpClient httpClient, ILogger<FinanceApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    // ============================================================
    // FINANCIAL PERIODS - Uses Wrapper Response
    // ============================================================

    public async Task<List<FinancialPeriodDto>> GetAllFinancialPeriodsAsync(CancellationToken ct = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(15));

            var response = await _httpClient.GetAsync("/api/finance/v1/PeriodClosing/All", cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cts.Token);
                _logger.LogWarning("Failed to get financial periods: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return new List<FinancialPeriodDto>();
            }

            // ✅ Financial Periods returns wrapper: { success: true, data: [] }
            var result = await response.Content.ReadFromJsonAsync<FinanceApiResponse<List<FinancialPeriodDto>>>(cancellationToken: cts.Token);
            return result?.Data ?? new List<FinancialPeriodDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting financial periods from Finance module");
            return new List<FinancialPeriodDto>();
        }
    }

    public async Task<FinancialPeriodDto?> GetFinancialPeriodAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var response = await _httpClient.GetAsync($"/api/finance/v1/PeriodClosing/Get/{id}", cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get financial period {Id}: {StatusCode}", id, response.StatusCode);
                return null;
            }

            // ✅ Financial Period returns wrapper: { success: true, data: { ... } }
            var result = await response.Content.ReadFromJsonAsync<FinanceApiResponse<FinancialPeriodDto>>(cancellationToken: cts.Token);
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting financial period {Id}", id);
            return null;
        }
    }

    // ============================================================
    // VENDORS - Returns Direct Array
    // ============================================================

   // In FinanceApiService.cs

   public async Task<List<VendorDto>> GetAllVendorsAsync(CancellationToken ct = default)
   {
       try
       {
           using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
           cts.CancelAfter(TimeSpan.FromSeconds(15));

           var response = await _httpClient.GetAsync("/api/finance/v1/Vendor/All", cts.Token);

           if (!response.IsSuccessStatusCode)
           {
               var errorContent = await response.Content.ReadAsStringAsync(cts.Token);
               _logger.LogWarning("Failed to get vendors: {StatusCode}, Error: {Error}",
                   response.StatusCode, errorContent);
               return new List<VendorDto>();
           }

           // ✅ Deserialize directly to List<VendorDto>
           var options = new JsonSerializerOptions
           {
               PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
               PropertyNameCaseInsensitive = true
           };

           var vendors = await response.Content.ReadFromJsonAsync<List<VendorDto>>(options, cts.Token);
           return vendors ?? new List<VendorDto>();
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "❌ Error getting vendors from Finance module");
           return new List<VendorDto>();
       }
   }

    public async Task<VendorDto?> GetVendorAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var response = await _httpClient.GetAsync($"/api/finance/v1/Vendor/Get/{id}", cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get vendor {Id}: {StatusCode}", id, response.StatusCode);
                return null;
            }

            // ✅ Vendor returns direct object: { ... }
            var vendor = await response.Content.ReadFromJsonAsync<VendorDto>(cancellationToken: cts.Token);
            return vendor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vendor {Id}", id);
            return null;
        }
    }

    public async Task<VendorDto?> GetVendorByCodeAsync(string code, CancellationToken ct = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var response = await _httpClient.GetAsync($"/api/finance/v1/Vendor/ByCode/{code}", cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get vendor by code {Code}: {StatusCode}", code, response.StatusCode);
                return null;
            }

            // ✅ Vendor returns direct object: { ... }
            var vendor = await response.Content.ReadFromJsonAsync<VendorDto>(cancellationToken: cts.Token);
            return vendor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vendor by code {Code}", code);
            return null;
        }
    }
}

// ============================================================
// DTO for API Response Wrapper
// ============================================================

public class FinanceApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }
}
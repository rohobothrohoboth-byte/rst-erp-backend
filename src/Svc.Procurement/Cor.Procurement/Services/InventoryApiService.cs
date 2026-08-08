using System.Net.Http.Json;
using Cor.Procurement.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace Cor.Procurement.Services;

public class InventoryApiService : IInventoryApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InventoryApiService> _logger;

    public InventoryApiService(HttpClient httpClient, ILogger<InventoryApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<WarehouseDto>> GetAllWarehousesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/inventory/v1/Warehouse", ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get warehouses: {StatusCode}", response.StatusCode);
                return new List<WarehouseDto>();
            }

            var result = await response.Content.ReadFromJsonAsync<InventoryApiResponse<List<WarehouseDto>>>(cancellationToken: ct);
            return result?.Data ?? new List<WarehouseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting warehouses from Inventory module");
            return new List<WarehouseDto>();
        }
    }

    public async Task<WarehouseDto?> GetWarehouseAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/inventory/v1/Warehouse/{id}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<InventoryApiResponse<WarehouseDto>>(cancellationToken: ct);
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting warehouse {Id}", id);
            return null;
        }
    }
}

public class InventoryApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public int? TotalCount { get; set; }
}
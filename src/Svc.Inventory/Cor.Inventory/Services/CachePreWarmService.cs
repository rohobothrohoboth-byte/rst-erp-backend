// Services/CachePreWarmService.cs

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.Inventory.Services;

public class CachePreWarmService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CachePreWarmService> _logger;

    public CachePreWarmService(IServiceProvider serviceProvider, ILogger<CachePreWarmService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var cachedService = scope.ServiceProvider.GetRequiredService<CachedReferenceDataService>();

            _logger.LogInformation("⏳ Pre-warming inventory cache...");

            // Pre-warm warehouses
            var warehouses = await cachedService.GetCachedWarehousesAsync(cancellationToken);
            _logger.LogInformation("✅ Cached {Count} warehouses", warehouses.Count);

            // Pre-warm stock levels
            var stockLevels = await cachedService.GetCachedStockLevelsAsync(cancellationToken);
            _logger.LogInformation("✅ Cached {Count} stock levels", stockLevels.Count);

            _logger.LogInformation("✅ Inventory cache pre-warmed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Cache pre-warm failed");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
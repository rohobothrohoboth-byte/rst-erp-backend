// Services/CachePreWarmService.cs

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.Finance.Services;

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

            _logger.LogInformation("⏳ Pre-warming cache...");

            await cachedService.GetCachedAccountsAsync();

            _logger.LogInformation("✅ Cache pre-warmed successfully");
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
// Services/CachePreWarmService.cs

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.Procurement.Services;

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

            // Warm up Local Copy Cache
            var localCopyService = scope.ServiceProvider.GetRequiredService<CachedLocalCopyService>();
            _logger.LogInformation("⏳ Pre-warming local copy cache...");

            await localCopyService.GetCompaniesAsync(cancellationToken);
            await localCopyService.GetBranchesAsync(cancellationToken);
            await localCopyService.GetDepartmentsAsync(cancellationToken);
            await localCopyService.GetPositionsAsync(cancellationToken);
            await localCopyService.GetJobGradesAsync(cancellationToken);
            await localCopyService.GetEmployeesAsync(cancellationToken);
            await localCopyService.GetPersonsAsync(cancellationToken);

            _logger.LogInformation("✅ Local copy cache pre-warmed successfully");

            // Warm up Reference Data Cache (Procurement only)
            var refDataService = scope.ServiceProvider.GetRequiredService<CachedReferenceDataService>();
            _logger.LogInformation("⏳ Pre-warming reference data cache...");

            await refDataService.GetCachedVendorsAsync(cancellationToken);
            await refDataService.GetCachedFinancialPeriodsAsync(cancellationToken);
            await refDataService.GetCachedPurchaseOrderStatusesAsync(cancellationToken);
            await refDataService.GetCachedRequisitionStatusesAsync(cancellationToken);

            _logger.LogInformation("✅ Reference data cache pre-warmed successfully");
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
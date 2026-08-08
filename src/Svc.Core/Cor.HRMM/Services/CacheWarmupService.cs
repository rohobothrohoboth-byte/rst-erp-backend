// Services/CacheWarmupService.cs
using Microsoft.Extensions.Caching.Memory;
using Shared.Helpers.Services;
using Helpers;
using MediatR;
using Cor.HRMM.Queries;

namespace Cor.HRMM.Services;

public class CacheWarmupService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CacheWarmupService> _logger;

    public CacheWarmupService(
        IServiceScopeFactory scopeFactory,
        ILogger<CacheWarmupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var lockService = scope.ServiceProvider.GetRequiredService<IDistributedLockService>();

        var warmupLockKey = "warmup:lock";

        if (await lockService.AcquireLockAsync(warmupLockKey, TimeSpan.FromMinutes(2), cancellationToken))
        {
            try
            {
                var warmupDone = await cache.GetAsync<string>("warmup:done", cancellationToken);
                if (warmupDone != null)
                {
                    _logger.LogInformation("✅ Cache warmup already done by another instance");
                    return;
                }

                _logger.LogInformation("🔥 Starting cache warmup...");

               // ✅ Correct - Sequential execution
               await WarmupPositionsAsync(mediator, cache, cancellationToken);
               await WarmupJobGradesAsync(mediator, cache, cancellationToken);
               await WarmupJgStepsAsync(mediator, cache, cancellationToken);

               // await Task.WhenAll(tasks);

                await cache.SetAsync("warmup:done", "done", TimeSpan.FromHours(1), cancellationToken);

                _logger.LogInformation("✅ Cache warmup completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Cache warmup failed!");
            }
            finally
            {
                await lockService.ReleaseLockAsync(warmupLockKey, cancellationToken);
            }
        }
        else
        {
            _logger.LogInformation("⏳ Another instance is warming up the cache, waiting...");

            for (int i = 0; i < 30; i++)
            {
                await Task.Delay(1000, cancellationToken);
                var warmupDone = await cache.GetAsync<string>("warmup:done", cancellationToken);
                if (warmupDone != null)
                {
                    _logger.LogInformation("✅ Cache warmup completed by another instance");
                    return;
                }
            }

            _logger.LogWarning("⚠️ Cache warmup timeout, proceeding anyway");
        }
    }

    private async Task WarmupPositionsAsync(IMediator mediator, ICacheService cache, CancellationToken ct)
    {
        var positions = await mediator.Send(new PositionAllQry(), ct);
        await cache.SetAsync("AllPositions", positions, TimeSpan.FromMinutes(5), ct);
        _logger.LogInformation($"✅ Positions cached: {positions?.Count ?? 0} items");
    }

    private async Task WarmupJobGradesAsync(IMediator mediator, ICacheService cache, CancellationToken ct)
    {
        var jobGrades = await mediator.Send(new JobGradeAllQry(), ct);
        await cache.SetAsync("AllJobGrades", jobGrades, TimeSpan.FromMinutes(5), ct);
        _logger.LogInformation($"✅ JobGrades cached: {jobGrades?.Count ?? 0} items");
    }

private async Task WarmupJgStepsAsync(IMediator mediator, ICacheService cache, CancellationToken ct)
{
    int retryCount = 3;
    while (retryCount > 0)
    {
        try
        {
            var jgSteps = await mediator.Send(new JgStepAllQry(), ct);
            await cache.SetAsync("AllJgSteps", jgSteps, TimeSpan.FromMinutes(5), ct);
            _logger.LogInformation($"✅ JgSteps cached: {jgSteps?.Count ?? 0} items");
            break;
        }
        catch (Exception ex) when (retryCount > 1)
        {
            _logger.LogWarning(ex, "⚠️ JgStep warmup failed, retrying... ({RetriesLeft} tries left)", retryCount - 1);
            retryCount--;
            await Task.Delay(1000, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ JgStep warmup failed permanently");
            throw;
        }
    }
}


    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
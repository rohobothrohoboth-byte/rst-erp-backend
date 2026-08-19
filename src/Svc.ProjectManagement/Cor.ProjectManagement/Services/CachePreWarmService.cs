// Services/CachePreWarmService.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cor.ProjectManagement.Services
{
    public class CachePreWarmService : BackgroundService
    {
        private readonly ILogger<CachePreWarmService> _logger;

        public CachePreWarmService(ILogger<CachePreWarmService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CachePreWarmService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Pre-warm cache
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in CachePreWarmService");
                }
            }

            _logger.LogInformation("CachePreWarmService stopped");
        }
    }
}
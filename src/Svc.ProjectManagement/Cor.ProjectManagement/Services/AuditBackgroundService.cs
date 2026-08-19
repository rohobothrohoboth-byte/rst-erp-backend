// Services/AuditBackgroundService.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cor.ProjectManagement.Services
{
    public class AuditBackgroundService : BackgroundService
    {
        private readonly ILogger<AuditBackgroundService> _logger;

        public AuditBackgroundService(ILogger<AuditBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuditBackgroundService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Process audit logs
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in AuditBackgroundService");
                }
            }

            _logger.LogInformation("AuditBackgroundService stopped");
        }
    }
}
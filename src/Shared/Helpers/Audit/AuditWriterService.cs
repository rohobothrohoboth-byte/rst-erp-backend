using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Helpers.Audit;

/// <summary>
/// Background writer: drains the audit channel and persists records in small batches
/// via a scoped DbContext. Shared across modules. Failures are swallowed so auditing
/// never affects request handling.
/// </summary>
public class AuditWriterService : BackgroundService
{
    private readonly Channel<AuditLog> _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuditWriterService> _logger;

    public AuditWriterService(Channel<AuditLog> channel, IServiceScopeFactory scopeFactory, ILogger<AuditWriterService> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var first in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            var batch = new List<AuditLog> { first };
            while (batch.Count < 100 && _channel.Reader.TryRead(out var next))
                batch.Add(next);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DbContext>();
                db.Set<AuditLog>().AddRange(batch);
                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist {Count} audit record(s)", batch.Count);
            }
        }
    }
}

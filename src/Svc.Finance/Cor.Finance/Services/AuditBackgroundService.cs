// Services/AuditBackgroundService.cs
using System.Threading.Channels;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cor.Finance.Services;

public class AuditBackgroundService : BackgroundService
{
    private readonly ILogger<AuditBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<AuditLog> _auditChannel; // ✅ Use AuditLog

    public AuditBackgroundService(
        ILogger<AuditBackgroundService> logger,
        IServiceProvider serviceProvider,
        Channel<AuditLog> auditChannel) // ✅ Use AuditLog
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _auditChannel = auditChannel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Audit Background Service started");

        try
        {
            await foreach (var auditLog in _auditChannel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await SaveAuditLogAsync(auditLog, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving audit log for user {User}", auditLog.UserName);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Audit Background Service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Audit Background Service encountered an error");
        }
        finally
        {
            _logger.LogInformation("Audit Background Service stopped");
        }
    }

    private async Task SaveAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

        // ✅ AuditLog is already the entity, just add it
        await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Saved audit log: {Action} by {User}", auditLog.Action, auditLog.UserName);
    }
}
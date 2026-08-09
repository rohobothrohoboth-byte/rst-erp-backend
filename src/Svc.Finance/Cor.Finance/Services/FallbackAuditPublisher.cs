// Services/FallbackAuditPublisher.cs

using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;

namespace Cor.Finance.Services;

public class FallbackAuditPublisher : IAuditLogPublisher
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FallbackAuditPublisher> _logger;
    private readonly IAuditLogPublisher? _rabbitMqPublisher;
    private bool _rabbitMqAvailable;

    public bool IsAvailable => true; // Always available (falls back to DB)

    public FallbackAuditPublisher(
        IServiceScopeFactory scopeFactory,
        ILogger<FallbackAuditPublisher> logger,
        IAuditLogPublisher? rabbitMqPublisher = null)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _rabbitMqPublisher = rabbitMqPublisher;

        // Check if RabbitMQ publisher is available
        if (rabbitMqPublisher != null && rabbitMqPublisher.IsAvailable)
        {
            _rabbitMqAvailable = true;
            _logger.LogInformation("✅ RabbitMQ publisher available, using it as primary");
        }
        else
        {
            _rabbitMqAvailable = false;
            _logger.LogWarning("⚠️ RabbitMQ unavailable, using direct DB fallback");
        }
    }

    public async Task PublishAsync(AuditLogEventDto auditEvent)
    {
        // ✅ Try RabbitMQ first if available
        if (_rabbitMqAvailable && _rabbitMqPublisher != null && _rabbitMqPublisher.IsAvailable)
        {
            try
            {
                await _rabbitMqPublisher.PublishAsync(auditEvent);
                _logger.LogDebug("📤 Audit event sent via RabbitMQ: {Action}", auditEvent.Action);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ RabbitMQ failed, falling back to direct DB save");
                _rabbitMqAvailable = false;
            }
        }

        // ✅ Fallback: Save directly to database
        await SaveDirectToDatabaseAsync(auditEvent);
    }

    private async Task SaveDirectToDatabaseAsync(AuditLogEventDto auditEvent)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = auditEvent.UserId,
                UserName = auditEvent.UserName,
                UserEmail = auditEvent.UserEmail,
                UserRole = auditEvent.UserRole,
                EntityType = auditEvent.EntityType ?? string.Empty,
                EntityId = auditEvent.EntityId,
                Action = auditEvent.Action ?? string.Empty,
                ActionDate = auditEvent.ActionDate,
                OldValues = auditEvent.OldValues,
                NewValues = auditEvent.NewValues,
                ChangesJson = auditEvent.ChangesJson,
                IpAddress = auditEvent.IpAddress,
                RequestId = auditEvent.RequestId,
                DurationMs = auditEvent.DurationMs,
                Status = auditEvent.Status,
                ErrorMessage = auditEvent.ErrorMessage,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            await dbContext.AuditLogs.AddAsync(auditLog);
            await dbContext.SaveChangesAsync();

            _logger.LogDebug("💾 Audit log saved directly to DB (fallback): {Action}", auditEvent.Action);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to save audit log to DB (fallback) for {Action}", auditEvent.Action);
        }
    }
}
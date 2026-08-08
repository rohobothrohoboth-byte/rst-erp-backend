// Services/DirectDbAuditPublisher.cs

using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;

namespace Cor.Finance.Services;

public class DirectDbAuditPublisher : IAuditLogPublisher
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DirectDbAuditPublisher> _logger;

    public bool IsAvailable => true;

    public DirectDbAuditPublisher(
        IServiceScopeFactory scopeFactory,
        ILogger<DirectDbAuditPublisher> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task PublishAsync(AuditLogEventDto auditEvent)
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
                EntityType = auditEvent.EntityType,
                EntityId = auditEvent.EntityId,
                Action = auditEvent.Action,
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

            _logger.LogDebug("💾 Audit log saved directly to DB: {Action}", auditEvent.Action);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to save audit log to DB for {Action}", auditEvent.Action);
        }
    }
}
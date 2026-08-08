// Services/IAuditLogPublisher.cs

using Cor.Finance.Models.DTOs;

namespace Cor.Finance.Services;

public interface IAuditLogPublisher
{
    Task PublishAsync(AuditLogEventDto auditEvent);
      bool IsAvailable { get; }
}
// Services/IAuditLogPublisher.cs

using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Services;

public interface IAuditLogPublisher
{
    Task PublishAsync(AuditLogEventDto auditEvent);
      bool IsAvailable { get; }
}
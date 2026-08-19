// Services/IAuditLogPublisher.cs

using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Services;

public interface IAuditLogPublisher
{
    Task PublishAsync(AuditLogEventDto auditEvent);
      bool IsAvailable { get; }
}
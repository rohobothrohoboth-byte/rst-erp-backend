using Shared.Helpers.Events;

namespace Svc.Notification.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(string entityName, string eventType, T data, CancellationToken ct = default);
}
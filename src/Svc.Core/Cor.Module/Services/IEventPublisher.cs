using Shared.Helpers.Events;

namespace Cor.Module.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(string entityName, string eventType, T data, CancellationToken ct = default);
}
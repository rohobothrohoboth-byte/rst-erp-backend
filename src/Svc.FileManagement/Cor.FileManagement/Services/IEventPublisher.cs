using Shared.Helpers.Events;

namespace Cor.FileManagement.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(string entityName, string eventType, T data, CancellationToken ct = default);
}
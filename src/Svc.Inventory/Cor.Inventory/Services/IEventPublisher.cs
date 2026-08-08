namespace Cor.Inventory.Services;

public interface IEventPublisher
{


        Task PublishAsync<T>(string entityName, string eventType, T data, CancellationToken ct = default);
}
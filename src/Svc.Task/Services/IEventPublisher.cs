using System.Threading;
using System.Threading.Tasks;

namespace Svc.Task.Services;

public interface IEventPublisher
{
    System.Threading.Tasks.Task PublishAsync<T>(string entityName, string eventType, T data, CancellationToken ct = default);
}
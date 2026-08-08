using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities.Local;
namespace Svc.HRM.Attendance.Services;

public interface IEventPublisher
{


        Task PublishAsync<T>(string entityName, string eventType, T data, CancellationToken ct = default);
}
namespace Svc.HRM.Payroll.Services;

public interface IEventPublisher
{
   //  Task PublishAsync<T>(string entityName, string eventType, T data, CancellationToken ct = default);


        Task PublishPayrollProcessedAsync(Guid payrollRunId, string payrollName, CancellationToken ct = default);
}
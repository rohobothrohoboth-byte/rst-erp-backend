using Svc.HRM.Payroll.Services;
namespace Svc.HRM.Payroll.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddPayrollServices(this IServiceCollection services)
    {
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<ITaxCalculator, TaxCalculator>();
        services.AddScoped<IPayslipGenerator, PayslipGenerator>();
        services.AddScoped<IEventPublisher, EventPublisher>();
        return services;
    }
}
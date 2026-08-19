using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Svc.Notification.Interfaces;
using Svc.Notification.Services;
using Svc.Notification.Data;
using System.Reflection;

namespace Svc.Notification.Extensions;

public static class ModuleRegistration
{
    public static IServiceCollection AddSvcNotification(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("NotificationDb"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(NotificationDbContext).Assembly.FullName)));

        // Register Dapper helper (keep for complex queries if needed)
        services.AddScoped<IDapperHelper, DapperHelper>();

        // Register MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}
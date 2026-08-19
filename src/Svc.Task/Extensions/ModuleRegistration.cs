using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Svc.Task.Interfaces;
using Svc.Task.Services;
using Svc.Task.Data;
using System.Reflection;

namespace Svc.Task.Extensions;

public static class ModuleRegistration
{
    public static IServiceCollection AddSvcTask(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<TaskDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("TaskDb"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(TaskDbContext).Assembly.FullName)));

        // Register Dapper helper (keep for complex queries if needed)
        services.AddScoped<IDapperHelper, DapperHelper>();

        // Register MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}
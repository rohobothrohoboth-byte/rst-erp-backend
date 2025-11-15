using Leave.App.Interfaces;
using Leave.Utility.Persistence;
using Leave.Utility.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Leave.Utility.Extensions;

public static class SvcCollExt
{
    public static IServiceCollection AddUtilitySvc(this IServiceCollection services, IConfiguration configuration)
    {
        // Keep EF Core for migrations/schema
        services.AddDbContext<HrmLeaveDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("HRMLeaveDbCon")));

        // Dapper and UoW
        services.AddScoped<DapperContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Generic Repo (scoped via UoW)
        services.AddScoped(typeof(IHrmLeaveRepo<>), typeof(HrmLeaveRepo<>));

        // Logging Service
        services.AddScoped<ILogService, LogService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

        return services;
    }
}
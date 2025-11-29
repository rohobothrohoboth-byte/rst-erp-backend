using Microsoft.EntityFrameworkCore;
using Svc.Identity.Interfaces;
using Svc.Identity.Persistence;
using Svc.Identity.Repos;

namespace Svc.Identity.Extensions;

public static class SvcCollExt
{
    public static IServiceCollection AddUtilitySvc(this IServiceCollection services, IConfiguration configuration)
    {
        // Keep EF Core for migrations/schema
        services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("authDbCon")));

        // Dapper and UoW
        services.AddScoped<DapperContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Generic Repo (scoped via UoW)
        services.AddScoped(typeof(IAuthRepo<>), typeof(AuthRepo<>));

        // Logging Service
        services.AddScoped<ILogService, LogService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

        return services;
    }
}

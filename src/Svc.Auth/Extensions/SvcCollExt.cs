using Microsoft.EntityFrameworkCore;
using Svc.Auth.Interfaces;
using Svc.Auth.Persistence;
using Svc.Auth.Repos;

namespace Svc.Auth.Extensions;

public static class SvcCollExt
{
    public static IServiceCollection AddUtilitySvc(this IServiceCollection services, IConfiguration configuration)
    {
        // Keep EF Core for migrations/schema
        services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("authMgrCon")));

        // Dapper and UoW
        services.AddScoped<DapperContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Generic Repo (scoped via UoW)
        services.AddScoped(typeof(IAuthMngrRepo<>), typeof(AuthMngrRepo<>));

        // Logging Service
        services.AddScoped<ILogService, LogService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

        return services;
    }
}


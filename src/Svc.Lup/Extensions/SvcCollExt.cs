using Microsoft.EntityFrameworkCore;
using Svc.Lup.Interfaces;
using Svc.Lup.Persistence;
using Svc.Lup.Repositories;

namespace Svc.Lup.Extensions;

public static class SvcCollExt
{
    public static IServiceCollection AddUtilitySvc(this IServiceCollection services, IConfiguration configuration)
    {
        // Keep EF Core for migrations/schema
        services.AddDbContext<LupDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("lupTableDbCon")));

        // Dapper and UoW
        services.AddScoped<DapperContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Generic Repo (scoped via UoW)
        services.AddScoped(typeof(ILupRepository<>), typeof(LupRepository<>));

        // Logging Service
        services.AddScoped<ILogService, LogService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        //services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(lupTablesController).Assembly));

        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.App.Interfaces;
using Module.Utility.Persistence;
using Module.Utility.Repositories;

namespace Module.Utility.Extensions;

public static class SvcCollExt
{
    public static IServiceCollection AddUtilitySvc(this IServiceCollection services, IConfiguration configuration)
    {
        // Keep EF Core for migrations/schema
        services.AddDbContext<CoreModuleDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("CorModuleDbCon")));

        // Dapper and UoW
        services.AddScoped<DapperContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Generic Repo (scoped via UoW)
        services.AddScoped(typeof(ICoreModuleRepo<>), typeof(CoreModuleRepo<>));

        // Logging Service
        services.AddScoped<ILogService, LogService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        //services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(AddCompCmd).Assembly));

        return services;
    }
}

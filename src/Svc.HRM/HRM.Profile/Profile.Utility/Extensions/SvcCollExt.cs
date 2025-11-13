using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Profile.App.Interfaces;
using Profile.Utility.Persistence;
using Profile.Utility.Repos;

namespace Profile.Utility.Extensions;

public static class SvcCollExt
{
    public static IServiceCollection AddUtilitySvc(this IServiceCollection services, IConfiguration configuration)
    {
        // Keep EF Core for migrations/schema
        services.AddDbContext<HrmProfileDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("HRMProDbCon")));

        // Dapper and UoW
        services.AddScoped<DapperContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Generic Repo (scoped via UoW)
        services.AddScoped(typeof(IHrmProfileRepo<>), typeof(HrmProfileRepo<>));

        // Logging Service
        services.AddScoped<ILogService, LogService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

        return services;
    }
}

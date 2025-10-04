using Cor.HRMM.Interfaces;
using Cor.HRMM.Persistence;
using Cor.HRMM.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Extensions;

public static class SvcCollExt
{
    public static IServiceCollection AddUtilitySvc(this IServiceCollection services, IConfiguration configuration)
    {
        // Keep EF Core for migrations/schema
        services.AddDbContext<coreHRMMDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("coreHRMMDbCon")));

        // Dapper and UoW
        services.AddScoped<DapperContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Generic Repo (scoped via UoW)
        services.AddScoped(typeof(ICorHRMMRepo<>), typeof(CorHRMMRepo<>));

        // Logging Service
        services.AddScoped<ILogService, LogService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        //services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(AddressController).Assembly));

        return services;
    }
}

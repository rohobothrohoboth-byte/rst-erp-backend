using Cor.App.Commands.Comp;
using Cor.App.Interfaces;
using Cor.Utility.Persistence;
using Cor.Utility.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cor.Utility.Extensions;

public static class SvcCollExt
{
    public static IServiceCollection AddUtilitySvc(this IServiceCollection services, IConfiguration configuration)
    {
        // Keep EF Core for migrations/schema
        services.AddDbContext<CoreDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("CoreDbCon")));

        // Dapper and UoW
        services.AddScoped<DapperContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Generic Repo (scoped via UoW)
        services.AddScoped(typeof(ICoreRepository<>), typeof(CoreRepository<>));

        // Logging Service
        services.AddScoped<ILogService, LogService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(AddCompCmd).Assembly));

        return services;
    }
}

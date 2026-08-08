using Cor.CRM.Interfaces;
using Cor.CRM.Persistence;
using Cor.CRM.Repos;
using Cor.CRM.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System.Reflection; // ✅ Add this

namespace Cor.CRM.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCRMServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<CrmDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("coreCRMDbCon")));

        // Register Infrastructure
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDapperHelper, DapperHelper>();
        services.AddScoped<IDbRetryHandler, DbRetryHandler>();

        services.AddScoped<ILogService, LogService>();

        // Register Services
        services.AddScoped<ILeadScoringService, LeadScoringService>();
        services.AddScoped<IUserClient, UserClient>();

        // ✅ Fix: Use Assembly.GetExecutingAssembly() to avoid ambiguity
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly));

        return services;
    }
}
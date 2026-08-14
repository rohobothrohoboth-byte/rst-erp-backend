// Extensions/ServiceExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using System.Reflection;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Services;
using Cor.ProjectManagement.Repositories;
using Cor.ProjectManagement.gRPCService;
using Shared.Helpers.Audit;
using Shared.Helpers.ExternalAccess;
using Shared.Helpers.Services;

namespace Cor.ProjectManagement.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddProjectManagementServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add DbContext
           // Extensions/ServiceExtensions.cs - Update DbContext registrations
           services.AddDbContext<ProjectDbContext>(options =>
               options.UseNpgsql(configuration.GetConnectionString("ProjectManagementConnection"),
                   builder => builder.MigrationsAssembly(typeof(ProjectDbContext).Assembly.FullName)));

           services.AddDbContext<ProjectDbContextReadOnly>(options =>
               options.UseNpgsql(configuration.GetConnectionString("ProjectManagementReadOnlyConnection"),
                   builder => builder.MigrationsAssembly(typeof(ProjectDbContext).Assembly.FullName)));

            services.AddDbContext<ProjectWarehouseContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("ProjectManagementWarehouseConnection"),
                    builder => builder.MigrationsAssembly(typeof(ProjectDbContext).Assembly.FullName)));

            // Add MediatR
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
            });

            // Add AutoMapper
            services.AddAutoMapper(typeof(Program));

            // Add FluentValidation
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            // Add Repositories
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITimesheetRepository, TimesheetRepository>();

            // Add Services
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITimesheetService, TimesheetService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IIntegrationService, IntegrationService>();

            // Add Background Services
            services.AddHostedService<AuditBackgroundService>();
            services.AddHostedService<CachePreWarmService>();

            // Add Caching
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "ProjectManagement";
            });

            // Add Health Checks
            services.AddHealthChecks()
                .AddDbContextCheck<ProjectDbContext>()
                .AddRedis(configuration.GetConnectionString("Redis"))
                .AddNpgSql(configuration.GetConnectionString("ProjectManagementConnection"));

            // Add gRPC Client
            services.AddGrpcClient<HrmGrpcService.HrmGrpcServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcServices:Hrm"]);
            });

            services.AddGrpcClient<FinanceGrpcService.FinanceGrpcServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcServices:Finance"]);
            });

            services.AddGrpcClient<CoreGrpcService.CoreGrpcServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcServices:Core"]);
            });

            return services;
        }
    }
}
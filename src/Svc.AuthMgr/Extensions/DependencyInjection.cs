using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Svc.AuthMgr.Interfaces;
using Svc.AuthMgr.Middlewares;
using Svc.AuthMgr.Persistence;

namespace Svc.AuthMgr.Extensions;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddApiVersioning(option =>
            {
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.DefaultApiVersion = new ApiVersion(1, 0);
                option.ReportApiVersions = true;
            }).AddMvc(option => { option.Conventions.Add(new VersionByNamespaceConvention()); })
            .AddApiExplorer(option =>
            {
                option.GroupNameFormat = "'v'V";
                option.SubstituteApiVersionInUrl = true;
            });

        builder.Services.AddOpenApi();
        return builder;
    }

    public static WebApplicationBuilder AddHttpClientServices(this WebApplicationBuilder builder)
    {
        //var gatewayUrl = builder.Configuration["Services:Gateway"];
        //var proUrl = builder.Configuration["Services:HRM.Profile"];

        //builder.Services.AddHttpClient<ILupClient, LupClient>(c => { c.BaseAddress = new Uri(new Uri(gatewayUrl!), proUrl); }).AddPolicyHandler(ResiliencePolicies.GetRetryPolicy()).AddPolicyHandler(ResiliencePolicies.GetTimeoutPolicy()).AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy());

        return builder;
    }

    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("authMgrCon")));
        builder.Services.AddScoped<ILogService, LogService>();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

        return builder;
    }

    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
            .WithTracing(tracing => tracing
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation())
            .WithMetrics(metrics => metrics
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation())
            .UseOtlpExporter();

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
        });

        return builder;
    }

    public static WebApplicationBuilder AddSwaggerService(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Auth Manager API",
                Version = "v1",
                Description = "API documentation for Auth Manager API Microservice",
                Contact = new OpenApiContact
                {
                    Name = "Development Team",
                    Email = "natnahel.shd@gmail.com.com"
                }
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });

        return builder;
    }

    public static WebApplicationBuilder AddAuthService(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>();

        return builder;
    }
}
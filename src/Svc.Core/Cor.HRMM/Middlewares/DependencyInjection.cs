using Asp.Versioning;
using Asp.Versioning.Conventions;
using Common;
using Cor.HRMM.Extensions;
using Cor.HRMM.Interfaces;
using Cor.HRMM.Persistence;
using Cor.HRMM.Repos;
using Cor.HRMM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text;

namespace Cor.HRMM.Middlewares;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.Services.AddCors(options => { options.AddPolicy("AllowAll", policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }); });
        builder.Services.AddHttpContextAccessor();
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

        builder.Services.AddDbContext<coreHRMMDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("coreHRMMDbCon")));
        builder.Services.AddScoped<DapperContext>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IAuthClient, AuthClient>();
        builder.Services.AddScoped<ILupClient, LupClient>();
        builder.Services.AddScoped<PerValService, PerValService>();
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        builder.Services.AddScoped<IAuthorizationHandler, PerAuthHandler>();
        builder.Services.AddScoped(typeof(ICorHRMMRepo<>), typeof(CorHRMMRepo<>));
        builder.Services.AddScoped<ILogService, LogService>();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        builder.Services.AddOpenApi();
        builder.Services.AddGrpc();
        return builder;
    }

    public static WebApplicationBuilder AddHttpClientServices(this WebApplicationBuilder builder)
    {
        var gatewayUrl = builder.Configuration["Services:Gateway"];
        var lupUrl = builder.Configuration["Services:Lup"];
        var corModuleUrl = builder.Configuration["Services:Cor.Module"];

        builder.Services
            .AddHttpClient<ILup, Lup>(c => { c.BaseAddress = new Uri(new Uri(gatewayUrl!), lupUrl); })
            .AddPolicyHandler(ResiliencePolicies.GetRetryPolicy())
            .AddPolicyHandler(ResiliencePolicies.GetTimeoutPolicy())
            .AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy());

        builder.Services
            .AddHttpClient<ICoreModuleClient, CoreModuleClient>(c =>
            {
                c.BaseAddress = new Uri(new Uri(gatewayUrl!), corModuleUrl);
            }).AddPolicyHandler(ResiliencePolicies.GetRetryPolicy())
            .AddPolicyHandler(ResiliencePolicies.GetTimeoutPolicy())
            .AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy());

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
        return builder;
    }


    public static WebApplicationBuilder AddSwaggerService(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Core HRMM API",
                Version = "v1",
                Description = "API documentation for Core HRMM API Microservice",
                Contact = new OpenApiContact
                {
                    Name = "Development Team",
                    Email = "natnahel.shd@gmail.com.com"
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath, true);

            //JWT Authentication (if needed)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
            });
        });

        return builder;
    }

    public static WebApplicationBuilder AddAuthService(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["AuthUrl"];
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = JwtCons.Issuer,
                ValidateAudience = true,
                ValidAudience = JwtCons.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtCons.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });

        builder.Services.AddAuthorization();

        return builder;
    }
}
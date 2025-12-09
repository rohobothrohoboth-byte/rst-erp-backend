using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Svc.Auth.Constants;
using Svc.Auth.Interfaces;
using Svc.Auth.Middlewares;
using Svc.Auth.Models.Entities;
using Svc.Auth.Persistence;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Svc.Auth.Services;

namespace Svc.Auth.Extensions;


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
        builder.Services.AddUtilitySvc(builder.Configuration);
        return builder;
    }

    public static WebApplicationBuilder AddHttpClientServices(this WebApplicationBuilder builder)
    {
        //var gatewayUrl = builder.Configuration["Services:Gateway"];
        //var proUrl = builder.Configuration["Services:HRM.Profile"];

        //builder.Services.AddHttpClient<IProClient, ProClient>(c => { c.BaseAddress = new Uri(new Uri(gatewayUrl!), proUrl); }).AddPolicyHandler(ResiliencePolicies.GetRetryPolicy()).AddPolicyHandler(ResiliencePolicies.GetTimeoutPolicy()).AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy());

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
        builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        builder.Services.AddScoped<IAuthorizationHandler, PerApiHandler>();
        builder.Services.AddScoped<ITokenService, TokenService>();

        //builder.Services.Configure<JwtAuthDto>(builder.Configuration.GetSection("Jwt"));

        //var jwtAuthOption = builder.Configuration.GetSection("Jwt").Get<JwtAuthDto>()!;
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = JwtCons.Issuer,
                    ValidAudience = JwtCons.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtCons.SecretKey))
                };
            });

        builder.Services.AddAuthorization();

        return builder;
    }
}
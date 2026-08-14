// Program.cs - FIXED with Kestrel Configuration
using Common;
using MediatR;
using Shared.Helpers.Services;
using Shared.Helpers.Audit;
using Shared.Helpers.ExternalAccess;
using Shared.Helpers.Extensions;
using Helpers;
using Serilog;
using Serilog.Events;
using Cor.ProjectManagement.Extensions;
using Cor.ProjectManagement.Middleware;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Asp.Versioning;
using StackExchange.Redis;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net.Security;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// ✅ CONFIGURATION LOADING
// ============================================================

// Load shared configuration
var sharedConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Shared", "Helpers", "appsettings.json");
if (File.Exists(sharedConfigPath))
{
    builder.Configuration.AddJsonFile(sharedConfigPath, optional: false, reloadOnChange: true);
    Console.WriteLine($"✅ Loaded shared configuration from: {sharedConfigPath}");
}

// Load service-specific configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Get ServiceHost and resolve placeholders
var serviceHost = builder.Configuration["ServiceHost"] ?? "localhost";
Console.WriteLine($"🏠 Service Host: {serviceHost}");

var configSections = builder.Configuration.AsEnumerable().ToList();
var updates = new Dictionary<string, string>();

foreach (var kvp in configSections)
{
    if (!string.IsNullOrEmpty(kvp.Value) && kvp.Value.Contains("{ServiceHost}"))
    {
        var newValue = kvp.Value.Replace("{ServiceHost}", serviceHost);
        updates[kvp.Key] = newValue;
    }
}

if (updates.Any())
{
    builder.Configuration.AddInMemoryCollection(updates);
}

// ============= CONFIGURATION HELPER =============
string GetConfig(string key, string? defaultValue = null)
{
    var value = builder.Configuration[key];
    if (!string.IsNullOrEmpty(value))
        return value;
    return defaultValue ?? string.Empty;
}

// ============================================================
// ✅ CONFIGURE KESTREL - THIS FIXES THE 502 ERROR
// ============================================================

// Get port from configuration
var projectManagementPortString = GetConfig("ServiceUrls:ProjectManagementApi", "https://localhost:7016");
var projectManagementPort = new Uri(projectManagementPortString).Port;
Console.WriteLine($"📡 Project Management Service Port: {projectManagementPort}");

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, projectManagementPort, listenOptions =>
        listenOptions.UseHttps()
    );
});

// ✅ Configure graceful shutdown
builder.Services.Configure<HostOptions>(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(10);
});

// ============= DATABASE CONNECTION =============
var dbConnectionString = GetConfig("ConnectionStrings:ProjectManagementConnection", null);

if (string.IsNullOrEmpty(dbConnectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "root";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "core.ProjectManagementDb";

    dbConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Include Error Detail=true";
}

// ============= REDIS =============
var redisConnectionString = builder.Configuration.GetConnectionString("redis")
    ?? GetConfig("Redis:ConnectionString", null)
    ?? $"{(Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost")}:{(Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379")},abortConnect=false";

Console.WriteLine($"🔗 Redis Connection: {redisConnectionString}");

// ============= RABBITMQ =============
var rabbitMqHost = GetConfig("RabbitMQ:Host", "localhost");
var rabbitMqPort = int.Parse(GetConfig("RabbitMQ:Port", "5672"));
var rabbitMqUsername = GetConfig("RabbitMQ:Username", "guest");
var rabbitMqPassword = GetConfig("RabbitMQ:Password", "guest");
var rabbitMqVirtualHost = GetConfig("RabbitMQ:VirtualHost", "/");

// ============= API KEYS =============
var coreApiKey = GetConfig("ApiKeys:CoreModule", "core_module_secret_key_2024");
var hrmmApiKey = GetConfig("ApiKeys:CoreHRMM", "core_module_secret_key_2024");
var profileApiKey = GetConfig("ApiKeys:ProfileModule", "profile_module_secret_key_2024");
var financeApiKey = GetConfig("ApiKeys:Finance", "finance_module_secret_key_2024");
var projectManagementApiKey = GetConfig("ApiKeys:ProjectManagement", "project_management_secret_key_2024");

// ============= SERVICE URLS =============
var corModUrl = GetConfig("ServiceUrls:CoreModuleApi", "https://localhost:7002");
var coreHrmmUrl = GetConfig("ServiceUrls:CoreHRMMApi", "https://localhost:7001");
var authUrl = GetConfig("ServiceUrls:AuthApi", "https://localhost:7000");
var hrmProUrl = GetConfig("ServiceUrls:HrmProApi", "https://localhost:7004");
var gatewayUrl = GetConfig("ServiceUrls:GatewayApi", "https://localhost:5000");
var financeApiUrl = GetConfig("ServiceUrls:FinanceApi", "https://localhost:7008");

// ============= CORS =============
var corsOrigins = GetConfig("Cors:AllowedOrigins", "http://localhost:5173,http://localhost:3000")
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(o => o.Trim())
    .ToArray();

// ============= LOGGING =============
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ============================================================
// ✅ SERVICES REGISTRATION
// ============================================================

// ============= BASIC SERVICES =============
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.MaxDepth = 64;
    });

// ============= SWAGGER =============
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

// ============= API VERSIONING =============
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");

// ============= MEDIATR =============
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// ============= FLUENTVALIDATION =============
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ============= DATABASE =============
builder.Services.AddDbContext<ProjectDbContext>(options =>
{
    options.UseNpgsql(dbConnectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

builder.Services.AddDbContext<ProjectDbContextReadOnly>(options =>
{
    options.UseNpgsql(dbConnectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    options.EnableSensitiveDataLogging(false);
});

// ============= REDIS =============
if (!string.IsNullOrEmpty(redisConnectionString))
{
    try
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = ConfigurationOptions.Parse(redisConnectionString);
            config.AbortOnConnectFail = false;
            config.ConnectTimeout = 5000;
            config.SyncTimeout = 5000;
            return ConnectionMultiplexer.Connect(config);
        });

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "ProjectManagement_";
        });

        builder.Services.AddSingleton<RedisCacheService>();
        builder.Services.AddSingleton<ICacheService, RedisCacheService>();
        Console.WriteLine("✅ Redis Cache ENABLED");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Redis failed: {ex.Message}. Falling back to MemoryCache.");
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSingleton<MemoryCacheService>();
        builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
    }
}
else
{
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSingleton<MemoryCacheService>();
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
    Console.WriteLine("✅ MemoryCache ENABLED");
}

// ============= RABBITMQ =============
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    return new ConnectionFactory
    {
        HostName = rabbitMqHost,
        Port = rabbitMqPort,
        UserName = rabbitMqUsername,
        Password = rabbitMqPassword,
        VirtualHost = rabbitMqVirtualHost,
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        DispatchConsumersAsync = true
    };
});

builder.Services.AddSingleton<IAuditLogPublisher, RabbitMQAuditPublisher>();
builder.Services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();

// ============= PROJECT MANAGEMENT SERVICES =============
builder.Services.AddProjectManagementServices(builder.Configuration);

// ============= SHARED SERVICES =============
builder.Services.AddExternalSystemAccess<ProjectDbContext>(builder.Configuration);
builder.Services.AddSharedAudit<ProjectDbContext>();

// ============= HEALTH CHECKS =============
var healthCheckBuilder = builder.Services.AddHealthChecks();
healthCheckBuilder.AddDbContextCheck<ProjectDbContext>("database", tags: new[] { "ready", "live" });
// Add API Key authentication scheme (shared)
builder.Services.AddAuthentication()
    .AddSharedApiKey();
// ============= AUTHENTICATION =============



var jwtSecret = JwtCons.SecretKey;
var jwtIssuer = JwtCons.Issuer;
var jwtAudience = JwtCons.Audience;


 if (!string.IsNullOrEmpty(builder.Configuration["Jwt:SecretKey"]))
 {
     jwtSecret = builder.Configuration["Jwt:SecretKey"];
 }

 if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
     throw new InvalidOperationException("JWT secret must be at least 32 bytes.");

 Console.WriteLine($"🔑 JWT Issuer: {jwtIssuer}");
 Console.WriteLine($"🔑 JWT Audience: {jwtAudience}");

// ✅ JWT Authentication - Registered ONCE here
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30),
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role,
        RequireSignedTokens = true,
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ JWT Authentication Failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"✅ JWT Token validated successfully for: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"⚠️ JWT Challenge: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, "ApiKey")
        .RequireAuthenticatedUser()
        .Build();
});

// ============= CORS =============
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins(corsOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// ============================================================
// ✅ BUILD APP
// ============================================================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();
app.UseResponseCaching();
app.UseCors("Default");

app.UseSharedAudit();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// ✅ Simple health endpoint
app.MapGet("/health", () => Results.Ok(new {
    status = "Healthy",
    service = "ProjectManagement API",
    timestamp = DateTime.UtcNow
}));

// ✅ Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

Console.WriteLine($"\n✅ Project Management Service started");
Console.WriteLine($"🔑 JWT Issuer: {jwtIssuer}");
Console.WriteLine($"🔑 JWT Audience: {jwtAudience}");
Console.WriteLine($"📝 Shared Audit: ENABLED");
Console.WriteLine($"🔑 API Key Authentication: ENABLED");
Console.WriteLine($"📦 Cache: {(redisConnectionString != null ? "Redis" : "MemoryCache")}");
Console.WriteLine($"📡 Port: {projectManagementPort}");
Console.WriteLine("\nPress Ctrl+C to stop");

await app.RunAsync();
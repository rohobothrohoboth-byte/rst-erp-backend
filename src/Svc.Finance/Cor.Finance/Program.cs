// Program.cs - Complete with Shared Configuration

using Common;
using Cor.Finance.Persistence;
using Cor.Finance.Services;
using Cor.Finance.gRPCService;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers.Services;
using Contracts;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Cor.Finance.Handlers;
using RabbitMQ.Client;
using StackExchange.Redis;
using Cor.Finance.HealthChecks;
using Shared.Helpers.Extensions;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Cor.Finance.Middlewares;
using Cor.Finance.Validators;
using FluentValidation;
using Cor.Finance.Filters;
using Cor.Finance.Queries;
using System.Security.Claims;
using System.Threading.Channels;
using Cor.Finance.Models.Entities;
using Cor.Finance.Seeding;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cor.Finance.Repositories;
using Cor.Finance.Models.Entities.Aggregates;
using Shared.Helpers;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Cor.Finance.Authentication;
using Microsoft.AspNetCore.Authorization;
using Polly;
using Polly.Extensions.Http;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Security.Cryptography.X509Certificates;
using Helpers;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// ✅ CONFIGURATION LOADING ORDER (Priority: High to Low)
// ============================================================

// ✅ Load shared configuration from src/Shared/Helpers/appsettings.json
var sharedConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Shared", "Helpers", "appsettings.json");
if (File.Exists(sharedConfigPath))
{
    builder.Configuration.AddJsonFile(sharedConfigPath, optional: false, reloadOnChange: true);
    Console.WriteLine($"✅ Loaded shared configuration from: {sharedConfigPath}");
}
else
{
    Console.WriteLine($"⚠️ Shared configuration not found at: {sharedConfigPath}");
}

// ✅ Load service-specific configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// ✅ Load environment-specific configuration
var environment = builder.Environment.EnvironmentName;
builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

// ✅ Add environment variables (highest priority - overrides everything)
builder.Configuration.AddEnvironmentVariables();

// ✅ Get ServiceHost and resolve placeholders
var serviceHost = builder.Configuration["ServiceHost"] ?? "localhost";
Console.WriteLine($"🏠 Service Host: {serviceHost}");

// ✅ Resolve all {ServiceHost} placeholders in configuration
var configSections = builder.Configuration.AsEnumerable().ToList();
var updates = new Dictionary<string, string?>();

foreach (var kvp in configSections)
{
    if (!string.IsNullOrEmpty(kvp.Value) && kvp.Value.Contains("{ServiceHost}"))
    {
        var newValue = kvp.Value.Replace("{ServiceHost}", serviceHost);
        updates[kvp.Key] = newValue;
        Console.WriteLine($"✅ Resolved {kvp.Key}: {newValue}");
    }
}

if (updates.Any())
{
    builder.Configuration.AddInMemoryCollection(updates);
}

// ✅ Configure Kestrel - Get port from resolved configuration
var financePortString = builder.Configuration["ServiceUrls:FinanceApi"] ?? "https://localhost:7008";
// Extract port from URL
var financePort = new Uri(financePortString).Port;
Console.WriteLine($"📡 Finance Service Port: {financePort}");

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, financePort, listenOptions => listenOptions.UseHttps());
});

// ✅ Configure graceful shutdown
builder.Services.Configure<HostOptions>(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(10);
});

// ============= LOGGING =============
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

Console.WriteLine("=== 🔍 FINANCE SERVICE DEBUG CONFIGURATION ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");

// ============= CONFIGURATION HELPER =============
string GetConfig(string key, string? defaultValue = null)
{
    var value = builder.Configuration[key];
    if (!string.IsNullOrEmpty(value))
        return value;

    if (defaultValue != null)
        return defaultValue;

    if (!builder.Environment.IsDevelopment())
        throw new InvalidOperationException($"Missing required configuration: {key}");

    return string.Empty;
}

// ============================================================
// ✅ LOAD ALL CONFIGURATION
// ============================================================

// ============= SERVICE URLS =============
var CorModUrl = GetConfig("ServiceUrls:CoreModuleApi", "https://localhost:7002");
var CorHrmmUrl = GetConfig("ServiceUrls:CoreHRMMApi", "https://localhost:7001");
var authUrl = GetConfig("ServiceUrls:AuthApi", "https://localhost:7000");
var hrmProUrl = GetConfig("ServiceUrls:HrmProApi", "https://localhost:7004");
var gatewayUrl = GetConfig("ServiceUrls:GatewayApi", "https://localhost:5000");

// ============= DATABASE =============
var dbConnectionString = GetConfig("ConnectionStrings:coreFinanceDbCon", null);

if (string.IsNullOrEmpty(dbConnectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "root";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "core.FinanceDbCon";

    dbConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Include Error Detail=true";
}

// ============= DATA WAREHOUSE =============
var warehouseConnectionString = GetConfig("ConnectionStrings:WarehouseDbCon", null);

if (string.IsNullOrEmpty(warehouseConnectionString))
{
    var whHost = Environment.GetEnvironmentVariable("WAREHOUSE_HOST") ?? "localhost";
    var whPort = Environment.GetEnvironmentVariable("WAREHOUSE_PORT") ?? "5432";
    var whUser = Environment.GetEnvironmentVariable("WAREHOUSE_USER") ?? "postgres";
    var whPassword = Environment.GetEnvironmentVariable("WAREHOUSE_PASSWORD") ?? "root";
    var whName = Environment.GetEnvironmentVariable("WAREHOUSE_DB") ?? "core.FinanceWarehouse";

    warehouseConnectionString = $"Host={whHost};Port={whPort};Database={whName};Username={whUser};Password={whPassword};Include Error Detail=true";
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

// ============= AUDIT LOG SETTINGS =============
var auditEnabled = GetConfig("AuditLog:Enabled", "true") == "true";
var auditQueueName = GetConfig("AuditLog:QueueName", "audit-events");
var auditBatchSize = int.Parse(GetConfig("AuditLog:BatchSize", "100"));
var auditFlushInterval = int.Parse(GetConfig("AuditLog:FlushIntervalSeconds", "5"));

// ============= CORS =============
var corsOrigins = GetConfig("Cors:AllowedOrigins", "http://localhost:5173,http://localhost:3000")
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(o => o.Trim())
    .ToArray();

// ============= LOG CONFIGURATION =============
Console.WriteLine("\n=== 📋 CONFIGURATION LOADED ===");
Console.WriteLine($"🌍 Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"🔗 Core Module URL: {CorModUrl}");
Console.WriteLine($"🔗 Core HRMM URL: {CorHrmmUrl}");
Console.WriteLine($"🔗 HRM Pro URL: {hrmProUrl}");
Console.WriteLine($"🔗 Auth URL: {authUrl}");
Console.WriteLine($"🔗 Gateway URL: {gatewayUrl}");
Console.WriteLine($"🗄️  Database: {dbConnectionString.Split(';').FirstOrDefault()?.Replace("Host=", "") ?? "unknown"}");
Console.WriteLine($"📦 Redis: {redisConnectionString.Split(',').FirstOrDefault() ?? "unknown"}");
Console.WriteLine($"🐰 RabbitMQ: {rabbitMqHost}:{rabbitMqPort}");
Console.WriteLine($"📝 Audit Log: {(auditEnabled ? "Enabled" : "Disabled")}");
Console.WriteLine($"🌐 CORS Origins: {string.Join(", ", corsOrigins)}");
Console.WriteLine("=====================================\n");

// ============================================================
// ✅ SERVICES REGISTRATION
// ============================================================

// ============= BASIC SERVICES =============
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = false;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.MaxDepth = 64;
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.WriteIndented = false;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGrpc();
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(RequestAmendmentHandler).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ============= DATABASE =============
builder.Services.AddDbContext<FinanceDbContext>(options =>
{
    options.UseNpgsql(dbConnectionString);
});
builder.Services.AddDbContext<FinanceDbContextReadOnly>(options =>
{
    options.UseNpgsql(dbConnectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    options.EnableSensitiveDataLogging(false);
});

// ✅ Add this - Register Warehouse DbContext
builder.Services.AddDbContext<FinanceWarehouseContext>(options =>
{
    options.UseNpgsql(warehouseConnectionString);
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
            options.InstanceName = "Finance_";
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

// Register aggregate repositories
builder.Services.AddScoped<IAggregateRepository<InvoiceAggregate>, AggregateRepository<InvoiceAggregate>>();
builder.Services.AddScoped<IAggregateRepository<PaymentAggregate>, AggregateRepository<PaymentAggregate>>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

// ============= AGGREGATE SERVICE =============
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<CachedReferenceDataService>();
builder.Services.AddHostedService<AggregateService>();

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
builder.Services.AddHostedService<AuditLogConsumerService>();
builder.Services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();

// ============= AUDIT CHANNEL =============
builder.Services.AddSingleton(Channel.CreateUnbounded<AuditLog>(
    new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false,
        AllowSynchronousContinuations = false
    }));

builder.Services.AddHostedService<AuditBackgroundService>();

// ============= API CLIENTS =============
bool isDev = builder.Environment.IsDevelopment();

builder.Services.AddSingleton<Func<HttpClientHandler>>(sp => () => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = isDev
        ? (sender, cert, chain, sslPolicyErrors) => true
        : null,
    UseCookies = false,
    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
    MaxConnectionsPerServer = 10
});

builder.Services.AddHttpClient<IHrmProApiService, HrmProApiService>(client =>
{
    client.BaseAddress = new Uri(hrmProUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("X-API-Key", profileApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "FinanceService");
})
.ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<Func<HttpClientHandler>>()())
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Services.AddHttpClient<ICoreModuleApiService, CoreModuleApiService>(client =>
{
    client.BaseAddress = new Uri(CorModUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("X-API-Key", coreApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "FinanceService");
})
.ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<Func<HttpClientHandler>>()())
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Services.AddHttpClient<ICoreHrmmApiService, CoreHrmmApiService>(client =>
{
    client.BaseAddress = new Uri(CorHrmmUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("X-API-Key", hrmmApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "FinanceService");
})
.ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<Func<HttpClientHandler>>()())
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

// ============= HOSTED SERVICES =============
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddHostedService<InitialSyncService>();
builder.Services.AddHostedService<EventConsumer>();

// ============= HEALTH CHECKS =============
builder.Services.AddHealthChecks()
    .AddDbContextCheck<FinanceDbContext>()
    .AddCheck<RedisHealthCheck>("Redis")
    .AddUrlGroup(new Uri($"{hrmProUrl}/health"), "HRM Pro");

// ============= CORS =============
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        if (isDev || corsOrigins.Length == 0)
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
            Console.WriteLine("⚠️ CORS: AllowAnyOrigin (Development mode)");
        }
        else
        {
            policy.WithOrigins(corsOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
            Console.WriteLine($"✅ CORS: Allowed origins: {string.Join(", ", corsOrigins)}");
        }
    });
});

// ============================================================
// ✅ API KEY AUTHENTICATION
// ============================================================

// Register API Key services
builder.Services.Configure<ApiKeySettings>(builder.Configuration.GetSection("ApiKey"));
builder.Services.Configure<ApiKeyRateLimitOptions>(builder.Configuration.GetSection("ApiKeyRateLimit"));
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<IExternalSystemService, ExternalSystemService>();

// Add API Key authentication scheme
builder.Services.AddAuthentication()
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

// ============================================================
// ✅ JWT AUTHENTICATION (ONLY ONCE)
// ============================================================

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

// ✅ Unified authorization policy that accepts both schemes
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, "ApiKey")
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddScoped<DbPerformanceTestSeeder>();

// ============================================================
// ✅ ADD EXTENSION METHODS (without duplicate Auth)
// ============================================================

// ✅ Call extension methods - AddAuthService is NOT called here
// because we already registered authentication above
Cor.Finance.Middlewares.DependencyInjection.AddApiServices(builder)
    .AddErrorHandling()
    .AddSwaggerService();
// NOTE: .AddAuthService() is NOT called - we already have authentication above

// ============================================================
// ✅ BUILD APP
// ============================================================

var app = builder.Build();

// ✅ Middleware Pipeline
app.UseResponseCompression();
app.UseResponseCaching();
app.UseCors("Default");
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
    app.MapScalarApiReference();

app.UseHttpsRedirection();

// ✅ Both authentication schemes will work
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuditLogMiddleware>();

// ✅ Database Migration + Cache Pre-warm
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    await db.Database.MigrateAsync();

    try
    {
        var cachedService = scope.ServiceProvider.GetRequiredService<CachedReferenceDataService>();
        await cachedService.GetCachedAccountsAsync();
        Console.WriteLine("✅ Cache pre-warmed successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Cache pre-warm failed: {ex.Message}");
    }
}

// ✅ Map endpoints
app.MapControllers();
app.MapGrpcService<FinanceGrpcService>();
app.MapHealthChecks("/health");

Console.WriteLine($"\n✅ Finance Service starting on https://0.0.0.0:{financePort}");
Console.WriteLine($"🔑 JWT Issuer: {JwtCons.Issuer}");
Console.WriteLine($"🔑 JWT Audience: {JwtCons.Audience}");
Console.WriteLine($"🔑 API Key Authentication: ENABLED");
Console.WriteLine($"📝 Audit Log: {(auditEnabled ? "Enabled" : "Disabled")}");
Console.WriteLine("📊 Debug logging is ENABLED - check console for details");
Console.WriteLine("\nPress Ctrl+C to stop");

// ✅ Graceful Shutdown
try
{
    await app.RunAsync();
}
catch (OperationCanceledException)
{
    Console.WriteLine("Application shutdown requested");
}
finally
{
    Console.WriteLine("Cleaning up resources...");
    await Log.CloseAndFlushAsync();
}

// ✅ Performance tracking middleware
app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    await next();
    stopwatch.Stop();

    context.Response.Headers["X-Response-Time-ms"] = stopwatch.ElapsedMilliseconds.ToString();
    context.Response.Headers["X-Dashboard-Generation-ms"] =
        context.Items["DashboardGenerationMs"]?.ToString() ?? "N/A";
});
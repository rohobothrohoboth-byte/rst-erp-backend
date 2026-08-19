using Common;
using Cor.Inventory.Persistence;
using Cor.Inventory.Services;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers.Services;
using Serilog;
using StackExchange.Redis;
using Asp.Versioning;
using Polly;
using Polly.Extensions.Http;
using RabbitMQ.Client;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.Threading.Channels;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Cor.Inventory.HealthChecks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Shared.Helpers.ExternalAccess;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// ✅ CONFIGURATION LOADING ORDER
// ============================================================

// Load shared configuration
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

// Load service-specific configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Load environment-specific configuration
var environment = builder.Environment.EnvironmentName;
builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

// Add environment variables
builder.Configuration.AddEnvironmentVariables();

// Get ServiceHost and resolve placeholders
var serviceHost = builder.Configuration["ServiceHost"] ?? "localhost";
Console.WriteLine($"🏠 Service Host: {serviceHost}");

// Resolve all {ServiceHost} placeholders
var configSections = builder.Configuration.AsEnumerable().ToList();
var updates = new Dictionary<string, string>();

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

// Configure Kestrel
var inventoryPortString = builder.Configuration["ServiceUrls:InventoryApi"] ?? "https://localhost:7014";
var inventoryPort = new Uri(inventoryPortString).Port;
Console.WriteLine($"📡 Inventory Service Port: {inventoryPort}");

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, inventoryPort, listenOptions => listenOptions.UseHttps());
});

// Configure graceful shutdown
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

Console.WriteLine("=== 🔍 INVENTORY SERVICE DEBUG CONFIGURATION ===");
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

// Service URLs
var authUrl = GetConfig("ServiceUrls:AuthApi", "https://localhost:7000");
var gatewayUrl = GetConfig("ServiceUrls:GatewayApi", "https://localhost:5000");

// Database
var dbConnectionString = GetConfig("ConnectionStrings:InventoryDbCon", null);

if (string.IsNullOrEmpty(dbConnectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "root";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "core.InventoryDb";

    dbConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Include Error Detail=true";
}

// Redis
var redisConnectionString = builder.Configuration.GetConnectionString("redis")
    ?? $"{(Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost")}:{(Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379")},abortConnect=false";

Console.WriteLine($"🔗 Redis Connection: {redisConnectionString}");

// RabbitMQ
var rabbitMqHost = GetConfig("RabbitMQ:Host", "localhost");
var rabbitMqPort = int.Parse(GetConfig("RabbitMQ:Port", "5672"));
var rabbitMqUsername = GetConfig("RabbitMQ:Username", "guest");
var rabbitMqPassword = GetConfig("RabbitMQ:Password", "guest");
var rabbitMqVirtualHost = GetConfig("RabbitMQ:VirtualHost", "/");

// API Keys
var inventoryApiKey = GetConfig("ApiKeys:Inventory", "inventory_module_secret_key_2024");

// CORS
var corsOrigins = GetConfig("Cors:AllowedOrigins", "http://localhost:5173,http://localhost:3000")
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(o => o.Trim())
    .ToArray();

Console.WriteLine("\n=== 📋 CONFIGURATION LOADED ===");
Console.WriteLine($"🌍 Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"🔗 Auth URL: {authUrl}");
Console.WriteLine($"🔗 Gateway URL: {gatewayUrl}");
Console.WriteLine($"🗄️  Database: {dbConnectionString.Split(';').FirstOrDefault()?.Replace("Host=", "") ?? "unknown"}");
Console.WriteLine($"📦 Redis: {redisConnectionString.Split(',').FirstOrDefault() ?? "unknown"}");
Console.WriteLine($"🐰 RabbitMQ: {rabbitMqHost}:{rabbitMqPort}");
Console.WriteLine($"🌐 CORS Origins: {string.Join(", ", corsOrigins)}");
Console.WriteLine("=====================================\n");

// ============= BASIC SERVICES =============
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

// ============= DATABASE =============
builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    options.UseNpgsql(dbConnectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
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
            options.InstanceName = "Inventory_";
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

builder.Services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();

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

// ============= SERVICES =============
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IWarehouseZoneService, WarehouseZoneService>();
builder.Services.AddScoped<IReorderService, ReorderService>();
builder.Services.AddScoped<IValuationService, ValuationService>();
builder.Services.AddScoped<IInvAnalyticsService, InvAnalyticsService>();
builder.Services.AddScoped<IInvDashboardService, InvDashboardService>();

// ✅ Register API Key Services (shared)
builder.Services.AddExternalSystemAccess<InventoryDbContext>(builder.Configuration);

// Add API Key authentication scheme (shared)
builder.Services.AddAuthentication()
    .AddSharedApiKey();

// ============= HEALTH CHECKS =============
builder.Services.AddHealthChecks()
    .AddDbContextCheck<InventoryDbContext>();

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

// ============= JWT AUTHENTICATION =============
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

// Enable [PerAuth("permission")] enforcement. This must be registered directly here
// because AddApiServices() (which also registers it) is not called in this Program.
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PerAuthHandler>();

// ============================================================
// ✅ BUILD APP
// ============================================================

var app = builder.Build();

// Middleware Pipeline
app.UseResponseCompression();
app.UseResponseCaching();
app.UseCors("Default");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API v1");
        c.RoutePrefix = "swagger";
    });
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// ✅ Both authentication schemes will work
app.UseAuthentication();
app.UseAuthorization();

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    await db.Database.MigrateAsync();
    Console.WriteLine("✅ Database migrations applied successfully");
}

// Map endpoints
app.MapControllers();
app.MapHealthChecks("/health");

Console.WriteLine($"\n✅ Inventory Service starting on https://0.0.0.0:{inventoryPort}");
Console.WriteLine($"🔗 Swagger: https://0.0.0.0:{inventoryPort}/swagger");
Console.WriteLine($"🔑 JWT Issuer: {jwtIssuer}");
Console.WriteLine($"🔑 JWT Audience: {jwtAudience}");
Console.WriteLine($"🔑 API Key Authentication: ENABLED");
Console.WriteLine("\nPress Ctrl+C to stop");

// Graceful Shutdown
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
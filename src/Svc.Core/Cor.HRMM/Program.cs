using Cor.HRMM.gRPCService;
using Cor.HRMM.Middlewares;
using Scalar.AspNetCore;
using Serilog;
using Cor.HRMM.Services;
using RabbitMQ.Client;
using Shared.Helpers.Services;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Cor.HRMM.HealthChecks;
using StackExchange.Redis;
using Cor.HRMM.Interfaces;
using Cor.HRMM.Repos;
using Shared.Helpers.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Shared.Helpers.ExternalAccess;
using Cor.HRMM.Persistence;
using Cor.HRMM.Middleware;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Shared.Helpers;
using Helpers;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// ✅ CONFIGURATION LOADING ORDER
// ============================================================

// ✅ Load shared configuration
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

// ✅ Add environment variables (highest priority)
builder.Configuration.AddEnvironmentVariables();

// ✅ Get ServiceHost and resolve placeholders
var serviceHost = builder.Configuration["ServiceHost"] ?? "localhost";
Console.WriteLine($"🏠 Service Host: {serviceHost}");

// ✅ Resolve all {ServiceHost} placeholders
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

// ✅ Configure Kestrel - Get port from resolved configuration
var coreHrmmPortString = builder.Configuration["ServiceUrls:CoreHRMMApi"] ?? "https://localhost:7001";
var coreHrmmPort = new Uri(coreHrmmPortString).Port;
Console.WriteLine($"📡 Core HRMM Service Port: {coreHrmmPort}");

// ✅ KESTREL CONFIGURATION
builder.WebHost.ConfigureKestrel(options =>
{
    var isProduction = environment == "Production";

    Console.WriteLine($"🔧 Configuring Kestrel for {environment} environment...");

    if (isProduction)
    {
        var certPath = builder.Configuration["Certificate:Path"] ?? "Certificates/prod-certificate.pfx";
        var certPassword = builder.Configuration["Certificate:Password"] ?? "YourSecurePassword123!";

        if (File.Exists(certPath))
        {
            try
            {
                var certificate = new X509Certificate2(certPath, certPassword);
                Console.WriteLine("✅ Production certificate loaded");
                options.Listen(IPAddress.Any, coreHrmmPort, listenOptions =>
                {
                    listenOptions.UseHttps(certificate);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading certificate: {ex.Message}");
                options.Listen(IPAddress.Any, coreHrmmPort, listenOptions => listenOptions.UseHttps());
            }
        }
        else
        {
            Console.WriteLine("⚠️ Production certificate not found, using development certificate");
            options.Listen(IPAddress.Any, coreHrmmPort, listenOptions => listenOptions.UseHttps());
        }
    }
    else
    {
        options.Listen(IPAddress.Any, coreHrmmPort, listenOptions => listenOptions.UseHttps());
        Console.WriteLine("✅ Development certificate configured");
    }
});

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

// ============= SERVICE URLS =============
var CorModUrl = GetConfig("ServiceUrls:CoreModuleApi", "https://localhost:7002");
var authUrl = GetConfig("ServiceUrls:AuthApi", "https://localhost:7000");
var hrmProUrl = GetConfig("ServiceUrls:HrmProApi", "https://localhost:7004");
var financeApiUrl = GetConfig("ServiceUrls:FinanceApi", "https://localhost:7008");
var gatewayApiUrl = GetConfig("ServiceUrls:GatewayApi", "https://localhost:5000");

// API Keys
var coreApiKey = GetConfig("ApiKeys:CoreModule", "core_module_secret_key_2024");
var hrmmApiKey = GetConfig("ApiKeys:CoreHRMM", "core_module_secret_key_2024");
var profileApiKey = GetConfig("ApiKeys:ProfileModule", "profile_module_secret_key_2024");

// Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

Console.WriteLine("=== 🔍 CORE HRMM SERVICE DEBUG CONFIGURATION ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"📡 Core Module URL: {CorModUrl}");
Console.WriteLine($"📡 Auth URL: {authUrl}");
Console.WriteLine($"📡 HRM Pro URL: {hrmProUrl}");

// ============= DATABASE =============
var dbConnectionString = GetConfig("ConnectionStrings:coreHRMMDbCon", null);

if (string.IsNullOrEmpty(dbConnectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "root";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "core.HRMM";

    dbConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Include Error Detail=true";
}

// ============= REDIS CACHING =============
var redisConnectionString = GetConfig("Redis:ConnectionString", null);

if (string.IsNullOrEmpty(redisConnectionString))
{
    var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
    var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";
    var redisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD") ?? "";
    var redisSsl = Environment.GetEnvironmentVariable("REDIS_SSL")?.ToLower() == "true";

    redisConnectionString = $"{redisHost}:{redisPort},abortConnect=false";
    if (!string.IsNullOrEmpty(redisPassword))
        redisConnectionString += $",password={redisPassword}";
    if (redisSsl)
        redisConnectionString += $",ssl=true";
}

Console.WriteLine($"🔗 Redis Connection: {redisConnectionString}");

var redisAvailable = false;
var workingConnectionString = redisConnectionString;

var connectionAttempts = new List<string>
{
    redisConnectionString,
    "localhost:6379,abortConnect=false",
    "redis:6379,abortConnect=false",
};

foreach (var attempt in connectionAttempts.Distinct())
{
    if (redisAvailable) break;

    try
    {
        Console.WriteLine($"🔄 Attempting Redis connection: {attempt}");
        var config = ConfigurationOptions.Parse(attempt);
        config.ConnectTimeout = 5000;
        config.SyncTimeout = 5000;
        config.AbortOnConnectFail = false;
        config.ConnectRetry = 3;

        using var connection = ConnectionMultiplexer.Connect(config);
        await Task.Delay(100);
        redisAvailable = connection.IsConnected;

        if (redisAvailable)
        {
            workingConnectionString = attempt;
            Console.WriteLine($"✅ Redis connection SUCCESSFUL");
            break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Redis connection FAILED: {ex.Message}");
        redisAvailable = false;
    }
}

// ============= REGISTER CACHE SERVICES =============
builder.Services.AddMemoryCache();

if (redisAvailable)
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        var config = ConfigurationOptions.Parse(workingConnectionString);
        config.AbortOnConnectFail = false;
        config.ConnectTimeout = 5000;
        config.SyncTimeout = 5000;
        return ConnectionMultiplexer.Connect(config);
    });

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = workingConnectionString;
        options.InstanceName = "CorHRMM_";
    });
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();
    builder.Services.AddSingleton<IDistributedLockService, DistributedLockService>();
    Console.WriteLine("✅ Redis Cache ENABLED");
}
else
{
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
    builder.Services.AddSingleton<IDistributedLockService, MemoryDistributedLockService>();
    Console.WriteLine("✅ MemoryCache ENABLED (Redis fallback)");
}

// ============= SSL BYPASS HANDLER =============
var sslHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
    MaxConnectionsPerServer = 50,
    AutomaticDecompression = DecompressionMethods.GZip
};

// ============= HTTP CLIENTS =============

// Core Module API
builder.Services.AddHttpClient<ICoreModuleApiService, CoreModuleApiService>(client =>
{
    client.BaseAddress = new Uri(CorModUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", coreApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "CoreHRMMService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(2));

// Auth API
builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(authUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", coreApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "CoreHRMMService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(2));

// ============= POLLY RETRY POLICIES =============
var retryPolicy = Policy<HttpResponseMessage>
    .Handle<HttpRequestException>()
    .OrResult(r => !r.IsSuccessStatusCode)
    .RetryAsync(3, onRetry: (outcome, retryCount, context) =>
    {
        Log.Warning("⚠️ Retry {RetryCount} for API call. Error: {Error}",
            retryCount, outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
    });

var circuitBreakerPolicy = Policy<HttpResponseMessage>
    .Handle<HttpRequestException>()
    .OrResult(r => r.StatusCode == HttpStatusCode.ServiceUnavailable)
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 3,
        durationOfBreak: TimeSpan.FromSeconds(30));

// Apply policies
builder.Services.AddHttpClient<ICoreModuleApiService, CoreModuleApiService>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<IAuthApiService, AuthApiService>()
    .AddPolicyHandler(retryPolicy);

// ============= RABBITMQ =============
var rabbitMqHost = GetConfig("RabbitMQ:Host", "localhost");
var rabbitMqPort = int.Parse(GetConfig("RabbitMQ:Port", "5672"));
var rabbitMqUsername = GetConfig("RabbitMQ:Username", "guest");
var rabbitMqPassword = GetConfig("RabbitMQ:Password", "guest");

builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    Console.WriteLine($"🔄 Connecting to RabbitMQ at {rabbitMqHost}:{rabbitMqPort}");

    return new ConnectionFactory
    {
        HostName = rabbitMqHost,
        Port = rabbitMqPort,
        UserName = rabbitMqUsername,
        Password = rabbitMqPassword,
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        RequestedHeartbeat = TimeSpan.FromSeconds(30),
        ContinuationTimeout = TimeSpan.FromSeconds(20)
    };
});

// ============= REGISTER SERVICES =============
builder.Services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddHostedService<EventConsumer>();
builder.Services.AddHostedService<InitialSyncService>();
builder.Services.AddHostedService<CacheWarmupService>();
builder.Services.AddScoped<IDbExceptionTranslator, DbExceptionTranslator>();

// ============= HEALTH CHECKS =============
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri($"{CorModUrl}/health"), "Core Module API")
    .AddCheck<SyncHealthCheck>("Sync Status")
    .AddCheck<CertificateHealthCheck>("Certificate");

// ============= RATE LIMITING =============
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 100;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 5;
    });

    options.AddFixedWindowLimiter("Write", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 30;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 5;
    });
});

// ================================================================
// ✅ CORS CONFIGURATION
// ================================================================

var corsOrigins = GetConfig("Cors:AllowedOrigins", "http://localhost:5173,http://localhost:3000")
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(o => o.Trim())
    .ToArray();

// Resolve {ServiceHost} in CORS origins
var resolvedOrigins = corsOrigins
    .Select(origin => origin.Replace("{ServiceHost}", serviceHost))
    .ToArray();

Console.WriteLine("🌐 CORS Allowed Origins:");
foreach (var origin in resolvedOrigins)
{
    Console.WriteLine($"   {origin}");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
            Console.WriteLine("⚠️ CORS: AllowAnyOrigin (Development mode)");
        }
        else
        {
            policy.WithOrigins(resolvedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
            Console.WriteLine($"✅ CORS: Allowed origins: {string.Join(", ", resolvedOrigins)}");
        }
    });
});

// ============= API KEY AUTHENTICATION (shared) =============
builder.Services.AddExternalSystemAccess<coreHRMMDbContext>(builder.Configuration);
builder.Services.Configure<ApiKeyRateLimitOptions>(builder.Configuration.GetSection("ApiKeyRateLimit"));

builder.Services.AddAuthentication()
    .AddSharedApiKey();

// ============= BUILDER EXTENSIONS =============
builder.AddApiServices()
    .AddErrorHandling()
    .AddSwaggerService()
    .AddAuthService();

var app = builder.Build();

// ✅ Middleware pipeline
app.UseMiddleware<ApiKeyRateLimiterMiddleware>();
app.MapDefaultEndpoints();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors("AllowAll");
app.MapGrpcService<CorHrmmListService>();

if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference(options => { options.WithTitle("Core HRMM API"); });
    app.ApplyMigration();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.ToString()
            }),
            totalDuration = report.TotalDuration.ToString()
        });
        await context.Response.WriteAsync(result);
    }
});

Console.WriteLine($"\n✅ Core HRMM Service starting on https://0.0.0.0:{coreHrmmPort}");
Console.WriteLine($"🔗 Auth URL: {authUrl}");
Console.WriteLine($"🔗 Core Module URL: {CorModUrl}");
Console.WriteLine($"🔑 API Key authentication enabled");
Console.WriteLine("📊 Debug logging is ENABLED");
Console.WriteLine("\nPress Ctrl+C to stop");

await app.RunAsync();

// ============================================================
// 📁 TYPES AND CLASSES
// ============================================================

public interface IAuthApiService
{
    Task<HttpResponseMessage> GetAsync(string endpoint, CancellationToken ct = default);
}

public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint, CancellationToken ct = default)
    {
        return await _httpClient.GetAsync(endpoint, ct);
    }
}

public class CertificateHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CertificateHealthCheck> _logger;

    public CertificateHealthCheck(IConfiguration configuration, ILogger<CertificateHealthCheck> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            if (environment == "Production")
            {
                var certPath = _configuration["Certificate:Path"] ?? "Certificates/prod-certificate.pfx";
                var certPassword = _configuration["Certificate:Password"] ?? "YourSecurePassword123!";

                if (File.Exists(certPath))
                {
                    var cert = new X509Certificate2(certPath, certPassword);

                    if (cert.NotAfter > DateTime.Now)
                    {
                        return Task.FromResult(HealthCheckResult.Healthy(
                            $"Certificate valid until {cert.NotAfter:yyyy-MM-dd}"));
                    }
                    else
                    {
                        return Task.FromResult(HealthCheckResult.Unhealthy(
                            $"Certificate expired on {cert.NotAfter:yyyy-MM-dd}"));
                    }
                }
                else
                {
                    return Task.FromResult(HealthCheckResult.Degraded("Certificate file not found"));
                }
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Healthy("Development certificate"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Certificate validation failed");
            return Task.FromResult(HealthCheckResult.Unhealthy("Certificate validation failed", ex));
        }
    }
}
using Leave.API.Middlewares;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Leave.App.Services;
using Leave.App.Interfaces;
using Leave.Utility.Repos;
using Leave.Utility.Persistence;
using Shared.Helpers.Extensions;
using Shared.Helpers;
using Helpers;
using Common;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Cryptography.X509Certificates;
using Polly;
using Polly.Extensions.Http;
using Shared.Helpers.Services;
using StackExchange.Redis;
using RabbitMQ.Client;
using System.Threading.RateLimiting;
var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. CONFIGURATION LOADING
// ============================================================

// Load shared configuration
var sharedConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Shared", "Helpers", "appsettings.json");
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

// ============================================================
// 2. CONFIGURATION HELPER
// ============================================================

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
// 3. SERVICE URLS
// ============================================================

var authUrl = GetConfig("ServiceUrls:AuthApi", "https://localhost:7000");
var corModUrl = GetConfig("ServiceUrls:CoreModuleApi", "https://localhost:7002");
var corHrmmUrl = GetConfig("ServiceUrls:CoreHRMMApi", "https://localhost:7001");
var hrmProUrl = GetConfig("ServiceUrls:HrmProApi", "https://localhost:7004");
var leavePortString = GetConfig("ServiceUrls:LeaveApi", "https://localhost:7003");
var leavePort = new Uri(leavePortString).Port;

// API Keys
var coreApiKey = GetConfig("ApiKeys:CoreModule", "core_module_secret_key_2024");
var hrmmApiKey = GetConfig("ApiKeys:CoreHRMM", "core_module_secret_key_2024");
var profileApiKey = GetConfig("ApiKeys:ProfileModule", "profile_module_secret_key_2024");

// ============================================================
// 4. KESTREL CONFIGURATION
// ============================================================

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, leavePort, listenOptions =>
    {
        if (environment == "Production")
        {
            var certPath = builder.Configuration["Certificate:Path"] ?? "Certificates/prod-certificate.pfx";
            var certPassword = builder.Configuration["Certificate:Password"] ?? "YourSecurePassword123!";

            if (File.Exists(certPath))
            {
                try
                {
                    var certificate = new X509Certificate2(certPath, certPassword);
                    Console.WriteLine("✅ Production certificate loaded");
                    listenOptions.UseHttps(certificate);
                }
                catch
                {
                    Console.WriteLine("⚠️ Using development certificate");
                    listenOptions.UseHttps();
                }
            }
            else
            {
                listenOptions.UseHttps();
            }
        }
        else
        {
            listenOptions.UseHttps();
            Console.WriteLine("✅ Development certificate configured");
        }
    });
});

// ============================================================
// 5. LOGGING
// ============================================================

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

Console.WriteLine("=== 🔍 LEAVE SERVICE DEBUG CONFIGURATION ===");
Console.WriteLine($"Environment: {environment}");
Console.WriteLine($"📡 Leave Service Port: {leavePort}");
Console.WriteLine($"📡 Auth URL: {authUrl}");
Console.WriteLine($"📡 Core HRMM URL: {corHrmmUrl}");
Console.WriteLine($"📡 HRM Pro URL: {hrmProUrl}");
Console.WriteLine($"📡 Core Module URL: {corModUrl}");

// ============================================================
// 6. DATABASE
// ============================================================

var dbConnectionString = GetConfig("ConnectionStrings:HRMLeaveDbCon", null);

if (string.IsNullOrEmpty(dbConnectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "root";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "HRM.Leave";

    dbConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Include Error Detail=true";
}

Console.WriteLine($"🔗 Database: {dbConnectionString}");



builder.Services.AddDbContextFactory<HrmLeaveDbContext>(options =>
options.UseNpgsql(dbConnectionString, npgsqlOptions =>
{
npgsqlOptions.EnableRetryOnFailure(
maxRetryCount: 5,
maxRetryDelay: TimeSpan.FromSeconds(30),
errorCodesToAdd: null);
npgsqlOptions.CommandTimeout(60);
})
.EnableDetailedErrors(environment == "Development")
.EnableSensitiveDataLogging(environment == "Development"));

// ✅ Register DbContext as scoped using the factory
builder.Services.AddScoped(sp =>
sp.GetRequiredService<IDbContextFactory<HrmLeaveDbContext>>().CreateDbContext());

// ✅ Remove the conflicting pool registrations
var poolDescriptor = builder.Services.FirstOrDefault(
d => d.ServiceType == typeof(Microsoft.EntityFrameworkCore.Internal.IDbContextPool<HrmLeaveDbContext>));
if (poolDescriptor != null)
{
builder.Services.Remove(poolDescriptor);
Console.WriteLine("✅ Removed DbContextPool registration");
}

// ============================================================
// 7. REDIS CACHING
// ============================================================

var redisConnectionString = GetConfig("Redis:ConnectionString", "localhost:6379,abortConnect=false");

builder.Services.AddMemoryCache();

try
{
    var redisConfig = ConfigurationOptions.Parse(redisConnectionString);
    redisConfig.ConnectTimeout = 3000;
    redisConfig.SyncTimeout = 3000;
    redisConfig.AbortOnConnectFail = false;
    redisConfig.ConnectRetry = 2;

    using var connection = ConnectionMultiplexer.Connect(redisConfig);
    if (connection.IsConnected)
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(connection);
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "Leave_";
        });
        builder.Services.AddSingleton<ICacheService, RedisCacheService>();
        Console.WriteLine("✅ Redis Cache ENABLED");
    }
    else
    {
        connection.Dispose();
        throw new Exception("Redis connection failed");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Redis connection failed: {ex.Message}. Using MemoryCache fallback.");
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
    Console.WriteLine("✅ MemoryCache ENABLED (Redis fallback)");
}

// ============================================================
// 8. SSL BYPASS HANDLER
// ============================================================

var sslHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
    MaxConnectionsPerServer = 50,
    AutomaticDecompression = DecompressionMethods.GZip
};



// ✅ Register the correct interfaces from Leave.App.Services

// Core Module API Client
builder.Services.AddHttpClient<Leave.App.Services.ICoreModuleApiService, Leave.App.Services.CoreModuleApiService>(client =>
{
    client.BaseAddress = new Uri(corModUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", coreApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "LeaveService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

// Core HRMM API Client
builder.Services.AddHttpClient<Leave.App.Services.ICoreHrmmApiService, Leave.App.Services.CoreHrmmApiService>(client =>
{
    client.BaseAddress = new Uri(corHrmmUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", hrmmApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "LeaveService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

// HRM Pro API Client
builder.Services.AddHttpClient<Leave.App.Services.IHrmProApiService,Leave.App.Services.HrmProApiService>(client =>
{
    client.BaseAddress = new Uri(hrmProUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", profileApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "LeaveService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(5));


// Auth API Client
builder.Services.AddHttpClient<IAuthApiService,AuthApiService>(client =>
{
    client.BaseAddress = new Uri(authUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", coreApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "LeaveService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(2));



// ============================================================
// 10. POLLY RETRY POLICIES
// ============================================================

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



builder.Services.AddHttpClient<IAuthApiService, AuthApiService>()
    .AddPolicyHandler(retryPolicy);

// ============================================================
// 11. RABBITMQ
// ============================================================

var rabbitMqHost = GetConfig("RabbitMQ:Host", "localhost");
var rabbitMqPort = int.Parse(GetConfig("RabbitMQ:Port", "5672"));
var rabbitMqUsername = GetConfig("RabbitMQ:Username", "guest");
var rabbitMqPassword = GetConfig("RabbitMQ:Password", "guest");

builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    return new ConnectionFactory
    {
        HostName = rabbitMqHost,
        Port = rabbitMqPort,
        UserName = rabbitMqUsername,
        Password = rabbitMqPassword,
        VirtualHost = GetConfig("RabbitMQ:VirtualHost", "/"),
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        DispatchConsumersAsync = true
    };
});

builder.Services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();
builder.Services.AddHostedService<EventConsumer>();
Console.WriteLine("✅ RabbitMQ ENABLED");

// ============================================================
// 12. JWT AUTHENTICATION
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            NameClaimType = "userName",
            RoleClaimType = "role",
        };
    });

builder.Services.AddAuthorization();

// ============================================================
// 13. REGISTER SERVICES
// ============================================================

// Dapper Helpers
builder.Services.AddScoped<IDbRetryHandler, DbRetryHandler>();
builder.Services.AddScoped<IDapperHelper, DapperHelper>();
builder.Services.AddScoped<ICoreDapperHelper, CoreDapperHelper>();
builder.Services.AddScoped<IHrmProDapperHelper, HrmProDapperHelper>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Business Services
builder.Services.AddScoped<IYearEndProcessingService, YearEndProcessingService>();
builder.Services.AddScoped<ILeaveLedgerService, LeaveLedgerService>();
builder.Services.AddScoped<ILvReqAppService, LvReqAppService>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<Helpers.IDbExceptionTranslator, Helpers.DbExceptionTranslator>();
builder.Services.AddScoped<IDbRetryHandler, DbRetryHandler>();
// Hosted Services
builder.Services.AddHostedService<InitialSyncService>();

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Leave.App.AppAssemblyMarker).Assembly);
});

// ============================================================
// 14. CORS
// ============================================================

var corsOrigins = GetConfig("Cors:AllowedOrigins", "http://localhost:5173,http://localhost:3000")
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(o => o.Trim())
    .ToArray();

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

// ============================================================
// 15. BUILDER EXTENSIONS
// ============================================================

builder.AddApiServices()
    .AddErrorHandling()
    .AddSwaggerService();

// ============================================================
// 16. BUILD APP
// ============================================================

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference(options => { options.WithTitle("HRM Leave API"); });
    app.ApplyMigration();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health Checks
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

Console.WriteLine($"\n✅ Leave Service starting on https://0.0.0.0:{leavePort}");
Console.WriteLine($"🔗 Auth URL: {authUrl}");
Console.WriteLine($"🔗 Core HRMM URL: {corHrmmUrl}");
Console.WriteLine($"🔗 HRM Pro URL: {hrmProUrl}");
Console.WriteLine($"🔗 Core Module URL: {corModUrl}");
Console.WriteLine("📊 Debug logging is ENABLED");
Console.WriteLine("\nPress Ctrl+C to stop");

await app.RunAsync();

// ============================================================
// 📁 INTERFACES AND SERVICES (Keep at bottom)
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

public interface ICoreHrmmApiService
{
    Task<HttpResponseMessage> GetAsync(string endpoint, CancellationToken ct = default);
}

public class CoreHrmmApiService : ICoreHrmmApiService
{
    private readonly HttpClient _httpClient;

    public CoreHrmmApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint, CancellationToken ct = default)
    {
        return await _httpClient.GetAsync(endpoint, ct);
    }
}

public interface IHrmProApiService
{
    Task<HttpResponseMessage> GetAsync(string endpoint, CancellationToken ct = default);
}

public class HrmProApiService : IHrmProApiService
{
    private readonly HttpClient _httpClient;

    public HrmProApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint, CancellationToken ct = default)
    {
        return await _httpClient.GetAsync(endpoint, ct);
    }
}

public interface ICoreModuleApiService
{
    Task<HttpResponseMessage> GetAsync(string endpoint, CancellationToken ct = default);
}

public class CoreModuleApiService : ICoreModuleApiService
{
    private readonly HttpClient _httpClient;

    public CoreModuleApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint, CancellationToken ct = default)
    {
        return await _httpClient.GetAsync(endpoint, ct);
    }
}
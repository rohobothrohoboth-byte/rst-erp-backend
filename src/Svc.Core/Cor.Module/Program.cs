using Cor.Module.gRPCService;
using Cor.Module.Middlewares;
using Cor.Module.HealthChecks;
using Scalar.AspNetCore;
using Serilog;
using Cor.Module.Services;
using RabbitMQ.Client;
using Shared.Helpers.Services;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Shared.Helpers.Extensions;
using System.Net;
using Shared.Helpers;
using Cor.Module.Authentication;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

/*// ✅ Configure Kestrel - FORCE PORT 80 inside container
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 80);  // ✅ Always port 80 inside container
    Console.WriteLine("✅ Kestrel listening on HTTP port 80");
});   // DISABLED*/

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

// ============= SERVICE URLS - FIXED =============
// ✅ USE HTTP AND DOCKER SERVICE NAMES
var CorHrmmUrl = "http://core-hrmm";
var authUrl = "http://auth";
var hrmProUrl = "http://hrm-profile";
var financeApiUrl = "http://finance";
var gatewayApiUrl = "http://gateway";

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

// ✅ DEBUG: Print configuration
Console.WriteLine("=== 🔍 CORE MODULE SERVICE DEBUG CONFIGURATION ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"📡 Core HRMM URL: {CorHrmmUrl}");
Console.WriteLine($"📡 Auth URL: {authUrl}");
Console.WriteLine($"📡 HRM Pro URL: {hrmProUrl}");

// ============= DATABASE =============
var dbConnectionString = GetConfig("ConnectionStrings:CorModuleDbCon", null);

if (string.IsNullOrEmpty(dbConnectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "postgres";  // ✅ FIXED: Use "postgres" instead of "localhost"
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "root";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "core.Module";

    dbConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Include Error Detail=true";
}

// ============= REDIS CACHING =============
var redisConnectionString = GetConfig("Redis:ConnectionString", null);

if (string.IsNullOrEmpty(redisConnectionString))
{
    var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "redis";  // ✅ FIXED: Use "redis" instead of "localhost"
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

// ✅ Try multiple connection attempts
var redisAvailable = false;
var workingConnectionString = redisConnectionString;

var connectionAttempts = new List<string>
{
    redisConnectionString,
    "redis:6379,abortConnect=false",
    "localhost:6379,abortConnect=false",
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
        await Task.Delay(200);

        redisAvailable = connection.IsConnected;

        if (redisAvailable)
        {
            workingConnectionString = attempt;
            Console.WriteLine($"✅ Redis connection SUCCESSFUL");

            try
            {
                var db = connection.GetDatabase();
                var pingResult = db.Ping();
                Console.WriteLine($"✅ Redis PING response: {pingResult.TotalMilliseconds}ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Redis PING failed: {ex.Message}");
            }
            break;
        }
        else
        {
            Console.WriteLine($"❌ Redis connection FAILED - IsConnected: false");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Redis connection FAILED: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
        }
        redisAvailable = false;
    }
}

// ============= REGISTER CACHE SERVICES =============

// ✅ ALWAYS register IMemoryCache
builder.Services.AddMemoryCache();

if (redisAvailable)
{
    // ✅ REGISTER IConnectionMultiplexer FIRST
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
        options.InstanceName = "Cor.Module";
    });
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();
    Console.WriteLine("✅ Redis Cache ENABLED");
}
else
{
    // ✅ REQUIRED: Register IDistributedCache fallback
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
    Console.WriteLine("✅ MemoryCache ENABLED (Redis fallback)");
}

// ============= HTTP CLIENT HANDLER - FIXED =============
// ✅ Use HTTP handler WITHOUT SSL bypass
var httpHandler = new HttpClientHandler
{
    MaxConnectionsPerServer = 50,
    AutomaticDecompression = DecompressionMethods.GZip
};

// ============= REGISTER HTTP CLIENTS =============

// ✅ Auth API Client - FIXED
builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(authUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", coreApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "CoreModuleService");
})
.ConfigurePrimaryHttpMessageHandler(() => httpHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(2));

// ============= RABBITMQ REGISTRATION - FIXED =============
// ✅ FIXED: Use double underscore for environment variables
var rabbitMqHost = GetConfig("RabbitMQ__Host", "rabbitmq");  // ✅ Changed to double underscore
var rabbitMqPort = int.Parse(GetConfig("RabbitMQ__Port", "5672"));  // ✅ Changed to double underscore
var rabbitMqUsername = GetConfig("RabbitMQ__Username", "guest");  // ✅ Changed to double underscore
var rabbitMqPassword = GetConfig("RabbitMQ__Password", "guest");  // ✅ Changed to double underscore

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

// ============= HEALTH CHECKS - FIXED =============
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri($"{CorHrmmUrl}/health"), "Core HRMM API")
    .AddUrlGroup(new Uri($"{authUrl}/health"), "Auth API")
    .AddUrlGroup(new Uri($"{hrmProUrl}/health"), "HRM Pro API");

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

// ============= API KEY AUTHENTICATION & AUTHORIZATION =============

// 1. Configuration
builder.Services.Configure<ApiKeySettings>(builder.Configuration.GetSection("ApiKey"));
builder.Services.Configure<ApiKeyRateLimitOptions>(builder.Configuration.GetSection("ApiKeyRateLimit"));

// 2. Services - ሁሉም Scoped
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<IExternalSystemService, ExternalSystemService>();

// 3. Authorization Components - ሁሉም Scoped
builder.Services.AddScoped<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// 4. Authentication - support BOTH browser JWTs AND internal API keys
var jwtSecret = Common.JwtCons.SecretKey;
if (!string.IsNullOrEmpty(builder.Configuration["Jwt:SecretKey"]))
    jwtSecret = builder.Configuration["Jwt:SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "JWT_OR_APIKEY";
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddPolicyScheme("JWT_OR_APIKEY", "JWT or ApiKey", options =>
{
    options.ForwardDefaultSelector = context =>
        context.Request.Headers.ContainsKey("X-API-Key")
            ? "ApiKey"
            : JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = Common.JwtCons.Issuer,
        ValidateAudience = true,
        ValidAudience = Common.JwtCons.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
})
.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

// 5. Authorization
builder.Services.AddAuthorization();

// ============= BUILDER EXTENSIONS =============
builder.AddApiServices()
    .AddErrorHandling()
    .AddSwaggerService();

var app = builder.Build();

// ============= MIDDLEWARE ORDER =============
app.UseMiddleware<ApiKeyRateLimiterMiddleware>();
app.MapDefaultEndpoints();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
// app.UseHttpsRedirection(); // ✅ DISABLED FOR DOCKER
app.UseRateLimiter();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<CorModListService>();

if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference(options => { options.WithTitle("Core Module API"); });
    app.ApplyMigration();
}

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

Console.WriteLine($"\n✅ Core Module Service starting on http://0.0.0.0:80");
Console.WriteLine($"🔗 Core HRMM URL: {CorHrmmUrl}");
Console.WriteLine($"🔗 Auth URL: {authUrl}");
Console.WriteLine($"🔗 HRM Pro URL: {hrmProUrl}");
Console.WriteLine("📊 Debug logging is ENABLED");
Console.WriteLine("\nPress Ctrl+C to stop");

await app.RunAsync();

// ============================================================
// 📁 INTERFACE AND SERVICE FOR AUTH API
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



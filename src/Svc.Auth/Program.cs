using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;
using Polly.Extensions.Http;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using Serilog;
using Svc.Auth.Middlewares;
using Svc.Auth.Services;
using System;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Svc.Auth.Persistence;
using Shared.Helpers.Services;
using Svc.Auth.HealthChecks;
using StackExchange.Redis;
using Svc.Auth.Commands;
using System.Net;
using Shared.Helpers;

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
var authPortString = builder.Configuration["ServiceUrls:AuthApi"] ?? "https://localhost:7000";
var authPort = new Uri(authPortString).Port;
Console.WriteLine($"📡 Auth Service Port: {authPort}");

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, authPort, listenOptions => listenOptions.UseHttps());
});

// --- Logging ---
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

// ✅ DEBUG: Print configuration
Console.WriteLine("=== 🔍 AUTH SERVICE DEBUG CONFIGURATION ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

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
/*var CorModUrl = GetConfig("ServiceUrls:CoreModuleApi", "https://localhost:7002");
var CorHrmmUrl = GetConfig("ServiceUrls:CoreHRMMApi", "https://localhost:7001");
var hrmProUrl = GetConfig("ServiceUrls:HrmProApi", "https://localhost:7004");
var financeApiUrl = GetConfig("ServiceUrls:FinanceApi", "https://localhost:7008");
var gatewayApiUrl = GetConfig("ServiceUrls:GatewayApi", "https://localhost:5000");*/
var CorModUrl = "https://192.168.1.7:7002";
var CorHrmmUrl = "https://192.168.1.7:7001";
var hrmProUrl = "https://192.168.1.7:7004";
var financeApiUrl = "https://192.168.1.7:7008";
var gatewayApiUrl = "https://192.168.1.7:5000";

// ============= API KEYS =============
var coreApiKey = GetConfig("ApiKeys:CoreModule", "core_module_secret_key_2024");
var hrmmApiKey = GetConfig("ApiKeys:CoreHRMM", "core_module_secret_key_2024");
var profileApiKey = GetConfig("ApiKeys:ProfileModule", "profile_module_secret_key_2024");

Console.WriteLine($"📡 Core Module URL: {CorModUrl}");
Console.WriteLine($"📡 Core HRMM URL: {CorHrmmUrl}");
Console.WriteLine($"📡 HRM Pro URL: {hrmProUrl}");

// Get connection strings
var connectionString = builder.Configuration.GetConnectionString("authMgrCon")
    ?? throw new InvalidOperationException("authMgrCon connection string not found");

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
        options.InstanceName = "Auth_";
    });
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();
    Console.WriteLine("✅ Redis Cache ENABLED");
}
else
{
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
    Console.WriteLine("✅ MemoryCache ENABLED (Redis fallback)");
}

// ============= SSL BYPASS HANDLER =============
var sslHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
    MaxConnectionsPerServer = 50,
    AutomaticDecompression = DecompressionMethods.GZip
};

// ============= HTTP CLIENTS WITH API KEYS =============

// Core Module API
builder.Services.AddHttpClient<ICoreModuleApiService, CoreModuleApiService>(client =>
{
    client.BaseAddress = new Uri(CorModUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", coreApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "AuthService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(2));

// Core HRMM API
builder.Services.AddHttpClient<ICoreHrmmApiService, CoreHrmmApiService>(client =>
{
    client.BaseAddress = new Uri(CorHrmmUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", hrmmApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "AuthService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(2));

// HRM Pro API
builder.Services.AddHttpClient<IHrmProApiService, HrmProApiService>(client =>
{
    client.BaseAddress = new Uri(hrmProUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", profileApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "AuthService");
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

// Apply policies to all HTTP clients
builder.Services.AddHttpClient<ICoreModuleApiService, CoreModuleApiService>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<ICoreHrmmApiService, CoreHrmmApiService>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<IHrmProApiService, HrmProApiService>()
    .AddPolicyHandler(retryPolicy);

// ============= HEALTH CHECKS =============
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "Database")
    .AddUrlGroup(new Uri($"{CorModUrl}/health"), "Core Module API")
    .AddUrlGroup(new Uri($"{CorHrmmUrl}/health"), "Core HRMM API")
    .AddUrlGroup(new Uri($"{hrmProUrl}/health"), "HRM Pro API")
    .AddCheck<SyncHealthCheck>("Sync Status");

// ============= RABBITMQ =============
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var host = GetConfig("RabbitMQ:Host", "localhost");
    var port = int.Parse(GetConfig("RabbitMQ:Port", "5672"));
    var username = GetConfig("RabbitMQ:Username", "guest");
    var password = GetConfig("RabbitMQ:Password", "guest");

    Console.WriteLine($"🔄 Connecting to RabbitMQ at {host}:{port}");

    return new ConnectionFactory
    {
        HostName = host,
        Port = port,
        UserName = username,
        Password = password,
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        RequestedHeartbeat = TimeSpan.FromSeconds(30)
    };
});

// ✅ EventConsumer
bool disableEventConsumer = Environment.GetEnvironmentVariable("DisableEventConsumer") == "true";
if (!disableEventConsumer)
{
    builder.Services.AddHostedService<EventConsumer>();
    Console.WriteLine("✅ EventConsumer ENABLED");
}
else
{
    Console.WriteLine("⚠️ EventConsumer DISABLED");
}

// ============= REGISTER SERVICES =============
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddHostedService<InitialSyncService>();
builder.Services.AddScoped<ISetupService, SetupService>();
builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddScoped<IAlertService, AlertService>();

// ============= CUSTOM SERVICES =============
builder.AddApiServices()
    .AddErrorHandling()
    .AddSwaggerService()
    .AddAuthService();

// ================================================================
// ✅ CORS CONFIGURATION - Read from appsettings.json
// ================================================================

// ✅ Read allowed origins from appsettings.json and resolve ServiceHost
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

if (allowedOrigins == null || allowedOrigins.Length == 0)
{
    allowedOrigins = new[]
    {
        $"http://{serviceHost}:1211",
        $"https://{serviceHost}:1211",
        "http://localhost:3000",
        "https://localhost:3000"
    };
}

// ✅ Resolve {ServiceHost} in CORS origins
var resolvedOrigins = allowedOrigins
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
        }
        else
        {
            policy.WithOrigins(resolvedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// ============= BUILD APP =============
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapGrpcService<AuthValidatorService>();

// ✅ CORS must be BEFORE Authentication and Authorization
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference(options => { options.WithTitle("Auth Manager API"); });

    // Apply migrations and seed data
    app.ApplyMigration();

    try
    {
        await app.ApplyAdminRole();
        Console.WriteLine("✅ Admin role seeded");

        await app.SeedPerModule();
        Console.WriteLine("✅ Modules seeded");

        await app.SeedPerMenu();
        Console.WriteLine("✅ Menus seeded");

        await app.SeedPerAccess();
        Console.WriteLine("✅ APIs seeded");

        await app.InitializePermissionRegistry();
        Console.WriteLine("✅ Permission registry (static) verified");

        Console.WriteLine("🎉 All seeding completed successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Seeding failed: {ex.Message}");
        throw;
    }
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

Console.WriteLine($"\n✅ Auth Service starting on https://0.0.0.0:{authPort}");
Console.WriteLine("🔑 JWT Issuer: RST_ERP.Svc.Auth");
Console.WriteLine("🔑 JWT Audience: RST_ERP");
Console.WriteLine("📊 Debug logging is ENABLED");
Console.WriteLine("\nPress Ctrl+C to stop");

await app.RunAsync();
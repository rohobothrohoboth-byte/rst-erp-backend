using Profile.API.gRPCService;
using Profile.API.Middlewares;
using Scalar.AspNetCore;
using Serilog;
using Profile.App.Queries;
using Profile.App.Services;
using Common;
using RabbitMQ.Client;
using Shared.Helpers.Services;
using Polly;
using MediatR;
using Polly.Extensions.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using Profile.App.HealthChecks;
using HealthChecks.Uris;
using Shared.Helpers.Extensions;
using StackExchange.Redis;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Shared.Helpers;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// ✅ CONFIGURATION LOADING ORDER
// ============================================================

// ✅ Load shared configuration
// ✅ CORRECT PATH - Goes up 3 levels to reach src/Shared/Helpers/
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
var hrmProPortString = builder.Configuration["ServiceUrls:HrmProApi"] ?? "https://localhost:7004";
var hrmProPort = new Uri(hrmProPortString).Port;
Console.WriteLine($"📡 HRM Pro Service Port: {hrmProPort}");

// ✅ KESTREL CONFIGURATION
void ConfigureKestrel(WebApplicationBuilder b)
{
    var isProduction = environment == "Production";

    Console.WriteLine($"🔧 Configuring Kestrel for {environment} environment...");

    b.WebHost.ConfigureKestrel(options =>
    {
        if (isProduction)
        {
            var certPath = b.Configuration["Certificate:Path"] ?? "Certificates/prod-certificate.pfx";
            var certPassword = b.Configuration["Certificate:Password"] ?? "YourSecurePassword123!";

            if (File.Exists(certPath))
            {
                try
                {
                    var certificate = new X509Certificate2(certPath, certPassword);
                    Console.WriteLine("✅ Production certificate loaded");
                    options.Listen(IPAddress.Any, hrmProPort, listenOptions =>
                    {
                        listenOptions.UseHttps(certificate);
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error loading certificate: {ex.Message}");
                    Console.WriteLine("⚠️ Falling back to development certificate");
                    options.Listen(IPAddress.Any, hrmProPort, listenOptions =>
                    {
                        listenOptions.UseHttps();
                    });
                }
            }
            else
            {
                Console.WriteLine("⚠️ Production certificate not found, using development certificate");
                options.Listen(IPAddress.Any, hrmProPort, listenOptions =>
                {
                    listenOptions.UseHttps();
                });
            }
        }
        else
        {
            Console.WriteLine("🔧 Using DEVELOPMENT certificate...");
            options.Listen(IPAddress.Any, hrmProPort, listenOptions =>
            {
                listenOptions.UseHttps();
            });
            Console.WriteLine("✅ Development certificate configured");
        }
    });
}

// ✅ Call the configuration
ConfigureKestrel(builder);

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
var corModUrl = GetConfig("ServiceUrls:CoreModuleApi", "https://localhost:7002");
var corHrmmUrl = GetConfig("ServiceUrls:CoreHRMMApi", "https://localhost:7001");
var hrmProUrl = GetConfig("ServiceUrls:HrmProApi", "https://localhost:7004");
var authUrl = GetConfig("ServiceUrls:AuthApi", "https://localhost:7000");
var financeApiUrl = GetConfig("ServiceUrls:FinanceApi", "https://localhost:7008");
var gatewayApiUrl = GetConfig("ServiceUrls:GatewayApi", "https://localhost:5000");

// API Keys
var coreApiKey = GetConfig("ApiKeys:CoreModule", "core_module_secret_key_2024");
var hrmmApiKey = GetConfig("ApiKeys:CoreHRMM", "core_module_secret_key_2024");
var profileApiKey = GetConfig("ApiKeys:ProfileModule", "profile_module_secret_key_2024");

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();

// ✅ DEBUG: Print configuration
Console.WriteLine("=== 🔍 HRM PRO SERVICE DEBUG CONFIGURATION ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"📡 Core HRMM URL: {corHrmmUrl}");
Console.WriteLine($"📡 Auth URL: {authUrl}");

// ============= DATABASE =============
// ============= DATABASE CONNECTION WITH POOLING =============
var dbConnectionString = GetConfig("ConnectionStrings:HRMProDbCon", null);

if (string.IsNullOrEmpty(dbConnectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "root";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "HRM.Pro";

    dbConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};" +
        $"Include Error Detail=true;Maximum Pool Size=50;Minimum Pool Size=5;Connection Idle Lifetime=300;Connection Pruning Interval=60;";
}

// ============= MEDIATR =============
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(EmployeePaginatedQry).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(EmployeeFilterOptionsQry).Assembly);
});

// ============= REDIS WITH PROPER DISPOSAL =============
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICachedReferenceService, CachedReferenceService>();

var redisConnectionString = builder.Configuration.GetConnectionString("redis")
    ?? GetConfig("Redis:ConnectionString", null)
    ?? $"{(Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost")}:{(Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379")},abortConnect=false";

Console.WriteLine($"🔗 Redis Connection: {redisConnectionString}");

try
{
    if (!string.IsNullOrEmpty(redisConnectionString))
    {
        var redisConfig = ConfigurationOptions.Parse(redisConnectionString);
        redisConfig.ConnectTimeout = 3000;
        redisConfig.SyncTimeout = 3000;
        redisConfig.AbortOnConnectFail = false;
        redisConfig.ConnectRetry = 2;
        redisConfig.DefaultDatabase = 0;

        // ✅ Use Lazy<ConnectionMultiplexer> to ensure lazy initialization
        var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(redisConfig));

        builder.Services.AddSingleton<Lazy<ConnectionMultiplexer>>(lazyConnection);
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp => lazyConnection.Value);
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "Profile_";
        });
        builder.Services.AddSingleton<ICacheService, RedisCacheService>();
        Console.WriteLine("✅ Redis Cache ENABLED");
    }
    else
    {
        throw new Exception("Redis connection string is empty");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Redis connection failed: {ex.Message}. Using MemoryCache fallback.");
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
    Console.WriteLine("✅ MemoryCache ENABLED (Redis fallback)");
}

// ============= MODIFIED CACHE WARM-UP WITH RETRY AND DELAYS =============


// ============= SSL BYPASS HANDLER =============
var sslHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
    MaxConnectionsPerServer = 50,
    AutomaticDecompression = DecompressionMethods.GZip
};

// ============= REGISTER HTTP CLIENTS =============

// ✅ Core Module API Client
builder.Services.AddHttpClient<ICoreModuleApiService, CoreModuleApiService>(client =>
{
    client.BaseAddress = new Uri(corModUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", coreApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "HRMProService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

// ✅ Core HRMM API Client
builder.Services.AddHttpClient<ICoreHrmmApiService, CoreHrmmApiService>(client =>
{
    client.BaseAddress = new Uri(corHrmmUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", hrmmApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "HRMProService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

// ✅ Auth API Client
builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(authUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", coreApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "HRMProService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

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

builder.Services.AddHttpClient<ICoreModuleApiService, CoreModuleApiService>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<ICoreHrmmApiService, CoreHrmmApiService>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<IAuthApiService, AuthApiService>()
    .AddPolicyHandler(retryPolicy);

 // ============= RABBITMQ REGISTRATION =============

var rabbitMqHost = GetConfig("RabbitMQ:Host", "localhost");
var rabbitMqPort = int.Parse(GetConfig("RabbitMQ:Port", "5672"));
var rabbitMqUsername = GetConfig("RabbitMQ:Username", "guest");
var rabbitMqPassword = GetConfig("RabbitMQ:Password", "guest");
var rabbitMqVirtualHost = GetConfig("RabbitMQ:VirtualHost", "/");

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
         builder.Services.AddHostedService<EventConsumer>();
         Console.WriteLine("✅ RabbitMQ ENABLED");


// ============================================
// REGISTER gRPC CLIENTS
// ============================================
builder.Services.AddScoped<IHrmProfileClient>(sp =>
{
    var logger = sp.GetService<ILogger<HrmProfileClient>>();
    return new HrmProfileClient(hrmProUrl, logger);
});


builder.Services.AddScoped<ICorModClient, CorModClient>();
builder.Services.AddScoped<ICorHrmmClient>(sp =>
{
    var logger = sp.GetService<ILogger<CorHrmmClient>>();
    return new CorHrmmClient(corHrmmUrl, logger);
});

// ============================================
// REGISTER SERVICES
// ============================================
builder.Services.AddScoped<IUserScopeService, UserScopeService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddHostedService<InitialSyncService>();


// ============= HEALTH CHECKS =============
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri($"{corModUrl}/health"), "Core Module API")
    .AddUrlGroup(new Uri($"{corHrmmUrl}/health"), "Core HRMM API")
    .AddUrlGroup(new Uri($"{authUrl}/health"), "Auth API");

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

// ============================================
// ✅ JWT AUTHENTICATION
// ============================================
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                Console.WriteLine($"JWT Challenge: {context.Error}");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// ============================================
// BUILDER EXTENSIONS
// ============================================
builder.AddApiServices()
    .AddErrorHandling()
    .AddSwaggerService();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapGrpcService<HrmProService>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference(options => { options.WithTitle("HRM Profile API"); });
    app.ApplyMigration();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
_ = Task.Run(async () =>
{
    try
    {
        // ✅ Wait for the application to fully start
        await Task.Delay(5000);

        using var scope = app.Services.CreateScope();
        var referenceService = scope.ServiceProvider.GetRequiredService<ICachedReferenceService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("🔥 Warming up cache...");

        // ✅ Warm up with delays between operations to allow connection pool to recover
        await referenceService.GetReferenceDataAsync(CancellationToken.None);
        logger.LogInformation("✅ Reference data cached");

        await Task.Delay(500);

        var statsQry = new EmployeeStatsQry();
        await mediator.Send(statsQry);
        logger.LogInformation("✅ Stats cached");

        await Task.Delay(500);

        var paginatedQry = new EmployeePaginatedQry { PageNumber = 1, PageSize = 10 };
        await mediator.Send(paginatedQry);
        logger.LogInformation("✅ First page employees cached");

        await Task.Delay(500);

        var filterQry = new EmployeeFilterOptionsQry();
        await mediator.Send(filterQry);
        logger.LogInformation("✅ Filter options cached");

        logger.LogInformation("✅ Cache warm-up completed successfully!");
    }
    catch (Exception ex)
    {
        // ✅ Don't let cache warm-up failure crash the app
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Cache warm-up failed: {Error}. Service will continue starting.", ex.Message);
    }
});
Console.WriteLine($"\n✅ HRM Profile Service starting on https://0.0.0.0:{hrmProPort}");
Console.WriteLine($"🔗 Auth URL: {authUrl}");
Console.WriteLine($"🔗 Core Module URL: {corModUrl}");
Console.WriteLine($"🔗 Core HRMM URL: {corHrmmUrl}");
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

public class CacheWarmupService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CacheWarmupService> _logger;

    public CacheWarmupService(IServiceProvider serviceProvider, ILogger<CacheWarmupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task WarmUpAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var referenceService = scope.ServiceProvider.GetRequiredService<ICachedReferenceService>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            _logger.LogInformation("🔥 Warming up cache...");

            // Warm up reference data
            await referenceService.GetReferenceDataAsync(CancellationToken.None);
            _logger.LogInformation("✅ Reference data cached");

            // Warm up stats
            var statsQry = new EmployeeStatsQry();
            await mediator.Send(statsQry);
            _logger.LogInformation("✅ Stats cached");

            // Warm up first page of employees
            var paginatedQry = new EmployeePaginatedQry { PageNumber = 1, PageSize = 10 };
            await mediator.Send(paginatedQry);
            _logger.LogInformation("✅ First page employees cached");

            // Warm up filter options
            var filterQry = new EmployeeFilterOptionsQry();
            await mediator.Send(filterQry);
            _logger.LogInformation("✅ Filter options cached");

            _logger.LogInformation("✅ Cache warm-up completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Cache warm-up failed: {Error}", ex.Message);
        }
    }
}


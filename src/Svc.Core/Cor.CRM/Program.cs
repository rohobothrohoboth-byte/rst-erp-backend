using Cor.CRM.Persistence;
using Cor.CRM.Services;
using Cor.CRM.gRPCService;
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
using Common;
using RabbitMQ.Client;
using StackExchange.Redis;
using Cor.CRM.HealthChecks;
using Cor.CRM.Extensions;
using Cor.CRM.Interfaces;
using Cor.CRM.Repos;
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
var crmPortString = builder.Configuration["ServiceUrls:CRMApi"] ?? "https://localhost:7012";
var crmPort = new Uri(crmPortString).Port;
Console.WriteLine($"📡 CRM Service Port: {crmPort}");

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, crmPort, listenOptions =>
    {
        listenOptions.UseHttps();
    });
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
var coreModuleUrl = GetConfig("ServiceUrls:CoreModuleApi", "https://localhost:7002");
var CorHrmmUrl = GetConfig("ServiceUrls:CoreHRMMApi", "https://localhost:7001");
var authUrl = GetConfig("ServiceUrls:AuthApi", "https://localhost:7000");
var hrmProUrl = GetConfig("ServiceUrls:HrmProApi", "https://localhost:7004");
var financeApiUrl = GetConfig("ServiceUrls:FinanceApi", "https://localhost:7008");
var gatewayApiUrl = GetConfig("ServiceUrls:GatewayApi", "https://localhost:5000");

// ============= API KEYS =============
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

Console.WriteLine("=== 🔍 CRM SERVICE DEBUG CONFIGURATION ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"📡 Core Module URL: {coreModuleUrl}");
Console.WriteLine($"📡 Core HRMM URL: {CorHrmmUrl}");
Console.WriteLine($"📡 Auth URL: {authUrl}");
Console.WriteLine($"📡 HRM Pro URL: {hrmProUrl}");

// ============= ADD SERVICES =============
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGrpc();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// Database
var dbConnectionString = GetConfig("ConnectionStrings:coreCRMDbCon", null);

if (string.IsNullOrEmpty(dbConnectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "root";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "core.CRMDb";

    dbConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Include Error Detail=true";
}

builder.Services.AddDbContext<CrmDbContext>(options =>
{
    options.UseNpgsql(dbConnectionString);
});

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
        await Task.Delay(200);

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

// Register Cache Services
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
        options.InstanceName = "CRM_";
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

// ============= RABBITMQ =============
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
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        RequestedHeartbeat = TimeSpan.FromSeconds(30),
        ContinuationTimeout = TimeSpan.FromSeconds(20)
    };
});

builder.Services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CrmDbContext>()
    .AddCheck<RedisHealthCheck>("Redis");

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
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CRMAdmin", policy =>
        policy.RequireRole("Admin", "CRMManager"));

    options.AddPolicy("CRMManager", policy =>
        policy.RequireRole("Admin", "CRMManager"));

    options.AddPolicy("CRMView", policy =>
        policy.RequireRole("Admin", "CRMManager", "SalesRep", "Support"));

    options.AddPolicy("CRMCreate", policy =>
        policy.RequireRole("Admin", "CRMManager", "SalesRep"));

    options.AddPolicy("CRMUpdate", policy =>
        policy.RequireRole("Admin", "CRMManager", "SalesRep"));

    options.AddPolicy("CRMDelete", policy =>
        policy.RequireRole("Admin", "CRMManager"));
});

// ============= REGISTER CRM SERVICES =============
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDapperHelper, DapperHelper>();
builder.Services.AddScoped<IDbRetryHandler, DbRetryHandler>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();

// HTTP Clients with resolved URLs
builder.Services.AddHttpClient<IUserClient, UserClient>(client =>
{
    client.BaseAddress = new Uri(authUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    return handler;
});

builder.Services.AddHttpClient<ICoreModuleApiService, CoreModuleApiService>(client =>
{
    client.BaseAddress = new Uri(coreModuleUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", coreApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "CRMService");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    return handler;
});

builder.Services.AddHttpClient<ICoreHrmmApiService, CoreHrmmApiService>(client =>
{
    client.BaseAddress = new Uri(CorHrmmUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", hrmmApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "CRMService");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    return handler;
});

builder.Services.AddHttpClient<IHrmProApiService, HrmProApiService>(client =>
{
    client.BaseAddress = new Uri(hrmProUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", profileApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "CRMService");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    return handler;
});

// Register LeadScoringService
builder.Services.AddScoped<ILeadScoringService>(sp =>
    new LeadScoringService(
        sp.GetRequiredService<IServiceScopeFactory>(),
        sp.GetRequiredService<ILogService>()
    )
);

// Background Services
builder.Services.AddHostedService<InitialSyncService>();
builder.Services.AddHostedService<EventConsumer>();
builder.Services.AddScoped<ISyncService, SyncService>();

builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // 1MB
    options.UseCaseSensitivePaths = false;
});

// ============= BUILD APP =============
var app = builder.Build();

// ============= DATABASE MIGRATION =============
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
    try
    {
        Log.Information("📦 Checking database migration...");
        await dbContext.Database.MigrateAsync();
        Log.Information("✅ Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ An error occurred while migrating the database.");
    }
}

// ============= PIPELINE =============
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("CRM API")
               .WithTheme(ScalarTheme.BluePlanet);
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();
app.MapControllers();
app.MapGrpcService<CrmGrpcService>();
app.MapHealthChecks("/health");

Console.WriteLine($"\n✅ CRM Service starting on https://0.0.0.0:{crmPort}");
Console.WriteLine($"🔗 Auth URL: {authUrl}");
Console.WriteLine($"🔗 Core Module URL: {coreModuleUrl}");
Console.WriteLine($"🔗 Core HRMM URL: {CorHrmmUrl}");
Console.WriteLine($"🔗 HRM Pro URL: {hrmProUrl}");
Console.WriteLine("\nPress Ctrl+C to stop");

await app.RunAsync();
using Svc.HRM.Payroll.Persistence;
using Svc.HRM.Payroll.Services;
using Svc.HRM.Payroll.Extensions;
using Svc.HRM.Payroll.Consumers;
using Svc.HRM.Payroll.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers.Services;
using Common;
using Shared.Helpers;
using Contracts;
using Scalar.AspNetCore;
using Serilog;
using RabbitMQ.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using Shared.Helpers.Config;
using StackExchange.Redis;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
var payrollPortString = builder.Configuration["ServiceUrls:PayrollApi"] ?? "https://localhost:7010";
var payrollPort = new Uri(payrollPortString).Port;
Console.WriteLine($"📡 Payroll Service Port: {payrollPort}");

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, payrollPort, listenOptions =>
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
var CorModUrl = GetConfig("ServiceUrls:CoreModuleApi", "https://localhost:7002");
var coreHrmmUrl = GetConfig("ServiceUrls:CoreHRMMApi", "https://localhost:7001");
var authUrl = GetConfig("ServiceUrls:AuthApi", "https://localhost:7000");
var hrmProUrl = GetConfig("ServiceUrls:HrmProApi", "https://localhost:7004");
var financeApiUrl = GetConfig("ServiceUrls:FinanceApi", "https://localhost:7008");
var gatewayApiUrl = GetConfig("ServiceUrls:GatewayApi", "https://localhost:5000");

// API Keys
var coreApiKey = GetConfig("ApiKeys:CoreModule", "core_module_secret_key_2024");
var hrmmApiKey = GetConfig("ApiKeys:CoreHRMM", "core_module_secret_key_2024");
var profileApiKey = GetConfig("ApiKeys:ProfileModule", "profile_module_secret_key_2024");

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
Console.WriteLine("=== 🔍 PAYROLL SERVICE DEBUG CONFIGURATION ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"📡 Core Module URL: {CorModUrl}");
Console.WriteLine($"📡 Core HRMM URL: {coreHrmmUrl}");
Console.WriteLine($"📡 Auth URL: {authUrl}");
Console.WriteLine($"📡 HRM Pro URL: {hrmProUrl}");

// ============= DATABASE =============
var dbConnectionString = GetConfig("ConnectionStrings:HrmPayrollDb", null);

if (string.IsNullOrEmpty(dbConnectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "root";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "HRM.PayrollDb";

    dbConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Include Error Detail=true";
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Add Database Context
builder.Services.AddDbContext<PayrollDbContext>(options =>
{
    options.UseNpgsql(dbConnectionString);
});

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// ================================================================
// ✅ JWT AUTHENTICATION
// ================================================================
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

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<PayrollHealthCheck>("payroll_health");

// ============ REDIS CACHING =============
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

// ============ REGISTER CACHE SERVICES =============

// ✅ ALWAYS register IMemoryCache
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
        options.InstanceName = "Payroll_";
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

// ============ RABBITMQ REGISTRATION =============
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

// ============ BUSINESS SERVICES =============
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<ITaxCalculator, TaxCalculator>();
builder.Services.AddScoped<IPayslipGenerator, PayslipGenerator>();
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddScoped<IPayrollProcessor, PayrollProcessor>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// ============ HOSTED SERVICES =============
builder.Services.AddHostedService<EmployeeEventConsumer>();

// ============ API CLIENTS =============

// SSL Bypass Handler
var sslHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
    MaxConnectionsPerServer = 50,
    AutomaticDecompression = DecompressionMethods.GZip
};

// Finance Client for gRPC
builder.Services.AddSingleton<IFinanceClient, FinanceClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetService<ILogger<FinanceClient>>();
    return new FinanceClient(config, logger);
});

// Employee Service Client
builder.Services.AddHttpClient<IEmployeeService, EmployeeService>(client =>
{
    client.BaseAddress = new Uri(hrmProUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", profileApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "PayrollService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(2));

// Core Module API Client
builder.Services.AddHttpClient<ICoreModuleApiService, CoreModuleApiService>(client =>
{
    client.BaseAddress = new Uri(CorModUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", coreApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "PayrollService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(2));

// Core HRMM API Client
builder.Services.AddHttpClient<ICoreHrmmApiService, CoreHrmmApiService>(client =>
{
    client.BaseAddress = new Uri(coreHrmmUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", hrmmApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "PayrollService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(2));

// HRM Pro API Client
builder.Services.AddHttpClient<IHrmProApiService, HrmProApiService>(client =>
{
    client.BaseAddress = new Uri(hrmProUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-API-Key", profileApiKey);
    client.DefaultRequestHeaders.Add("X-Service-Name", "PayrollService");
})
.ConfigurePrimaryHttpMessageHandler(() => sslHandler)
.SetHandlerLifetime(TimeSpan.FromMinutes(2));

// RabbitMQ Config
builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));

// ============ BUILD APP =============
var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference(options => { options.WithTitle("HRM Payroll API"); });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Apply migrations
await app.ApplyMigrationsAsync();

Console.WriteLine($"\n✅ Payroll Service starting on https://0.0.0.0:{payrollPort}");
Console.WriteLine($"🔗 Auth URL: {authUrl}");
Console.WriteLine($"🔗 Core Module URL: {CorModUrl}");
Console.WriteLine($"🔗 Core HRMM URL: {coreHrmmUrl}");
Console.WriteLine($"🔗 HRM Pro URL: {hrmProUrl}");
Console.WriteLine("📊 Debug logging is ENABLED");
Console.WriteLine("\nPress Ctrl+C to stop");

await app.RunAsync();
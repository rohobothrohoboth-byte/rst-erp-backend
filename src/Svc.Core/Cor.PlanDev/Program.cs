using Common;
using System.Text.Json;
using Cor.PlanDev.Persistence;
using Cor.PlanDev.Services;
using Cor.PlanDev.Middlewares;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers.Services;
using Serilog;
using StackExchange.Redis;
using Asp.Versioning;
using RabbitMQ.Client;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Cor.PlanDev.HealthChecks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

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
var planDevPortString = builder.Configuration["ServiceUrls:PlanDevApi"] ?? "https://localhost:7015";
var planDevPort = new Uri(planDevPortString).Port;
Console.WriteLine($"📡 Plan & Development Service Port: {planDevPort}");

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, planDevPort, listenOptions => listenOptions.UseHttps());
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

Console.WriteLine("=== 🔍 PLAN & DEVELOPMENT SERVICE DEBUG CONFIGURATION ===");
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

// ============================================================
// ✅ DATABASE CONNECTION WITH FALLBACKS
// ============================================================

string dbConnectionString = string.Empty;
var connectionAttempts = new List<string>();

// Option 1: Try environment variables first (for Aspire)
var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");

if (!string.IsNullOrEmpty(dbHost) && !string.IsNullOrEmpty(dbPassword))
{
    var connString = $"Host={dbHost};Port={dbPort ?? "5432"};Database={dbName ?? "core.PlanDevDb"};Username={dbUser ?? "postgres"};Password={dbPassword};Include Error Detail=true";
    connectionAttempts.Add(connString);
    Console.WriteLine($"✅ Option 1: Using connection string from environment variables");
}

// Option 2: Try appsettings.json
var configConnectionString = builder.Configuration["ConnectionStrings:planDevDbCon"];
if (!string.IsNullOrEmpty(configConnectionString))
{
    connectionAttempts.Add(configConnectionString);
    Console.WriteLine($"✅ Option 2: Using connection string from appsettings.json");
}

// Option 3: Try default values
var defaultConnString = "Host=localhost;Port=5432;Database=core.PlanDevDb;Username=postgres;Password=root;Include Error Detail=true";
connectionAttempts.Add(defaultConnString);

// Option 4: Try localhost with default password
connectionAttempts.Add("Host=localhost;Port=5432;Database=core.PlanDevDb;Username=postgres;Password=postgres;Include Error Detail=true");

// Option 5: Try with no password
connectionAttempts.Add("Host=localhost;Port=5432;Database=core.PlanDevDb;Username=postgres;Include Error Detail=true");

// Try each connection string
foreach (var attempt in connectionAttempts.Distinct())
{
    if (!string.IsNullOrEmpty(dbConnectionString)) break;

    try
    {
        Console.WriteLine($"\n🔄 Attempting connection with: {attempt.Substring(0, Math.Min(60, attempt.Length))}...");

        // Test connection with a simple query
        using var testConn = new Npgsql.NpgsqlConnection(attempt);
        await testConn.OpenAsync();
        using var cmd = testConn.CreateCommand();
        cmd.CommandText = "SELECT 1";
        await cmd.ExecuteScalarAsync();

        dbConnectionString = attempt;
        Console.WriteLine($"✅ Connection successful!");

        // Extract and log the password (masked)
        var passwordMatch = System.Text.RegularExpressions.Regex.Match(attempt, "Password=([^;]+)");
        if (passwordMatch.Success)
        {
            var pwd = passwordMatch.Groups[1].Value;
            var masked = pwd.Length > 4 ? pwd.Substring(0, 2) + "..." + pwd.Substring(pwd.Length - 2) : "***";
            Console.WriteLine($"🔑 Using password: {masked}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Connection failed: {ex.Message}");
        // Continue to next attempt
    }
}

if (string.IsNullOrEmpty(dbConnectionString))
{
    Console.WriteLine("\n❌ All connection attempts failed!");
    Console.WriteLine("Please check:");
    Console.WriteLine("1. PostgreSQL is running");
    Console.WriteLine("2. The password is correct");
    Console.WriteLine("3. The database exists");
    Console.WriteLine("\nTo create the database, run:");
    Console.WriteLine("  createdb -U postgres core.PlanDevDb");
    Console.WriteLine("Or connect to PostgreSQL and run:");
    Console.WriteLine("  CREATE DATABASE \"core.PlanDevDb\";");

    throw new InvalidOperationException("Could not connect to PostgreSQL");
}

Console.WriteLine($"\n✅ Final connection string established");

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
var planDevApiKey = GetConfig("ApiKeys:PlanDev", "plandev_module_secret_key_2024");

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
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
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

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ============= DATABASE =============
builder.Services.AddDbContext<PlanDevDbContext>(options =>
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
            options.InstanceName = "PlanDev_";
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
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// ============= HEALTH CHECKS =============
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PlanDevDbContext>()
    .AddCheck<PlanDevHealthCheck>("PlanDev");

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

builder.Services.AddAuthorization();

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Plan & Development API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PlanDevDbContext>();
    await db.Database.MigrateAsync();
    Console.WriteLine("✅ Database migrations applied successfully");
}

// Map endpoints
app.MapControllers();
app.MapHealthChecks("/health");

Console.WriteLine($"\n✅ Plan & Development Service starting on https://0.0.0.0:{planDevPort}");
Console.WriteLine($"🔗 Swagger: https://0.0.0.0:{planDevPort}/swagger");
Console.WriteLine($"🔑 JWT Issuer: {jwtIssuer}");
Console.WriteLine($"🔑 JWT Audience: {jwtAudience}");
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
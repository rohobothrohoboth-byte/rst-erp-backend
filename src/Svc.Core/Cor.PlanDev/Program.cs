
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
using Npgsql;
 // ⭐ ADD THIS - MISSING Npgsql using

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
var planDevPortString = builder.Configuration["ServiceUrls:PlanDevApi"] ?? "http://plandev";
var planDevPort = new Uri(planDevPortString).Port;
Console.WriteLine($"📡 Plan & Development Service Port: {planDevPort}");

/*builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, planDevPort);  // ✅ Use 'planDevPort'
});
 // DISABLED*/

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
var authUrl = GetConfig("ServiceUrls:AuthApi", "http://auth");
var gatewayUrl = GetConfig("ServiceUrls:GatewayApi", "http://gateway");

// ============================================================
// ✅ DATABASE CONNECTION WITH FALLBACKS - CORRECTED
// ============================================================

string dbConnectionString = string.Empty;

// Build connection string options with proper fallbacks
var connectionOptions = new List<(string Host, string Port, string Database, string User, string Password)>();

// Option 1: Try environment variables first (for Aspire)
var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "core.PlanDevDb";

if (!string.IsNullOrEmpty(dbPassword))
{
    connectionOptions.Add((dbHost, dbPort, dbName, dbUser, dbPassword));
    Console.WriteLine($"✅ Option 1: Using connection string from environment variables");
}

// Option 2: Try appsettings.json
var configConnectionString = builder.Configuration["ConnectionStrings:planDevDbCon"];
if (!string.IsNullOrEmpty(configConnectionString))
{
    // Parse the connection string to extract values
    try
    {
        var connBuilder = new NpgsqlConnectionStringBuilder(configConnectionString);
        connectionOptions.Add((connBuilder.Host, connBuilder.Port.ToString(), connBuilder.Database, connBuilder.Username, connBuilder.Password));
        Console.WriteLine($"✅ Option 2: Using connection string from appsettings.json");
    }
    catch
    {
        // If parsing fails, add as-is
        connectionOptions.Add(("localhost", "5432", "core.PlanDevDb", "postgres", "root"));
        Console.WriteLine($"⚠️ Could not parse appsettings connection string, using defaults");
    }
}

// Option 3: Try default values (if not already added)
if (connectionOptions.Count == 0)
{
    connectionOptions.Add(("localhost", "5432", "core.PlanDevDb", "postgres", "root"));
    connectionOptions.Add(("localhost", "5432", "core.PlanDevDb", "postgres", "postgres"));
    Console.WriteLine($"✅ Option 3: Using default connection strings");
}

// Try each connection option
var successfulConnection = false;
var usedPasswords = new HashSet<string>();

foreach (var option in connectionOptions.Distinct())
{
    if (successfulConnection) break;

    // Skip if we've already tried this password
    if (usedPasswords.Contains(option.Password)) continue;
    usedPasswords.Add(option.Password);

    var testConnString = $"Host={option.Host};Port={option.Port};Database=postgres;Username={option.User};Password={option.Password};Include Error Detail=true;";

    try
    {
        Console.WriteLine($"\n🔄 Testing PostgreSQL connection with user '{option.User}'...");
        Console.WriteLine($"   Host: {option.Host}:{option.Port}");
        Console.WriteLine($"   Password: {(string.IsNullOrEmpty(option.Password) ? "(empty)" : "****")}");

        using var testConn = new NpgsqlConnection(testConnString);
        await testConn.OpenAsync();

        // Check if database exists
        var dbExists = false;
        using var checkDbCmd = testConn.CreateCommand();
        checkDbCmd.CommandText = "SELECT 1 FROM pg_database WHERE datname = @dbName";
        checkDbCmd.Parameters.AddWithValue("@dbName", option.Database);
        dbExists = (await checkDbCmd.ExecuteScalarAsync()) != null;

        if (!dbExists)
        {
            Console.WriteLine($"📊 Database '{option.Database}' does not exist. Creating it...");
            using var createDbCmd = testConn.CreateCommand();
            createDbCmd.CommandText = $"CREATE DATABASE \"{option.Database}\"";
            await createDbCmd.ExecuteNonQueryAsync();
            Console.WriteLine($"✅ Database '{option.Database}' created successfully!");
        }
        else
        {
            Console.WriteLine($"✅ Database '{option.Database}' exists.");
        }

        // Test connection to the actual database
        var finalConnString = $"Host={option.Host};Port={option.Port};Database={option.Database};Username={option.User};Password={option.Password};Include Error Detail=true;Maximum Pool Size=50;Minimum Pool Size=5;Connection Idle Lifetime=300;Connection Pruning Interval=60;";

        using var finalTest = new NpgsqlConnection(finalConnString);
        await finalTest.OpenAsync();

        // Test query
        using var testCmd = finalTest.CreateCommand();
        testCmd.CommandText = "SELECT 1";
        await testCmd.ExecuteScalarAsync();

        dbConnectionString = finalConnString;
        successfulConnection = true;
        Console.WriteLine($"✅ Connection successful to database '{option.Database}'!");
        Console.WriteLine($"🔑 Using password: {new string('*', option.Password.Length)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Connection failed: {ex.Message}");
        // Continue to next attempt
    }
}

if (!successfulConnection || string.IsNullOrEmpty(dbConnectionString))
{
    Console.WriteLine("\n❌ All connection attempts failed!");
    Console.WriteLine("Please check:");
    Console.WriteLine("1. PostgreSQL is running and accessible");
    Console.WriteLine("2. The password is correct");
    Console.WriteLine("3. The database exists or can be created");
    Console.WriteLine("4. PostgreSQL is configured to accept connections");

    Console.WriteLine("\nTo manually create the database, run:");
    Console.WriteLine("  psql -U postgres -c \"CREATE DATABASE \\\"core.PlanDevDb\\\";\"");
    Console.WriteLine("\nOr use the default connection string:");
    Console.WriteLine("  Host=localhost;Port=5432;Database=core.PlanDevDb;Username=postgres;Password=root;Include Error Detail=true");

    throw new InvalidOperationException("Could not connect to PostgreSQL");
}

Console.WriteLine($"\n✅ Connection string established successfully");

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

builder.Services.AddSingleton<Func<HttpClientHandler>>(sp => () =>
{
    var handler = new HttpClientHandler
    {
        UseCookies = false,
        AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
        MaxConnectionsPerServer = 10
    };

    // Conditionally set the certificate validation callback
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    }

    return handler;
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

// // app.UseHttpsRedirection(); // DISABLED FOR DOCKER // Disabled for Docker
app.UseAuthentication();
app.UseAuthorization();

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PlanDevDbContext>();
    try
    {
        await db.Database.MigrateAsync();
        Console.WriteLine("✅ Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Migration failed: {ex.Message}");
        Console.WriteLine("⚠️ Continuing startup - database may need manual migration");
    }
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



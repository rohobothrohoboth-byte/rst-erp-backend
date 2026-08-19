 // E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Program.cs

using Cor.FileManagement.Persistence;
using Cor.FileManagement.Services;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers.Services;
using Contracts;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common;
using RabbitMQ.Client;
using StackExchange.Redis;
using Cor.FileManagement.HealthChecks;
using Shared.Helpers.Extensions;
using Shared.Helpers;
using System.Net;
using Cor.FileManagement.Options;
var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
if (!Directory.Exists(wwwrootPath))
    Directory.CreateDirectory(wwwrootPath);
if (!Directory.Exists(Path.Combine(wwwrootPath, "uploads")))
    Directory.CreateDirectory(Path.Combine(wwwrootPath, "uploads"));
if (!Directory.Exists(Path.Combine(wwwrootPath, "thumbnails")))
    Directory.CreateDirectory(Path.Combine(wwwrootPath, "thumbnails"));

Console.WriteLine($"✅ wwwroot directory: {wwwrootPath}");

// ✅ Now create the builder
var builder = WebApplication.CreateBuilder(args);
// ✅ Load shared configuration FIRST
var sharedConfigPath = Path.Combine(AppContext.BaseDirectory, "appsettings.shared.json");
if (File.Exists(sharedConfigPath))
{
    builder.Configuration.AddJsonFile(sharedConfigPath, optional: false, reloadOnChange: true);
    Console.WriteLine($"✅ Loaded shared config from: {sharedConfigPath}");
}
else
{
    Console.WriteLine($"⚠️ Shared config not found at: {sharedConfigPath}");
}

// Load module-specific configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Load environment-specific configuration
var environment = builder.Environment.EnvironmentName;
builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

// Environment variables (highest priority)
builder.Configuration.AddEnvironmentVariables();

// ✅ Resolve ServiceHost placeholders
var serviceHost = builder.Configuration["ServiceHost"] ?? "localhost";
Console.WriteLine($"🏠 Service Host: {serviceHost}");

// Resolve ServiceUrls
var serviceUrls = builder.Configuration.GetSection("ServiceUrls");
var resolvedUrls = new Dictionary<string, string>();

foreach (var section in serviceUrls.GetChildren())
{
    var value = section.Value;
    if (!string.IsNullOrEmpty(value) && value.Contains("{ServiceHost}"))
    {
        value = value.Replace("{ServiceHost}", serviceHost);
        resolvedUrls[$"ServiceUrls:{section.Key}"] = value;
        Console.WriteLine($"✅ Resolved {section.Key}: {value}");
    }
}

// Resolve CORS origins
var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<List<string>>()
    ?.Select(origin => origin.Replace("{ServiceHost}", serviceHost))
    .ToArray() ?? Array.Empty<string>();

// Update configuration with resolved values
if (resolvedUrls.Any())
{
    builder.Configuration.AddInMemoryCollection(resolvedUrls);
}

// ✅ ADD THIS: Configure Kestrel - Get port from resolved configuration
var fileApiUrl = builder.Configuration["ServiceUrls:FileApi"] ?? "https://localhost:7009";
// Extract port from URL
var filePort = new Uri(fileApiUrl).Port;
Console.WriteLine($"📡 File Management Service Port: {filePort}");
/*
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 80);  // Force port 80 (HTTP)
});  // DISABLED*/

// ✅ Add this for graceful shutdown
builder.Services.Configure<HostOptions>(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(10);
});

// --- Logging ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

// ✅ DEBUG: Print configuration
Console.WriteLine("=== 🔍 FILE MANAGEMENT SERVICE DEBUG CONFIGURATION ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Service Host: {serviceHost}");
Console.WriteLine($"File API URL: {fileApiUrl}");
Console.WriteLine($"File API Port: {filePort}");
Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Register IHttpContextAccessor - REQUIRED
builder.Services.AddHttpContextAccessor();

// ✅ API Versioning
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

// ✅ MediatR - Register all handlers from the assembly
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// ✅ Database
builder.Services.AddDbContext<FileDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("fileManagementDbCon")
        ?? "Host=localhost;Port=5432;Database=core.FileManagementDb;Username=postgres;Password=root";
    options.UseNpgsql(connectionString);
});

// ============= REDIS CACHING =============
var redisConnectionString = builder.Configuration["Redis:ConnectionString"];

if (string.IsNullOrEmpty(redisConnectionString))
{
    redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379,abortConnect=false";
}

Console.WriteLine($"Redis Connection: {redisConnectionString}");

var redisAvailable = false;
var workingConnectionString = redisConnectionString;

var connectionAttempts = new List<string>
{
    redisConnectionString,
    "localhost:6379,password=kRrwYqqHWnnZpMhnn30way,ssl=true,abortConnect=false",
    "localhost:6379,password=kRrwYqqHWnnZpMhnn30way,abortConnect=false",
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
            Console.WriteLine($"✅ Redis connection SUCCESSFUL with: {attempt}");

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

// ✅ ALWAYS register IMemoryCache (required for MemoryCacheService)
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
        options.InstanceName = "FileManagement_";
    });
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();
    Console.WriteLine("✅ Redis Cache ENABLED");
}
else
{
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
    Console.WriteLine("✅ MemoryCache ENABLED (Redis fallback)");
}

// ============= RABBITMQ REGISTRATION =============
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var host = config["RabbitMQ:Host"] ?? "localhost";
    var port = int.Parse(config["RabbitMQ:Port"] ?? "5672");
    var username = config["RabbitMQ:Username"] ?? "guest";
    var password = config["RabbitMQ:Password"] ?? "guest";

    Console.WriteLine($"🔄 Connecting to RabbitMQ at {host}:{port} as {username}");

    return new ConnectionFactory
    {
        HostName = host,
        Port = port,
        UserName = username,
        Password = password,
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        RequestedHeartbeat = TimeSpan.FromSeconds(30),
        ContinuationTimeout = TimeSpan.FromSeconds(20)
    };
});

// ✅ Register RabbitMQ Event Publisher
builder.Services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();

// ============= HEALTH CHECKS =============
builder.Services.AddHealthChecks()
    .AddDbContextCheck<FileDbContext>()
    .AddCheck<RedisHealthCheck>("Redis");

// ✅ CORS - Using resolved origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        if (corsOrigins.Length > 0)
        {
            policy.WithOrigins(corsOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            // Fallback: Allow all (development only)
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

// ✅ Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JwtCons.Issuer,
            ValidateAudience = true,
            ValidAudience = JwtCons.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtCons.SecretKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    // ✅ Admin/Manager only operations
    options.AddPolicy("FileManagementAdmin", policy =>
        policy.RequireRole("Admin", "FileManager", "SuperAdmin"));

    // ✅ All authenticated users can perform basic operations
    options.AddPolicy("FileManagementUser", policy =>
        policy.RequireAuthenticatedUser());
});

// Enable [PerAuth("permission")] enforcement (checks the JWT `ph` bitmask against
// the shared Common.Permissions registry).
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PerAuthHandler>();

// ✅ Register Services
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IFileValidationService, FileValidationService>();
builder.Services.AddScoped<IFileThumbnailService, FileThumbnailService>();

// ✅ FIX: Use builder.Configuration instead of Configuration
var maxFileSize = builder.Configuration.GetValue<int>("FileStorage:MaxFileSizeMB");
var previewEnabled = builder.Configuration.GetValue<bool>("Preview:Enabled");
var sessionTimeout = builder.Configuration.GetValue<int>("Security:SessionTimeoutMinutes");

// ✅ You can also register these as options if needed
builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection("FileStorage"));
builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection("Security"));
builder.Services.Configure<PreviewOptions>(builder.Configuration.GetSection("Preview"));

// ============ BUILD THE APP ============
var app = builder.Build();

// ============ DATABASE MIGRATION ============
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FileDbContext>();
    try
    {
        Log.Information("📦 Checking database migration...");
        var created = dbContext.Database.EnsureCreated();
        if (created)
            Log.Information("✅ Database created successfully!");
        else
            Log.Information("✅ Database already exists.");

        await dbContext.Database.MigrateAsync();
        Log.Information("✅ Database migration completed successfully.");

        // Seed default folders
        await FileDbSeeder.SeedDefaultFolders(dbContext);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ An error occurred while migrating the database.");
    }
}

// ============ CONFIGURE PIPELINE ============
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("File Management API")
               .WithTheme(ScalarTheme.BluePlanet);
    });
}

// // // app.UseHttpsRedirection(); // DISABLED FOR DOCKER // Disabled for Docker // Disabled for Docker
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// ✅ Serve static files
app.UseStaticFiles();

app.MapControllers();
app.MapHealthChecks("/health");

// ============ RUN THE APP ============
app.Run();



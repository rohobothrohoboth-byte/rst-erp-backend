using Common;
using Recruit.API.Middlewares;
using Scalar.AspNetCore;
using Serilog;
using Recruit.App.Services;
using Shared.Helpers.Extensions;
using Shared.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using HealthChecks.Uris;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
var recruitPortString = builder.Configuration["ServiceUrls:HRMRecruitApi"] ?? "https://localhost:7005";
var recruitPort = new Uri(recruitPortString).Port;
Console.WriteLine($"📡 Recruit Service Port: {recruitPort}");

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, recruitPort, listenOptions =>
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
var CorHrmmUrl = GetConfig("ServiceUrls:CoreHRMMApi", "https://localhost:7001");
var authUrl = GetConfig("ServiceUrls:AuthApi", "https://localhost:7000");
var hrmProUrl = GetConfig("ServiceUrls:HrmProApi", "https://localhost:7004");
var financeApiUrl = GetConfig("ServiceUrls:FinanceApi", "https://localhost:7008");
var gatewayApiUrl = GetConfig("ServiceUrls:GatewayApi", "https://localhost:5000");

// API Keys
var coreApiKey = GetConfig("ApiKeys:CoreModule", "core_module_secret_key_2024");
var hrmmApiKey = GetConfig("ApiKeys:CoreHRMM", "core_module_secret_key_2024");
var profileApiKey = GetConfig("ApiKeys:ProfileModule", "profile_module_secret_key_2024");

// ============= DATABASE =============
var dbConnectionString = GetConfig("ConnectionStrings:RecruitDbCon", null);

if (string.IsNullOrEmpty(dbConnectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "root";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "Recruit";

    dbConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Include Error Detail=true";
}

// Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

Console.WriteLine("=== 🔍 RECRUIT SERVICE DEBUG CONFIGURATION ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"📡 Core Module URL: {CorModUrl}");
Console.WriteLine($"📡 Core HRMM URL: {CorHrmmUrl}");
Console.WriteLine($"📡 Auth URL: {authUrl}");
Console.WriteLine($"📡 HRM Pro URL: {hrmProUrl}");

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

// ============= BUILDER EXTENSIONS =============
builder.AddApiServices()
    .AddErrorHandling()
    .AddSwaggerService();

// ✅ Register gRPC clients
/*builder.Services.AddScoped<ICorModClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetService<ILogger<CorModClient>>();
    // Use the resolved coreModuleUrl
    var url = CorModUrl;
    Console.WriteLine($"📡 CorModClient URL: {url}");
    return new CorModClient(url, logger);
});*/
builder.Services.AddScoped<ICorModClient, CorModClient>();
builder.Services.AddScoped<ICorHrmmClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetService<ILogger<CorHrmmClient>>();
    var url = CorHrmmUrl;
    Console.WriteLine($"📡 CorHrmmClient URL: {url}");
    return new CorHrmmClient(url, logger);
});

builder.Services.AddScoped<IHrmProfileClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetService<ILogger<HrmProfileClient>>();
    var url = hrmProUrl;
    Console.WriteLine($"📡 HrmProfileClient URL: {url}");
    return new HrmProfileClient(url, logger);
});

builder.Services.AddScoped<IRecruitNotificationService, RecruitNotificationService>();
builder.Services.AddHttpClient();

// ============= HEALTH CHECKS =============
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri($"{CorModUrl}/health"), "Core Module API")
    .AddUrlGroup(new Uri($"{CorHrmmUrl}/health"), "Core HRMM API")
    .AddUrlGroup(new Uri($"{authUrl}/health"), "Auth API")
    .AddUrlGroup(new Uri($"{hrmProUrl}/health"), "HRM Pro API");

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference(options => { options.WithTitle("HRM Recruit API"); });
    app.ApplyMigration();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine($"\n✅ Recruit Service starting on https://0.0.0.0:{recruitPort}");
Console.WriteLine($"🔗 Auth URL: {authUrl}");
Console.WriteLine($"🔗 Core Module URL: {CorModUrl}");
Console.WriteLine($"🔗 Core HRMM URL: {CorHrmmUrl}");
Console.WriteLine($"🔗 HRM Pro URL: {hrmProUrl}");
Console.WriteLine("📊 Debug logging is ENABLED");
Console.WriteLine("\nPress Ctrl+C to stop");

await app.RunAsync();
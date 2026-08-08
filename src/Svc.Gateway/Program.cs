using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Svc.Gateway;
using System.Text;
using Svc.Notification.Extensions;
using Svc.Task.Extensions;
using System.Net;
using Shared.Helpers;

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

// ✅ Find and replace all {ServiceHost} placeholders
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

// Apply updates
if (updates.Any())
{
    builder.Configuration.AddInMemoryCollection(updates);
}

// ✅ Force HTTP on port 5000 and HTTPS on 5001
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5000, listenOptions =>
    {
        // HTTP
    });
    options.Listen(IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

builder.AddServiceDefaults();

// ========== REGISTER MODULE SERVICES ==========
builder.Services.AddSvcNotification(builder.Configuration);
builder.Services.AddSvcTask(builder.Configuration);

// ========== REGISTER NOTIFICATION HTTP CLIENT ==========
var notificationServiceUrl = builder.Configuration["ServiceUrls:NotificationApi"] ?? "https://localhost:7007";
builder.Services.AddHttpClient<Svc.Task.Services.INotificationService, Svc.Task.Services.NotificationServiceClient>(client =>
{
    client.BaseAddress = new Uri(notificationServiceUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// ========== REVERSE PROXY WITH SSL TRUST ==========
// ✅ Add Reverse Proxy with custom HTTP client configuration
var reverseProxyBuilder = builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ✅ Configure the HTTP client for the reverse proxy
reverseProxyBuilder.Services.ConfigureHttpClientDefaults(http =>
{
    http.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        MaxConnectionsPerServer = 100,
        AutomaticDecompression = DecompressionMethods.GZip
    });
    http.SetHandlerLifetime(TimeSpan.FromMinutes(2));
});

// ========== AUTHENTICATION ==========
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
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

// ========== CORS ==========
var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<List<string>>()
    ?.Select(origin => origin.Replace("{ServiceHost}", serviceHost))
    .ToArray() ?? new[] {
        "http://localhost:1211",
        "http://localhost:1212",
        $"http://{serviceHost}:1211",
        $"http://{serviceHost}:1212",
        $"http://{serviceHost}:5000",
        "http://localhost:5000"
    };

// ✅ Also resolve Cors:AllowedOrigins if exists
var corsAllowed = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<List<string>>();
if (corsAllowed != null && corsAllowed.Any())
{
    corsOrigins = corsAllowed
        .Select(origin => origin.Replace("{ServiceHost}", serviceHost))
        .ToArray();
}

Console.WriteLine($"📍 CORS Origins: {string.Join(", ", corsOrigins)}");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ✅ Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.MapHealthChecks("/health");

app.Run();
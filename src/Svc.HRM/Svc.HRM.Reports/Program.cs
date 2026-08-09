using System.Net;
using System.Text;
using Asp.Versioning;
using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Svc.HRM.Reports.Services;

var builder = WebApplication.CreateBuilder(args);

var sharedConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Shared", "Helpers", "appsettings.json");
if (File.Exists(sharedConfigPath))
    builder.Configuration.AddJsonFile(sharedConfigPath, optional: false, reloadOnChange: true);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

var serviceHost = builder.Configuration["ServiceHost"] ?? "localhost";
var updates = builder.Configuration.AsEnumerable()
    .Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value!.Contains("{ServiceHost}"))
    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!.Replace("{ServiceHost}", serviceHost));
if (updates.Count > 0)
    builder.Configuration.AddInMemoryCollection(updates!);

string GetConfig(string key, string? defaultValue = null)
{
    var value = builder.Configuration[key];
    if (!string.IsNullOrEmpty(value)) return value;
    if (defaultValue != null) return defaultValue;
    if (!builder.Environment.IsDevelopment())
        throw new InvalidOperationException($"Missing required configuration: {key}");
    return string.Empty;
}

var portString = GetConfig("ServiceUrls:ReportsApi", "https://localhost:7018");
var port = new Uri(portString).Port;
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, port, listenOptions => listenOptions.UseHttps());
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var jwtSecret = builder.Configuration["Jwt:SecretKey"] ?? JwtCons.SecretKey;
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
            ValidIssuer = JwtCons.Issuer,
            ValidateAudience = true,
            ValidAudience = JwtCons.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<Svc.HRM.Reports.Services.ForwardAuthHandler>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        if (builder.Environment.IsDevelopment())
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        else
            policy.AllowAnyMethod().AllowAnyHeader();
    });
});

void AddUpstream(string name, string url)
{
    builder.Services.AddHttpClient(name, client =>
    {
        client.BaseAddress = new Uri(url);
        client.Timeout = TimeSpan.FromSeconds(60);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("X-Service-Name", "HrReportsService");
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    })
    .AddHttpMessageHandler<Svc.HRM.Reports.Services.ForwardAuthHandler>();
}

AddUpstream("profile", GetConfig("ServiceUrls:HrmProApi", "https://localhost:7004"));
AddUpstream("attendance", GetConfig("ServiceUrls:AttendanceApi", "https://localhost:7011"));
AddUpstream("leave", GetConfig("ServiceUrls:HrmLeaveApi", GetConfig("ServiceUrls:LeaveApi", "https://localhost:7003")));
AddUpstream("payroll", GetConfig("ServiceUrls:PayrollApi", "https://localhost:7010"));
AddUpstream("recruit", GetConfig("ServiceUrls:HrmRecruitApi", GetConfig("ServiceUrls:HrmRecruit", "https://localhost:7005")));

builder.Services.AddScoped<IHrReportService, HrReportService>();
builder.Services.AddHealthChecks();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseMiddleware<Svc.HRM.Reports.Middleware.ApiExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
Console.WriteLine($"HR Reports Service starting on https://0.0.0.0:{port}");
await app.RunAsync();

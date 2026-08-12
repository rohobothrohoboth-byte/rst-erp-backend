using System.Net;
using System.Text;
using Asp.Versioning;
using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Svc.HRM.Performance.Persistence;
using Svc.HRM.Performance.Services;

var builder = WebApplication.CreateBuilder(args);

var sharedConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Shared", "Helpers", "appsettings.json");
if (File.Exists(sharedConfigPath))
    builder.Configuration.AddJsonFile(sharedConfigPath, optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// The configured URL may contain an unsubstituted {ServiceHost} placeholder
// (e.g. "https://{ServiceHost}:5005"), which is not a valid Uri, so extract the
// port from the trailing :NNNN instead of parsing the whole URL. Env override:
// PERFORMANCE_PORT.
var urlString = builder.Configuration["ServiceUrls:PerformanceApi"] ?? "https://localhost:5005";
var port = 5005;
if (int.TryParse(builder.Configuration["PERFORMANCE_PORT"], out var envPort)) { port = envPort; }
else { var m = System.Text.RegularExpressions.Regex.Match(urlString, @":(\d+)"); if (m.Success && int.TryParse(m.Groups[1].Value, out var p)) { port = p; } }
builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Any, port, lo => lo.UseHttps()));

// NOTE: read a specific key ("HrmPerformanceDb") that intentionally does NOT match the
// Aspire database resource name ("performanceDb"). Aspire injects ConnectionStrings__performanceDb
// with its own generated password parameter, which does not match the actual Postgres
// password ("root"), causing 28P01 auth failures. Every working service in this repo
// sidesteps that by building the connection string from POSTGRES_* env vars (password
// "root"), pointing at localhost:5432 — the same instance they all use.
var conn = builder.Configuration["ConnectionStrings:HrmPerformanceDb"];
if (string.IsNullOrEmpty(conn))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "root";
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "HRM.PerformanceDb";
    conn = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Include Error Detail=true";
}
builder.Services.AddDbContext<PerformanceDbContext>(o => o.UseNpgsql(conn));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(o => { o.DefaultApiVersion = new ApiVersion(1, 0); o.AssumeDefaultVersionWhenUnspecified = true; o.ReportApiVersions = true; })
    .AddApiExplorer(o => { o.GroupNameFormat = "'v'VVV"; o.SubstituteApiVersionInUrl = true; });
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, ValidIssuer = JwtCons.Issuer,
            ValidateAudience = true, ValidAudience = JwtCons.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtCons.SecretKey)),
            ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PerAuthHandler>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<PerformanceDbContext>().Database.Migrate();
}
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

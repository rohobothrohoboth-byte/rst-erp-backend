using System.Net;
using System.Text;
using Asp.Versioning;
using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Svc.HRM.Training.Persistence;
using Svc.HRM.Training.Services;

var builder = WebApplication.CreateBuilder(args);

var sharedConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Shared", "Helpers", "appsettings.json");
if (File.Exists(sharedConfigPath))
    builder.Configuration.AddJsonFile(sharedConfigPath, optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

var urlString = builder.Configuration["ServiceUrls:TrainingApi"] ?? "https://localhost:5007";
var port = new Uri(urlString).Port;
builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Any, port, lo => lo.UseHttps()));

var conn = builder.Configuration.GetConnectionString("TrainingDb")
    ?? "Host=localhost;Port=5432;Database=HRM.TrainingDb;Username=postgres;Password=root";
builder.Services.AddDbContext<TrainingDbContext>(o => o.UseNpgsql(conn));

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
builder.Services.AddScoped<ITrainingService, TrainingService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<TrainingDbContext>().Database.EnsureCreated();
}
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

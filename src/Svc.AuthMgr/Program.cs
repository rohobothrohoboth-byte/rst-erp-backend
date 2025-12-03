using Scalar.AspNetCore;
using Serilog;
using Svc.AuthMgr.Extensions;
using Svc.AuthMgr.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// --- Logging ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

// --- CORS ---
builder.Services.AddCors(options => { options.AddPolicy("AllowAll", policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }); });


builder.AddApiServices()
    .AddHttpClientServices()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddSwaggerService()
    .AddAuthService();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

// Development-only: Swagger + detailed errors
if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference(options => { options.WithTitle("Core HRMM API"); });
    await app.ApplyMigrationsAsync();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
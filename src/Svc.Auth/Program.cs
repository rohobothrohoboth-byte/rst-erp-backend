using Scalar.AspNetCore;
using Serilog;
using Svc.Auth.Extensions;
using Svc.Auth.Middlewares;
using Svc.Auth.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Logging ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

builder.AddApiServices()
    .AddErrorHandling()
    .AddSwaggerService()
    .AddAuthService();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapGrpcService<AuthValidatorService>();

if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference(options => { options.WithTitle("Auth Manager API"); });
    app.ApplyMigration();
    await app.ApplyAdminRole();
    await app.ApplyPerModSeed();
    //await app.ApplyPerMenuSeed();
    //await app.ApplyPerApiSeed();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
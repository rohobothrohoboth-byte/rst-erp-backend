using Recruit.API.Middlewares;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
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

await app.RunAsync();
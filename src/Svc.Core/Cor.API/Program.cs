using System.Data;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Cor.API.Middlewares;
using Cor.App.Interfaces;
using Cor.Utility.Extensions;
using Cor.Utility.Persistence;
using Cor.Utility.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

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
builder.Services.AddDbContext<CoreDbContext>(opt => { opt.UseNpgsql(builder.Configuration.GetConnectionString("CoreDbCon")); });

// --- Add controllers + API versioning + problem details ---
builder.Services.AddControllers();
builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.ReportApiVersions = true;
}).AddMvc(option => { option.Conventions.Add(new VersionByNamespaceConvention()); })
  .AddApiExplorer(option =>
{
    option.GroupNameFormat = "'v'V";
    option.SubstituteApiVersionInUrl = true;
});
builder.Services.AddProblemDetails();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Core API", Version = "v1" });
    c.SchemaGeneratorOptions = new SchemaGeneratorOptions { SchemaIdSelector = type => type.FullName };
});

// --- Dapper Context + UnitOfWork (preserve your existing wiring) ---
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var context = sp.GetRequiredService<DapperContext>();
    return context.CreateConnection();
});
builder.Services.AddUtilitySvc(builder.Configuration);


var app = builder.Build();

// --- global exception middleware ---
app.UseMiddleware<ExceptionMiddleware>();

// Development-only: Swagger + detailed errors
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API v1"); });
    app.ApplyMigration();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

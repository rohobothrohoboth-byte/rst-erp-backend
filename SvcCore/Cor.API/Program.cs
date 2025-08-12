using Asp.Versioning.Conventions;
using Cor.API.Middlewares;
using Cor.App.Interfaces;
using Cor.Utility.Extensions;
using Cor.Utility.Repositories;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
// --- Logging ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        //policy.WithOrigins("http://localhost:5173/").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

// --- Logging ---
Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day).Enrich.FromLogContext().MinimumLevel.Information().CreateLogger();
builder.Host.UseSerilog();

// --- Add services ---
builder.Services.AddControllers();
builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    option.ReportApiVersions = true;
}).AddMvc(option =>
{
    option.Conventions.Add(new VersionByNamespaceConvention());
}).AddApiExplorer(option =>
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


// --- Dapper Context + UnitOfWork ---
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var context = sp.GetRequiredService<DapperContext>();
    return context.CreateConnection();
});
builder.Services.AddUtilitySvc(builder.Configuration);

var app = builder.Build();

// --- Middlewares ---
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
//    if (db.Database.GetAppliedMigrations().Any()) db.Database.Migrate();
//}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API v1"); });
    app.ApplyMigration();
}

app.UseCors("AllowAll");
//app.UseCors("BDAFrontEnd");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
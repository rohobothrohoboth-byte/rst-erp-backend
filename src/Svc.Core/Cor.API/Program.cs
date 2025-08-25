using System.Text.Json.Serialization;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Cor.API.Middlewares;
using Cor.Utility.Extensions;
using Cor.Utility.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.Authority = builder.Configuration["IdentityServiceUrl"];
        option.RequireHttpsMetadata = false; // For development only, set to true in production
        option.TokenValidationParameters.ValidateAudience = false;
        option.TokenValidationParameters.NameClaimType = "username";
    });


// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Core API", Version = "v1" });
    //c.SchemaFilter<EnumSchemaFilter>();
    //c.SchemaGeneratorOptions = new SchemaGeneratorOptions { SchemaIdSelector = type => type.FullName };
});

builder.Services.AddUtilitySvc(builder.Configuration);

var app = builder.Build();

// --- global exception middleware ---
app.UseMiddleware<ExceptionMiddleware>();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection(); // Must be before Swagger

// Development-only: Swagger + detailed errors
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API v1"); });
    app.ApplyMigration();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
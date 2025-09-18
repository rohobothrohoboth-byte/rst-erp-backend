using Asp.Versioning;
using Asp.Versioning.Conventions;
using Cor.HRMM.Extensions;
using Cor.HRMM.Middlewares;
using Cor.HRMM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;
//using Svc.Lup.Extensions;
//using Svc.Lup.Middlewares;

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

var gatewayUrl = builder.Configuration["Services:GatewayService"];
var lupUrl = builder.Configuration["Services:LupService"];

builder.Services.AddHttpClient<ILupClient, LupClient>(c => { c.BaseAddress = new Uri(new Uri(gatewayUrl!), lupUrl); });

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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "core.HRMM API", Version = "v1" });
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
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "core.HRMM API v1"); });
    app.ApplyMigration();
    //await app.ApplySeedAsync();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
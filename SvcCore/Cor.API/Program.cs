using System.Data;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Cor.API.Middlewares;
using Cor.App.Interfaces;
using Cor.Utility.Extensions;
using Cor.Utility.Persistence;        // CoreDbContext
using Cor.Utility.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});

// --- EF Core DbContext (CoreDbContext) ---
builder.Services.AddDbContext<CoreDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("CoreDbCon"))
);

// --- Authentication (OIDC / JWKS via Authority) ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        // Use MetadataAddress if you prefer: builder.Configuration["Auth:Authority"] + "/.well-known/openid-configuration"
        o.MetadataAddress = $"{builder.Configuration["Auth:Authority"]}/.well-known/openid-configuration";
        // If Authority is a URL like "http://auth.api:1213", ensure RequireHttpsMetadata=false in development
        o.RequireHttpsMetadata = bool.Parse(builder.Configuration["Auth:RequireHttpsMetadata"] ?? "true");
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true
        };
        o.RefreshOnIssuerKeyNotFound = true;
    });

// --- Authorization policies ---
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("perm:branch.read", p => p.RequireClaim("permission", "branch.read"));
    o.AddPolicy("perm:branch.write", p => p.RequireClaim("permission", "branch.write"));
});

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
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        Description = "Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme{ Reference=new OpenApiReference{ Type=ReferenceType.SecurityScheme, Id="Bearer"}}, Array.Empty<string>() }
    });
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

// build
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

// standard middleware pipeline
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

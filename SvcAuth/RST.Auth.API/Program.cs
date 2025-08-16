using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RST.Auth.API.Data;
using RST.Auth.API.Models;
using System.Text;
using Microsoft.AspNetCore.Identity;
using RST.Auth.API.Auth;
using RST.Auth.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ------------------------
// Configuration
// ------------------------
var configuration = builder.Configuration;

// EF Core
builder.Services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("AuthConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // for dev
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("permissions.manage", p => p.Requirements.Add(new PermissionRequirement("permissions.manage")));
    opts.AddPolicy("roles.manage", p => p.Requirements.Add(new PermissionRequirement("roles.manage")));
    opts.AddPolicy("introspect.access", p => p.Requirements.Add(new PermissionRequirement("introspect.access")));
    opts.AddPolicy("keys.rotate", p => p.Requirements.Add(new PermissionRequirement("keys.rotate")));
});

// Controllers
builder.Services.AddControllers();

// Swagger + JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RST Auth API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT with Bearer prefix",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            }, new string[]{}
        }
    });
});


var app = builder.Build();

// ------------------------
// Middleware
// ------------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RST Auth API V1");
    c.RoutePrefix = string.Empty;
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ------------------------
// Seed initial data
// ------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.EnsureAsync(services);
}

app.Run();

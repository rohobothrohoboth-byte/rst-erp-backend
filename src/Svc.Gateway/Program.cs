using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Svc.Gateway;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer((option =>
//    {
//        option.Authority = builder.Configuration["IdentityServiceUrl"];
//        option.RequireHttpsMetadata = false; // For development only, set to true in production
//        option.TokenValidationParameters.ValidateAudience = false;
//        option.TokenValidationParameters.NameClaimType = "username";
//    }));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["AuthUrl"];
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = JwtCons.Issuer,
        ValidateAudience = true,
        ValidAudience = JwtCons.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtCons.SecretKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

// --- CORS ---
builder.Services.AddCors(options => { options.AddPolicy("AllowAll", policy => { policy.WithOrigins("http://localhost:1211").AllowAnyMethod().AllowAnyHeader().AllowCredentials(); }); });


var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();

app.Run();

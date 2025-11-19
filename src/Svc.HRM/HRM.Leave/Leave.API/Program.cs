using Asp.Versioning;
using Asp.Versioning.Conventions;
using Leave.API.Middlewares;
using Leave.App.Services;
using Leave.Utility.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using Serilog;

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

var gatewayUrl = builder.Configuration["Services:Gateway"];
//var lupUrl = builder.Configuration["Services:Lup"];
//var corHRMMUrl = builder.Configuration["Services:CorHRMM"];
var corModUrl = builder.Configuration["Services:CorMod"];
var hrmProUrl = builder.Configuration["Services:HrmProfile"];

//builder.Services.AddHttpClient<ILup, Lup>(c => { c.BaseAddress = new Uri(new Uri(gatewayUrl!), lupUrl); }).AddPolicyHandler(ResiliencePolicies.GetRetryPolicy()).AddPolicyHandler(ResiliencePolicies.GetTimeoutPolicy()).AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy());

//builder.Services.AddHttpClient<ICorHRMM, CorHRMM>(c => { c.BaseAddress = new Uri(new Uri(gatewayUrl!), corHRMMUrl); }).AddPolicyHandler(ResiliencePolicies.GetRetryPolicy()).AddPolicyHandler(ResiliencePolicies.GetTimeoutPolicy()).AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<ICorMod, CorMod>(c => { c.BaseAddress = new Uri(new Uri(gatewayUrl!), corModUrl); }).AddPolicyHandler(ResiliencePolicies.GetRetryPolicy()).AddPolicyHandler(ResiliencePolicies.GetTimeoutPolicy()).AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IHrmProfile, HrmProfile>(c => { c.BaseAddress = new Uri(new Uri(gatewayUrl!), hrmProUrl); }).AddPolicyHandler(ResiliencePolicies.GetRetryPolicy()).AddPolicyHandler(ResiliencePolicies.GetTimeoutPolicy()).AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy());

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
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HRM Leave API",
        Version = "v1",
        Description = "API documentation for HRM Leave Microservice",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "natnahel.shd@gmail.com.com"
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    // JWT Authentication (if needed)
    //c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    //{
    //    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    //    Description = "Please insert JWT token",
    //    Name = "Authorization",
    //    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
    //    Scheme = "bearer",
    //    BearerFormat = "JWT"
    //});

    //c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    //{
    //    {
    //        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    //        {
    //            Reference = new Microsoft.OpenApi.Models.OpenApiReference
    //            {
    //                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        },
    //        Array.Empty<string>()
    //    }
    //});
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
    //app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "HRM Leave API v1"); c.RoutePrefix = string.Empty; });
    app.ApplyMigration();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
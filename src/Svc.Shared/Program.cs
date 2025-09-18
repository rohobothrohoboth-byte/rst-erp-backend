using Svc.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Register typed clients
builder.Services.AddHttpClient<IRegionClient, RegionClient>(c => { c.BaseAddress = new Uri(builder.Configuration["Services:LupService"]); });


var app = builder.Build();

app.MapControllers();
app.Run();

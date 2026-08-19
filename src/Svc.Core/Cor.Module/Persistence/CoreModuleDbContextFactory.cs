using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Cor.Module.Persistence;

// Design-time factory used by `dotnet ef` (migrations / scaffolding).
public class CoreModuleDbContextFactory : IDesignTimeDbContextFactory<CoreModuleDbContext>
{
    public CoreModuleDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("CorModuleDbCon")
            ?? "Host=localhost;Port=5432;Database=core.Module;Username=postgres;Password=root";

        var optionsBuilder = new DbContextOptionsBuilder<CoreModuleDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new CoreModuleDbContext(optionsBuilder.Options);
    }
}

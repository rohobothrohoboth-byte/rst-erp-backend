using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Cor.Utility.Persistence
{
    public class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
    {
        public CoreDbContext CreateDbContext(string[] args)
        {
            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Build options
            var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
            var connStr = configuration.GetConnectionString("CoreDbCon");
            optionsBuilder.UseNpgsql(connStr); // or UseSqlServer(connStr)

            // Return the context
            return new CoreDbContext(optionsBuilder.Options);
        }
    }
}

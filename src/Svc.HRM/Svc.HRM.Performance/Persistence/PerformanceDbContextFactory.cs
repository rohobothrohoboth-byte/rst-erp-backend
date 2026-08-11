using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Svc.HRM.Performance.Persistence;

// Design-time factory so `dotnet ef migrations` can build the model without
// running Program.cs (and without needing a live database).
public class PerformanceDbContextFactory : IDesignTimeDbContextFactory<PerformanceDbContext>
{
    public PerformanceDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PerformanceDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=HRM.PerformanceDb;Username=postgres;Password=root")
            .Options;
        return new PerformanceDbContext(options);
    }
}

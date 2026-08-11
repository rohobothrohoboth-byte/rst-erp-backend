using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Svc.HRM.Training.Persistence;

// Design-time factory so `dotnet ef migrations` can build the model without
// running Program.cs (and without needing a live database).
public class TrainingDbContextFactory : IDesignTimeDbContextFactory<TrainingDbContext>
{
    public TrainingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TrainingDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=HRM.TrainingDb;Username=postgres;Password=root")
            .Options;
        return new TrainingDbContext(options);
    }
}

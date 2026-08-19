using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Profile.Utility.Persistence;

// Design-time factory so `dotnet ef migrations` can build the model without
// running Program.cs (and without needing a live database).
public class HrmProfileDbContextFactory : IDesignTimeDbContextFactory<HrmProfileDbContext>
{
    public HrmProfileDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__HRMProDbCon")
            ?? "Host=localhost;Port=5432;Database=HRM.Pro;Username=postgres;Password=root";

        var options = new DbContextOptionsBuilder<HrmProfileDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new HrmProfileDbContext(options);
    }
}

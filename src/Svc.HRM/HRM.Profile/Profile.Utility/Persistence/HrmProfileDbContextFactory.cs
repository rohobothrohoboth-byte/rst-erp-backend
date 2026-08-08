using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Profile.Utility.Persistence;

public class HrmProfileDbContextFactory : IDesignTimeDbContextFactory<HrmProfileDbContext>
{
    public HrmProfileDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HrmProfileDbContext>();
        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__HRMProDbCon")
                 ?? "Host=localhost;Port=5432;Database=HRM.Pro;Username=postgres;Password=root";
        optionsBuilder.UseNpgsql(cs);
        return new HrmProfileDbContext(optionsBuilder.Options);
    }
}

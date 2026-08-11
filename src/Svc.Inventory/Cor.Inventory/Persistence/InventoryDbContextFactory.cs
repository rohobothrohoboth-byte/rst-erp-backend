using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cor.Inventory.Persistence;

public class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=core.InventoryDb;Username=postgres;Password=root");
        return new InventoryDbContext(optionsBuilder.Options);
    }
}

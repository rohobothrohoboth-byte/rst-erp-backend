using Cor.Inventory.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Persistence;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<StockLevel> StockLevels { get; set; }
    public DbSet<ExternalSystem> ExternalSystems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Warehouse indexes
        modelBuilder.Entity<Warehouse>()
            .HasIndex(w => w.Code)
            .IsUnique();

        modelBuilder.Entity<Warehouse>()
            .HasIndex(w => w.Name);

        modelBuilder.Entity<Warehouse>()
            .HasIndex(w => w.IsActive);

        // StockLevel indexes
        modelBuilder.Entity<StockLevel>()
            .HasIndex(s => new { s.WarehouseId, s.ProductId })
            .IsUnique();

        modelBuilder.Entity<StockLevel>()
            .HasIndex(s => s.ProductCode);

        // ✅ QuantityAvailable is [NotMapped] so no index needed
        // If you need to query by availability, use a computed column:
        // modelBuilder.Entity<StockLevel>()
        //     .Property(s => s.QuantityAvailable)
        //     .HasComputedColumnSql("\"QuantityOnHand\" - \"QuantityReserved\"", stored: true);
    }
}
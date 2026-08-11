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
    public DbSet<Category> Categories { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<MaterialRequest> MaterialRequests { get; set; }
    public DbSet<MaterialAssignment> MaterialAssignments { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product catalog + employee-materials domain
        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Unit>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<MaterialRequest>().HasQueryFilter(m => !m.IsDeleted);
        modelBuilder.Entity<MaterialAssignment>().HasQueryFilter(m => !m.IsDeleted);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Sku)
            .IsUnique();

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
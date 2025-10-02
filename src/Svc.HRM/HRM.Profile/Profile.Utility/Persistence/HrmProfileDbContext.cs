using Microsoft.EntityFrameworkCore;

namespace Profile.Utility.Persistence;

public class HrmProfileDbContext : DbContext
{
    public HrmProfileDbContext(DbContextOptions<HrmProfileDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.HasPostgresExtension("pgcrypto");

        //modelBuilder.Entity<Company>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });

    }

    //public DbSet<Company> Company { get; set; }

}



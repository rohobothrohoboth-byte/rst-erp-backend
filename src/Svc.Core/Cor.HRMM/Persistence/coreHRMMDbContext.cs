using Cor.HRMM.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Persistence;

public class coreHRMMDbContext : DbContext
{
    public coreHRMMDbContext(DbContextOptions<coreHRMMDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        //modelBuilder.AddInboxStateEntity();
        //modelBuilder.AddOutboxMessageEntity();
        //modelBuilder.AddOutboxStateEntity();
        modelBuilder.HasPostgresExtension("pgcrypto");

        //modelBuilder.Entity<Address>(entity =>
        //{
        //    entity.HasKey(e => e.Id);
        //    entity.Property(e => e.Id).HasColumnType("uuid").ValueGeneratedNever();
        //    entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        //});

        modelBuilder.Entity<Address>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });


    }

    public DbSet<Address> Address { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }

}

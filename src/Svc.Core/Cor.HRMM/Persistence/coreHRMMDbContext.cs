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

        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.Entity<Address>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<BenefitSetting>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EducationQual>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<JobGrade>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<Position>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<PositionBenefit>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<PositionEducation>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<PositionExp>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<PositionReq>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<SalaryScale>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
    }

    public DbSet<Address> Address { get; set; }
    public DbSet<BenefitSetting> BenefitSetting { get; set; }
    public DbSet<EducationQual> EducationQual { get; set; }
    public DbSet<JobGrade> JobGrade { get; set; }
    public DbSet<Position> Position { get; set; }
    public DbSet<PositionBenefit> PositionBenefit { get; set; }
    public DbSet<PositionEducation> PositionEducation { get; set; }
    public DbSet<PositionExp> PositionExp { get; set; }
    public DbSet<PositionReq> PositionReq { get; set; }
    public DbSet<SalaryScale> SalaryScale { get; set; }

}

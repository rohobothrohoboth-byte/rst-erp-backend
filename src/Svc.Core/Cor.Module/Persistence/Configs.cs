using Cor.Module.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cor.Module.Persistence;

// Base Entity Configuration
public abstract class BaseEntityConfig<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.DateAdd).IsRequired().HasColumnType("timestamp with time zone");
        b.Property(x => x.DateMod).HasColumnType("timestamp with time zone");
        b.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        b.Property(x => x.xmin).HasColumnName("xmin").HasColumnType("xid").IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
        b.HasIndex(x => x.IsDeleted);
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

// Company Configuration
public class CompanyConfig : BaseEntityConfig<Company>
{
    public override void Configure(EntityTypeBuilder<Company> b)
    {
        base.Configure(b);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameAm).IsRequired().HasMaxLength(200);
        b.Property(x => x.TaxId).HasMaxLength(50);
        b.Property(x => x.Phone).HasMaxLength(50);
        b.Property(x => x.Email).HasMaxLength(100);
        b.Property(x => x.Address).HasMaxLength(500);
        b.Property(x => x.LogoUrl).HasMaxLength(500);
        b.HasIndex(x => x.Name).IsUnique();
        b.HasIndex(x => x.TaxId).IsUnique().HasFilter("\"TaxId\" IS NOT NULL");
    }
}

public class BranchConfig : BaseEntityConfig<Branch>
{
    public override void Configure(EntityTypeBuilder<Branch> b)
    {
        base.Configure(b);

        // ?? REMOVE all sequence and default value logic
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameAm).HasMaxLength(200);
        b.Property(x => x.Code).IsRequired().HasMaxLength(10);  // ? No default value
        b.Property(x => x.Location).HasMaxLength(200);
        b.Property(x => x.OpenDate).IsRequired();
        b.Property(x => x.BranchType).HasMaxLength(50).IsRequired();
        b.Property(x => x.BranchStat).HasMaxLength(20).IsRequired();
        b.Property(x => x.CompId).IsRequired();

        b.HasOne(x => x.Comp)
            .WithMany(x => x.Branches)
            .HasForeignKey(x => x.CompId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Code).IsUnique();
        b.HasIndex(x => x.Name).IsUnique();
        b.HasIndex(x => x.CompId);
        b.HasIndex(x => new { x.CompId, x.Name }).IsUnique();
    }
}
// Department Configuration
public class DepartmentConfig : BaseEntityConfig<Department>
{
    public override void Configure(EntityTypeBuilder<Department> b)
    {
        base.Configure(b);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameAm).HasMaxLength(200);
        b.Property(x => x.DeptStat).HasMaxLength(20).IsRequired();
        b.Property(x => x.BranchId).IsRequired();

        b.HasOne(x => x.Branch)
            .WithMany(x => x.Departments)
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.BranchId);
        b.HasIndex(x => new { x.BranchId, x.Name }).IsUnique();
    }
}

// FiscalYear Configuration
public class FiscalYearConfig : BaseEntityConfig<FiscalYear>
{
    public override void Configure(EntityTypeBuilder<FiscalYear> b)
    {
        base.Configure(b);
        b.Property(x => x.Name).IsRequired().HasMaxLength(50);
        b.Property(x => x.DateStart).IsRequired();
        b.Property(x => x.DateEnd).IsRequired();
        b.Property(x => x.IsActive).HasMaxLength(3).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
        b.HasIndex(x => x.IsActive);
        b.HasIndex(x => new { x.DateStart, x.DateEnd });
    }
}

// Period Configuration
public class PeriodConfig : BaseEntityConfig<Period>
{
    public override void Configure(EntityTypeBuilder<Period> b)
    {
        base.Configure(b);
        b.Property(x => x.Name).IsRequired().HasMaxLength(50);
        b.Property(x => x.DateStart).IsRequired();
        b.Property(x => x.DateEnd).IsRequired();
        b.Property(x => x.IsActive).HasMaxLength(3).IsRequired();
        b.Property(x => x.Quarter).HasMaxLength(10).IsRequired();
        b.Property(x => x.FiscalYearId).IsRequired();

        // Now FiscalYear has Periods navigation property
        b.HasOne(x => x.FiscalYear)
            .WithMany(x => x.Periods) // This now exists
            .HasForeignKey(x => x.FiscalYearId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.FiscalYearId);
        b.HasIndex(x => x.IsActive);
        b.HasIndex(x => new { x.FiscalYearId, x.Name }).IsUnique();
    }
}

// Holiday Configuration
public class HolidayConfig : BaseEntityConfig<Holiday>
{
    public override void Configure(EntityTypeBuilder<Holiday> b)
    {
        base.Configure(b);
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.Property(x => x.Date).IsRequired();
        b.Property(x => x.IsPublic).IsRequired();
        b.Property(x => x.FiscalYearId).IsRequired();

        // Now FiscalYear has Holidays navigation property
        b.HasOne(x => x.FiscalYear)
            .WithMany(x => x.Holidays) // This now exists
            .HasForeignKey(x => x.FiscalYearId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.FiscalYearId);
        b.HasIndex(x => x.Date);
        b.HasIndex(x => new { x.FiscalYearId, x.Date }).IsUnique();
    }
}
using Cor.Module.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cor.Module.Persistence;

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

public class BranchConfiguration : BaseEntityConfig<Branch>
{
    public override void Configure(EntityTypeBuilder<Branch> b)
    {
        base.Configure(b);
        b.Property(b => b.Name).IsRequired().HasMaxLength(200);
        b.Property(b => b.NameAm).HasMaxLength(200);
        b.Property(x => x.Code).HasMaxLength(10).IsRequired().HasDefaultValueSql("'BR-' || LPAD(nextval('bra_code_seq')::text, 7, '0')").ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        b.Property(b => b.Location).HasMaxLength(200);
        b.Property(b => b.OpenDate).IsRequired();
        b.Property(b => b.BranchType).HasMaxLength(50).IsRequired();
        b.Property(b => b.BranchStat).HasMaxLength(20).IsRequired();
        b.Property(b => b.CompId).IsRequired();
        b.HasOne(b => b.Comp).WithMany().HasForeignKey(b => b.CompId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(b => b.Code).IsUnique();
        b.HasIndex(b => b.Name).IsUnique();
        b.HasIndex(b => b.CompId);
        b.HasIndex(b => new { b.CompId, b.Name }).IsUnique();
    }
}

public class CompanyConfiguration : BaseEntityConfig<Company>
{
    public override void Configure(EntityTypeBuilder<Company> b)
    {
        base.Configure(b);
        b.Property(c => c.Name).IsRequired().HasMaxLength(200);
        b.Property(c => c.NameAm).IsRequired().HasMaxLength(200);
        b.HasIndex(c => c.Name).IsUnique();
    }
}

public class DepartmentConfiguration : BaseEntityConfig<Department>
{
    public override void Configure(EntityTypeBuilder<Department> b)
    {
        base.Configure(b);
        b.Property(d => d.Name).IsRequired().HasMaxLength(200);
        b.Property(d => d.NameAm).HasMaxLength(200);
        b.Property(d => d.DeptStat).HasMaxLength(20).IsRequired();
        b.Property(d => d.BranchId).IsRequired();
        b.HasOne(d => d.Branch).WithMany().HasForeignKey(d => d.BranchId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(d => d.BranchId);
        b.HasIndex(d => new { d.BranchId, d.Name }).IsUnique();
    }
}

public class FiscalYearConfiguration : BaseEntityConfig<FiscalYear>
{
    public override void Configure(EntityTypeBuilder<FiscalYear> b)
    {
        base.Configure(b);
        b.Property(fy => fy.Name).IsRequired().HasMaxLength(50);
        b.Property(fy => fy.DateStart).IsRequired();
        b.Property(fy => fy.DateEnd).IsRequired();
        b.Property(fy => fy.IsActive).HasMaxLength(3).IsRequired();
        b.HasIndex(fy => fy.Name).IsUnique();
        b.HasIndex(fy => fy.IsActive);
    }
}

public class HolidayConfiguration : BaseEntityConfig<Holiday>
{
    public override void Configure(EntityTypeBuilder<Holiday> b)
    {
        base.Configure(b);
        b.Property(h => h.Name).IsRequired().HasMaxLength(100);
        b.Property(h => h.Date).IsRequired();
        b.Property(h => h.IsPublic).IsRequired();
        b.Property(h => h.FiscalYearId).IsRequired();
        b.HasOne(h => h.FiscalYear).WithMany().HasForeignKey(h => h.FiscalYearId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(h => h.FiscalYearId);
        b.HasIndex(h => h.Date);
        b.HasIndex(h => new { h.FiscalYearId, h.Date }).IsUnique();
    }
}

public class PeriodConfiguration : BaseEntityConfig<Period>
{
    public override void Configure(EntityTypeBuilder<Period> b)
    {
        base.Configure(b);
        b.Property(p => p.Name).IsRequired().HasMaxLength(50);
        b.Property(p => p.DateStart).IsRequired();
        b.Property(p => p.DateEnd).IsRequired();
        b.Property(p => p.IsActive).HasMaxLength(3).IsRequired();
        b.Property(p => p.Quarter).HasMaxLength(10).IsRequired();
        b.Property(p => p.FiscalYearId).IsRequired();
        b.HasOne(p => p.FiscalYear).WithMany().HasForeignKey(p => p.FiscalYearId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(p => p.FiscalYearId);
        b.HasIndex(p => p.IsActive);
        b.HasIndex(p => new { p.FiscalYearId, p.Name }).IsUnique();
    }
}
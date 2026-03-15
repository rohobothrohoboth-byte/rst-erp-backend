using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Persistence;

public abstract class BaseEntityConfig<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.DateAdd).IsRequired().HasColumnType("timestamp with time zone");
        b.Property(x => x.DateMod).HasColumnType("timestamp with time zone");
        b.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        b.HasIndex(x => x.IsDeleted);
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class PerApiConfig : BaseEntityConfig<PerApi>
{
    public override void Configure(EntityTypeBuilder<PerApi> b)
    {
        base.Configure(b);
        b.Property(b => b.Key).IsRequired().HasMaxLength(200);
        b.Property(b => b.Desc).IsRequired().HasMaxLength(200);
        b.HasIndex(b => b.Key).IsUnique();
        b.HasIndex(b => b.Desc);
        b.HasIndex(b => b.PerMenuId);
        b.HasIndex(b => new { b.PerMenuId, b.Key }).IsUnique();
    }
}

public class PerMenuConfig : BaseEntityConfig<PerMenu>
{
    public override void Configure(EntityTypeBuilder<PerMenu> b)
    {
        base.Configure(b);
        b.Property(b => b.Key).IsRequired().HasMaxLength(200);
        b.Property(b => b.Label).IsRequired().HasMaxLength(200);
        b.Property(b => b.Path).IsRequired().HasMaxLength(200);
        b.Property(b => b.Icon).IsRequired().HasMaxLength(200);
        b.Property(b => b.IsChild).IsRequired();
        b.Property(b => b.Order).IsRequired();
        b.Property(b => b.PerModuleId).IsRequired();
        b.HasIndex(b => b.Key).IsUnique();
        b.HasIndex(b => b.PerModuleId);
        b.HasIndex(b => b.ParentId);
        b.HasIndex(b => new { b.PerModuleId, b.Key }).IsUnique();
    }
}

public class PerModuleConfig : BaseEntityConfig<PerModule>
{
    public override void Configure(EntityTypeBuilder<PerModule> b)
    {
        base.Configure(b);
        b.Property(b => b.Key).IsRequired().HasMaxLength(200);
        b.Property(b => b.Desc).HasMaxLength(200);
        b.HasIndex(b => b.Key).IsUnique();
        b.HasIndex(b => b.Desc);
    }
}

public class RefreshTokenConfig : BaseEntityConfig<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        base.Configure(b);
        b.HasIndex(b => b.Token).IsUnique();
        b.HasIndex(b => b.IsRevoked);
        b.HasIndex(b => b.UserId);
        b.HasIndex(b => b.ExpiryDate);
        b.HasIndex(b => b.RevokedDate);
    }
}

public class UserPerApiConfig : BaseEntityConfig<UserPerApi>
{
    public override void Configure(EntityTypeBuilder<UserPerApi> b)
    {
        base.Configure(b);
        b.Property(b => b.UserId).IsRequired().HasMaxLength(100);
        b.Property(b => b.PerApiId).IsRequired().HasMaxLength(100);
        b.HasIndex(b => b.UserId);
        b.HasIndex(b => b.PerApiId);
        b.HasIndex(b => new { b.UserId, b.PerApiId }).IsUnique();
    }
}

public class UserPerMenuConfig : BaseEntityConfig<UserPerMenu>
{
    public override void Configure(EntityTypeBuilder<UserPerMenu> b)
    {
        base.Configure(b);
        b.Property(b => b.UserId).IsRequired().HasMaxLength(100);
        b.Property(b => b.PerMenuId).IsRequired().HasMaxLength(100);
        b.HasIndex(b => b.UserId);
        b.HasIndex(b => b.PerMenuId);
        b.HasIndex(b => new { b.UserId, b.PerMenuId }).IsUnique();
    }
}

public class UserPerModuleConfig : BaseEntityConfig<UserPerModule>
{
    public override void Configure(EntityTypeBuilder<UserPerModule> b)
    {
        base.Configure(b);
        b.Property(b => b.UserId).IsRequired().HasMaxLength(100);
        b.Property(b => b.PerModuleId).IsRequired().HasMaxLength(100);
        b.HasIndex(b => b.UserId);
        b.HasIndex(b => b.PerModuleId);
        b.HasIndex(b => new { b.UserId, b.PerModuleId }).IsUnique();
    }
}
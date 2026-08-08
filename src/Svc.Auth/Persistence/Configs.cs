using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Persistence;

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
        b.HasIndex(x => x.IsDeleted);
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

// PerModule Configuration
public class PerModuleConfig : BaseEntityConfig<PerModule>
{
    public override void Configure(EntityTypeBuilder<PerModule> b)
    {
        base.Configure(b);
        b.Property(x => x.Key).IsRequired().HasMaxLength(200);
        b.Property(x => x.Desc).HasMaxLength(200);
        b.Property(x => x.Icon).HasMaxLength(50);
        b.Property(x => x.Order).IsRequired();
        b.HasIndex(x => x.Key).IsUnique();
        b.HasIndex(x => x.Desc);
        b.HasIndex(x => x.Order);
    }
}

// PerMenu Configuration
public class PerMenuConfig : BaseEntityConfig<PerMenu>
{
    public override void Configure(EntityTypeBuilder<PerMenu> b)
    {
        base.Configure(b);
        b.Property(x => x.Key).IsRequired().HasMaxLength(200);
        b.Property(x => x.Label).IsRequired().HasMaxLength(200);
        b.Property(x => x.Path).IsRequired().HasMaxLength(200);
        b.Property(x => x.Icon).IsRequired().HasMaxLength(200);
        b.Property(x => x.IsChild).IsRequired();
        b.Property(x => x.Order).IsRequired();
        b.Property(x => x.PerModuleId).IsRequired();
        b.Property(x => x.ParentId).IsRequired(false);

        b.HasOne(x => x.PerModule)
            .WithMany(x => x.PerMenus)
            .HasForeignKey(x => x.PerModuleId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Key).IsUnique();
        b.HasIndex(x => x.PerModuleId);
        b.HasIndex(x => x.ParentId);
        b.HasIndex(x => new { x.PerModuleId, x.Key }).IsUnique();
    }
}

// PerApi Configuration
public class PerApiConfig : BaseEntityConfig<PerApi>
{
    public override void Configure(EntityTypeBuilder<PerApi> b)
    {
        base.Configure(b);
        b.Property(x => x.Key).IsRequired().HasMaxLength(200);
        b.Property(x => x.Desc).IsRequired().HasMaxLength(200);
        b.Property(x => x.PerMenuId).IsRequired();

        b.HasOne(x => x.PerMenu)
            .WithMany(x => x.PerApis)
            .HasForeignKey(x => x.PerMenuId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Key).IsUnique();
        b.HasIndex(x => x.Desc);
        b.HasIndex(x => x.PerMenuId);
        b.HasIndex(x => new { x.PerMenuId, x.Key }).IsUnique();
    }
}

// RefreshToken Configuration
public class RefreshTokenConfig : BaseEntityConfig<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        base.Configure(b);
        b.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        b.Property(x => x.Token).IsRequired();
        b.Property(x => x.ExpiryDate).IsRequired();
        b.Property(x => x.IsRevoked).IsRequired();

        b.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.Token).IsUnique();
        b.HasIndex(x => x.IsRevoked);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.ExpiryDate);
        b.HasIndex(x => x.RevokedDate);
    }
}

// UserPerModule Configuration
public class UserPerModuleConfig : BaseEntityConfig<UserPerModule>
{
    public override void Configure(EntityTypeBuilder<UserPerModule> b)
    {
        base.Configure(b);
        b.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        b.Property(x => x.PerModuleId).IsRequired();

        b.HasOne(x => x.User)
            .WithMany(x => x.PerModule)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.PerModule)
            .WithMany(x => x.UserPerModules)
            .HasForeignKey(x => x.PerModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.PerModuleId);
        b.HasIndex(x => new { x.UserId, x.PerModuleId }).IsUnique();
    }
}

// UserPerMenu Configuration
public class UserPerMenuConfig : BaseEntityConfig<UserPerMenu>
{
    public override void Configure(EntityTypeBuilder<UserPerMenu> b)
    {
        base.Configure(b);
        b.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        b.Property(x => x.PerMenuId).IsRequired();

        b.HasOne(x => x.User)
            .WithMany(x => x.PerMenu)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.PerMenu)
            .WithMany(x => x.UserPerMenus)
            .HasForeignKey(x => x.PerMenuId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.PerMenuId);
        b.HasIndex(x => new { x.UserId, x.PerMenuId }).IsUnique();
    }
}

// UserPerApi Configuration
public class UserPerApiConfig : BaseEntityConfig<UserPerApi>
{
    public override void Configure(EntityTypeBuilder<UserPerApi> b)
    {
        base.Configure(b);
        b.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        b.Property(x => x.PerApiId).IsRequired();

        b.HasOne(x => x.User)
            .WithMany(x => x.PerApi)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.PerApi)
            .WithMany(x => x.UserPerApis)
            .HasForeignKey(x => x.PerApiId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.PerApiId);
        b.HasIndex(x => new { x.UserId, x.PerApiId }).IsUnique();
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Persistence;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasPostgresExtension("pg_trgm");

        builder.Entity<AppUser>().ToTable("AppUser");
        builder.Entity<AppRole>().ToTable("AppRole");
        builder.Entity<IdentityUser>().ToTable("User");
        builder.Entity<IdentityRole>().ToTable("Role");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");

        builder.Entity<PerModule>().HasIndex(p => p.Key).IsUnique();
        builder.Entity<UserPerModule>().HasKey(up => new { up.UserId, up.PerModuleId });
        builder.Entity<UserPerMenu>().HasKey(up => new { up.UserId, up.PerMenuId });
        builder.Entity<UserPerApi>().HasKey(up => new { up.UserId, up.PerApiId });
        builder.Entity<AppUser>(en =>
        {
            en.HasIndex(e => e.EmployeeId).IsUnique();
            en.HasIndex(e => e.Id).IsUnique();
        });
        builder.Entity<AppRole>(en =>
        {
            en.HasIndex(e => e.Name).IsUnique();
            en.HasIndex(e => e.Id).IsUnique();
        });

        builder.Entity<RefreshToken>(en =>
        {
            en.HasKey(e => e.Id);
            en.Property(e => e.UserId).HasMaxLength(300);
            en.Property(e => e.Token).HasMaxLength(1000);
            en.HasIndex(e => e.Token).IsUnique();
            en.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<PerMenu>(en =>
        {
            en.HasIndex(e => e.Id).IsUnique();
            en.HasIndex(e => e.Key).IsUnique();
            en.HasIndex(e => e.PerModuleId).IsUnique();
        });
        builder.Entity<PerApi>(en =>
        {
            en.HasIndex(e => e.Id).IsUnique();
            en.HasIndex(e => e.Key).IsUnique();
            en.HasIndex(e => e.PerMenuId).IsUnique();
        });
    }

    public DbSet<PerModule> PerModule { get; set; }
    public DbSet<PerMenu> PerMenu { get; set; }
    public DbSet<PerApi> PerApi { get; set; }
    public DbSet<UserPerModule> UserPerModule { get; set; }
    public DbSet<UserPerMenu> UserPerMenu { get; set; }
    public DbSet<UserPerApi> UserPerApi { get; set; }



}
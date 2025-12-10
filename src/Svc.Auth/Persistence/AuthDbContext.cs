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

        builder.Entity<AppUser>().HasIndex(p => p.EmployeeId).IsUnique();
        builder.Entity<AppUser>().HasIndex(p => p.Id).IsUnique();
        builder.Entity<AppRole>().HasIndex(p => p.Name).IsUnique();
        builder.Entity<AppRole>().HasIndex(p => p.Id).IsUnique();
        builder.Entity<PerModule>().HasIndex(p => p.Key).IsUnique();
        builder.Entity<PerMenu>().HasIndex(p => p.Key).IsUnique();
        builder.Entity<PerApi>().HasIndex(p => p.Key).IsUnique();
        builder.Entity<UserPerModule>().HasKey(up => new { up.UserId, up.PerModuleId });
        builder.Entity<UserPerMenu>().HasKey(up => new { up.UserId, up.PerMenuId });
        builder.Entity<UserPerApi>().HasKey(up => new { up.UserId, up.PerApiId });
    }

    public DbSet<PerModule> PerModule { get; set; }
    public DbSet<PerMenu> PerMenu { get; set; }
    public DbSet<PerApi> PerApi { get; set; }
    public DbSet<UserPerModule> UserPerModule { get; set; }
    public DbSet<UserPerMenu> UserPerMenu { get; set; }
    public DbSet<UserPerApi> UserPerApi { get; set; }



}
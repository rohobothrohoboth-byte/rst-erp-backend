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

        builder.Entity<AppUser>().ToTable("UserProfile");
        builder.Entity<IdentityUser>().ToTable("User");
        builder.Entity<IdentityRole>().ToTable("Role");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");

        builder.Entity<AppUser>().HasIndex(p => p.EmployeeId).HasMethod("gin").IsUnique();
        builder.Entity<PerModule>().HasIndex(p => p.Key).HasMethod("gin").IsUnique();
        builder.Entity<PerMenu>().HasIndex(p => p.Key).HasMethod("gin").IsUnique();
        builder.Entity<PerApi>().HasIndex(p => p.Key).HasMethod("gin").IsUnique();
    }

    public DbSet<PerModule> PerModule { get; set; }
    public DbSet<PerMenu> PerMenu { get; set; }
    public DbSet<PerApi> PerApi { get; set; }



}
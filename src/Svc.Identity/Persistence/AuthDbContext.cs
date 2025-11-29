using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Svc.Identity.Entities;

namespace Svc.Identity.Persistence;

public class AuthDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserPermission>().HasKey(up => new { up.UserId, up.PermissionId });
        // Seed initial permissions if needed
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
}
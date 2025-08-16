using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RST.Auth.API.Models;

namespace RST.Auth.API.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<RevokedAccessToken> RevokedAccessTokens => Set<RevokedAccessToken>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
            b.Entity<RolePermission>().HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);
            b.Entity<RolePermission>().HasOne(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionId);
            b.Entity<Permission>().HasIndex(x => x.Name).IsUnique();
            b.Entity<RefreshToken>().HasOne(x => x.User).WithMany(u => u.RefreshTokens).HasForeignKey(x => x.UserId);
            b.Entity<RevokedAccessToken>().HasIndex(x => x.Jti).IsUnique();
        }
    }
}

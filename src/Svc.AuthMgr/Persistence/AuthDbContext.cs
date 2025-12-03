using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Svc.AuthMgr.Persistence;

public class AuthDbContext(DbContextOptions<AuthDbContext> options)  : IdentityDbContext (options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<IdentityUser>().ToTable("User");
        builder.Entity<IdentityRole>().ToTable("Role");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
    }


    

}

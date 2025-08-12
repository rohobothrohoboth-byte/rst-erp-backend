using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RST.Auth.API.Data;
using RST.Auth.API.Models;

namespace RST.Auth.API.Services
{
    public static class SeedData
    {
        public static async Task EnsureSeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            var permissions = new[] {
                "permissions.manage",
                "roles.manage",
                "users.manage",
                "introspect.access"
            };

            foreach (var perm in permissions)
            {
                if (!await db.Permissions.AnyAsync(p => p.Name == perm))
                    db.Permissions.Add(new Permission { Name = perm });
            }
            await db.SaveChangesAsync();

            // Roles
            var adminRole = "Administrator";
            if (!await roleMgr.RoleExistsAsync(adminRole))
                await roleMgr.CreateAsync(new IdentityRole(adminRole));

            // Map all permissions to Administrator
            var allPerms = await db.Permissions.ToListAsync();
            var adminRoleEntity = await roleMgr.FindByNameAsync(adminRole);

            foreach (var perm in allPerms)
            {
                if (!await db.RolePermissions.AnyAsync(rp => rp.RoleId == adminRoleEntity.Id && rp.PermissionId == perm.Id))
                {
                    db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = adminRoleEntity.Id,
                        PermissionId = perm.Id
                    });
                }
            }
            await db.SaveChangesAsync();

            // Create admin user
            var adminEmail = "admin@example.com";
            if (await userMgr.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userMgr.CreateAsync(adminUser, "Admin#1234");
                if (result.Succeeded)
                {
                    await userMgr.AddToRoleAsync(adminUser, adminRole);
                }
            }
        }
    }
}

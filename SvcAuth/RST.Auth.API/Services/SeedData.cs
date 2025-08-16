using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RST.Auth.API.Data;
using RST.Auth.API.Models;

namespace RST.Auth.API.Services;

public static class SeedData
{
    //public static async Task EnsureAsync(IServiceProvider sp)
    //{
    //    using var scope = sp.CreateScope();
    //    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    //    var um = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    //    var rm = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    //    await db.Database.MigrateAsync();

    //    // roles
    //    var roles = new[] { "Admin", "User" };
    //    foreach (var r in roles)
    //    {
    //        if (!await rm.RoleExistsAsync(r))
    //        {
    //            await rm.CreateAsync(new IdentityRole(r));
    //        }
    //    }

    //    // permissions
    //    var perms = new[]
    //    {
    //        "permissions.manage", "roles.manage", "introspect.access", "keys.rotate", "branch.read", "branch.write"
    //    };

    //    foreach (var p in perms)
    //    {
    //        if (!await db.Permissions.AnyAsync(x => x.Name == p))
    //        {
    //            db.Permissions.Add(new Permission { Name = p });
    //        }
    //    }
    //    await db.SaveChangesAsync();

    //    // map Admin -> all permissions
    //    var admin = await rm.FindByNameAsync("Admin");
    //    var permIds = await db.Permissions.Select(p => p.Id).ToListAsync();
    //    foreach (var pid in permIds)
    //    {
    //        if (!await db.RolePermissions.AnyAsync(rp => rp.RoleId == admin!.Id && rp.PermissionId == pid))
    //        {
    //            db.RolePermissions.Add(new RolePermission { RoleId = admin!.Id, PermissionId = pid });
    //        }
    //    }
    //    await db.SaveChangesAsync();

    //    // admin user
    //    var adminUser = await um.FindByNameAsync("admin");
    //    if (adminUser == null)
    //    {
    //        adminUser = new ApplicationUser { UserName = "admin", Email = "admin@example.com" };
    //        await um.CreateAsync(adminUser, "Admin#1234");
    //        await um.AddToRoleAsync(adminUser, "Admin");
    //    }
    //}
    public static async Task EnsureAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var um = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var rm = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Apply migrations
        Console.WriteLine("[SeedData] Applying migrations...");
        await db.Database.MigrateAsync();

        // 1️⃣ Roles
        var roles = new[] { "Admin", "User" };
        foreach (var r in roles)
        {
            if (!await rm.RoleExistsAsync(r))
            {
                await rm.CreateAsync(new IdentityRole(r));
                Console.WriteLine($"[SeedData] Created role: {r}");
            }
        }

        // 2️⃣ Permissions
        var perms = new[] { "permissions.manage", "roles.manage", "introspect.access", "keys.rotate", "branch.read", "branch.write" };
        var existingPerms = await db.Permissions.Select(p => p.Name).ToListAsync();
        var missingPerms = perms.Except(existingPerms).ToList();
        if (missingPerms.Any())
        {
            db.Permissions.AddRange(missingPerms.Select(p => new Permission { Name = p }));
            await db.SaveChangesAsync();
            Console.WriteLine($"[SeedData] Added permissions: {string.Join(", ", missingPerms)}");
        }

        // 3️⃣ Map Admin -> all permissions
        var adminRole = await rm.FindByNameAsync("Admin");
        var permIds = await db.Permissions.Select(p => p.Id).ToListAsync();
        foreach (var pid in permIds)
        {
            if (!await db.RolePermissions.AnyAsync(rp => rp.RoleId == adminRole.Id && rp.PermissionId == pid))
            {
                db.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = pid });
            }
        }

        // 4️⃣ Map User -> branch.read only
        var userRole = await rm.FindByNameAsync("User");
        var readPermId = await db.Permissions.Where(p => p.Name == "branch.read").Select(p => p.Id).FirstOrDefaultAsync();
        if (!await db.RolePermissions.AnyAsync(rp => rp.RoleId == userRole.Id && rp.PermissionId == readPermId))
        {
            db.RolePermissions.Add(new RolePermission { RoleId = userRole.Id, PermissionId = readPermId });
        }
        await db.SaveChangesAsync();

        // 5️⃣ Admin user
        var adminUser = await um.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@example.com",
                EmailConfirmed = true
            };
            await um.CreateAsync(adminUser, "Admin#1234");
            await um.AddToRoleAsync(adminUser, "Admin");
            Console.WriteLine("[SeedData] Admin user created: admin@example.com / Admin#1234");
        }
        else
        {
            Console.WriteLine("[SeedData] Admin user already exists.");
        }
    }
}
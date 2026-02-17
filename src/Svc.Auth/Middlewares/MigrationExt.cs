using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Models.Entities;
using Svc.Auth.Persistence;
using Svc.Auth.Seeder;

namespace Svc.Auth.Middlewares;

public static class MigrationExt
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        dbContext.Database.Migrate();
    }

    public static async Task ApplyAdminRole(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var rMgr = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var uMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roles = RoleSeeder.GetRoles().ToList();
        foreach (var role in roles)
        {
            if (string.IsNullOrEmpty(role.Name))
            {
                continue;
            }
            if (!await rMgr.RoleExistsAsync(role.Name))
            {
                await rMgr.CreateAsync(role);
            }
            else
            {
                var existing = await rMgr.FindByNameAsync(role.Name);
                if (existing != null && existing.Desc == role.Desc)
                {
                    continue;
                }

                if (existing != null)
                {
                    existing.Desc = role.Desc;
                    await rMgr.UpdateAsync(existing);
                }
            }
        }
        await RoleSeeder.SeedAdmin(uMgr, rMgr);
    }

    public static async Task ApplyPerModSeed(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        if (!await dbContext.Database.CanConnectAsync()) { return; }

        var dbPermissions = dbContext.PerModule.Select(x => x.Key).ToList();
        var existingKeys = new HashSet<string>(dbPermissions, StringComparer.OrdinalIgnoreCase);
        var seedList = PerModuleSeeder.GetPerModule().ToList();
        var newPermissions = seedList.Where(p => !existingKeys.Contains(p.Key)).ToList();
        if (newPermissions.Any())
        {
            await dbContext.PerModule.AddRangeAsync(newPermissions);
            await dbContext.SaveChangesAsync();
        }
    }
}
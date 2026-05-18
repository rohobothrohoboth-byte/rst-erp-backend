using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Commands;
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

    public static async Task SeedPerModule(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        if (!await dbContext.Database.CanConnectAsync()) { return; }

        var dbPer = await dbContext.PerModule.AsNoTracking().Select(x => x.Key).ToListAsync();
        var addedKeys = new HashSet<string>(dbPer, StringComparer.OrdinalIgnoreCase);
        var seedList = SeedPerList.GetPerModule().ToList();
        var newPer = seedList.Where(p => !addedKeys.Contains(p.Key)).ToList();
        if (newPer.Count != 0)
        {
            await dbContext.PerModule.AddRangeAsync(newPer);
            await dbContext.SaveChangesAsync();
        }
    }

    public static async Task SeedPerMenu(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            if (!await dbContext.Database.CanConnectAsync()) { return; }
            var existingKeys = await dbContext.PerMenu.AsNoTracking().Select(x => x.Key).ToListAsync();
            var existingSet = new HashSet<string>(existingKeys, StringComparer.OrdinalIgnoreCase);
            var seedItems = SeedPerList.GetPerMenu();
            var newItems = seedItems.Where(x => !existingSet.Contains(x.Key)).ToList();
            if (newItems.Count == 0) { return; }
            await mediator.Send(new PerMenuSeedCmd { AddDto = newItems });
        }
        catch
        {
            throw;
        }
    }

    //public static async Task SeedPerAccess(this IApplicationBuilder app)
    //{
    //    using var scope = app.ApplicationServices.CreateScope();
    //    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    //    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    //    try
    //    {
    //        if (!await dbContext.Database.CanConnectAsync()) { return; }
    //        var existingKeys = await dbContext.PerMenu.AsNoTracking().Select(x => x.Key).ToListAsync();
    //        var existingSet = new HashSet<string>(existingKeys, StringComparer.OrdinalIgnoreCase);
    //        var seedItems = SeedPerList.GetPerMenu();
    //        var newItems = seedItems.Where(x => !existingSet.Contains(x.Key)).ToList();
    //        if (newItems.Count == 0) { return; }
    //        await mediator.Send(new PerMenuSeedCmd { AddDto = newItems });
    //    }
    //    catch
    //    {
    //        throw;
    //    }
    //}
}
using Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Commands;
using Svc.Auth.Models.Entities;
using Svc.Auth.Persistence;
using Svc.Auth.Seeder;
using Svc.Auth.Queries;
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
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
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
                    logger.LogInformation($"Created role: {role.Name}");
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
                        logger.LogInformation($"Updated role: {role.Name}");
                    }
                }
            }

            await RoleSeeder.SeedAdmin(uMgr, rMgr);
            logger.LogInformation("Admin role seeding completed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error applying admin role");
            throw;
        }
    }

   public static async Task SeedPerModule(this IApplicationBuilder app)
   {
       using var scope = app.ApplicationServices.CreateScope();
       await using var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
       var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

       try
       {
           if (!await dbContext.Database.CanConnectAsync())
           {
               logger.LogWarning("Cannot connect to database for module seeding");
               return;
           }

           // Include soft-deleted modules
           var dbPer = await dbContext.PerModule
               .IgnoreQueryFilters()
               .Select(x => new { x.Key, x.IsDeleted })
               .ToListAsync();

           var existingKeys = dbPer.Where(x => !x.IsDeleted).Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
           var softDeletedKeys = dbPer.Where(x => x.IsDeleted).Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

           var seedList = SeedPerList.GetPerModule().ToList();
           logger.LogInformation($"Found {seedList.Count} modules in seed data");

           var newModules = seedList.Where(p => !existingKeys.Contains(p.Key) && !softDeletedKeys.Contains(p.Key)).ToList();
           var softDeletedModules = seedList.Where(p => softDeletedKeys.Contains(p.Key)).ToList();

           // Reactivate soft-deleted modules
           if (softDeletedModules.Any())
           {
               foreach (var module in softDeletedModules)
               {
                   var entity = await dbContext.PerModule
                       .IgnoreQueryFilters()
                       .FirstOrDefaultAsync(x => x.Key == module.Key);

                   if (entity != null)
                   {
                       entity.IsDeleted = false;
                       entity.Desc = module.Desc;
                       entity.Icon = module.Icon;
                       entity.Order = module.Order;
                       entity.DateMod = DateTime.UtcNow;
                       logger.LogInformation($"?? Reactivated module: {module.Key}");
                   }
               }
               await dbContext.SaveChangesAsync();
           }

           // Add new modules
           if (newModules.Any())
           {
               logger.LogInformation($"Adding {newModules.Count} new modules");
               await dbContext.PerModule.AddRangeAsync(newModules);
               await dbContext.SaveChangesAsync();
               logger.LogInformation($"? Added {newModules.Count} modules");
           }

           logger.LogInformation("? Module seeding completed");
       }
       catch (Exception ex)
       {
           logger.LogError(ex, "Error seeding modules");
           throw;
       }
   }

 public static async Task SeedPerMenu(this IApplicationBuilder app)
 {
     using var scope = app.ApplicationServices.CreateScope();
     var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
     var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
     var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

     try
     {
         if (!await dbContext.Database.CanConnectAsync())
         {
             logger.LogWarning("Cannot connect to database for menu seeding");
             return;
         }

         var existingMenus = await dbContext.PerMenu
             .IgnoreQueryFilters()
             .Select(x => new { x.Key, x.IsDeleted })
             .ToListAsync();

         var existingKeys = existingMenus.Where(x => !x.IsDeleted).Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

         var seedItems = SeedPerList.GetPerMenu().ToList();
         logger.LogInformation($"Found {seedItems.Count} menus in seed data");

         // Prune stale menus: soft-delete active menus that are no longer defined
         // in the seeder. This removes leftovers/duplicates from older seeder
         // versions (e.g. renamed leave menus) that would otherwise keep showing
         // in the sidebar because seeding only ever adds, never removes.
         var seedKeys = seedItems.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
         var activeMenus = await dbContext.PerMenu.Where(m => !m.IsDeleted).ToListAsync();
         var staleMenus = activeMenus.Where(m => !seedKeys.Contains(m.Key)).ToList();
         if (staleMenus.Count > 0)
         {
             foreach (var stale in staleMenus)
             {
                 stale.IsDeleted = true;
                 stale.DateMod = DateTime.UtcNow;
                 logger.LogWarning($"Pruned stale menu not in seed list: {stale.Key} ({stale.Label})");
             }
             await dbContext.SaveChangesAsync();
             logger.LogInformation($"Pruned {staleMenus.Count} stale menu(s) not present in the seed list");
         }

         var itemsToProcess = seedItems
             .Where(x => !existingKeys.Contains(x.Key))
             .ToList();

         if (itemsToProcess.Count == 0)
         {
             logger.LogInformation("No new menus to seed");
             return;
         }

         logger.LogInformation($"Processing {itemsToProcess.Count} menu items");

         // ? Send to handler - UnitOfWork will handle the transaction and saving
         await mediator.Send(new PerMenuSeedCmd { AddDto = itemsToProcess });

         // ? Remove this - UnitOfWork.Commit already saves!
         // await dbContext.SaveChangesAsync();

         // ? Verify menus were saved
         var savedCount = await dbContext.PerMenu.CountAsync();
         logger.LogInformation($"? Menu seeding completed. Total menus in DB: {savedCount}");

         if (savedCount == 0 && itemsToProcess.Count > 0)
         {
             throw new Exception($"Menus were not saved! Processed {itemsToProcess.Count} items but DB shows 0.");
         }
     }
     catch (Exception ex)
     {
         logger.LogError(ex, "Error seeding menus");
         throw;
     }
 }
  public static async Task SeedPerAccess(this IApplicationBuilder app)
  {
      using var scope = app.ApplicationServices.CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

      try
      {
          if (!await dbContext.Database.CanConnectAsync())
          {
              logger.LogWarning("Cannot connect to database for API seeding");
              return;
          }

          // Get all existing APIs (including soft-deleted)
          var existingApis = await dbContext.PerApi
              .IgnoreQueryFilters()
              .Select(x => new { x.Key, x.IsDeleted })
              .ToListAsync();

          var existingKeys = existingApis.Where(x => !x.IsDeleted).Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

          var seedItems = SeedPerList.GetPerAccess().ToList();
          logger.LogInformation($"Found {seedItems.Count} APIs in seed data");

          var itemsToProcess = seedItems
              .Where(x => !existingKeys.Contains(x.Key))
              .ToList();

          if (itemsToProcess.Count == 0)
          {
              logger.LogInformation("No new APIs to seed");
              return;
          }

          logger.LogInformation($"Processing {itemsToProcess.Count} API items");

          // Send to handler - ExecuteInTransactionAsync handles the transaction
          await mediator.Send(new PerAccessSeedCmd { AddDto = itemsToProcess });

          // ? Remove this - ExecuteInTransactionAsync already saves
          // await dbContext.SaveChangesAsync();

          // Verify APIs were saved
          var savedCount = await dbContext.PerApi.CountAsync();
          logger.LogInformation($"? API seeding completed. Total APIs in DB: {savedCount}");
      }
      catch (Exception ex)
      {
          logger.LogError(ex, "Error seeding APIs");
          throw;
      }
  }


    // Initialize the in-memory permission registry (PermissionMap.IndexMap) from
    // the seeded permission keys so the JWT `ph` bitmask and [PerAuth] policies
    // cover every real permission, not just the legacy static list. Keys are
    // ordered deterministically so bit indices are stable across restarts.
    public static async Task InitializePermissionRegistry(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            if (!await dbContext.Database.CanConnectAsync())
            {
                logger.LogWarning("Cannot connect to database for permission registry init");
                return;
            }

            var keys = await dbContext.PerApi
                .Where(a => !a.IsDeleted)
                .Select(a => a.Key)
                .ToListAsync();

            PermissionMap.Initialize(keys);
            logger.LogInformation($"Permission registry initialized with {PermissionMap.IndexMap.Count} permissions");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error initializing permission registry");
        }
    }

    // Helper method to seed everything in the correct order
    public static async Task SeedAllPermissions(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Starting complete permission seeding...");

            // Seed in correct order
            logger.LogInformation("Step 1: Seeding modules...");
            await app.SeedPerModule();

            logger.LogInformation("Step 2: Seeding menus...");
            await app.SeedPerMenu();

            logger.LogInformation("Step 3: Seeding APIs...");
            await app.SeedPerAccess();

            logger.LogInformation("All permission seeding completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during complete permission seeding");
            throw;
        }
    }
}
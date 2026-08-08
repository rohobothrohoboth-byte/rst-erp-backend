using MediatR;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using Svc.Auth.Queries;

namespace Svc.Auth.Commands;

public sealed class PerMenuSeedCmd : IRequest { public List<PerMenuSeedDto> AddDto { get; set; } = default!; }
public sealed class PerAccessSeedCmd : IRequest { public List<PerAccessSeedDto> AddDto { get; set; } = default!; }

public sealed class PerMenuSeed(IUnitOfWork _uow) : IRequestHandler<PerMenuSeedCmd>
{
    public async Task Handle(PerMenuSeedCmd request, CancellationToken ct)
    {
        if (request.AddDto == null || request.AddDto.Count == 0)
        {
            Console.WriteLine("?? PerMenuSeed: No menu DTOs provided");
            return;
        }

        Console.WriteLine($"?? PerMenuSeed: Processing {request.AddDto.Count} menu DTOs");

        // ? STEP 1: Query data BEFORE starting the transaction
        var allMenus = await _uow.Set<PerMenu>()
            .IgnoreQueryFilters()
            .Select(m => new { m.Key, m.Id, m.IsDeleted })
            .ToListAsync(ct);

        var existingMenuKeys = new HashSet<string>(
            allMenus.Where(m => !m.IsDeleted).Select(m => m.Key),
            StringComparer.OrdinalIgnoreCase);

        var softDeletedKeys = allMenus
            .Where(m => m.IsDeleted)
            .Select(m => m.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Console.WriteLine($"?? Existing menus: {existingMenuKeys.Count}, Soft-deleted: {softDeletedKeys.Count}");

        // Get ALL modules from database
        var allModules = await _uow.Set<PerModule>()
            .Where(m => !m.IsDeleted)
            .Select(m => new { m.Key, m.Id })
            .ToListAsync(ct);

        var moduleDict = allModules.ToDictionary(
            x => x.Key,
            x => x.Id,
            StringComparer.OrdinalIgnoreCase);

        Console.WriteLine($"?? Found {moduleDict.Count} modules in database");

        // Check which DTOs have valid modules
        var validDtos = new List<PerMenuSeedDto>();
        foreach (var dto in request.AddDto)
        {
            if (string.IsNullOrWhiteSpace(dto.ModKey))
            {
                Console.WriteLine($"   ? Skipping {dto.Key}: No ModKey provided");
                continue;
            }

            if (moduleDict.TryGetValue(dto.ModKey, out _))
            {
                validDtos.Add(dto);
            }
            else
            {
                Console.WriteLine($"   ? Skipping {dto.Key}: Module '{dto.ModKey}' not found");
            }
        }

        Console.WriteLine($"?? Valid DTOs: {validDtos.Count}");

        if (validDtos.Count == 0)
        {
            Console.WriteLine("?? No valid DTOs with existing modules. Cannot proceed.");
            return;
        }

        // Separate parent and child menus
        var parentDtos = validDtos.Where(x => !x.IsChild).ToList();
        var childDtos = validDtos.Where(x => x.IsChild).ToList();

        Console.WriteLine($"?? Parents: {parentDtos.Count}, Children: {childDtos.Count}");

        // Build dictionary of existing parent IDs
        var menuIdDict = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        foreach (var menu in allMenus.Where(m => !m.IsDeleted))
        {
            menuIdDict[menu.Key] = menu.Id;
        }

        // ? STEP 2: Use ExecuteInTransactionAsync - DO NOT call Begin/Commit/Rollback inside
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var menusToAdd = new List<PerMenu>();

            // Process parent menus first
            Console.WriteLine("?? Processing parent menus...");
            foreach (var dto in parentDtos.OrderBy(x => x.Order))
            {
                if (existingMenuKeys.Contains(dto.Key))
                {
                    Console.WriteLine($"   ?? Parent already exists: {dto.Key}");
                    continue;
                }

                if (softDeletedKeys.Contains(dto.Key))
                {
                    var softDeletedEntity = await _uow.Set<PerMenu>()
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x => x.Key == dto.Key, ct);

                    if (softDeletedEntity != null)
                    {
                        softDeletedEntity.IsDeleted = false;
                        softDeletedEntity.Label = dto.Label;
                        softDeletedEntity.Path = dto.Path ?? "";
                        softDeletedEntity.Icon = dto.Icon ?? "";
                        softDeletedEntity.Order = dto.Order;
                        softDeletedEntity.DateMod = DateTime.UtcNow;

                        if (moduleDict.TryGetValue(dto.ModKey, out var newModuleId))
                        {
                            softDeletedEntity.PerModuleId = newModuleId;
                        }

                        await _uow.Update(softDeletedEntity);
                        menuIdDict[softDeletedEntity.Key] = softDeletedEntity.Id;
                        existingMenuKeys.Add(softDeletedEntity.Key);
                        Console.WriteLine($"   ?? Reactivated parent: {dto.Key}");
                    }
                    continue;
                }

                if (!moduleDict.TryGetValue(dto.ModKey, out var moduleId))
                {
                    Console.WriteLine($"   ? Skipping {dto.Key}: Module {dto.ModKey} not found");
                    continue;
                }

                var entity = new PerMenu
                {
                    Id = Guid.NewGuid(),
                    PerModuleId = moduleId,
                    Key = dto.Key,
                    Label = dto.Label,
                    Path = dto.Path ?? "",
                    Icon = dto.Icon ?? "",
                    IsChild = false,
                    ParentId = null,
                    Order = dto.Order,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                };

                menusToAdd.Add(entity);
                menuIdDict[entity.Key] = entity.Id;
                existingMenuKeys.Add(entity.Key);
                Console.WriteLine($"   ? Added parent: {dto.Key}");
            }

            // Process child menus
            Console.WriteLine("?? Processing child menus...");
            foreach (var dto in childDtos.OrderBy(x => x.Order))
            {
                if (existingMenuKeys.Contains(dto.Key))
                {
                    Console.WriteLine($"   ?? Child already exists: {dto.Key}");
                    continue;
                }

                if (softDeletedKeys.Contains(dto.Key))
                {
                    var softDeletedEntity = await _uow.Set<PerMenu>()
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x => x.Key == dto.Key, ct);

                    if (softDeletedEntity != null)
                    {
                        softDeletedEntity.IsDeleted = false;
                        softDeletedEntity.Label = dto.Label;
                        softDeletedEntity.Path = dto.Path ?? "";
                        softDeletedEntity.Icon = dto.Icon ?? "";
                        softDeletedEntity.Order = dto.Order;
                        softDeletedEntity.DateMod = DateTime.UtcNow;

                        if (moduleDict.TryGetValue(dto.ModKey, out var newModuleId))
                        {
                            softDeletedEntity.PerModuleId = newModuleId;
                        }

                        if (!string.IsNullOrWhiteSpace(dto.ParKey) && menuIdDict.TryGetValue(dto.ParKey, out var parentId))
                        {
                            softDeletedEntity.ParentId = parentId;
                        }

                        await _uow.Update(softDeletedEntity);
                        menuIdDict[softDeletedEntity.Key] = softDeletedEntity.Id;
                        existingMenuKeys.Add(softDeletedEntity.Key);
                        Console.WriteLine($"   ?? Reactivated child: {dto.Key}");
                    }
                    continue;
                }

                if (!moduleDict.TryGetValue(dto.ModKey, out var moduleId))
                {
                    Console.WriteLine($"   ? Skipping {dto.Key}: Module {dto.ModKey} not found");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(dto.ParKey))
                {
                    Console.WriteLine($"   ? Skipping {dto.Key}: No parent key provided");
                    continue;
                }

                if (!menuIdDict.TryGetValue(dto.ParKey, out var parentMenuId))
                {
                    Console.WriteLine($"   ? Skipping {dto.Key}: Parent '{dto.ParKey}' not found");
                    continue;
                }

                var entity = new PerMenu
                {
                    Id = Guid.NewGuid(),
                    PerModuleId = moduleId,
                    Key = dto.Key,
                    Label = dto.Label,
                    Path = dto.Path ?? "",
                    Icon = dto.Icon ?? "",
                    IsChild = true,
                    ParentId = parentMenuId,
                    Order = dto.Order,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                };

                menusToAdd.Add(entity);
                menuIdDict[entity.Key] = entity.Id;
                existingMenuKeys.Add(entity.Key);
                Console.WriteLine($"   ? Added child: {dto.Key} (Parent: {dto.ParKey})");
            }

            Console.WriteLine($"?? Total menus to add: {menusToAdd.Count}");

            if (menusToAdd.Any())
            {
                foreach (var menu in menusToAdd)
                {
                    await _uow.Add(menu, ct);
                }
                Console.WriteLine($"?? Prepared {menusToAdd.Count} menus for saving");
            }

            // ? NO Commit/Rollback here - ExecuteInTransactionAsync handles it

        }, ct);

        Console.WriteLine($"? Menu seeding completed!");
    }
}
public sealed class PerAccessSeed(IUnitOfWork _uow) : IRequestHandler<PerAccessSeedCmd>
{
    public async Task Handle(PerAccessSeedCmd request, CancellationToken ct)
    {
        if (request.AddDto == null || request.AddDto.Count == 0)
        {
            Console.WriteLine("?? PerAccessSeed: No API DTOs provided");
            return;
        }

        Console.WriteLine($"?? PerAccessSeed: Processing {request.AddDto.Count} API DTOs");

        // Log first 5 DTOs for debugging
        Console.WriteLine("?? Sample API DTOs:");
        foreach (var dto in request.AddDto.Take(5))
        {
            Console.WriteLine($"   Key: {dto.Key}, MenuKey: {dto.MenuKey}");
        }

        // ? STEP 1: Query data BEFORE starting the transaction
        // Get ALL menus from database (including newly added ones)
        var allMenus = await _uow.Set<PerMenu>()
            .IgnoreQueryFilters()
            .Select(m => new { m.Key, m.Id, m.IsDeleted })
            .ToListAsync(ct);

        var menuDict = allMenus
            .Where(m => !m.IsDeleted)
            .ToDictionary(
                x => x.Key,
                x => x.Id,
                StringComparer.OrdinalIgnoreCase);

        Console.WriteLine($"?? Found {menuDict.Count} active menus in database");

        // Log all menu keys for debugging
        if (menuDict.Any())
        {
            Console.WriteLine($"?? Menu keys in DB: {string.Join(", ", menuDict.Keys.Take(20))}");
            if (menuDict.Count > 20)
            {
                Console.WriteLine($"   ... and {menuDict.Count - 20} more");
            }
        }

        // Get all existing APIs (including soft-deleted)
        var existingApis = await _uow.Set<PerApi>()
            .IgnoreQueryFilters()
            .Select(a => new { a.Key, a.Id, a.IsDeleted })
            .ToListAsync(ct);

        var existingApiKeys = new HashSet<string>(
            existingApis.Where(a => !a.IsDeleted).Select(a => a.Key),
            StringComparer.OrdinalIgnoreCase);

        var softDeletedKeys = existingApis
            .Where(a => a.IsDeleted)
            .Select(a => a.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Console.WriteLine($"?? Existing APIs: {existingApiKeys.Count}, Soft-deleted: {softDeletedKeys.Count}");

        // Find missing menus for warning
        var allMenuKeys = request.AddDto
            .Select(x => x.MenuKey)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var missingMenuKeys = allMenuKeys
            .Where(k => !menuDict.ContainsKey(k))
            .ToList();

        if (missingMenuKeys.Any())
        {
            Console.WriteLine($"?? WARNING: {missingMenuKeys.Count} menus not found in database:");
            foreach (var key in missingMenuKeys.Take(20))
            {
                Console.WriteLine($"   Missing: {key}");
            }
            if (missingMenuKeys.Count > 20)
            {
                Console.WriteLine($"   ... and {missingMenuKeys.Count - 20} more");
            }
        }

        // ? STEP 2: Use ExecuteInTransactionAsync for the write operations
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var apisToAdd = new List<PerApi>();
            int skippedCount = 0;
            int reactivatedCount = 0;

            // Process each API
            foreach (var dto in request.AddDto.OrderBy(x => x.Key))
            {
                // Check if API exists and is not deleted
                if (existingApiKeys.Contains(dto.Key))
                {
                    skippedCount++;
                    continue;
                }

                // Check if soft-deleted - reactivate it
                if (softDeletedKeys.Contains(dto.Key))
                {
                    var softDeletedEntity = await _uow.Set<PerApi>()
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x => x.Key == dto.Key, ct);

                    if (softDeletedEntity != null)
                    {
                        softDeletedEntity.IsDeleted = false;
                        softDeletedEntity.Desc = dto.Desc;
                        softDeletedEntity.DateMod = DateTime.UtcNow;

                        if (!string.IsNullOrWhiteSpace(dto.MenuKey) && menuDict.TryGetValue(dto.MenuKey, out var newMenuId))
                        {
                            softDeletedEntity.PerMenuId = newMenuId;
                            Console.WriteLine($"   ?? Reactivated API: {dto.Key} (Menu: {dto.MenuKey} -> {newMenuId})");
                        }
                        else
                        {
                            Console.WriteLine($"   ?? Reactivated API but menu not found: {dto.Key} (Menu: {dto.MenuKey})");
                        }

                        await _uow.Update(softDeletedEntity);
                        existingApiKeys.Add(softDeletedEntity.Key);
                        reactivatedCount++;
                    }
                    continue;
                }

                // Validate menu
                if (string.IsNullOrWhiteSpace(dto.MenuKey))
                {
                    Console.WriteLine($"   ? Skipping {dto.Key}: No menu key provided");
                    skippedCount++;
                    continue;
                }

                if (!menuDict.TryGetValue(dto.MenuKey, out var menuId))
                {
                    Console.WriteLine($"   ? Skipping {dto.Key}: Menu '{dto.MenuKey}' not found");
                    skippedCount++;
                    continue;
                }

                // Create new API
                var entity = new PerApi
                {
                    Id = Guid.NewGuid(),
                    PerMenuId = menuId,
                    Key = dto.Key,
                    Desc = dto.Desc,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                };

                apisToAdd.Add(entity);
                existingApiKeys.Add(entity.Key);
                Console.WriteLine($"   ? Added API: {dto.Key} (Menu: {dto.MenuKey} -> {menuId})");
            }

            Console.WriteLine($"?? Skipped {skippedCount} items (existing), Reactivated {reactivatedCount} items");

            // Save all new APIs
            if (apisToAdd.Any())
            {
                foreach (var api in apisToAdd)
                {
                    await _uow.Add(api, ct);
                }
                Console.WriteLine($"?? Prepared {apisToAdd.Count} new APIs for saving");
            }
            else
            {
                Console.WriteLine("?? No new APIs to add");
            }

            // ? NO Commit/Rollback here - ExecuteInTransactionAsync handles it

        }, ct);

        Console.WriteLine("? API seeding completed successfully!");
    }
}
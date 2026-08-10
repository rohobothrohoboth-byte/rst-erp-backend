// Svc.Auth/Queries/MenuQry.cs

using Common;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using System.Text.Json;

namespace Svc.Auth.Queries;

public class GetUserMenuStructureQry : IRequest<object>
{
    public string UserId { get; set; } = string.Empty;
}

public class GetUserPermissionsQry : IRequest<List<string>>
{
    public string UserId { get; set; } = string.Empty;
}

public class GetUserMenuStructureHandler : IRequestHandler<GetUserMenuStructureQry, object>
{
    private readonly IUnitOfWork _uow;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;

    public GetUserMenuStructureHandler(IUnitOfWork uow, IMemoryCache cache, IConfiguration configuration)
    {
        _uow = uow;
        _cache = cache;
        _configuration = configuration;
    }

    public async Task<object> Handle(GetUserMenuStructureQry request, CancellationToken ct)
    {
        var cacheKey = $"menu_structure_{request.UserId}";

        // Try get from cache
        if (_cache.TryGetValue(cacheKey, out object? cachedMenu))
            return cachedMenu!;

        // Get user with position
        var user = await _uow.Set<AppUser>()
            .Where(u => u.Id == request.UserId)
            .Select(u => new { u.Id, u.PositionId })
            .FirstOrDefaultAsync(ct);

        if (user == null)
            return new List<object>();

        // The sidebar is driven by the menus explicitly assigned to the user
        // (UserPerMenu) — this is what the admin selects on the Menus tab.
        var assignedMenuIds = await _uow.Set<UserPerMenu>()
            .Where(x => x.UserId == user.Id && !x.IsDeleted)
            .Select(x => x.PerMenuId)
            .Distinct()
            .ToListAsync(ct);

        if (assignedMenuIds.Count == 0)
        {
            var empty = new List<ModuleTokenDto>();
            _cache.Set(cacheKey, empty, GetCacheOptions());
            return empty;
        }

        // Granted API actions (own + position) are attached to each menu as A[].
        var userApiKeys = await _uow.Set<UserPerApi>()
            .Where(x => x.UserId == user.Id && !x.IsDeleted)
            .Select(x => x.PerApi.Key)
            .ToListAsync(ct);

        // Get position permissions (if any)
        var positionApiKeys = new List<string>();
        if (user.PositionId.HasValue)
        {
            positionApiKeys = await _uow.Set<PositionPerApi>()
                .Where(ppa => ppa.PositionId == user.PositionId.Value && !ppa.IsDeleted)
                .Select(ppa => ppa.PerApi.Key)
                .ToListAsync(ct);
        }

        var allApiKeys = userApiKeys.Union(positionApiKeys).Distinct().ToList();

        // Build the menu tree from the assigned menus (+ their ancestors).
        var modules = await BuildMenuTree(assignedMenuIds, allApiKeys, ct);

        _cache.Set(cacheKey, modules, GetCacheOptions());

        return modules;
    }

    private static MemoryCacheEntryOptions GetCacheOptions() =>
        new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(25))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(35));

    private async Task<List<ModuleTokenDto>> BuildMenuTree(List<Guid> assignedMenuIds, List<string> apiKeys, CancellationToken ct)
    {
        // Get the connection string from configuration
        var connectionString = _configuration.GetConnectionString("AuthConnection")
            ?? _configuration.GetConnectionString("authMgrCon");

        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback: Use EF Core if connection string not available
            return await BuildMenuTreeWithEF(assignedMenuIds, apiKeys, ct);
        }

        using var connection = new NpgsqlConnection(connectionString);

        // Include the assigned menus AND all of their ancestors (recursive CTE) so
        // parent/group nodes still render even when only child menus were assigned.
        // Only API actions the user actually has are attached (no IS NULL leak).
        var sql = @"
            WITH RECURSIVE menu_closure AS (
                SELECT m.""Id"", m.""ParentId""
                FROM ""PerMenu"" m
                WHERE m.""Id"" = ANY(@MenuIds) AND m.""IsDeleted"" = false
                UNION
                SELECT p.""Id"", p.""ParentId""
                FROM ""PerMenu"" p
                INNER JOIN menu_closure c ON c.""ParentId"" = p.""Id""
                WHERE p.""IsDeleted"" = false
            )
            SELECT
                pm.""Key"" AS ModKey,
                pm.""Desc"" AS ModDesc,
                mn.""Key"" AS MenuKey,
                mn.""Id"" AS MenuId,
                mn.""Label"",
                mn.""Path"",
                mn.""Icon"",
                mn.""IsChild"",
                mn.""Order"",
                mn.""ParentId"",
                pa.""Key"" AS ApiKey
            FROM menu_closure mc
            INNER JOIN ""PerMenu"" mn ON mn.""Id"" = mc.""Id"" AND mn.""IsDeleted"" = false
            INNER JOIN ""PerModule"" pm ON pm.""Id"" = mn.""PerModuleId"" AND pm.""IsDeleted"" = false
            LEFT JOIN ""PerApi"" pa ON pa.""PerMenuId"" = mn.""Id""
                AND pa.""Key"" = ANY(@ApiKeys)
                AND (pa.""IsDeleted"" IS NULL OR pa.""IsDeleted"" = false)
            ORDER BY pm.""Key"", mn.""Order"", pa.""Key""";

        var parameters = new { MenuIds = assignedMenuIds.ToArray(), ApiKeys = apiKeys.ToArray() };

        var data = (await connection.QueryAsync<FlatPermissionDto>(sql, parameters)).ToList();

        // Build menu tree
        return BuildMenuTreeFromData(data);
    }

    private async Task<List<ModuleTokenDto>> BuildMenuTreeWithEF(List<Guid> assignedMenuIds, List<string> apiKeys, CancellationToken ct)
    {
        // Fallback using EF Core. Compute the ancestor closure in memory.
        var allMenus = await _uow.Set<PerMenu>()
            .Where(m => !m.IsDeleted)
            .Select(m => new { m.Id, m.ParentId })
            .ToListAsync(ct);
        var byId = allMenus.ToDictionary(m => m.Id, m => m.ParentId);

        var closure = new HashSet<Guid>();
        foreach (var id in assignedMenuIds)
        {
            var cur = (Guid?)id;
            while (cur.HasValue && byId.ContainsKey(cur.Value))
            {
                if (!closure.Add(cur.Value)) break; // chain already processed
                cur = byId[cur.Value];
            }
        }

        var apiKeySet = new HashSet<string>(apiKeys);

        var query = from pm in _uow.Set<PerModule>().Where(m => !m.IsDeleted)
                    join mn in _uow.Set<PerMenu>().Where(m => !m.IsDeleted) on pm.Id equals mn.PerModuleId
                    where closure.Contains(mn.Id)
                    join pa in _uow.Set<PerApi>().Where(a => !a.IsDeleted && apiKeySet.Contains(a.Key)) on mn.Id equals pa.PerMenuId into paJoin
                    from pa in paJoin.DefaultIfEmpty()
                    orderby pm.Key, mn.Order, pa.Key
                    select new FlatPermissionDto
                    {
                        ModKey = pm.Key,
                        ModDesc = pm.Desc,
                        MenuKey = mn.Key,
                        MenuId = mn.Id,
                        Label = mn.Label,
                        Path = mn.Path,
                        Icon = mn.Icon,
                        IsChild = mn.IsChild,
                        Order = mn.Order,
                        ParentId = mn.ParentId,
                        ApiKey = pa != null ? pa.Key : null
                    };

        var data = await query.ToListAsync(ct);
        return BuildMenuTreeFromData(data);
    }

    private List<ModuleTokenDto> BuildMenuTreeFromData(List<FlatPermissionDto> data)
    {
        var modules = data.GroupBy(x => x.ModKey).Select(module =>
        {
            var moduleFirst = module.First();

            var menuDict = module.Where(x => x.MenuKey != null)
                .GroupBy(x => x.MenuId)
                .ToDictionary(g => g.Key, g =>
                {
                    var first = g.First();
                    var actions = g
                        .Where(x => x.ApiKey != null)
                        .Select(x => x.ApiKey!)
                        .Distinct()
                        .ToList();

                    return new MenuTokenDto
                    {
                        K = first.MenuKey!,
                        L = first.Label!,
                        P = first.Path!,
                        I = first.Icon!,
                        O = first.Order,
                        A = actions
                    };
                });

            var roots = new List<MenuTokenDto>();
            foreach (var item in module.GroupBy(x => x.MenuId))
            {
                var first = item.First();
                if (!menuDict.TryGetValue(first.MenuId, out var current)) continue;

                if (first.ParentId == null)
                {
                    roots.Add(current);
                }
                else if (menuDict.TryGetValue(first.ParentId.Value, out var parent))
                {
                    parent.C ??= new List<MenuTokenDto>();
                    if (!parent.C.Any(c => c.K == current.K))
                        parent.C.Add(current);
                }
                else
                {
                    roots.Add(current);
                }
            }

            return new ModuleTokenDto
            {
                K = module.Key!,
                L = moduleFirst.ModDesc!,
                M = SortMenus(roots)
            };
        }).OrderBy(m => m.K).ToList();

        return modules;
    }

    private static List<MenuTokenDto> SortMenus(List<MenuTokenDto> menus)
    {
        foreach (var menu in menus)
        {
            if (menu.C != null && menu.C.Count > 0)
                menu.C = SortMenus(menu.C);
        }
        menus.Sort((a, b) => a.O.CompareTo(b.O));
        return menus;
    }
}

// Handler for GetUserPermissionsQry
public class GetUserPermissionsHandler : IRequestHandler<GetUserPermissionsQry, List<string>>
{
    private readonly IUnitOfWork _uow;

    public GetUserPermissionsHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<string>> Handle(GetUserPermissionsQry request, CancellationToken ct)
    {
        var user = await _uow.Set<AppUser>()
            .Where(u => u.Id == request.UserId)
            .Select(u => new { u.Id, u.PositionId })
            .FirstOrDefaultAsync(ct);

        if (user == null)
            return new List<string>();

        var userApiKeys = await _uow.Set<UserPerApi>()
            .Where(x => x.UserId == user.Id && !x.IsDeleted)
            .Select(x => x.PerApi.Key)
            .ToListAsync(ct);

        var positionApiKeys = new List<string>();
        if (user.PositionId.HasValue)
        {
            positionApiKeys = await _uow.Set<PositionPerApi>()
                .Where(ppa => ppa.PositionId == user.PositionId.Value && !ppa.IsDeleted)
                .Select(ppa => ppa.PerApi.Key)
                .ToListAsync(ct);
        }

        return userApiKeys.Union(positionApiKeys).Distinct().ToList();
    }
}

// DTOs
public class FlatPermissionDto
{
    public string? ModKey { get; set; }
    public string? ModDesc { get; set; }
    public string? MenuKey { get; set; }
    public Guid MenuId { get; set; }
    public string? Label { get; set; }
    public string? Path { get; set; }
    public string? Icon { get; set; }
    public bool IsChild { get; set; }
    public int Order { get; set; }
    public Guid? ParentId { get; set; }
    public string? ApiKey { get; set; }
}

public class ModuleTokenDto
{
    public string K { get; set; } = string.Empty;
    public string L { get; set; } = string.Empty;
    public List<MenuTokenDto>? M { get; set; }
}

public class MenuTokenDto
{
    public string K { get; set; } = string.Empty;
    public string L { get; set; } = string.Empty;
    public string P { get; set; } = string.Empty;
    public string I { get; set; } = string.Empty;
    public int O { get; set; }
    public List<string>? A { get; set; }
    public List<MenuTokenDto>? C { get; set; }
}
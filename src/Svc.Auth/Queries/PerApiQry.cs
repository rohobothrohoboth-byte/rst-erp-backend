using Dapper;
using Helpers;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

// ==================== QUERY DEFINITIONS ====================

public class PerApiAllQry : IRequest<List<PerApiListDto>> { }
public class PerApiByIdQry : IRequest<PerApiListDto?> { public Guid Id { get; set; } }
public class PerApiByMenuIdQry : IRequest<MenuPerApiListDto?> { public Guid Id { get; set; } }
public class PerApiByUserIdQry : IRequest<List<MenuPerApiListDto>> { public string Id { get; set; } = default!; }

public class PerApiFilteredByUserQry : IRequest<List<MenuPerApiListDto>>
{
    public string UserId { get; set; } = string.Empty;
    public List<Guid> MenuIds { get; set; } = new();
}

public class PerMenuFilteredByUserQry : IRequest<List<ModPerMenuListDto>>
{
    public string UserId { get; set; } = string.Empty;
    public List<Guid> ModuleIds { get; set; } = new();
}

// ==================== HANDLERS ====================

public class PerApiAllQryHandler : IRequestHandler<PerApiAllQry, List<PerApiListDto>>
{
    private readonly IDapperHelper _dapper;
    public PerApiAllQryHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PerApiListDto>> Handle(PerApiAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<PerApi>(v, x => x.Id, x => x.Key, x => x.Desc)
            .SelectAs<PerApi, PerApiListDto>(v, x => x.Desc, d => d.Name)
            .SelectAs<PerMenu, PerApiListDto>(c, x => x.Label, d => d.PerMenu)
            .From<PerApi>(v)
            .Join<PerApi, PerMenu>(v, c, x => x.PerMenuId, x => x.Id)
            .OrderBy<PerApi>(v, x => x.Key, desc: false);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<PerApiListDto>(ct);

        return list;
    }
}

public class PerApiByIdQryHandler : IRequestHandler<PerApiByIdQry, PerApiListDto?>
{
    private readonly IDapperHelper _dapper;
    public PerApiByIdQryHandler(IDapperHelper dapper) { _dapper = dapper; }
    public async Task<PerApiListDto?> Handle(PerApiByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<PerApi>(v, x => x.Id, x => x.Key)
            .SelectAs<PerApi, PerApiListDto>(v, x => x.Desc, d => d.Name)
            .SelectAs<PerMenu, PerApiListDto>(c, x => x.Label, d => d.PerMenu)
            .From<PerApi>(v)
            .Join<PerApi, PerMenu>(v, c, x => x.PerMenuId, x => x.Id)
            .Where<PerApi>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PerApiListDto>(sql, parameters, ct);
        if (data == null) return null;
        return data;
    }
}

public class PerApiByMenuIdQryHandler : IRequestHandler<PerApiByMenuIdQry, MenuPerApiListDto?>
{
    private readonly IDapperHelper _dapper;
    public PerApiByMenuIdQryHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<MenuPerApiListDto?> Handle(PerApiByMenuIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<PerApi>(c, x => x.Id, x => x.Desc, x => x.PerMenuId)
            .SelectAs<PerApi, PerMenuApiRow>(c, x => x.Id, x => x.ApiId)
            .Select<PerMenu>(v, x => x.Id, x => x.Label)
            .From<PerMenu>(v)
            .LeftJoin<PerMenu, PerApi>(v, c, x => x.Id, x => x.PerMenuId)
            .Where<PerApi>(c, x => x.PerMenuId == request.Id)
            .OrderBy<PerApi>(c, x => x.Desc, desc: false);

        var (sql, param) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, param, ct);
        var parser = reader.GetRowParser<PerMenuApiRow>();

        MenuPerApiListDto? result = null;
        var apiList = new List<NameList>();

        while (await reader.ReadAsync(ct))
        {
            var row = parser(reader);
            if (result == null)
            {
                result = new MenuPerApiListDto
                {
                    PerMenuId = row.PerMenuId,
                    PerMenu = row.Label,
                    PerApiList = apiList
                };
            }

            if (row.ApiId != Guid.Empty)
            {
                apiList.Add(new NameList
                {
                    Id = row.ApiId,
                    Name = row.Desc
                });
            }
        }

        return result;
    }
}

public class PerApiByUserIdQryHandler : IRequestHandler<PerApiByUserIdQry, List<MenuPerApiListDto>>
{
    private readonly IDapperHelper _dapper;
    public PerApiByUserIdQryHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<MenuPerApiListDto>> Handle(PerApiByUserIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        const string d = "d";
        var qb = new QueryBuilder()
            .Select<UserPerMenu>(v, x => x.UserId, x => x.PerMenuId)
            .Select<PerMenu>(c, x => x.Label)
            .Select<PerApi>(d, x => x.Desc, x => x.PerMenuId)
            .SelectAs<PerApi, UserMenuApiRow>(d, x => x.Id, x => x.ApiId)
            .From<UserPerMenu>(v)
            .Join<UserPerMenu, PerMenu>(v, c, x => x.PerMenuId, x => x.Id)
            .LeftJoin<PerMenu, PerApi>(c, d, x => x.Id, x => x.PerMenuId)
            .Where<UserPerMenu>(v, x => x.UserId == request.Id)
            .OrderBy<PerMenu>(c, x => x.Label, desc: false);

        var (sql, param) = qb.Build();

        await using var reader = await _dapper.ExecuteReaderAsync(sql, param, ct);
        var parser = reader.GetRowParser<UserMenuApiRow>();

        var dict = new Dictionary<Guid, MenuPerApiListDto>();

        while (await reader.ReadAsync(ct))
        {
            var row = parser(reader);

            if (!dict.TryGetValue(row.PerMenuId, out var menu))
            {
                menu = new MenuPerApiListDto
                {
                    PerMenuId = row.PerMenuId,
                    PerMenu = row.Label,
                    PerApiList = []
                };

                dict[row.PerMenuId] = menu;
            }

            if (row.ApiId != Guid.Empty)
            {
                menu.PerApiList.Add(new NameList
                {
                    Id = row.ApiId,
                    Name = row.Desc
                });
            }
        }

        return dict.Values.ToList();
    }
}

public class PerApiFilteredByUserHandler : IRequestHandler<PerApiFilteredByUserQry, List<MenuPerApiListDto>>
{
    private readonly IDapperHelper _dapper;

    public PerApiFilteredByUserHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<MenuPerApiListDto>> Handle(PerApiFilteredByUserQry request, CancellationToken ct)
    {
        if (request.MenuIds == null || request.MenuIds.Count == 0)
        {
            return new List<MenuPerApiListDto>();
        }

        const string sql = @"
            SELECT DISTINCT
                m.Id AS PerMenuId,
                m.Label AS PerMenu,
                a.Id AS ApiId,
                a.[Key] AS ApiKey,
                a.[Desc] AS ApiDesc
            FROM PerMenu m
            INNER JOIN PerApi a ON a.PerMenuId = m.Id AND a.IsDeleted = 0
            WHERE m.Id IN @MenuIds AND m.IsDeleted = 0
            ORDER BY m.[Order], a.[Key]";

        var parameters = new { MenuIds = request.MenuIds };

        var result = await _dapper.QueryAsync<dynamic>(sql, parameters, ct);

        var grouped = result
            .GroupBy(x => new { x.PerMenuId, x.PerMenu })
            .Select(g => new MenuPerApiListDto
            {
                PerMenuId = g.Key.PerMenuId,
                PerMenu = g.Key.PerMenu,
                PerApiList = g.Select(x => new NameList
                {
                    Id = x.ApiId,
                    Name = x.ApiDesc ?? x.ApiKey
                }).ToList()
            })
            .ToList();

        return grouped;
    }
}

public class PerMenuFilteredByUserHandler : IRequestHandler<PerMenuFilteredByUserQry, List<ModPerMenuListDto>>
{
    private readonly IDapperHelper _dapper;

    public PerMenuFilteredByUserHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<ModPerMenuListDto>> Handle(PerMenuFilteredByUserQry request, CancellationToken ct)
    {
        if (request.ModuleIds == null || request.ModuleIds.Count == 0)
        {
            return new List<ModPerMenuListDto>();
        }

        const string sql = @"
            SELECT DISTINCT
                mod.Id AS ModuleId,
                mod.[Desc] AS ModuleDesc,
                m.Id AS MenuId,
                m.Label AS MenuLabel
            FROM PerModule mod
            INNER JOIN PerMenu m ON m.PerModuleId = mod.Id AND m.IsDeleted = 0
            WHERE mod.Id IN @ModuleIds AND mod.IsDeleted = 0
            ORDER BY mod.[Key], m.[Order]";

        var parameters = new { ModuleIds = request.ModuleIds };

        var result = await _dapper.QueryAsync<dynamic>(sql, parameters, ct);

        var grouped = result
            .GroupBy(x => new { x.ModuleId, x.ModuleDesc })
            .Select(g => new ModPerMenuListDto
            {
                PerModuleId = g.Key.ModuleId,
                PerModule = g.Key.ModuleDesc,
                PerMenuList = g.Select(x => new NameList
                {
                    Id = x.MenuId,
                    Name = x.MenuLabel
                }).ToList()
            })
            .ToList();

        return grouped;
    }
}


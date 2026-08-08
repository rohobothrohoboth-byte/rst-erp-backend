using Dapper;
using Helpers;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public sealed class PerMenuAllQry : IRequest<List<PerMenuListDto>> { }
public sealed class PerMenuByIdQry : IRequest<PerMenuListDto?> { public Guid Id { get; set; } }
public sealed class PerMenuByKeyQry : IRequest<NameList?> { public string Key { get; set; } = default!; }
public sealed class PerMenuByModIdQry : IRequest<ModPerMenuListDto?> { public Guid Id { get; set; } }
public sealed class PerMenuByUserIdQry : IRequest<List<ModPerMenuListDto>> { public string Id { get; set; } = default!; }
public sealed class MenuIdsByKeysQry : IRequest<List<KeyIdDto>> { public List<string> Keys { get; init; } = []; }
public sealed class GetMenuTreeQry : IRequest<List<PerMenuDto>> { };
public sealed class PerMenuAll(IDapperHelper _dapper) : IRequestHandler<PerMenuAllQry, List<PerMenuListDto>>
{
    public async Task<List<PerMenuListDto>> Handle(PerMenuAllQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                v.""Id"",
                v.""PerModuleId"",
                v.""ParentId"",
                v.""Order"",
                v.""IsChild"",
                v.""Key"",
                v.""Label"",
                v.""Path"",
                v.""Icon"",
                v.""IsDeleted"",
                v.""DateAdd"",
                v.""DateMod"",
                c.""Label"" AS ""ParentLabel"",
                c.""Key"" AS ""ParentKey"",
                d.""Desc"" AS ""ModuleDesc""
            FROM ""PerMenu"" v
            LEFT JOIN ""PerMenu"" c ON v.""ParentId"" = c.""Id"" AND c.""IsDeleted"" = false
            INNER JOIN ""PerModule"" d ON v.""PerModuleId"" = d.""Id"" AND d.""IsDeleted"" = false
            WHERE v.""IsDeleted"" = false
            ORDER BY v.""Order"" ASC";

        var rows = await _dapper.QueryAsync<PerMenuJoinRow>(sql, null, ct);

        return rows.Select(row => new PerMenuListDto
        {
            Id = row.Id,
            PerModuleId = row.PerModuleId,
            Order = row.Order,
            IsChild = row.IsChild,
            Key = row.Key,
            Label = row.Label,
            IsChildStr = row.IsChild.ToString(),
            Parent = row.ParentLabel ?? "",
            ParentKey = row.ParentKey ?? "",
            Module = row.ModuleDesc,
            Path = row.Path,
            Icon = row.Icon,
            IsDeleted = row.IsDeleted,
            DateAdd = row.DateAdd,
            DateMod = row.DateMod
        }).ToList();
    }
}


public sealed class PerMenuByKey(IDapperHelper _dapper) : IRequestHandler<PerMenuByKeyQry, NameList?>
{
    public async Task<NameList?> Handle(PerMenuByKeyQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                ""Id"",
                ""Label"" AS ""Name""
            FROM ""PerMenu""
            WHERE ""Key"" = @Key AND ""IsDeleted"" = false";

        var result = await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, new { Key = request.Key }, ct);
        return result;
    }
}

public sealed class PerMenuById(IDapperHelper _dapper) : IRequestHandler<PerMenuByIdQry, PerMenuListDto?>
{
    public async Task<PerMenuListDto?> Handle(PerMenuByIdQry request, CancellationToken ct)
    {


        const string sql = @"
            SELECT
                v.""Id"",
                v.""PerModuleId"",
                v.""ParentId"",
                v.""Order"",
                v.""IsChild"",
                v.""Key"",
                v.""Label"",
                v.""Path"",
                v.""Icon"",
                v.""IsDeleted"",
                v.""DateAdd"",
                v.""DateMod"",
                p.""Label"" AS ""ParentLabel"",
                p.""Key"" AS ""ParentKey"",
                m.""Desc"" AS ""ModuleDesc""
            FROM ""PerMenu"" v
            LEFT JOIN ""PerMenu"" p ON v.""ParentId"" = p.""Id"" AND p.""IsDeleted"" = false
            INNER JOIN ""PerModule"" m ON v.""PerModuleId"" = m.""Id"" AND m.""IsDeleted"" = false
            WHERE v.""Id"" = @Id AND v.""IsDeleted"" = false";

        var row = await _dapper.QueryFirstOrDefaultAsync<PerMenuJoinRow>(sql, new { Id = request.Id }, ct);

        if (row == null)
        {

            return null;
        }



        return new PerMenuListDto
        {
            Id = row.Id,
            PerModuleId = row.PerModuleId,
            Order = row.Order,
            IsChild = row.IsChild,
            Key = row.Key,
            Label = row.Label,
            IsChildStr = row.IsChild.ToString(),
            Parent = row.ParentLabel ?? "",
            ParentKey = row.ParentKey ?? "",
            Module = row.ModuleDesc,
            Path = row.Path,
            Icon = row.Icon,
            IsDeleted = row.IsDeleted,
            DateAdd = row.DateAdd,
            DateMod = row.DateMod
        };
    }
}

public sealed class PerMenuByUserId(IDapperHelper _dapper) : IRequestHandler<PerMenuByUserIdQry, List<ModPerMenuListDto>>
{
    public async Task<List<ModPerMenuListDto>> Handle(PerMenuByUserIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        const string d = "d";
        var qb = new QueryBuilder()
            .Select<UserPerModule>(v, x => x.UserId, x => x.PerModuleId)
            .SelectAs<PerModule, UserModuleMenuRow>(c, x => x.Id, x => x.ModuleId)
            .SelectAs<PerModule, UserModuleMenuRow>(c, x => x.Desc, x => x.ModuleDesc)
            .Select<PerMenu>(d, x => x.PerModuleId)
            .SelectAs<PerMenu, UserModuleMenuRow>(d, x => x.Id, x => x.MenuId)
            .SelectAs<PerMenu, UserModuleMenuRow>(d, x => x.Label, x => x.MenuLabel)
            .From<UserPerModule>(v)
            .Join<UserPerModule, PerModule>(v, c, x => x.PerModuleId, x => x.Id)
            .LeftJoin<PerModule, PerMenu>(c, d, x => x.Id, x => x.PerModuleId)
            .Where<UserPerModule>(v, x => x.UserId == request.Id)
            .OrderBy<PerModule>(c, x => x.Desc);
        var (sql, param) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, param, ct);
        var parser = reader.GetRowParser<UserModuleMenuRow>();

        var dict = new Dictionary<Guid, ModPerMenuListDto>();
        while (await reader.ReadAsync(ct))
        {
            var row = parser(reader);

            if (!dict.TryGetValue(row.ModuleId, out var module))
            {
                module = new ModPerMenuListDto
                {
                    PerModuleId = row.ModuleId,
                    PerModule = row.ModuleDesc,
                    PerMenuList = []
                };

                dict[row.ModuleId] = module;
            }

            if (row.MenuId != Guid.Empty)
            {
                module.PerMenuList.Add(new NameList
                {
                    Id = row.MenuId,
                    Name = row.MenuLabel
                });
            }
        }

        return dict.Values.ToList();
    }
}

public sealed class MenuIdsByKeys(IDapperHelper _dapper) : IRequestHandler<MenuIdsByKeysQry, List<KeyIdDto>>
{
    public async Task<List<KeyIdDto>> Handle(MenuIdsByKeysQry request, CancellationToken ct)
    {
        if (request.Keys == null || request.Keys.Count == 0) { return []; }
        var keys = request.Keys.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        if (keys.Length == 0) { return []; }

        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerMenu>(v, x => x.Id, x => x.Key)
            .From<PerMenu>(v)
            .WhereIn<PerMenu>(v, x => x.Key, keys);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryAsync<KeyIdDto>(sql, parameters, ct);
        return data?.ToList() ?? [];
    }
}

public sealed class GetMenuTree(IDapperHelper _dapper) : IRequestHandler<GetMenuTreeQry, List<PerMenuDto>>
{
    private static List<PerMenuDto> BuildTree(List<PerMenuDto> allMenus, Guid? parentId)
    {
        return [.. allMenus.Where(m => m.ParentId == parentId).Select(m =>
            {
                m.Children = BuildTree(allMenus, m.Id);
                return m;
            }).OrderBy(x => x.Order)];
    }

    public async Task<List<PerMenuDto>> Handle(GetMenuTreeQry request, CancellationToken ct)
    {
        const string v = "v";
        const string m = "m";
        var qb = new QueryBuilder()
            .Select<PerMenu>(v, x => x.Id, x => x.Key, x => x.Label, x => x.Path, x => x.Icon, x => x.Order, x => x.PerModuleId, x => x.ParentId!)
            .SelectAs<PerModule, PerMenuDto>(m, x => x.Desc, x => x.Module)
            .From<PerMenu>(v)
            .LeftJoin<PerMenu, PerModule>(v, m, x => x.PerModuleId, x => x.Id)
            .OrderBy<PerMenu>(v, x => x.Order, desc: false);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var allMenus = await reader.ToListAsync<PerMenuDto>(ct);
        return BuildTree(allMenus, parentId: null);
    }
}
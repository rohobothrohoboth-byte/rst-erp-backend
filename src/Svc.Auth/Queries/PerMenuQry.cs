using Dapper;
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



public sealed class PerMenuAll(IDapperHelper _dapper) : IRequestHandler<PerMenuAllQry, List<PerMenuListDto>>
{
    public async Task<List<PerMenuListDto>> Handle(PerMenuAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        const string d = "d";
        var qb = new QueryBuilder()
            .Select<PerMenu>(v, x => x.Id, x => x.PerModuleId, x => x.ParentId, x => x.Order, x => x.IsChild, x => x.Key, x => x.Label, x => x.Path, x => x.Icon, x => x.IsDeleted, x => x.DateAdd, x => x.DateMod)
            .SelectAs<PerMenu, PerMenuJoinRow>(c, x => x.Label, x => x.ParentLabel)
            .SelectAs<PerMenu, PerMenuJoinRow>(c, x => x.Key, x => x.ParentKey)
            .Select<PerModule>(d, x => x.Id)
            .SelectAs<PerModule, PerMenuJoinRow>(d, x => x.Desc, x => x.ModuleDesc)
            .From<PerMenu>(v)
            .LeftJoin<PerMenu, PerMenu>(v, c, x => x.ParentId, x => x.Id)
            .Join<PerMenu, PerModule>(v, d, x => x.PerModuleId, x => x.Id)
            .OrderBy<PerMenu>(v, x => x.Order);
        var (sql, param) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, param, ct);
        var parser = reader.GetRowParser<PerMenuJoinRow>();

        var result = new List<PerMenuListDto>();
        while (await reader.ReadAsync(ct))
        {
            var row = parser(reader);

            result.Add(new PerMenuListDto
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
            });
        }

        return result;
    }
}

public sealed class PerMenuById(IDapperHelper _dapper) : IRequestHandler<PerMenuByIdQry, PerMenuListDto?>
{
    public async Task<PerMenuListDto?> Handle(PerMenuByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        const string d = "d";
        var qb = new QueryBuilder()
            .Select<PerMenu>(v, x => x.Id, x => x.PerModuleId, x => x.ParentId, x => x.Order, x => x.IsChild, x => x.Key, x => x.Label, x => x.Path, x => x.Icon, x => x.IsDeleted, x => x.DateAdd, x => x.DateMod)
            .Select<PerMenu>(c, x => x.Label, x => x.Key)
            .Select<PerModule>(d, x => x.Id, x => x.Desc)
            .From<PerMenu>(v)
            .LeftJoin<PerMenu, PerMenu>(v, c, x => x.ParentId, x => x.Id)
            .Join<PerMenu, PerModule>(v, d, x => x.PerModuleId, x => x.Id)
            .Where<PerMenu>(v, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<PerMenuJoinRow>(sql, parameters, ct);
        if (row == null) return null;

        var m = new PerMenuListDto
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
        return m;
    }
}

public sealed class PerMenuByKey(IDapperHelper _dapper) : IRequestHandler<PerMenuByKeyQry, NameList?>
{
    public async Task<NameList?> Handle(PerMenuByKeyQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerMenu>(v, x => x.Id)
            .SelectAs<PerMenu, NameList>(v, x => x.Label, d => d.Name)
            .From<PerMenu>(v)
            .Where<PerMenu>(v, p => p.Key == request.Key)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, parameters, ct);
        if (data == null) return null;
        return data;
    }
}

public sealed class PerMenuByModId(IDapperHelper _dapper) : IRequestHandler<PerMenuByModIdQry, ModPerMenuListDto?>
{
    public async Task<ModPerMenuListDto?> Handle(PerMenuByModIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<PerMenu>(c, x => x.PerModuleId)
            .SelectAs<PerMenu, ModuleMenuRow>(c, x => x.Label, x => x.MenuLabel)
            .SelectAs<PerMenu, ModuleMenuRow>(c, x => x.Id, x => x.MenuId)
            .SelectAs<PerModule, ModuleMenuRow>(v, x => x.Desc, x => x.ModuleDesc)
            .From<PerMenu>(v)
            .LeftJoin<PerModule, PerMenu>(v, c, x => x.Id, x => x.PerModuleId)
            .Where<PerMenu>(c, x => x.PerModuleId == request.Id)
            .OrderBy<PerMenu>(c, x => x.Label);
        var (sql, param) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, param, ct);
        var parser = reader.GetRowParser<ModuleMenuRow>();

        ModPerMenuListDto? result = null;
        var menus = new List<NameList>();
        while (await reader.ReadAsync(ct))
        {
            var row = parser(reader);

            if (result == null)
            {
                result = new ModPerMenuListDto
                {
                    PerModuleId = row.PerModuleId,
                    PerModule = row.ModuleDesc,
                    PerMenuList = menus
                };
            }

            if (row.MenuId != Guid.Empty)
            {
                menus.Add(new NameList
                {
                    Id = row.MenuId,
                    Name = row.MenuLabel
                });
            }
        }

        return result;
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
            .WhereIn<PerModule>(v, x => x.Key, keys);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryAsync<KeyIdDto>(sql, parameters, ct);
        return data?.ToList() ?? [];
    }
}
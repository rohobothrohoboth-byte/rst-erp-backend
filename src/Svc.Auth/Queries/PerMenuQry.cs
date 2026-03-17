using Dapper;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class PerMenuAllQry : IRequest<List<PerMenuListDto>> { }
public class PerMenuByIdQry : IRequest<PerMenuListDto?> { public Guid Id { get; set; } }
public class PerMenuByKeyQry : IRequest<NameList?> { public string Key { get; set; } = default!; }
public class PerMenuByModIdQry : IRequest<ModPerMenuListDto?> { public Guid Id { get; set; } }
public class PerMenuByUserIdQry : IRequest<List<ModPerMenuListDto>> { public string Id { get; set; } = default!; }



public class PerMenuAllQryHandler : IRequestHandler<PerMenuAllQry, List<PerMenuListDto>>
{
    private readonly IDapperHelper _dapper;
    public PerMenuAllQryHandler(IDapperHelper dapper) { _dapper = dapper;}

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

public class PerMenuByIdQryHandler : IRequestHandler<PerMenuByIdQry, PerMenuListDto?>
{
    private readonly IDapperHelper _dapper;
    public PerMenuByIdQryHandler(IDapperHelper dapper) { _dapper = dapper; }

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

public class PerMenuByKeyQryHandler : IRequestHandler<PerMenuByKeyQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public PerMenuByKeyQryHandler(IDapperHelper dapper) { _dapper = dapper; }
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

public class PerMenuByModIdQryHandler : IRequestHandler<PerMenuByModIdQry, ModPerMenuListDto?>
{
    private readonly IDapperHelper _dapper;
    public PerMenuByModIdQryHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<ModPerMenuListDto?> Handle(PerMenuByModIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<PerMenu>(c, x => x.PerModuleId)
            .SelectAs<PerMenu, ModuleMenuRow>(c, x => x.Label, x => x.MenuLabel)
            .SelectAs<PerMenu, ModuleMenuRow>(c, x => x.Id, x => x.MenuId)
            //.Select<PerModule>(v, x => x.Id, x => x.Desc)
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

public class PerMenuByUserIdQryHandler : IRequestHandler<PerMenuByUserIdQry, List<ModPerMenuListDto>>
{
    private readonly IDapperHelper _dapper;
    public PerMenuByUserIdQryHandler(IDapperHelper dapper) { _dapper = dapper;  }

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
using Dapper;
using Helpers;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class PerApiAllQry : IRequest<List<PerApiListDto>> { }
public class PerApiByIdQry : IRequest<PerApiListDto?> { public Guid Id { get; set; } }
public class PerApiByMenuIdQry : IRequest<MenuPerApiListDto?> { public Guid Id { get; set; } }
public class PerApiByUserIdQry : IRequest<List<MenuPerApiListDto>> { public string Id { get; set; } = default!; }



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
    private readonly IMediator _med;
    public PerApiByUserIdQryHandler(IDapperHelper dapper, IMediator med) { _dapper = dapper; _med = med; }

    public async Task<List<MenuPerApiListDto>> Handle(PerApiByUserIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        const string d = "d";
        var qb = new QueryBuilder()
            .Select<UserPerApi>(v, x => x.UserId, x => x.PerApiId)
            .Select<PerMenu>(c, x => x.Label)
            .SelectAs<PerMenu, UserMenuApiRow>(c, x => x.Id, x => x.PerMenuId)
            .Select<PerApi>(d, x => x.Desc)
            .SelectAs<PerApi, UserMenuApiRow>(d, x => x.Id, x => x.ApiId)
            .From<UserPerApi>(v)
            .Join<UserPerApi, PerMenu>(v, c, x => x.PerApiId, x => x.Id)
            .LeftJoin<PerMenu, PerApi>(c, d, x => x.Id, x => x.PerMenuId)
            .Where<UserPerApi>(v, x => x.UserId == request.Id)
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
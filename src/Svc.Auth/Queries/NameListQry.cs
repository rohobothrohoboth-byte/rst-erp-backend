using Helpers;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class ModuleNameAllQry : IRequest<List<NameList>> { }
public class ModuleNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class PerMenuNameAllQry : IRequest<List<NameList>> { }
public class PerMenuNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class PerApiNameAllQry : IRequest<List<NameList>> { }
public class PerApiNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }



public class ModuleNameAllQryHandler : IRequestHandler<ModuleNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public ModuleNameAllQryHandler(IDapperHelper dapper) { _dapper = dapper; }
    public async Task<List<NameList>> Handle(ModuleNameAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id)
            .SelectAs<PerModule, NameList>(v, x => x.Desc, d => d.Name)
            .From<PerModule>(v)
            .OrderBy<PerModule>(v, x => x.Key, desc: false);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<NameList>(ct);
        return list;
    }
}

public class ModuleNameByIdQryHandler : IRequestHandler<ModuleNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public ModuleNameByIdQryHandler(IDapperHelper dapper) { _dapper = dapper; }
    public async Task<NameList?> Handle(ModuleNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id)
            .SelectAs<PerModule, NameList>(v, x => x.Desc, d => d.Name)
            .From<PerModule>(v)
            .Where<PerModule>(v, p => p.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, parameters, ct);
        if (data == null) return null;
        return data;
    }
}

public class PerMenuNameAllQryHandler : IRequestHandler<PerMenuNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public PerMenuNameAllQryHandler(IDapperHelper dapper) { _dapper = dapper; }
    public async Task<List<NameList>> Handle(PerMenuNameAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerMenu>(v, x => x.Id)
            .SelectAs<PerMenu, NameList>(v, x => x.Label, d => d.Name)
            .From<PerMenu>(v)
            .OrderBy<PerMenu>(v, x => x.Key, desc: false);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<NameList>(ct);
        return list;
    }
}

public class PerMenuNameByIdQryHandler : IRequestHandler<PerMenuNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public PerMenuNameByIdQryHandler(IDapperHelper dapper) { _dapper = dapper; }
    public async Task<NameList?> Handle(PerMenuNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerMenu>(v, x => x.Id)
            .SelectAs<PerMenu, NameList>(v, x => x.Label, d => d.Name)
            .From<PerMenu>(v)
            .Where<PerMenu>(v, p => p.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, parameters, ct);
        if (data == null) return null;
        return data;
    }
}

public class PerApiNameAllQryHandler : IRequestHandler<PerApiNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public PerApiNameAllQryHandler(IDapperHelper dapper) { _dapper = dapper; }
    public async Task<List<NameList>> Handle(PerApiNameAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerApi>(v, x => x.Id)
            .SelectAs<PerApi, NameList>(v, x => x.Desc, d => d.Name)
            .From<PerApi>(v)
            .OrderBy<PerApi>(v, x => x.Key, desc: false);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<NameList>(ct);
        return list;
    }
}

public class PerApiNameByIdQryHandler : IRequestHandler<PerApiNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public PerApiNameByIdQryHandler(IDapperHelper dapper) { _dapper = dapper; }
    public async Task<NameList?> Handle(PerApiNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerApi>(v, x => x.Id)
            .SelectAs<PerApi, NameList>(v, x => x.Desc, d => d.Name)
            .From<PerApi>(v)
            .Where<PerApi>(v, p => p.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, parameters, ct);
        if (data == null) return null;
        return data;
    }
}

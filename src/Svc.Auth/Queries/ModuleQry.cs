using Helpers;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public sealed class ModuleAllQry : IRequest<List<ModuleListDto>> { }
public sealed class ModuleByIdQry : IRequest<ModuleListDto?> { public Guid Id { get; set; } }
public sealed class ModIdBykeyQry : IRequest<IdDto?> { public string Key { get; set; } = default!; }
public sealed class ModIdsBykeysQry : IRequest<List<KeyIdDto>> { public List<string> Keys { get; init; } = []; }


public sealed class ModuleAll(IDapperHelper _dapper) : IRequestHandler<ModuleAllQry, List<ModuleListDto>>
{
    public async Task<List<ModuleListDto>> Handle(ModuleAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id, x => x.Key)
            .SelectAs<PerModule, ModuleListDto>(v, x => x.Desc, d => d.Name)
            .From<PerModule>(v)
            .OrderBy<PerModule>(v, x => x.Key, desc: false);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<ModuleListDto>(ct);
        return list;
    }
}

public sealed class ModuleById(IDapperHelper _dapper) : IRequestHandler<ModuleByIdQry, ModuleListDto?>
{
    public async Task<ModuleListDto?> Handle(ModuleByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id, x => x.Key)
            .SelectAs<PerModule, ModuleListDto>(v, x => x.Desc, d => d.Name)
            .From<PerModule>(v)
            .Where<PerModule>(v, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<ModuleListDto>(sql, parameters, ct);
        if (data == null) return null;
        return data;
    }
}

public sealed class ModIdBykey(IDapperHelper _dapper) : IRequestHandler<ModIdBykeyQry, IdDto?>
{
    public async Task<IdDto?> Handle(ModIdBykeyQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id)
            .From<PerModule>(v)
            .Where<PerModule>(v, x => x.Key == request.Key)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<IdDto>(sql, parameters, ct);
        if (data == null) return null;
        return data;
    }
}

public sealed class ModIdsBykeys(IDapperHelper _dapper) : IRequestHandler<ModIdsBykeysQry, List<KeyIdDto>>
{
    public async Task<List<KeyIdDto>> Handle(ModIdsBykeysQry request, CancellationToken ct)
    {
        if (request.Keys == null || request.Keys.Count == 0) { return []; }
        var keys = request.Keys.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        if (keys.Length == 0) { return []; }

        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id, x => x.Key)
            .From<PerModule>(v)
            .WhereIn<PerModule>(v, x => x.Key, keys);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryAsync<KeyIdDto>(sql, parameters, ct);
        return data?.ToList() ?? [];
    }
}
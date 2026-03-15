using Helpers;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class ModuleAllQry : IRequest<List<ModuleListDto>> { }
public class ModuleByIdQry : IRequest<ModuleListDto?> { public Guid Id { get; set; }}



public class ModuleAllQryHandler : IRequestHandler<ModuleAllQry, List<ModuleListDto>>
{
    private readonly IDapperHelper _dapper;
    public ModuleAllQryHandler(IDapperHelper dapper) { _dapper = dapper; }

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

public class ModuleByIdQryHandler : IRequestHandler<ModuleByIdQry, ModuleListDto?>
{
    private readonly IDapperHelper _dapper;
    public ModuleByIdQryHandler(IDapperHelper dapper) { _dapper = dapper; }

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
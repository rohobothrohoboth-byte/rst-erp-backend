using Helpers;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

// ==================== QUERY DEFINITIONS ====================

public sealed class ModuleAllQry : IRequest<List<ModuleListDto>> { }
public sealed class ModuleByIdQry : IRequest<ModuleListDto?> { public Guid Id { get; set; } }
public sealed class ModIdBykeyQry : IRequest<IdDto?> { public string Key { get; set; } = default!; }
public sealed class ModIdsBykeysQry : IRequest<List<KeyIdDto>> { public List<string> Keys { get; init; } = []; }

// Used by PerModuleHandlers.cs
public class PerModuleAllQry : IRequest<List<PerModuleListDto>> { }
public class PerModuleByIdQry : IRequest<PerModuleListDto> { public Guid Id { get; set; } }

// ==================== HANDLERS ====================

// ? FIXED: Separate handler classes, not nested
public sealed class ModuleAllHandler : IRequestHandler<ModuleAllQry, List<ModuleListDto>>
{
    private readonly IDapperHelper _dapper;

    public ModuleAllHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<ModuleListDto>> Handle(ModuleAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id, x => x.Key!, x => x.Desc!, x => x.Icon!, x => x.Order)
            .SelectAs<PerModule, ModuleListDto>(v, x => x.Desc, d => d.Name)
            .Select<PerModule>(v, x => x.Icon!)
            .Select<PerModule>(v, x => x.Order)
            .From<PerModule>(v)
            .OrderBy<PerModule>(v, x => x.Order, desc: false)
            .Where<PerModule>(v, x => x.IsDeleted == false);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<ModuleListDto>(ct);
        return list;
    }
}

public sealed class ModuleByIdHandler : IRequestHandler<ModuleByIdQry, ModuleListDto?>
{
    private readonly IDapperHelper _dapper;

    public ModuleByIdHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<ModuleListDto?> Handle(ModuleByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id, x => x.Key!, x => x.Desc!, x => x.Icon!, x => x.Order)
            .SelectAs<PerModule, ModuleListDto>(v, x => x.Desc, d => d.Name)
            .Select<PerModule>(v, x => x.Icon!)
            .Select<PerModule>(v, x => x.Order)
            .From<PerModule>(v)
            .Where<PerModule>(v, x => x.Id == request.Id)
            .Where<PerModule>(v, x => x.IsDeleted == false)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<ModuleListDto?>(sql, parameters, ct);
        if (data == null) return null;
        return data;
    }
}

public sealed class ModIdBykeyHandler : IRequestHandler<ModIdBykeyQry, IdDto?>
{
    private readonly IDapperHelper _dapper;

    public ModIdBykeyHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<IdDto?> Handle(ModIdBykeyQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id)
            .From<PerModule>(v)
            .Where<PerModule>(v, x => x.Key == request.Key)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<IdDto?>(sql, parameters, ct);
        if (data == null) return null;
        return data;
    }
}

public sealed class ModIdsBykeysHandler : IRequestHandler<ModIdsBykeysQry, List<KeyIdDto>>
{
    private readonly IDapperHelper _dapper;

    public ModIdsBykeysHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<KeyIdDto>> Handle(ModIdsBykeysQry request, CancellationToken ct)
    {
        if (request.Keys == null || request.Keys.Count == 0) { return []; }
        var keys = request.Keys.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        if (keys.Length == 0) { return []; }

        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id, x => x.Key!)
            .From<PerModule>(v)
            .WhereIn<PerModule>(v, x => x.Key, keys);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryAsync<KeyIdDto>(sql, parameters, ct);
        return data?.ToList() ?? [];
    }
}

// ==================== PER MODULE HANDLERS ====================

public class PerModuleAllHandler : IRequestHandler<PerModuleAllQry, List<PerModuleListDto>>
{
    private readonly IDapperHelper _dapper;

    public PerModuleAllHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<PerModuleListDto>> Handle(PerModuleAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id, x => x.Key!, x => x.Desc!, x => x.Icon!, x => x.Order, x => x.DateAdd, x => x.DateMod!, x => x.IsDeleted)
            .From<PerModule>(v)
            .Where<PerModule>(v, x => x.IsDeleted == false)
            .OrderBy<PerModule>(v, x => x.Order, desc: false);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<PerModuleListDto>(ct);
        return list;
    }
}

public class PerModuleByIdHandler : IRequestHandler<PerModuleByIdQry, PerModuleListDto>
{
    private readonly IDapperHelper _dapper;

    public PerModuleByIdHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<PerModuleListDto> Handle(PerModuleByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PerModule>(v, x => x.Id, x => x.Key!, x => x.Desc!, x => x.Icon!, x => x.Order, x => x.DateAdd, x => x.DateMod!, x => x.IsDeleted)
            .From<PerModule>(v)
            .Where<PerModule>(v, x => x.Id == request.Id)
            .Where<PerModule>(v, x => x.IsDeleted == false)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PerModuleListDto>(sql, parameters, ct);
        if (data == null)
            throw new DomainException($"MODULE with id [{request.Id}] NOT FOUND.");
        return data;
    }
}
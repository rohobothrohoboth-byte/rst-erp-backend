using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Helpers;
using MediatR;

namespace Cor.Module.Queries;


public class DbcAllQry : IRequest<List<DbcResDto>> { }
public class DbcByIdQry : IRequest<DbcResDto?> { public Guid Id { get; set; } }



public class DbcAllHandler : IRequestHandler<DbcAllQry, List<DbcResDto>>
{
    private readonly IDapperHelper _dapper;
    public DbcAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<DbcResDto>> Handle(DbcAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string b = "b";
        var qb = new QueryBuilder()
            .SelectAs<Department, DbcResDto>(b, x => x.Id, d => d.DeptId)
            .Select<Department>(v, x => x.Id, x => x.BranchId)
            .Select<Branch>(b, x => x.CompId)
            .Join<Department, Branch>(v, b, x => x.BranchId, x => x.Id)
            .From<Department>(v);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var dataL = await reader.ToListAsync<DbcResDto>(ct);
        return dataL;
    }
}

public class DbcByIdHandler : IRequestHandler<DbcByIdQry, DbcResDto?>
{
    private readonly IDapperHelper _dapper;
    public DbcByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<DbcResDto?> Handle(DbcByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string b = "b";
        var qb = new QueryBuilder()
            .SelectAs<Department, DbcResDto>(b, x => x.Id, d => d.DeptId)
            .Select<Department>(v, x => x.Id, x => x.BranchId)
            .Select<Branch>(b, x => x.CompId)
            .Join<Department, Branch>(v, b, x => x.BranchId, x => x.Id)
            .From<Department>(v)
            .Where<Department>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var dataL = await reader.FirstOrDefaultAsync<DbcResDto>(ct);
        return dataL;

    }
}
using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.Module.Queries;

public class AllDeptsQry : IRequest<List<DeptListDto>> { }
public class DeptByIdQry : IRequest<DeptListDto?> { public Guid Id { get; set; } }



public class AllDeptsHandler : IRequestHandler<AllDeptsQry, List<DeptListDto>>
{
    private readonly IDapperHelper _dapper;
    public AllDeptsHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<DeptListDto>> Handle(AllDeptsQry request, CancellationToken ct)
    {
        const string v = "v";
        const string b = "b";
        var qb = new QueryBuilder()
            .Select<Department>(v, x => x.Id, x => x.Name, x => x.NameAm, x => x.DeptStat, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<Branch, DeptListDto>(b, x => x.Name, d => d.Branch)
            .SelectAs<Branch, DeptListDto>(b, x => x.NameAm, d => d.BranchAm)
            .From<Department>(v)
            .Join<Department, Branch>(v, b, x => x.BranchId, x => x.Id)
            .OrderBy<Department>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<DeptListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<DeptListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new DeptListDto
            {
                Id = data.Id,
                Name = data.Name,
                NameAm = data.NameAm,
                DeptStat = data.DeptStat,
                DeptStatStr = MyEnumHelper.FormatEnum<DeptStat>(data.DeptStat),
                Branch = data.Branch,
                BranchAm = data.BranchAm,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return dataL;
    }
}

public class DeptByIdHandler : IRequestHandler<DeptByIdQry, DeptListDto?>
{
    private readonly IDapperHelper _dapper;
    public DeptByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<DeptListDto?> Handle(DeptByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string b = "b";
        var qb = new QueryBuilder()
            .Select<Department>(v, x => x.Id, x => x.Name, x => x.NameAm, x => x.DeptStat, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<Branch, DeptListDto>(b, x => x.Name, d => d.Branch)
            .SelectAs<Branch, DeptListDto>(b, x => x.NameAm, d => d.BranchAm)
            .From<Department>(v)
            .Join<Department, Branch>(v, b, x => x.BranchId, x => x.Id)
            .Where<Department>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<DeptListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new DeptListDto
        {
            Id = data.Id,
            Name = data.Name,
            NameAm = data.NameAm,
            DeptStat = data.DeptStat,
            DeptStatStr = MyEnumHelper.FormatEnum<DeptStat>(data.DeptStat),
            Branch = data.Branch,
            BranchAm = data.BranchAm,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
    }
}
using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Helpers;
using MediatR;

namespace Cor.Module.Queries;

public class AllBranchesQry : IRequest<List<BranchListDto>> { }
public class BranchByIdQry : IRequest<BranchListDto?> { public Guid Id { get; set; } }
public class BranchByCompQry : IRequest<List<BranchListDto>> { public Guid Id { get; set; } }



public class AllBranchesHandler : IRequestHandler<AllBranchesQry, List<BranchListDto>>
{
    private readonly IDapperHelper _dapper;
    public AllBranchesHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<BranchListDto>> Handle(AllBranchesQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<Branch>(v, x => x.Id, x => x.Name, x => x.NameAm, x => x.Code, x => x.CompId, x => x.Location, x => x.OpenDate, x => x.BranchType, x => x.BranchStat, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<Company, BranchListDto>(c, x => x.Name, d => d.Comp)
            .SelectAs<Company, BranchListDto>(c, x => x.NameAm, d => d.CompAm)
            .From<Branch>(v)
            .Join<Branch, Company>(v, c, x => x.CompId, x => x.Id)
            .OrderBy<Branch>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<BranchListDto>(ct);

        foreach (var data in list)
        {
            data.BranchTypeStr = MyEnumHelper.FormatEnum<BranchType>(data.BranchType);
            data.BranchStatStr = MyEnumHelper.FormatEnum<BranchStat>(data.BranchStat);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class BranchByIdHandler : IRequestHandler<BranchByIdQry, BranchListDto?>
{
    private readonly IDapperHelper _dapper;
    public BranchByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<BranchListDto?> Handle(BranchByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<Branch>(v, x => x.Id, x => x.Name, x => x.NameAm, x => x.Code, x => x.CompId, x => x.Location, x => x.OpenDate, x => x.BranchType, x => x.BranchStat, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<Company, BranchListDto>(c, x => x.Name, d => d.Comp)
            .SelectAs<Company, BranchListDto>(c, x => x.NameAm, d => d.CompAm)
            .From<Branch>(v)
            .Join<Branch, Company>(v, c, x => x.CompId, x => x.Id)
            .Where<Branch>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<BranchListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.BranchTypeStr = MyEnumHelper.FormatEnum<BranchType>(data.BranchType);
        data.BranchStatStr = MyEnumHelper.FormatEnum<BranchStat>(data.BranchStat);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class BranchByCompHandler : IRequestHandler<BranchByCompQry, List<BranchListDto>>
{
    private readonly IDapperHelper _dapper;
    public BranchByCompHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<BranchListDto>> Handle(BranchByCompQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<Branch>(v, x => x.Id, x => x.Name, x => x.NameAm, x => x.Code, x => x.CompId, x => x.Location, x => x.OpenDate, x => x.BranchType, x => x.BranchStat, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<Company, BranchListDto>(c, x => x.Name, d => d.Comp)
            .SelectAs<Company, BranchListDto>(c, x => x.NameAm, d => d.CompAm)
            .From<Branch>(v)
            .Join<Branch, Company>(v, c, x => x.CompId, x => x.Id)
            .Where<Branch>(v, x => x.CompId == request.Id);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<BranchListDto>(ct);

        foreach (var item in list)
        {
            item.BranchTypeStr = MyEnumHelper.FormatEnum<BranchType>(item.BranchType);
            item.BranchStatStr = MyEnumHelper.FormatEnum<BranchStat>(item.BranchStat);
            item.RowVersion = item.xmin.ToString();
        }

        return list;
    }
}
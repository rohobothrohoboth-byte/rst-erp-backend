using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.Module.Queries;

public class AllCompsQry : IRequest<List<CompListDto>> { }
public class CompByIdQry : IRequest<CompListDto?> { public Guid Id { get; set; } }



public class GetCompsHandler : IRequestHandler<AllCompsQry, List<CompListDto>>
{
    private readonly IDapperHelper _dapper;
    public GetCompsHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<CompListDto>> Handle(AllCompsQry request, CancellationToken ct)
    {
        const string c = "c";
        const string b = "b";
        var qb = new QueryBuilder()
            .Select<Company>(c, x => x.Id, x => x.Name, x => x.NameAm, x => x.TaxId!, x => x.Phone!, x => x.Email!, x => x.Address!, x => x.Website!, x => x.LogoUrl!, x => x.StampUrl!, x => x.Motto!, x => x.Mission!, x => x.Vision!, x => x.Values!, x => x.Structure!, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectRaw("COUNT(b.\"Id\") AS \"CountBra\"")
            .From<Company>(c)
            .LeftJoin<Company, Branch>(c, b, x => x.Id, x => x.CompId)
            .GroupBy(SqlGen.Col<Company>(c, x => x.Id), SqlGen.Col<Company>(c, x => x.Name));

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<CompListDto>(ct);

        foreach (var item in list)
        {
            item.BranchCount = $"{item.CountBra}";
            item.RowVersion = item.xmin.ToString();
        }
        return list;
    }
}

public class GetCompByIdHandler : IRequestHandler<CompByIdQry, CompListDto?>
{
    private readonly IDapperHelper _dapper;
    public GetCompByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<CompListDto?> Handle(CompByIdQry request, CancellationToken ct)
    {
        const string c = "c";
        const string b = "b";
        var qb = new QueryBuilder()
            .Select<Company>(c, x => x.Id, x => x.Name, x => x.NameAm, x => x.TaxId!, x => x.Phone!, x => x.Email!, x => x.Address!, x => x.Website!, x => x.LogoUrl!, x => x.StampUrl!, x => x.Motto!, x => x.Mission!, x => x.Vision!, x => x.Values!, x => x.Structure!, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectRaw("COUNT(b.\"Id\") AS \"CountBra\"")
            .From<Company>(c)
            .LeftJoin<Company, Branch>(c, b, x => x.Id, x => x.CompId)
            .GroupBy(SqlGen.Col<Company>(c, x => x.Id), SqlGen.Col<Company>(c, x => x.Name))
            .Where<Company>(c, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<CompListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.BranchCount = $"{data.CountBra}";
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}
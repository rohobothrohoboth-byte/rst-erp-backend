using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.Module.Queries;

public class BraCompListQry : IRequest<List<NameList>> { }
public class BraCompByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class BraAllNameQry : IRequest<List<NameList>> { }
public class BraNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class DeptByBraQry : IRequest<List<BranchDeptList>> { public Guid Id { get; set; } }
public class DeptAllNameQry : IRequest<List<NameAmList>> { }
public class DeptNameByIdQry : IRequest<NameAmList?> { public Guid Id { get; set; } }
public class CompAllNameQry : IRequest<List<NameList>> { }
public class CompNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class FiscYearAllNameQry : IRequest<List<NameList>> { }
public class FiscYearNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class FiscYearActiveQry : IRequest<List<NameList>> { }
public class PeriodAllNameQry : IRequest<List<NameList>> { }
public class PeriodNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }



public class BraCompListHandler : IRequestHandler<BraCompListQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public BraCompListHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(BraCompListQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<Branch>(v, x => x.Id, x => x.Name)
            .SelectAs<Company, NameAmList>(c, x => x.Name, d => d.NameAm)
            .From<Branch>(v)
            .Join<Branch, Company>(v, c, x => x.CompId, x => x.Id);

        var (sql, parameters) = qb.Build();
        var dataL = new List<NameList>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<NameAmList>();
        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new NameList
            {
                Id = data.Id,
                Name = $"{data.Name} => {data.NameAm}"
            });
        }
        return dataL;
    }
}

public class BraCompByIdHandler : IRequestHandler<BraCompByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public BraCompByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(BraCompByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<Branch>(v, x => x.Id, x => x.Name)
            .SelectAs<Company, NameAmList>(c, x => x.Name, d => d.NameAm)
            .From<Branch>(v)
            .Join<Branch, Company>(v, c, x => x.CompId, x => x.Id)
            .Where<Branch>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<NameAmList>(sql, parameters, ct);
        if (data == null) return null;

        return new NameList
        {
            Id = data.Id,
            Name = $"{data.Name} => {data.NameAm}"
        };
    }
}

public class BraAllNameHandler : IRequestHandler<BraAllNameQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public BraAllNameHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(BraAllNameQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().SelectDto<Branch, NameList>(v).From<Branch>(v);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<NameList>(ct);
        return list;
    }
}

public class BraNameByIdHandler : IRequestHandler<BraNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public BraNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(BraNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().SelectDto<Branch, NameList>(v).From<Branch>(v).Where<Branch>(v, x => x.Id == request.Id).Limit(1);
        var (sql, parameters) = qb.Build();

        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var dto = await reader.FirstOrDefaultAsync<NameList>(ct);

        if (dto == null) { return null; }
        return dto;
    }
}

public class DeptByBraHandler : IRequestHandler<DeptByBraQry, List<BranchDeptList>>
{
    private readonly IDapperHelper _dapper;
    public DeptByBraHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<BranchDeptList>> Handle(DeptByBraQry request, CancellationToken ct)
    {
        const string v = "v";
        const string jg = "jg";
        var qb = new QueryBuilder()
            .Select<Department>(v, x => x.Id, x => x.BranchId)
            .SelectAs<Department, BranchDeptList>(v, x => x.Name, d => d.Dept)
            .SelectAs<Branch, BranchDeptList>(v, x => x.Name, d => d.Branch)
            .From<Department>(v)
            .Join<Department, Branch>(v, jg, x => x.BranchId, x => x.Id)
            .Where<Department>(v, x => x.BranchId == request.Id);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<BranchDeptList>(ct);
        return list;
    }
}

public class DeptAllNameHandler : IRequestHandler<DeptAllNameQry, List<NameAmList>>
{
    private readonly IDapperHelper _dapper;
    public DeptAllNameHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameAmList>> Handle(DeptAllNameQry request, CancellationToken ct)
    {
        const string v = "v";
        const string jg = "jg";
        var qb = new QueryBuilder()
            .Select<Department>(v, x => x.Id, x => x.Name)
            .SelectAs<Branch, NameAmList>(v, x => x.Name, d => d.NameAm)
            .From<Department>(v)
            .Join<Department, Branch>(v, jg, x => x.BranchId, x => x.Id);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<NameAmList>(ct);
        return list;
    }
}

public class DeptNameByIdHandler : IRequestHandler<DeptNameByIdQry, NameAmList?>
{
    private readonly IDapperHelper _dapper;
    public DeptNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameAmList?> Handle(DeptNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string jg = "jg";
        var qb = new QueryBuilder()
            .Select<Department>(v, x => x.Id, x => x.Name)
            .SelectAs<Branch, NameAmList>(v, x => x.Name, d => d.NameAm)
            .From<Department>(v)
            .Join<Department, Branch>(v, jg, x => x.BranchId, x => x.Id)
            .Where<Department>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.FirstOrDefaultAsync<NameAmList>(ct);
        return list;
    }
}

public class CompAllNameHandler : IRequestHandler<CompAllNameQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public CompAllNameHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(CompAllNameQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().SelectDto<Company, NameList>(v).From<Company>(v);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<NameList>(ct);
        return list;
    }
}

public class CompNameByIdHandler : IRequestHandler<CompNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public CompNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(CompNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().SelectDto<Company, NameList>(v).From<Company>(v).Where<Company>(v, x => x.Id == request.Id).Limit(1);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.FirstOrDefaultAsync<NameList>(ct);
        return list;
    }
}

public class FiscYearAllNameHandler : IRequestHandler<FiscYearAllNameQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public FiscYearAllNameHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(FiscYearAllNameQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().SelectDto<FiscalYear, NameList>(v).From<FiscalYear>(v);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<NameList>(ct);
        return list;
    }
}

public class FiscYearNameByIdHandler : IRequestHandler<FiscYearNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public FiscYearNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(FiscYearNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().SelectDto<FiscalYear, NameList>(v).From<FiscalYear>(v).Where<FiscalYear>(v, x => x.Id == request.Id).Limit(1);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.FirstOrDefaultAsync<NameList>(ct);
        return list;
    }
}

public class FiscYearActiveHandler : IRequestHandler<FiscYearActiveQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public FiscYearActiveHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(FiscYearActiveQry request, CancellationToken ct)
    {
        var stat = BoolToStr.EnumToString(YesNo.Yes);
        const string v = "v";
        var qb = new QueryBuilder()
            .SelectDto<FiscalYear, NameList>(v)
            .From<FiscalYear>(v)
            .Where<FiscalYear>(v, f => f.IsActive == stat && f.DateEnd >= DateTime.UtcNow);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<NameList>(ct);
        return list;
    }
}

public class PeriodAllNameHandler : IRequestHandler<PeriodAllNameQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public PeriodAllNameHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(PeriodAllNameQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().SelectDto<Period, NameList>(v).From<Period>(v);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<NameList>(ct);
        return list;
    }
}

public class PeriodNameByIdHandler : IRequestHandler<PeriodNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public PeriodNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(PeriodNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().SelectDto<Period, NameList>(v).From<Period>(v).Where<Period>(v, x => x.Id == request.Id).Limit(1);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.FirstOrDefaultAsync<NameList>(ct);
        return list;
    }
}
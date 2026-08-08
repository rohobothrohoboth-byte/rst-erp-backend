using Common;
using Dapper;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class GetPosEmpQry : IRequest<EmpPosResDto?> { public Guid Id { get; set; } }
public class GetEmpIdByIdQry : IRequest<HrmEmpId?> { public Guid Id { get; set; } }
public class GetEmpIdAllQry : IRequest<List<HrmEmpId>> { }



public class GetPosEmpHandler : IRequestHandler<GetPosEmpQry, EmpPosResDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHRMM;

    public GetPosEmpHandler(IDapperHelper dapper, ICorHrmmClient corHRMM)
    {
        _dapper = dapper;
        _corHRMM = corHRMM;
    }

    public async Task<EmpPosResDto?> Handle(GetPosEmpQry request, CancellationToken ct)
    {
        const string e = "e";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.PositionId)
            .From<Employee>(e)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<Employee>(sql, parameters, ct);
        if (row == null) return null;

        var posReq = await _corHRMM.GetPosReq(row.PositionId.ToString(), ct);
        if (posReq.Id == null) { return null; }
        var res = new EmpPosResDto
        {
            Id = Guid.Parse(posReq.Id),
            PositionId = row.PositionId,
            SaturdayWorkOption = posReq.SaturdayWorkOption,
            SundayWorkOption = posReq.SundayWorkOption
        };

        return res;
    }
}

public class GetEmpIdByIdHandler : IRequestHandler<GetEmpIdByIdQry, HrmEmpId?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;

    public GetEmpIdByIdHandler(IDapperHelper dapper, ICorModClient corMod)
    {
        _dapper = dapper;
        _corMod = corMod;
    }

    public async Task<HrmEmpId?> Handle(GetEmpIdByIdQry request, CancellationToken ct)
    {
        const string e = "e";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.DepartmentId)
            .From<Employee>(e)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<Employee>(sql, parameters, ct);
        if (data == null) return null;

        var dept = await _corMod.GetDbc(data.DepartmentId.ToString(), ct);

        var c = new HrmEmpId
        {
            Id = data.Id,
            PositionId = data.PositionId,
            DeptId = data.DepartmentId,
            BranchId = dept != null ? Guid.Parse(dept!.BranchId) : Guid.Empty,
            CompanyId = dept != null ? Guid.Parse(dept!.CompId) : Guid.Empty,
            JgStepId = data.JobGradeId,
            JgId = data.JobGradeId
        };
        return c;
    }
}

public class GetEmpIdAllHandler : IRequestHandler<GetEmpIdAllQry, List<HrmEmpId>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;

    public GetEmpIdAllHandler(IDapperHelper dapper, ICorModClient corMod)
    {
        _dapper = dapper;
        _corMod = corMod;
    }

    public async Task<List<HrmEmpId>> Handle(GetEmpIdAllQry request, CancellationToken ct)
    {
        var deptTask = await _corMod.GetListDbc(ct);
        var deptDict = deptTask.Res.ToDictionary(d => Guid.Parse(d.DeptId));

        const string e = "e";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.DepartmentId, x => x.JobGradeId, x => x.PositionId)
            .From<Employee>(e)
            .OrderBy<Employee>(e, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<EmpJoinRow>();
        var dataL = new List<HrmEmpId>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            deptDict.TryGetValue(data.DepartmentId, out var dept);

            dataL.Add(new HrmEmpId
            {
                Id = data.Id,
                PositionId = data.PositionId,
                DeptId = data.DepartmentId,
                BranchId = dept != null ? Guid.Parse(dept!.BranchId) : Guid.Empty,
                CompanyId = dept != null ? Guid.Parse(dept!.CompId) : Guid.Empty,
                JgStepId = data.JobGradeId,
                JgId = data.JobGradeId
            });
        }

        return dataL;
    }
}
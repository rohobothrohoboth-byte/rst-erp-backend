using Common;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class LeaveAppChainByPolicyIdQry : IRequest<List<LeaveAppChainListDto>> { public Guid Id { get; set; } }
public class LeaveAppChainByIdQry : IRequest<LeaveAppChainListDto?> { public Guid Id { get; set; } }
public class ActiveLeaveAppChainQry : IRequest<LeaveAppChainListDto?> { public Guid Id { get; set; } }



public class LeaveAppChainByPolicyIdHandler : IRequestHandler<LeaveAppChainByPolicyIdQry, List<LeaveAppChainListDto>>
{
    private readonly IDapperHelper _dapper;
    public LeaveAppChainByPolicyIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<LeaveAppChainListDto>> Handle(LeaveAppChainByPolicyIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string p = "p";
        const string s = "s";
        var qb = new QueryBuilder()
            .Select<LeaveAppChain>(v, x => x.Id, x => x.EffectiveFrom, x => x.EffectiveTo, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeavePolicy, LeaveAppChainListDto>(p, x => x.Name, d => d.LeavePolicy)
            .From<LeaveAppChain>(v)
            .Join<LeaveAppChain, LeavePolicy>(v, p, x => x.LeavePolicyId, x => x.Id)
            .LeftJoin<LeaveAppChain, LeaveAppStep>(v, s, x => x.Id, x => x.LeaveAppChainId)
            .OrderBy<LeaveAppChain>(v, x => x.DateAdd, desc: true)
            .Where<LeaveAppChain>(v, x => x.LeavePolicyId == request.Id);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<LeaveAppChainListDto>(ct);

        foreach (var item in list)
        {
            item.IsActiveStr = BoolToStr.FormatStat(item.IsActive);
            item.RowVersion = item.xmin.ToString();
        }
        return list;
    }
}

public class LeaveAppChainByIdHandler : IRequestHandler<LeaveAppChainByIdQry, LeaveAppChainListDto?>
{
    private readonly IDapperHelper _dapper;
    public LeaveAppChainByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<LeaveAppChainListDto?> Handle(LeaveAppChainByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string p = "p";
        const string s = "s";
        var qb = new QueryBuilder()
            .Select<LeaveAppChain>(v, x => x.Id, x => x.EffectiveFrom, x => x.EffectiveTo, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeavePolicy, LeaveAppChainListDto>(p, x => x.Name, d => d.LeavePolicy)
            .From<LeaveAppChain>(v)
            .Join<LeaveAppChain, LeavePolicy>(v, p, x => x.LeavePolicyId, x => x.Id)
            .LeftJoin<LeaveAppChain, LeaveAppStep>(v, s, x => x.Id, x => x.LeaveAppChainId)
            .Where<LeaveAppChain>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeaveAppChainListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class ActiveLeaveAppChainHandler : IRequestHandler<ActiveLeaveAppChainQry, LeaveAppChainListDto?>
{
    private readonly IDapperHelper _dapper;
    public ActiveLeaveAppChainHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<LeaveAppChainListDto?> Handle(ActiveLeaveAppChainQry request, CancellationToken ct)
    {
        const string v = "v";
        const string p = "p";
        const string s = "s";
        var qb = new QueryBuilder()
            .Select<LeaveAppChain>(v, x => x.Id, x => x.EffectiveFrom, x => x.EffectiveTo, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeavePolicy, LeaveAppChainListDto>(p, x => x.Name, d => d.LeavePolicy)
            .From<LeaveAppChain>(v)
            .Join<LeaveAppChain, LeavePolicy>(v, p, x => x.LeavePolicyId, x => x.Id)
            .LeftJoin<LeaveAppChain, LeaveAppStep>(v, s, x => x.Id, x => x.LeaveAppChainId)
            .OrderBy<LeaveAppChain>(v, x => x.DateAdd, desc: true)
            .Where<LeaveAppChain>(v, x => x.LeavePolicyId == request.Id && x.IsActive == true)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeaveAppChainListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}
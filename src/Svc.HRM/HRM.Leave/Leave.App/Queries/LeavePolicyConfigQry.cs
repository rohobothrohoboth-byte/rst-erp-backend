using Common;
using Dapper;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class PolicyConfigByPolicyIdQry : IRequest<List<LeavePolicyConfigListDto>> { public Guid Id { get; set; } }
public class LeavePolicyConfigByIdQry : IRequest<LeavePolicyConfigListDto?> { public Guid Id { get; set; } }
public class ActivePolicyConfigQry : IRequest<LeavePolicyConfigListDto?> { public Guid Id { get; set; } }



public class PolicyConfigByPolicyIdHandler : IRequestHandler<PolicyConfigByPolicyIdQry, List<LeavePolicyConfigListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;
    public PolicyConfigByPolicyIdHandler(IDapperHelper dapper, ICorModClient corMod)
    {
        _dapper = dapper;
        _corMod = corMod;
    }

    public async Task<List<LeavePolicyConfigListDto>> Handle(PolicyConfigByPolicyIdQry request, CancellationToken ct)
    {
        var fyTask = await _corMod.GetListFiscalYear(ct);
        var fyDict = fyTask.Res.ToDictionary(d => Guid.Parse(d.Id));

        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicyConfig>(v, x => x.Id, x => x.AnnualEntitlement, x => x.AccrualFrequency, x => x.AccrualRate, x => x.MaxDaysPerReq, x => x.MaxCarryOverDays, x => x.MinServiceMonths, x => x.IsActive, x => x.FiscalYearId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeavePolicy, LeavePolicyConfigListDto>(c, x => x.Name, d => d.LeavePolicy)
            .From<LeavePolicyConfig>(v)
            .Join<LeavePolicyConfig, LeavePolicy>(v, c, x => x.LeavePolicyId, x => x.Id)
            .Where<LeavePolicyConfig>(v, x => x.LeavePolicyId == request.Id);

        var (sql, parameters) = qb.Build();
        var result = new List<LeavePolicyConfigListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<LeavePolicyConfigListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            fyDict.TryGetValue(data.FiscalYearId, out var fy);

            result.Add(new LeavePolicyConfigListDto
            {
                Id = data.Id,
                AnnualEntitlement = data.AnnualEntitlement,
                AccrualFrequency = data.AccrualFrequency,
                AccrualRate = data.AccrualRate,
                MaxDaysPerReq = data.MaxDaysPerReq,
                MaxCarryOverDays = data.MaxCarryOverDays,
                MinServiceMonths = data.MinServiceMonths,
                IsActive = data.IsActive,
                AnnualEntitlementStr = $"{data.AnnualEntitlement} day/s",
                AccrualFrequencyStr = MyEnumHelper.FormatEnum<AccrualFrequency>(data.AccrualFrequency),
                AccrualRateStr = $"{data.AccrualRate} day/s",
                MaxDaysPerReqStr = $"{data.MaxDaysPerReq} day/s",
                MaxCarryOverDaysStr = $"{data.MaxCarryOverDays} day/s",
                MinServiceMonthsStr = $"{data.MinServiceMonths} month/s",
                IsActiveStr = BoolToStr.FormatStat(data.IsActive),
                FiscalYear = fy?.Name ?? "",
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }

        return result;
    }
}

public class LeavePolicyConfigByIdHandler : IRequestHandler<LeavePolicyConfigByIdQry, LeavePolicyConfigListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;
    public LeavePolicyConfigByIdHandler(IDapperHelper dapper, ICorModClient corMod)
    {
        _dapper = dapper;
        _corMod = corMod;
    }

    public async Task<LeavePolicyConfigListDto?> Handle(LeavePolicyConfigByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicyConfig>(v, x => x.Id, x => x.AnnualEntitlement, x => x.AccrualFrequency, x => x.AccrualRate, x => x.MaxDaysPerReq, x => x.MaxCarryOverDays, x => x.MinServiceMonths, x => x.IsActive, x => x.FiscalYearId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeavePolicy, LeavePolicyConfigListDto>(c, x => x.Name, d => d.LeavePolicy)
            .From<LeavePolicyConfig>(v)
            .Join<LeavePolicyConfig, LeavePolicy>(v, c, x => x.LeavePolicyId, x => x.Id)
            .Where<LeavePolicyConfig>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeavePolicyConfigListDto>(sql, parameters, ct);
        if (data == null) return null;

        var fy = await _corMod.GetFiscalYear(data.FiscalYearId.ToString(), ct);
        data.AnnualEntitlementStr = $"{data.AnnualEntitlement} day/s";
        data.AccrualFrequencyStr = MyEnumHelper.FormatEnum<AccrualFrequency>(data.AccrualFrequency);
        data.AccrualRateStr = $"{data.AccrualRate} day/s";
        data.MaxDaysPerReqStr = $"{data.MaxDaysPerReq} day/s";
        data.MaxCarryOverDaysStr = $"{data.MaxCarryOverDays} day/s";
        data.MinServiceMonthsStr = $"{data.MinServiceMonths} month/s";
        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.FiscalYear = fy.Res.Name != null ? fy.Res.Name : "NOT AVAILABLE";
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class ActivePolicyConfigHandler : IRequestHandler<ActivePolicyConfigQry, LeavePolicyConfigListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;
    public ActivePolicyConfigHandler(IDapperHelper dapper, ICorModClient corMod)
    {
        _dapper = dapper;
        _corMod = corMod;
    }

    public async Task<LeavePolicyConfigListDto?> Handle(ActivePolicyConfigQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicyConfig>(v, x => x.Id, x => x.AnnualEntitlement, x => x.AccrualFrequency, x => x.AccrualRate, x => x.MaxDaysPerReq, x => x.MaxCarryOverDays, x => x.MinServiceMonths, x => x.IsActive, x => x.FiscalYearId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeavePolicy, LeavePolicyConfigListDto>(c, x => x.Name, d => d.LeavePolicy)
            .From<LeavePolicyConfig>(v)
            .Join<LeavePolicyConfig, LeavePolicy>(v, c, x => x.LeavePolicyId, x => x.Id)
            .Where<LeavePolicyConfig>(v, x => x.LeavePolicyId == request.Id && x.IsActive == true)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeavePolicyConfigListDto>(sql, parameters, ct);
        if (data == null) return null;

        var fy = await _corMod.GetFiscalYear(data.FiscalYearId.ToString(), ct);
        data.AnnualEntitlementStr = $"{data.AnnualEntitlement} day/s";
        data.AccrualFrequencyStr = MyEnumHelper.FormatEnum<AccrualFrequency>(data.AccrualFrequency);
        data.AccrualRateStr = $"{data.AccrualRate} day/s";
        data.MaxDaysPerReqStr = $"{data.MaxDaysPerReq} day/s";
        data.MaxCarryOverDaysStr = $"{data.MaxCarryOverDays} day/s";
        data.MinServiceMonthsStr = $"{data.MinServiceMonths} month/s";
        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.FiscalYear = fy.Res.Name != null ? fy.Res.Name : "NOT AVAILABLE";
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}
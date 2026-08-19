using Common;
using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class OnboardingAssignAllQry : IRequest<List<OnboardingAssignmentListDto>> { }
public class OnboardingAssignByIdQry : IRequest<OnboardingAssignmentListDto?> { public Guid Id { get; set; } }
public class OnboardingAssignByEmployeeQry : IRequest<List<OnboardingAssignmentListDto>> { public Guid EmployeeId { get; set; } }
public class OnboardingAssignByTaskQry : IRequest<List<OnboardingAssignmentListDto>> { public Guid TaskId { get; set; } }

internal static class OnboardingAssignMapper
{
    public static OnboardingAssignmentListDto Map(OnboardingAssignRawDto raw) => new()
    {
        Id = raw.Id,
        EmployeeId = raw.EmployeeId,
        TaskId = raw.OnboardingTaskId,
        TaskName = raw.TaskName,
        TaskDescription = raw.Description,
        Status = raw.Status,
        IsMandatory = raw.IsMandatory,
        ScheduledDate = raw.ScheduledDate,
        CompletedDate = raw.CompletedDate,
        RowVersion = raw.xmin.ToString(),
        CreatedAt = raw.DateAdd,
        UpdatedAt = raw.DateMod,
    };

    // Best-effort enrichment of employee display fields via the Profile gRPC service.
    public static async Task Enrich(List<OnboardingAssignmentListDto> list, IHrmProfileClient hrmProfile, CancellationToken ct)
    {
        var cache = new Dictionary<Guid, (string name, string dept, string pos)>();
        foreach (var item in list)
        {
            if (item.EmployeeId == Guid.Empty) { continue; }
            if (!cache.TryGetValue(item.EmployeeId, out var info))
            {
                try
                {
                    var basic = await hrmProfile.GetEmpBasicInfo(item.EmployeeId.ToString(), ct);
                    info = (basic?.EmpFullName ?? "", basic?.Department ?? "", basic?.Position ?? "");
                }
                catch
                {
                    info = ("", "", "");
                }
                cache[item.EmployeeId] = info;
            }
            item.EmployeeName = info.name;
            item.Department = info.dept;
            item.Position = info.pos;
        }
    }
}

public class OnboardingAssignAllHandler : IRequestHandler<OnboardingAssignAllQry, List<OnboardingAssignmentListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;
    public OnboardingAssignAllHandler(IDapperHelper dapper, IHrmProfileClient hrmProfile) { _dapper = dapper; _hrmProfile = hrmProfile; }

    public async Task<List<OnboardingAssignmentListDto>> Handle(OnboardingAssignAllQry request, CancellationToken ct)
    {
        const string a = "a"; const string t = "t";
        var qb = new QueryBuilder()
            .Select<OnboardingAssign>(a, x => x.Id, x => x.IsMandatory, x => x.Status, x => x.ScheduledDate, x => x.CompletedDate!, x => x.EmployeeId, x => x.OnboardingTaskId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .Select<OnboardingTask>(t, x => x.TaskName, x => x.Description)
            .From<OnboardingAssign>(a)
            .Join<OnboardingAssign, OnboardingTask>(a, t, x => x.OnboardingTaskId, x => x.Id)
            .Where<OnboardingAssign>(a, x => x.IsDeleted == false)
            .OrderBy<OnboardingAssign>(a, x => x.ScheduledDate);

        var (sql, parameters) = qb.Build();
        var raw = (await _dapper.QueryAsync<OnboardingAssignRawDto>(sql, parameters, ct)).ToList();
        var list = raw.Select(OnboardingAssignMapper.Map).ToList();
        await OnboardingAssignMapper.Enrich(list, _hrmProfile, ct);
        return list;
    }
}

public class OnboardingAssignByIdHandler : IRequestHandler<OnboardingAssignByIdQry, OnboardingAssignmentListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;
    public OnboardingAssignByIdHandler(IDapperHelper dapper, IHrmProfileClient hrmProfile) { _dapper = dapper; _hrmProfile = hrmProfile; }

    public async Task<OnboardingAssignmentListDto?> Handle(OnboardingAssignByIdQry request, CancellationToken ct)
    {
        const string a = "a"; const string t = "t";
        var qb = new QueryBuilder()
            .Select<OnboardingAssign>(a, x => x.Id, x => x.IsMandatory, x => x.Status, x => x.ScheduledDate, x => x.CompletedDate!, x => x.EmployeeId, x => x.OnboardingTaskId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .Select<OnboardingTask>(t, x => x.TaskName, x => x.Description)
            .From<OnboardingAssign>(a)
            .Join<OnboardingAssign, OnboardingTask>(a, t, x => x.OnboardingTaskId, x => x.Id)
            .Where<OnboardingAssign>(a, x => x.Id == request.Id && x.IsDeleted == false)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var raw = await _dapper.QueryFirstOrDefaultAsync<OnboardingAssignRawDto>(sql, parameters, ct);
        if (raw == null) { return null; }
        var dto = OnboardingAssignMapper.Map(raw);
        await OnboardingAssignMapper.Enrich(new List<OnboardingAssignmentListDto> { dto }, _hrmProfile, ct);
        return dto;
    }
}

public class OnboardingAssignByEmployeeHandler : IRequestHandler<OnboardingAssignByEmployeeQry, List<OnboardingAssignmentListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;
    public OnboardingAssignByEmployeeHandler(IDapperHelper dapper, IHrmProfileClient hrmProfile) { _dapper = dapper; _hrmProfile = hrmProfile; }

    public async Task<List<OnboardingAssignmentListDto>> Handle(OnboardingAssignByEmployeeQry request, CancellationToken ct)
    {
        const string a = "a"; const string t = "t";
        var qb = new QueryBuilder()
            .Select<OnboardingAssign>(a, x => x.Id, x => x.IsMandatory, x => x.Status, x => x.ScheduledDate, x => x.CompletedDate!, x => x.EmployeeId, x => x.OnboardingTaskId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .Select<OnboardingTask>(t, x => x.TaskName, x => x.Description)
            .From<OnboardingAssign>(a)
            .Join<OnboardingAssign, OnboardingTask>(a, t, x => x.OnboardingTaskId, x => x.Id)
            .Where<OnboardingAssign>(a, x => x.EmployeeId == request.EmployeeId && x.IsDeleted == false)
            .OrderBy<OnboardingAssign>(a, x => x.ScheduledDate);

        var (sql, parameters) = qb.Build();
        var raw = (await _dapper.QueryAsync<OnboardingAssignRawDto>(sql, parameters, ct)).ToList();
        var list = raw.Select(OnboardingAssignMapper.Map).ToList();
        await OnboardingAssignMapper.Enrich(list, _hrmProfile, ct);
        return list;
    }
}

public class OnboardingAssignByTaskHandler : IRequestHandler<OnboardingAssignByTaskQry, List<OnboardingAssignmentListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;
    public OnboardingAssignByTaskHandler(IDapperHelper dapper, IHrmProfileClient hrmProfile) { _dapper = dapper; _hrmProfile = hrmProfile; }

    public async Task<List<OnboardingAssignmentListDto>> Handle(OnboardingAssignByTaskQry request, CancellationToken ct)
    {
        const string a = "a"; const string t = "t";
        var qb = new QueryBuilder()
            .Select<OnboardingAssign>(a, x => x.Id, x => x.IsMandatory, x => x.Status, x => x.ScheduledDate, x => x.CompletedDate!, x => x.EmployeeId, x => x.OnboardingTaskId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .Select<OnboardingTask>(t, x => x.TaskName, x => x.Description)
            .From<OnboardingAssign>(a)
            .Join<OnboardingAssign, OnboardingTask>(a, t, x => x.OnboardingTaskId, x => x.Id)
            .Where<OnboardingAssign>(a, x => x.OnboardingTaskId == request.TaskId && x.IsDeleted == false)
            .OrderBy<OnboardingAssign>(a, x => x.ScheduledDate);

        var (sql, parameters) = qb.Build();
        var raw = (await _dapper.QueryAsync<OnboardingAssignRawDto>(sql, parameters, ct)).ToList();
        var list = raw.Select(OnboardingAssignMapper.Map).ToList();
        await OnboardingAssignMapper.Enrich(list, _hrmProfile, ct);
        return list;
    }
}

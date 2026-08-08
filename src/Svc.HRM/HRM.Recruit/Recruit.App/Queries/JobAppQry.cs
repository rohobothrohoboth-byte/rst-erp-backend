using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.App.Services;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class JobAppAllQry : IRequest<List<JobAppListDto>> { }
public class JobAppByIdQry : IRequest<JobAppListDto?> { public Guid Id { get; set; } }
public class JobAppByJobPostQry : IRequest<List<JobAppListDto>> { public Guid Id { get; set; } }



public class JobAppAllHandler : IRequestHandler<JobAppAllQry, List<JobAppListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IJobAppService _jobAppService;

    public JobAppAllHandler(IDapperHelper dapper, IJobAppService jobAppService)
    {
        _dapper = dapper;
        _jobAppService = jobAppService;
    }

    public async Task<List<JobAppListDto>> Handle(JobAppAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<JobApplication>(v, x => x.Id, x => x.AppliedDate, x => x.Status, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<JobApplication>(v)
            .OrderBy<JobApplication>(v, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        var list = (await _dapper.QueryAsync<JobAppListDto>(sql, parameters, ct)).ToList();
        if (list.Count == 0) { return []; }

        var ids = list.Select(x => x.Id).ToList();
        var jobInfos = await _jobAppService.GetJobAppBatchInfo(ids, ct);

        foreach (var data in list)
        {
            jobInfos.TryGetValue(data.Id, out var jAppInfo);
            data.StatusStr = MyEnumHelper.FormatEnum<ApplicationStatus>(data.Status);
            data.Applicant = jAppInfo?.Applicant ?? "";
            data.JobPostingNum = jAppInfo?.PostNumber ?? "";
            data.Position = jAppInfo?.Position ?? "";
            data.Department = jAppInfo?.Department ?? "";
            data.Period = jAppInfo?.Period ?? "";
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class JobAppByIdHandler : IRequestHandler<JobAppByIdQry, JobAppListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly IJobAppService _jobAppService;

    public JobAppByIdHandler(IDapperHelper dapper, IJobAppService jobAppService)
    {
        _dapper = dapper;
        _jobAppService = jobAppService;
    }
// Recruit.App/Queries/JobAppQry.cs - JobAppByIdHandler

public async Task<JobAppListDto?> Handle(JobAppByIdQry request, CancellationToken ct)
{
    const string v = "v";
    var qb = new QueryBuilder()
        .Select<JobApplication>(v, x => x.Id, x => x.AppliedDate, x => x.Status, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
        .From<JobApplication>(v)
        .Where<JobApplication>(v, x => x.Id == request.Id)
        .Limit(1);
    var (sql, parameters) = qb.Build();
    var data = await _dapper.QueryFirstOrDefaultAsync<JobAppListDto>(sql, parameters, ct);
    if (data == null) return null;

    var jAppInfo = await _jobAppService.GetJobAppInfo(data.Id, ct);
    data.StatusStr = MyEnumHelper.FormatEnum<ApplicationStatus>(data.Status);
    data.Applicant = jAppInfo?.Applicant ?? "";
    data.JobPostingNum = jAppInfo?.PostNumber ?? "";
    data.Position = jAppInfo?.Position ?? "";
    data.Department = jAppInfo?.Department ?? "";
    data.Period = jAppInfo?.Period ?? "";
    data.RowVersion = data.xmin.ToString();

    // ? Add these fields for the evaluation page
    data.JobApplicationId = jAppInfo?.JobApplicationId ?? data.Id;
    data.PostNumber = jAppInfo?.PostNumber ?? "";
    data.ReqNumber = jAppInfo?.ReqNumber ?? "";
    data.JgStep = jAppInfo?.JgStep ?? "";
    data.Title = jAppInfo?.Title ?? "";
    data.ContractType = jAppInfo?.ContractType ?? "";
    data.WorkLocation = jAppInfo?.WorkLocation ?? "";
    data.Qualification = jAppInfo?.Qualification ?? "";
    data.KeySkills = jAppInfo?.KeySkills ?? "";
    data.PlanCode = jAppInfo?.PlanCode ?? "";
    data.Desc = jAppInfo?.Desc ?? "";
    data.PreGender = jAppInfo?.PreGender ?? "";

    return data;
}
}
public class JobAppByJobPostHandler : IRequestHandler<JobAppByJobPostQry, List<JobAppListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IJobAppService _jobAppService;

    public JobAppByJobPostHandler(IDapperHelper dapper, IJobAppService jobAppService)
    {
        _dapper = dapper;
        _jobAppService = jobAppService;
    }

    public async Task<List<JobAppListDto>> Handle(JobAppByJobPostQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<JobApplication>(v, x => x.Id, x => x.AppliedDate, x => x.Status, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<JobApplication>(v)
            .Where<JobApplication>(v, x => x.JobPostingId == request.Id)
            .OrderBy<JobApplication>(v, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        var list = (await _dapper.QueryAsync<JobAppListDto>(sql, parameters, ct)).ToList();        
        if (list.Count == 0) { return []; }

        var ids = list.Select(x => x.Id).ToList();
        var jobInfos = await _jobAppService.GetJobAppBatchInfo(ids, ct);

        foreach (var data in list)
        {
            jobInfos.TryGetValue(data.Id, out var jAppInfo);
            data.StatusStr = MyEnumHelper.FormatEnum<ApplicationStatus>(data.Status);
            data.Applicant = jAppInfo?.Applicant ?? "";
            data.JobPostingNum = jAppInfo?.PostNumber ?? "";
            data.Position = jAppInfo?.Position ?? "";
            data.Department = jAppInfo?.Department ?? "";
            data.Period = jAppInfo?.Period ?? "";
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}
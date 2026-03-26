using Common;
using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class JobPostingAllQry : IRequest<List<JobPostingListDto>> { }
public class JobPostingByIdQry : IRequest<JobPostingListDto?> { public Guid Id { get; set; } }
public class JobPostingByJobReqIdQry : IRequest<List<JobPostingListDto>> { public Guid Id { get; set; } }
public class JobPostingByWfpIdQry : IRequest<List<JobPostingListDto>> { public Guid Id { get; set; } }
public class JobPostingViewQry : IRequest<JobPostingViewDto> { public Guid Id { get; set; } }



public class JobPostingAllHandler : IRequestHandler<JobPostingAllQry, List<JobPostingListDto>>
{
    private readonly IDapperHelper _dapper;
    public JobPostingAllHandler(IDapperHelper dapper) { _dapper = dapper; }
    public async Task<List<JobPostingListDto>> Handle(JobPostingAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string r = "r";
        const string rr = "rr";
        var qb = new QueryBuilder()
            .Select<JobPosting>(v, x => x.Id, x => x.PostNumber, x => x.Status, x => x.PostType, x => x.PublishedDate, x => x.DeadlineDate, x => x.ClosedDate, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<JobRequisition, JobPostingListDto>(r, x => x.ReqNumber, d => d.ReqNumber)
            .Select<JobReqReview>(rr, x => x.ReqQuantity, x => x.AppQuantity)
            .From<JobPosting>(v)
            .Join<JobPosting, JobRequisition>(v, r, x => x.JobReqId, x => x.Id)
            .LeftJoin<JobRequisition, JobReqReview>(r, rr, x => x.Id, x => x.JobReqId)
            .OrderBy<JobPosting>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<JobPostingListDto>(ct);

        foreach (var data in list)
        {
            data.ReqAppQuan = data.ReqQuantity.HasValue && data.AppQuantity.HasValue ? $"{data.ReqQuantity.Value} | {data.AppQuantity.Value}" : "0 | 0";
            data.StatusStr = MyEnumHelper.FormatEnum<PostingStatus>(data.Status);
            data.PostTypeStr = MyEnumHelper.FormatEnum<JobPostingType>(data.PostType);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class JobPostingByIdHandler : IRequestHandler<JobPostingByIdQry, JobPostingListDto?>
{
    private readonly IDapperHelper _dapper;
    public JobPostingByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<JobPostingListDto?> Handle(JobPostingByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string r = "r";
        const string rr = "rr";
        var qb = new QueryBuilder()
            .Select<JobPosting>(v, x => x.Id, x => x.PostNumber, x => x.Status, x => x.PostType, x => x.PublishedDate, x => x.DeadlineDate, x => x.ClosedDate, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<JobRequisition, JobPostingListDto>(r, x => x.ReqNumber, d => d.ReqNumber)
            .Select<JobReqReview>(rr, x => x.ReqQuantity, x => x.AppQuantity)
            .From<JobPosting>(v)
            .Join<JobPosting, JobRequisition>(v, r, x => x.JobReqId, x => x.Id)
            .Join<JobReqReview, JobRequisition>(rr, r, x => x.JobReqId, x => x.Id)
            .Where<JobPosting>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<JobPostingListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.ReqAppQuan = data.ReqQuantity.HasValue && data.AppQuantity.HasValue ? $"{data.ReqQuantity.Value} | {data.AppQuantity.Value}" : "0 | 0";
        data.StatusStr = MyEnumHelper.FormatEnum<PostingStatus>(data.Status);
        data.PostTypeStr = MyEnumHelper.FormatEnum<JobPostingType>(data.PostType);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class JobPostingByJobReqIdHandler : IRequestHandler<JobPostingByJobReqIdQry, List<JobPostingListDto>>
{
    private readonly IDapperHelper _dapper;
    public JobPostingByJobReqIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<JobPostingListDto>> Handle(JobPostingByJobReqIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string r = "r";
        const string rr = "rr";
        var qb = new QueryBuilder()
            .Select<JobPosting>(v, x => x.Id, x => x.PostNumber, x => x.Status, x => x.PostType, x => x.PublishedDate, x => x.DeadlineDate, x => x.ClosedDate, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<JobRequisition, JobPostingListDto>(r, x => x.ReqNumber, d => d.ReqNumber)
            .Select<JobReqReview>(rr, x => x.ReqQuantity, x => x.AppQuantity)
            .From<JobPosting>(v)
            .Join<JobPosting, JobRequisition>(v, r, x => x.JobReqId, x => x.Id)
            .Join<JobReqReview, JobRequisition>(rr, r, x => x.JobReqId, x => x.Id)
            .Where<JobPosting>(v, x => x.JobReqId == request.Id)
            .OrderBy<JobPosting>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<JobPostingListDto>(ct);

        foreach (var data in list)
        {
            data.ReqAppQuan = data.ReqQuantity.HasValue && data.AppQuantity.HasValue ? $"{data.ReqQuantity.Value} | {data.AppQuantity.Value}" : "0 | 0";
            data.StatusStr = MyEnumHelper.FormatEnum<PostingStatus>(data.Status);
            data.PostTypeStr = MyEnumHelper.FormatEnum<JobPostingType>(data.PostType);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class JobPostingByWfpIdHandler : IRequestHandler<JobPostingByWfpIdQry, List<JobPostingListDto>>
{
    private readonly IDapperHelper _dapper;
    public JobPostingByWfpIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<JobPostingListDto>> Handle(JobPostingByWfpIdQry request, CancellationToken ct)
    {
        const string jr = "jr";
        const string jp = "jp";
        const string jrr = "jrr";
        var stat = BoolToStr.EnumToString(ReqStatus.Approved);
        var qb = new QueryBuilder()
            .Select<JobRequisition>(jr, x => x.Id, x => x.ReqNumber)
            .Select<JobPosting>(jp, x => x.Id, x => x.PostNumber, x => x.Status, x => x.PostType, x => x.PublishedDate, x => x.DeadlineDate, x => x.ClosedDate, x => x.IsDeleted, x => x.DateAdd, x => x.DateMod, x => x.xmin, x => x.JobReqId)
            .Select<JobReqReview>(jrr, x => x.JobReqId, x => x.ReqQuantity, x => x.AppQuantity)
            .From<JobRequisition>(jr)
            .Join<JobRequisition, JobPosting>(jr, jp, x => x.Id, x => x.JobReqId)
            .LeftJoin<JobPosting, JobReqReview>(jp, jrr, x => x.Id, x => x.JobReqId)
            .Where<JobRequisition>(jr, x => x.WorkforcePlanId == request.Id && x.Status == stat)
            .OrderBy<JobPosting>(jp, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<JobPostingListDto>(ct);

        foreach (var data in list)
        {
            data.ReqAppQuan = data.ReqQuantity.HasValue && data.AppQuantity.HasValue ? $"{data.ReqQuantity.Value} | {data.AppQuantity.Value}" : "0 | 0";
            data.StatusStr = MyEnumHelper.FormatEnum<PostingStatus>(data.Status);
            data.PostTypeStr = MyEnumHelper.FormatEnum<JobPostingType>(data.PostType);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class JobPostingViewHandler : IRequestHandler<JobPostingViewQry, JobPostingViewDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHrmm;
    private readonly ICorModClient _corMod;
    private readonly IHrmProfileClient _hrmProfile;
    public JobPostingViewHandler(IDapperHelper dapper, ICorHrmmClient corHrmmClient, ICorModClient corModClient, IHrmProfileClient hrmProfileClient)
    {
        _dapper = dapper;
        _corHrmm = corHrmmClient;
        _corMod = corModClient;
        _hrmProfile = hrmProfileClient;
    }

    public async Task<JobPostingViewDto> Handle(JobPostingViewQry request, CancellationToken ct)
    {
        const string v = "v";
        const string jr = "jr";
        const string jrr = "jrr";
        const string jd = "jd";
        const string wp = "wp";
        var qb = new QueryBuilder()
            .Select<JobPosting>(v, x => x.Id, x => x.PostNumber, x => x.Status, x => x.PostType, x => x.PublishedDate, x => x.DeadlineDate, x => x.ClosedDate, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .Select<JobRequisition>(jr, x => x.ReqNumber, x => x.ReqReason, x => x.BudgetCode, x => x.JgStepId, x => x.PositionId)
            .Select<JobReqReview>(jrr, x => x.ReqQuantity, x => x.AppQuantity)
            .Select<JobDec>(jd, x => x.Title, x => x.Desc, x => x.Qualification, x => x.KeySkills, x => x.WorkLocation, x => x.PreGender, x => x.ContractType)
            .Select<WorkforcePlan>(jd, x => x.DepartmentId, x => x.RequistionById, x => x.PeriodId)
            .From<JobPosting>(v)
            .Join<JobPosting, JobRequisition>(v, jr, x => x.JobReqId, x => x.Id)
            .Join<JobRequisition, JobDec>(v, jr, x => x.JobDecId, x => x.Id)
            .Join<JobReqReview, JobRequisition>(jrr, jr, x => x.JobReqId, x => x.Id)
            .Join<JobRequisition, WorkforcePlan>(jrr, jr, x => x.WorkforcePlanId, x => x.Id)
            .Where<JobPosting>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<JobPostingViewDto>(sql, parameters, ct);
        if (data == null) return null;

        var posTask = _corHrmm.GetPosition(data.PositionId.ToString(), ct);
        var jStepTask = _corHrmm.GetJgStep(data.JgStepId.ToString(), ct);
        var deptTask = _corMod.GetDept(data.DepartmentId.ToString(), ct);
        var periodTask = _corMod.GetPeriod(data.PeriodId.ToString()!, ct);
        var empTask = _hrmProfile.GetEmp(data.RequistionById.ToString(), ct);
        await Task.WhenAll(posTask, jStepTask, deptTask, periodTask, empTask);

        var pos = posTask.Result;
        var jgs = jStepTask.Result;
        var dept = deptTask.Result;
        var per = periodTask.Result;
        var emp = empTask.Result;

        var c = new JobPostingViewDto
        {
            Id = data.Id,
            PublishedDate = data.PublishedDate,
            DeadlineDate = data.DeadlineDate,
            ClosedDate = data.ClosedDate,
            PostNumber = data.PostNumber,
            ReqNumber = data.ReqNumber,
            StatusStr = MyEnumHelper.FormatEnum<PostingStatus>(data.Status),
            PostTypeStr = MyEnumHelper.FormatEnum<JobPostingType>(data.PostType),
            ReqReason = data.ReqReason,
            ReqQuantity = data.ReqQuantity.HasValue ? data.ReqQuantity : 0,
            AppQuantity = data.AppQuantity.HasValue ? data.AppQuantity : 0,
            BudgetCode = data.BudgetCode,
            Position = pos.Res?.Name ?? "",
            JgStep = jgs.Res?.Name ?? "",
            Department = dept.Res?.Name ?? "",
            Period = per.Name ?? "",
            RequistionBy = emp.Res?.Name ?? "",
            Title = data.Title,
            Desc = data.Desc,
            Qualification = data.Qualification,
            KeySkills = data.KeySkills,
            WorkLocation = data.WorkLocation,
            PreGenderStr = MyEnumHelper.FormatEnum<Gender>(data.PreGender),
            ContractTypeStr = MyEnumHelper.FormatEnum<EmpNature>(data.ContractType),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
        return c;
    }
}
using Common;
using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class VacancyListQry : IRequest<List<VacancyListDto>> { }
public class VacancyDetailQry : IRequest<VacancyDetailDto?> { public Guid Id { get; set; } }



public class VacancyListHandler : IRequestHandler<VacancyListQry, List<VacancyListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _hrmmClient;
    private readonly ICorModClient _modClient;
    public VacancyListHandler(IDapperHelper dapper, ICorHrmmClient hrmmClient, ICorModClient modClient)
    {
        _dapper = dapper;
        _hrmmClient = hrmmClient;
        _modClient = modClient;
    }

    public async Task<List<VacancyListDto>> Handle(VacancyListQry request, CancellationToken ct)
    {
        var posTask = _hrmmClient.GetListPosition(ct);
        var jgsTask = _hrmmClient.GetListJgStep(ct);
        var deptTask = _modClient.GetListDept(ct);
        await Task.WhenAll(posTask, jgsTask, deptTask);
        var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));
        var jgsDict = jgsTask.Result.Res.ToDictionary(j => Guid.Parse(j.Id));
        var deptDict = deptTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));

        var stat = BoolToStr.EnumToString(PostingStatus.Published);
        const string v = "v";
        const string jr = "jr";
        const string w = "w";
        const string jd = "jd";
        var qb = new QueryBuilder()
            .Select<JobPosting>(v, x => x.Id, x => x.PostNumber, x => x.PublishedDate, x => x.DeadlineDate, x => x.JobReqId)
            .Select<JobRequisition>(jr, x => x.PositionId, x => x.JgStepId, x => x.WorkforcePlanId, x => x.JobDecId)
            .Select<JobDec>(jd, x => x.EmpNature, x => x.PreGender)
            .Select<WorkforcePlan>(w, x => x.DepartmentId)
            .SelectAs<JobRequisition, VacancyListDto>(jr, x => x.ReqQuantity, d => d.NumOpen)
            .SelectAs<JobDec, VacancyListDto>(jd, x => x.WorkLocation, d => d.Location)
            .From<JobPosting>(v)
            .Join<JobPosting, JobRequisition>(v, jr, x => x.JobReqId, x => x.Id)
            .LeftJoin<JobRequisition, WorkforcePlan>(jr, w, x => x.WorkforcePlanId, x => x.Id)
            .LeftJoin<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
            .Where<JobPosting>(v, x => x.Status == stat)
            .OrderBy<JobPosting>(v, x => x.PublishedDate, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<VacancyListDto>(ct);

        foreach (var data in list)
        {
            posDict.TryGetValue(data.PositionId, out var pos);
            jgsDict.TryGetValue(data.JgStepId, out var jgs);
            deptDict.TryGetValue(data.DepartmentId, out var dept);

            data.PreGenderStr = MyEnumHelper.FormatEnum<PositionGender>(data.PreGender);
            data.EmpNatureStr = MyEnumHelper.FormatEnum<EmpNature>(data.EmpNature);
            data.Position = pos?.Name ?? "";
            data.JobGrade = jgs?.Name ?? "";
            data.Department = dept?.Name ?? "";
        }

        return list;
    }
}

public class VacancyDetailHandler : IRequestHandler<VacancyDetailQry, VacancyDetailDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _hrmmClient;
    private readonly ICorModClient _modClient;
    public VacancyDetailHandler(IDapperHelper dapper, ICorHrmmClient hrmmClient, ICorModClient modClient)
    {
        _dapper = dapper;
        _hrmmClient = hrmmClient;
        _modClient = modClient;
    }

    public async Task<VacancyDetailDto?> Handle(VacancyDetailQry request, CancellationToken ct)
    {
        var stat = BoolToStr.EnumToString(PostingStatus.Published);
        const string v = "v";
        const string jr = "jr";
        const string w = "w";
        const string jd = "jd";
        var qb = new QueryBuilder()
            .Select<JobPosting>(v, x => x.Id, x => x.PostNumber, x => x.PublishedDate, x => x.DeadlineDate, x => x.JobReqId)
            .Select<JobRequisition>(jr, x => x.PositionId, x => x.JgStepId, x => x.WorkforcePlanId, x => x.JobDecId)
            .Select<JobDec>(jd, x => x.EmpNature, x => x.PreGender, x => x.WorkArr)
            .Select<WorkforcePlan>(w, x => x.DepartmentId)
            .SelectAs<JobDec, VacancyDetailDto>(jd, x => x.Desc, d => d.JobDesc)
            .SelectAs<JobRequisition, VacancyDetailDto>(jr, x => x.ReqQuantity, d => d.NumOpen)
            .SelectAs<JobDec, VacancyDetailDto>(jd, x => x.WorkLocation, d => d.Location)
            .From<JobPosting>(v)
            .Join<JobPosting, JobRequisition>(v, jr, x => x.JobReqId, x => x.Id)
            .LeftJoin<JobRequisition, WorkforcePlan>(jr, w, x => x.WorkforcePlanId, x => x.Id)
            .LeftJoin<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
            .Where<JobPosting>(v, x => x.Status == stat && x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<VacancyDetailDto>(sql, parameters, ct);
        if (data == null) return null;

        var posTask = _hrmmClient.GetPosition((data.PositionId).ToString(), ct);
        var jgsTask = _hrmmClient.GetJgStep((data.JgStepId).ToString(), ct);
        var slyTask = _hrmmClient.GetJgStepSalary((data.JgStepId).ToString(), ct);
        var deptTask = _modClient.GetDept((data.DepartmentId).ToString(), ct);
        await Task.WhenAll(posTask, jgsTask, slyTask, deptTask);
        var pos = posTask.Result.Res;
        var jgs = jgsTask.Result.Res;
        var sly = slyTask.Result;
        var dept = deptTask.Result.Res;

        var kRespo = new List<string>();
        var rQual = new List<string>();
        var kSkill = new List<string>();
        kRespo.Add("Responsiblity 1");
        kRespo.Add("Responsiblity 2");
        kRespo.Add("Responsiblity 3");
        rQual.Add("Qualification 1");
        rQual.Add("Qualification 2");
        rQual.Add("Qualification 3");
        kSkill.Add("Skill 1");
        kSkill.Add("Skill 2");
        kSkill.Add("Skill 3");

        data.PreGenderStr = MyEnumHelper.FormatEnum<PositionGender>(data.PreGender);
        data.EmpNatureStr = MyEnumHelper.FormatEnum<EmpNature>(data.EmpNature);
        data.WorkArrStr = MyEnumHelper.FormatEnum<WorkArrangement>(data.WorkArr);
        data.Position = pos?.Name ?? "";
        data.JobGrade = jgs?.Name ?? "";
        data.Salary = sly?.Salary ?? "";
        data.Department = dept?.Name ?? "";
        data.KeyRespo = kRespo;
        data.ReqQual = rQual;
        data.KeySkills = kSkill;

        return data;
    }
}
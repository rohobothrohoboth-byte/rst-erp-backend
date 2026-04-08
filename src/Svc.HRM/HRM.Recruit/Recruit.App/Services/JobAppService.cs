using Common;
using Dapper;
using Helpers;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Services;

public interface IJobAppService
{
    Task<JobAppIdDto?> GetJobAppId(Guid Id, CancellationToken ctx);
    Task<JobAppInfoDto?> GetJobAppInfo(Guid Id, CancellationToken ctx);
    Task<Dictionary<Guid, JobAppInfoDto>> GetJobAppBatchInfo(List<Guid> ids, CancellationToken ctx);

}

public class JobAppService : IJobAppService
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfileClient;
    private readonly ICorModClient _corModClient;
    private readonly ICorHrmmClient _corHrmmClient;

    public JobAppService(IDapperHelper dapper, IHrmProfileClient hrmProfileClient, ICorModClient corModClient, ICorHrmmClient corHrmmClient)
    {
        _dapper = dapper;
        _hrmProfileClient = hrmProfileClient;
        _corModClient = corModClient;
        _corHrmmClient = corHrmmClient;
    }

    public async Task<Dictionary<Guid, JobAppInfoDto>> GetJobAppBatchInfo(List<Guid> ids, CancellationToken ct)
    {
        if (ids == null || ids.Count == 0) { return []; }

        var posTask = _corHrmmClient.GetListPosition(ct);
        var stepTask = _corHrmmClient.GetListJgStep(ct);
        var deptTask = _corModClient.GetListDept(ct);
        var periodTask = _corModClient.GetListPeriod(ct);
        var empTask = _hrmProfileClient.GetListEmp(ct);
        await Task.WhenAll(posTask, stepTask, deptTask, periodTask, empTask);

        var deptDict = deptTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));
        var stepDict = stepTask.Result.Res.ToDictionary(j => Guid.Parse(j.Id));
        var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));
        var periodDict = periodTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));
        var empDict = empTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));

        const string ja = "ja";
        const string jp = "jp";
        const string jr = "jr";
        const string jd = "jd";
        const string wf = "wf";
        var qb = new QueryBuilder()
            .Select<JobApplication>(ja, x => x.Id, x => x.ApplicantId, x => x.EmployeeId, x => x.PostType, x => x.JobPostingId)
            .Select<JobPosting>(jp, x => x.PostNumber, x => x.JobReqId)
            .Select<JobRequisition>(jr, x => x.ReqNumber, x => x.PositionId, x => x.JgStepId, x => x.JobDecId, x => x.WorkforcePlanId)
            .Select<JobDec>(jd, x => x.KeyRespo, x => x.Desc, x => x.ReqQual, x => x.KeySkills, x => x.WorkLocation, x => x.PreGender, x => x.EmpNature, x => x.WorkArr)
            .Select<WorkforcePlan>(wf, x => x.PlanCode, x => x.DepartmentId, x => x.PeriodId)
            .From<JobApplication>(ja)
            .Join<JobApplication, JobPosting>(ja, jp, x => x.JobPostingId, x => x.Id)
            .Join<JobPosting, JobRequisition>(jp, jr, x => x.JobReqId, x => x.Id)
            .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
            .Join<JobRequisition, WorkforcePlan>(jr, wf, x => x.WorkforcePlanId, x => x.Id)
            .WhereIn<JobApplication>(ja, x => x.Id, ids);

        var (sql, parameters) = qb.Build();
        var result = new Dictionary<Guid, JobAppInfoDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<JobAppInfoDto>();

        var rows = new List<JobAppInfoDto>();
        while (await reader.ReadAsync(ct)) { rows.Add(parser(reader)); }

        if (rows.Count == 0) return result;

        var extApplicantIds = rows.Where(x => x.PostType == BoolToStr.EnumToString(JobPostingType.External) && x.ApplicantId.HasValue).Select(x => x.ApplicantId!.Value).Distinct().ToList();
        const string a = "a";
        const string p = "p";
        var applicantDict = new Dictionary<Guid, string>();

        if (extApplicantIds.Count != 0)
        {
            var qb2 = new QueryBuilder()
                .Select<Applicant>(a, x => x.Id, x => x.PersonId)
                .Select<ApplicantPerson>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName)
                .From<Applicant>(a)
                .Join<Applicant, ApplicantPerson>(a, p, x => x.PersonId, x => x.Id)
                .WhereIn<Applicant>(a, x => x.Id, extApplicantIds);
            var (sql2, param) = qb2.Build();
            await using var reader2 = await _dapper.ExecuteReaderAsync(sql2, param, ct);
            var parser2 = reader2.GetRowParser<ApplicantJoinRow>();

            while (await reader2.ReadAsync(ct))
            {
                var row = parser2(reader2);
                applicantDict[row.Id] = $"{row.FirstName} {row.MiddleName} {row.LastName}";
            }
        }

        foreach (var r in rows)
        {
            posDict.TryGetValue(r.PositionId, out var pos);
            stepDict.TryGetValue(r.JgStepId, out var step);
            deptDict.TryGetValue(r.DepartmentId, out var dept);
            periodDict.TryGetValue(r.PeriodId, out var period);

            r.Position = pos?.Name ?? "";
            r.JgStep = step?.Name ?? "";
            r.Department = dept?.Name ?? "";
            r.Period = period?.Name ?? "";
            r.PreGender = MyEnumHelper.FormatEnum<Gender>(r.PreGender);
            r.ContractType = MyEnumHelper.FormatEnum<EmpNature>(r.ContractType);

            if (r.PostType == BoolToStr.EnumToString(JobPostingType.External))
            {
                if (r.ApplicantId.HasValue && applicantDict.TryGetValue(r.ApplicantId.Value, out var name))
                    r.Applicant = name;
                else
                    r.Applicant = "NOT AVAILABLE";
            }
            else
            {
                if (r.EmployeeId.HasValue && empDict.TryGetValue(r.EmployeeId.Value, out var name))
                    r.Applicant = name?.Name ?? "";
                else
                    r.Applicant = "NOT AVAILABLE";
            }

            result[r.JobApplicationId] = r;
        }

        return result;
    }

    public async Task<JobAppIdDto?> GetJobAppId(Guid Id, CancellationToken ct)
    {
        const string ja = "ja";
        const string jp = "jp";
        const string jr = "jr";
        const string wf = "wf";
        var qb = new QueryBuilder()
            .Select<JobApplication>(ja, x => x.Id, x => x.ApplicantId, x => x.EmployeeId, x => x.JobPostingId)
            .Select<JobPosting>(jp, x => x.Id, x => x.JobReqId)
            .Select<JobRequisition>(jr, x => x.Id, x => x.PositionId, x => x.JgStepId, x => x.WorkforcePlanId, x => x.JobDecId)
            .Select<WorkforcePlan>(wf, x => x.Id, x => x.DepartmentId, x => x.PeriodId)
            .From<JobApplication>(ja)
            .Join<JobApplication, JobPosting>(ja, jp, x => x.JobPostingId, x => x.Id)
            .Join<JobPosting, JobRequisition>(jp, jr, x => x.JobReqId, x => x.Id)
            .Join<JobRequisition, WorkforcePlan>(jr, wf, x => x.WorkforcePlanId, x => x.Id)
            .Where<JobApplication>(ja, x => x.Id == Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<JobAppIdDto>(sql, parameters, ct);
        if (row == null) return null;
        return row;
    }

    public async Task<JobAppInfoDto?> GetJobAppInfo(Guid Id, CancellationToken ct)
    {
        const string ja = "ja";
        const string jp = "jp";
        const string jr = "jr";
        const string jd = "jd";
        const string wf = "wf";
        var qb = new QueryBuilder()
            .Select<JobApplication>(ja, x => x.Id, x => x.ApplicantId, x => x.EmployeeId, x => x.PostType, x => x.JobPostingId)
            .Select<JobPosting>(jp, x => x.Id, x => x.PostNumber, x => x.JobReqId)
            .Select<JobRequisition>(jr, x => x.Id, x => x.PositionId, x => x.JgStepId, x => x.WorkforcePlanId, x => x.JobDecId, x => x.ReqNumber)
            .Select<JobDec>(jd, x => x.Id, x => x.KeyRespo, x => x.Desc, x => x.ReqQual, x => x.KeySkills, x => x.WorkLocation, x => x.PreGender, x => x.EmpNature, x => x.WorkArr)
            .Select<WorkforcePlan>(wf, x => x.Id, x => x.DepartmentId, x => x.PeriodId, x => x.PlanCode)
            .From<JobApplication>(ja)
            .Join<JobApplication, JobPosting>(ja, jp, x => x.JobPostingId, x => x.Id)
            .Join<JobPosting, JobRequisition>(jp, jr, x => x.JobReqId, x => x.Id)
            .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
            .Join<JobRequisition, WorkforcePlan>(jr, wf, x => x.WorkforcePlanId, x => x.Id)
            .Where<JobApplication>(ja, x => x.Id == Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<JobAppInfoDto>(sql, parameters, ct);
        if (row == null) return null;

        string applicantName;
        var externalType = BoolToStr.EnumToString(JobPostingType.External);

        if (row.PostType == externalType && row.ApplicantId.HasValue)
        {
            const string a = "a";
            const string p = "p";
            var qbApp = new QueryBuilder()
                .Select<Applicant>(a, x => x.Id, x => x.PersonId)
                .Select<ApplicantPerson>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName)
                .From<Applicant>(a)
                .Join<Applicant, ApplicantPerson>(a, p, x => x.PersonId, x => x.Id)
                .Where<Applicant>(a, x => x.Id == row.ApplicantId.Value);

            var (sqlApp, paramApp) = qbApp.Build();
            await using var rApp = await _dapper.ExecuteReaderAsync(sqlApp, paramApp, ct);
            var parserApp = rApp.GetRowParser<ApplicantJoinRow>();
            if (await rApp.ReadAsync(ct))
            {
                var appRow = parserApp(rApp);
                applicantName = $"{appRow.FirstName} {appRow.MiddleName} {appRow.LastName}";
            }
            else
            {
                applicantName = "NOT AVAILABLE";
            }
        }
        else if (row.EmployeeId.HasValue)
        {
            var emp = await _hrmProfileClient.GetEmp(row.EmployeeId.Value.ToString(), ct);
            applicantName = emp?.Res.Name ?? "NOT AVAILABLE";
        }
        else
        {
            applicantName = "NOT AVAILABLE";
        }

        var posTask = _corHrmmClient.GetPosition(row.PositionId.ToString(), ct);
        var jStepTask = _corHrmmClient.GetJgStep(row.JgStepId.ToString(), ct);
        var deptTask = _corModClient.GetDept(row.DepartmentId.ToString(), ct);
        var periodTask = _corHrmmClient.GetJgStep(row.PeriodId.ToString()!, ct);
        await Task.WhenAll(posTask, jStepTask, deptTask, periodTask);

        var pos = posTask.Result;
        var jStep = jStepTask.Result;
        var dept = deptTask.Result;
        var perd = periodTask.Result;

        return new JobAppInfoDto
        {
            JobApplicationId = Id,
            PostType = row.PostType,
            ApplicantId = row.ApplicantId,
            EmployeeId = row.EmployeeId,
            Applicant = applicantName,
            PostNumber = row.PostNumber,
            ReqNumber = row.ReqNumber,
            PlanCode = row.PlanCode,
            Title = row.Title,
            Desc = row.Desc,
            Qualification = row.Qualification,
            KeySkills = row.KeySkills,
            WorkLocation = row.WorkLocation,
            PreGender = MyEnumHelper.FormatEnum<Gender>(row.PreGender),
            ContractType = MyEnumHelper.FormatEnum<EmpNature>(row.ContractType),
            PositionId = row.PositionId,
            JgStepId = row.JgStepId,
            DepartmentId = row.DepartmentId,
            PeriodId = row.PeriodId,
            Position = pos?.Res.Name ?? "NOT AVAILABLE",
            JgStep = jStep?.Res.Name ?? "NOT AVAILABLE",
            Department = dept?.Res.Name ?? "NOT AVAILABLE",
            Period = perd?.Res.Name ?? "NOT AVAILABLE"
        };
    }

}
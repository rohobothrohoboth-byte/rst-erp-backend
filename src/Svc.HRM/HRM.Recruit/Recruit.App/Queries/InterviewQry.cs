// Recruit.App/Queries/InterviewQry.cs

using Common;
using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.App.Services;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

// ============================================
// QUERIES
// ============================================

public class InterviewByApplicantQry : IRequest<List<InterviewListDto>>
{
    public Guid ApplicantId { get; set; }
}

public class InterviewByJobPostingQry : IRequest<List<InterviewListDto>>
{
    public Guid JobPostingId { get; set; }
}

public class InterviewByIdQry : IRequest<InterviewDetailDto?>
{
    public Guid Id { get; set; }
}

public class InterviewAllQry : IRequest<List<InterviewListDto>> { }


// ============================================
// HANDLERS
// ============================================
// Recruit.App/Queries/InterviewQry.cs - Fix InterviewAllHandler
// Recruit.App/Queries/InterviewQry.cs - Fixed InterviewAllHandler

public class InterviewAllHandler : IRequestHandler<InterviewAllQry, List<InterviewListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;
    private readonly ICorModClient _corMod;
    private readonly ICorHrmmClient _corHrmm;
    private readonly IJobAppService _jobAppService;

    public InterviewAllHandler(
        IDapperHelper dapper,
        IHrmProfileClient hrmProfile,
        ICorModClient corMod,
        ICorHrmmClient corHrmm,
        IJobAppService jobAppService)
    {
        _dapper = dapper;
        _hrmProfile = hrmProfile;
        _corMod = corMod;
        _corHrmm = corHrmm;
        _jobAppService = jobAppService;
    }

    public async Task<List<InterviewListDto>> Handle(InterviewAllQry request, CancellationToken ct)
    {
        const string i = "i";
        const string ja = "ja";
        const string jp = "jp";
        const string jr = "jr";
        const string wfp = "wfp";
        const string jd = "jd";

        var qb = new QueryBuilder()
            .Select<Interview>(i,
                x => x.Id,
                x => x.ApplicantId,
                x => x.JobPostingId,
                x => x.InterviewType,
                x => x.ScheduledDate,
                x => x.Location!,
                x => x.MeetingLink!,
                x => x.Notes!,
                x => x.InterviewerId!,
                x => x.Status,
                x => x.DateAdd,
                x => x.DateMod!,
                x => x.xmin)
            .Select<JobApplication>(ja,
                x => x.ApplicantId!,
                x => x.EmployeeId!,
                x => x.JobPostingId,
                x => x.Status,
                x => x.PostType)
            .Select<JobPosting>(jp,
                x => x.PostNumber,
                x => x.JobReqId)
            .Select<JobRequisition>(jr,
                x => x.PositionId,
                x => x.JgStepId,
                x => x.JobDecId,
                x => x.WorkforcePlanId)
            .Select<WorkforcePlan>(wfp,
                x => x.DepartmentId)
            .Select<JobDec>(jd,
                x => x.Desc,
                x => x.KeyRespo,
                x => x.ReqQual,
                x => x.KeySkills,
                x => x.WorkLocation,
                x => x.EmpNature)
            .SelectAs<JobApplication, InterviewListDto>(ja, x => x.ApplicantId!, d => d.ApplicantId)
            .SelectAs<JobRequisition, InterviewListDto>(jr, x => x.PositionId, d => d.PositionId)
            .SelectAs<JobDec, InterviewListDto>(jd, x => x.Desc, d => d.Position)
            .From<Interview>(i)
            .Join<Interview, JobApplication>(i, ja, x => x.ApplicantId, x => x.Id)
            .Join<Interview, JobPosting>(i, jp, x => x.JobPostingId, x => x.Id)
            .Join<JobPosting, JobRequisition>(jp, jr, x => x.JobReqId, x => x.Id)
            .Join<JobRequisition, WorkforcePlan>(jr, wfp, x => x.WorkforcePlanId, x => x.Id)
            .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
            .Where<Interview>(i, x => x.IsDeleted == false)
            .OrderBy<Interview>(i, x => x.ScheduledDate, desc: true);

        var (sql, parameters) = qb.Build();

        // ✅ Execute the query and get the list
        var list = await _dapper.QueryAsync<InterviewListDto>(sql, parameters, ct);
        var resultList = list.ToList();

        if (!resultList.Any())
            return resultList;

        // ✅ Get position names
        var positionIds = resultList.Where(x => x.PositionId != Guid.Empty).Select(x => x.PositionId).Distinct().ToList();
        var positionDict = new Dictionary<Guid, string>();
        if (positionIds.Any())
        {
            try
            {
                var positions = await _corHrmm.GetListPosition(ct);
                if (positions?.Res != null)
                {
                    positionDict = positions.Res.ToDictionary(p => Guid.Parse(p.Id), p => p.Name ?? "N/A");
                }
            }
            catch { /* Ignore */ }
        }

        // ✅ Get all JobApplication IDs from the interviews
        var jobAppIds = resultList.Select(x => x.ApplicantId).Distinct().ToList();
        var nameDict = new Dictionary<Guid, string>();

        if (jobAppIds.Any())
        {
            // ✅ Get all JobApplications in one batch - using QueryAsync (not ExecuteReader)
            var jobApps = await _dapper.QueryAsync<dynamic>(
                "SELECT \"Id\", \"EmployeeId\", \"ApplicantId\" FROM \"JobApplication\" WHERE \"Id\" = ANY(@Ids) AND \"IsDeleted\" = false",
                new { Ids = jobAppIds.ToArray() }, ct);

            var jobAppDict = new Dictionary<Guid, (Guid? EmployeeId, Guid? ApplicantId)>();
            foreach (var jobApp in jobApps)
            {
                var id = (Guid)jobApp.Id;
                var employeeId = jobApp.EmployeeId as Guid?;
                var applicantId = jobApp.ApplicantId as Guid?;
                jobAppDict[id] = (employeeId, applicantId);
            }

            // ✅ Process each JobApplication to get the name
            foreach (var jobAppId in jobAppIds)
            {
                string name = "N/A";

                if (jobAppDict.TryGetValue(jobAppId, out var jobApp))
                {
                    // Check if it's an internal applicant (has EmployeeId)
                    if (jobApp.EmployeeId.HasValue && jobApp.EmployeeId.Value != Guid.Empty)
                    {
                        try
                        {
                            var emp = await _hrmProfile.GetEmp(jobApp.EmployeeId.Value.ToString(), ct);
                            name = emp?.Res?.Name ?? "N/A";
                        }
                        catch
                        {
                            name = "N/A";
                        }
                    }
                    // Check if it's an external applicant (has ApplicantId)
                    else if (jobApp.ApplicantId.HasValue && jobApp.ApplicantId.Value != Guid.Empty)
                    {
                        try
                        {
                            var externalSql = @"
                                SELECT
                                    pp.""FirstName"" || ' ' || pp.""MiddleName"" || ' ' || pp.""LastName"" as Name
                                FROM ""Applicant"" a
                                JOIN ""ApplicantPerson"" pp ON a.""PersonId"" = pp.""Id""
                                WHERE a.""Id"" = @ApplicantId AND a.""IsDeleted"" = false
                            ";
                            var external = await _dapper.QueryFirstOrDefaultAsync<dynamic>(
                                externalSql, new { ApplicantId = jobApp.ApplicantId.Value }, ct);
                            name = external?.Name?.ToString() ?? "N/A";
                        }
                        catch
                        {
                            name = "N/A";
                        }
                    }
                }

                nameDict[jobAppId] = name;
            }
        }

        // ✅ Process each result
        foreach (var data in resultList)
        {
            data.InterviewTypeStr = MyEnumHelper.FormatEnum<InterviewType>(data.InterviewType);
            data.StatusStr = MyEnumHelper.FormatEnum<InterviewStatus>(data.Status);

            // Set position name
            if (positionDict.TryGetValue(data.PositionId, out var positionName))
            {
                data.Position = positionName;
            }
            else
            {
                data.Position = "N/A";
            }

            // ✅ Set applicant name from dictionary
            if (nameDict.TryGetValue(data.ApplicantId, out var applicantName))
            {
                data.ApplicantName = applicantName;
            }
            else
            {
                data.ApplicantName = "N/A";
            }

            // Get interviewer name
            if (data.InterviewerId.HasValue)
            {
                try
                {
                    var emp = await _hrmProfile.GetEmp(data.InterviewerId.Value.ToString(), ct);
                    data.InterviewerName = emp?.Res?.Name ?? "N/A";
                }
                catch
                {
                    data.InterviewerName = "N/A";
                }
            }
        }

        return resultList;
    }
}
// --------------------------------------------
// InterviewByApplicantHandler
// --------------------------------------------
public class InterviewByApplicantHandler : IRequestHandler<InterviewByApplicantQry, List<InterviewListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;
    private readonly ICorModClient _corMod;
    private readonly ICorHrmmClient _corHrmm;
    private readonly IJobAppService _jobAppService;

    public InterviewByApplicantHandler(
        IDapperHelper dapper,
        IHrmProfileClient hrmProfile,
        ICorModClient corMod,
        ICorHrmmClient corHrmm,
        IJobAppService jobAppService)
    {
        _dapper = dapper;
        _hrmProfile = hrmProfile;
        _corMod = corMod;
        _corHrmm = corHrmm;
        _jobAppService = jobAppService;
    }

    public async Task<List<InterviewListDto>> Handle(InterviewByApplicantQry request, CancellationToken ct)
    {
        const string i = "i";
        const string ja = "ja";
        const string jp = "jp";
        const string jr = "jr";
        const string wfp = "wfp";
        const string jd = "jd";

        var qb = new QueryBuilder()
            .Select<Interview>(i,
                x => x.Id,
                x => x.ApplicantId,
                x => x.JobPostingId,
                x => x.InterviewType,
                x => x.ScheduledDate,
                x => x.Location!,
                x => x.MeetingLink!,
                x => x.Notes!,
                x => x.InterviewerId!,
                x => x.Status!,
                x => x.DateAdd,
                x => x.DateMod!,
                x => x.xmin)
            .Select<JobApplication>(ja,
                x => x.ApplicantId!,
                x => x.EmployeeId!,
                x => x.JobPostingId,
                x => x.Status,
                x => x.PostType)
            .Select<JobPosting>(jp,
                x => x.PostNumber,
                x => x.JobReqId)
            .Select<JobRequisition>(jr,
                x => x.PositionId,
                x => x.JgStepId,
                x => x.JobDecId,
                x => x.WorkforcePlanId)
            .Select<WorkforcePlan>(wfp,
                x => x.DepartmentId)
            .Select<JobDec>(jd,
                x => x.Desc,
                x => x.KeyRespo,
                x => x.ReqQual,
                x => x.KeySkills,
                x => x.WorkLocation,
                x => x.EmpNature)
            .SelectAs<JobApplication, InterviewListDto>(ja, x => x.ApplicantId!, d => d.ApplicantId)
            .SelectAs<JobRequisition, InterviewListDto>(jr, x => x.PositionId, d => d.PositionId)
            .SelectAs<JobDec, InterviewListDto>(jd, x => x.Desc, d => d.Position)
            .From<Interview>(i)
            .Join<Interview, JobApplication>(i, ja, x => x.ApplicantId, x => x.Id)
            .Join<Interview, JobPosting>(i, jp, x => x.JobPostingId, x => x.Id)
            .Join<JobPosting, JobRequisition>(jp, jr, x => x.JobReqId, x => x.Id)
            .Join<JobRequisition, WorkforcePlan>(jr, wfp, x => x.WorkforcePlanId, x => x.Id)
            .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
            .Where<Interview>(i, x => x.ApplicantId == request.ApplicantId && x.IsDeleted == false)
            .OrderBy<Interview>(i, x => x.ScheduledDate, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<InterviewListDto>(ct);

        // Get position names
        var positionIds = list.Where(x => x.PositionId != Guid.Empty).Select(x => x.PositionId).Distinct().ToList();
        var positionDict = new Dictionary<Guid, string>();
        if (positionIds.Any())
        {
            try
            {
                var positions = await _corHrmm.GetListPosition(ct);
                if (positions?.Res != null)
                {
                    positionDict = positions.Res.ToDictionary(p => Guid.Parse(p.Id), p => p.Name ?? "N/A");
                }
            }
            catch { /* Ignore */ }
        }

        foreach (var data in list)
        {
            data.InterviewTypeStr = MyEnumHelper.FormatEnum<InterviewType>(data.InterviewType);
            data.StatusStr = MyEnumHelper.FormatEnum<InterviewStatus>(data.Status);

            if (positionDict.TryGetValue(data.PositionId, out var positionName))
            {
                data.Position = positionName;
            }
            else
            {
                data.Position = "N/A";
            }

            // Get applicant name
            try
            {
                var jobApp = await _dapper.QueryFirstOrDefaultAsync<dynamic>(
                    "SELECT \"EmployeeId\", \"ApplicantId\" FROM \"JobApplication\" WHERE \"Id\" = @Id AND \"IsDeleted\" = false",
                    new { Id = data.ApplicantId }, ct);

                if (jobApp != null)
                {
                    if (jobApp.EmployeeId != null && jobApp.EmployeeId != Guid.Empty)
                    {
                        try
                        {
                            var emp = await _hrmProfile.GetEmp(jobApp.EmployeeId.ToString(), ct);
                            data.ApplicantName = emp?.Res?.Name ?? "N/A";
                        }
                        catch { data.ApplicantName = "N/A"; }
                    }
                    else if (jobApp.ApplicantId != null && jobApp.ApplicantId != Guid.Empty)
                    {
                        var externalSql = @"
                            SELECT
                                pp.""FirstName"" || ' ' || pp.""MiddleName"" || ' ' || pp.""LastName"" as Name
                            FROM ""Applicant"" a
                            JOIN ""ApplicantPerson"" pp ON a.""PersonId"" = pp.""Id""
                            WHERE a.""Id"" = @ApplicantId AND a.""IsDeleted"" = false
                        ";
                        var external = await _dapper.QueryFirstOrDefaultAsync<dynamic>(
                            externalSql, new { ApplicantId = jobApp.ApplicantId }, ct);
                        data.ApplicantName = external?.Name?.ToString() ?? "N/A";
                    }
                    else
                    {
                        data.ApplicantName = "N/A";
                    }
                }
                else
                {
                    data.ApplicantName = "N/A";
                }
            }
            catch
            {
                data.ApplicantName = "N/A";
            }

            // Get interviewer name
            if (data.InterviewerId.HasValue)
            {
                try
                {
                    var emp = await _hrmProfile.GetEmp(data.InterviewerId.Value.ToString(), ct);
                    data.InterviewerName = emp?.Res?.Name ?? "N/A";
                }
                catch
                {
                    data.InterviewerName = "N/A";
                }
            }
        }

        return list;
    }
}

// --------------------------------------------
// InterviewByJobPostingHandler
// --------------------------------------------
public class InterviewByJobPostingHandler : IRequestHandler<InterviewByJobPostingQry, List<InterviewListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;
    private readonly ICorModClient _corMod;
    private readonly ICorHrmmClient _corHrmm;
    private readonly IJobAppService _jobAppService;

    public InterviewByJobPostingHandler(
        IDapperHelper dapper,
        IHrmProfileClient hrmProfile,
        ICorModClient corMod,
        ICorHrmmClient corHrmm,
        IJobAppService jobAppService)
    {
        _dapper = dapper;
        _hrmProfile = hrmProfile;
        _corMod = corMod;
        _corHrmm = corHrmm;
        _jobAppService = jobAppService;
    }

    public async Task<List<InterviewListDto>> Handle(InterviewByJobPostingQry request, CancellationToken ct)
    {
        // Similar implementation as above but with different where clause
        const string i = "i";
        const string ja = "ja";
        const string jp = "jp";
        const string jr = "jr";
        const string wfp = "wfp";
        const string jd = "jd";

        var qb = new QueryBuilder()
            .Select<Interview>(i,
                x => x.Id,
                x => x.ApplicantId,
                x => x.JobPostingId,
                x => x.InterviewType,
                x => x.ScheduledDate,
                x => x.Location!,
                x => x.MeetingLink!,
                x => x.Notes!,
                x => x.InterviewerId!,
                x => x.Status,
                x => x.DateAdd,
                x => x.DateMod!,
                x => x.xmin)
            .Select<JobApplication>(ja,
                x => x.ApplicantId!,
                x => x.EmployeeId!,
                x => x.JobPostingId,
                x => x.Status,
                x => x.PostType)
            .Select<JobPosting>(jp,
                x => x.PostNumber,
                x => x.JobReqId)
            .Select<JobRequisition>(jr,
                x => x.PositionId,
                x => x.JgStepId,
                x => x.JobDecId,
                x => x.WorkforcePlanId)
            .Select<WorkforcePlan>(wfp,
                x => x.DepartmentId)
            .Select<JobDec>(jd,
                x => x.Desc,
                x => x.KeyRespo,
                x => x.ReqQual,
                x => x.KeySkills,
                x => x.WorkLocation,
                x => x.EmpNature)
            .SelectAs<JobApplication, InterviewListDto>(ja, x => x.ApplicantId!, d => d.ApplicantId)
            .SelectAs<JobRequisition, InterviewListDto>(jr, x => x.PositionId, d => d.PositionId)
            .SelectAs<JobDec, InterviewListDto>(jd, x => x.Desc, d => d.Position)
            .From<Interview>(i)
            .Join<Interview, JobApplication>(i, ja, x => x.ApplicantId, x => x.Id)
            .Join<Interview, JobPosting>(i, jp, x => x.JobPostingId, x => x.Id)
            .Join<JobPosting, JobRequisition>(jp, jr, x => x.JobReqId, x => x.Id)
            .Join<JobRequisition, WorkforcePlan>(jr, wfp, x => x.WorkforcePlanId, x => x.Id)
            .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
            .Where<Interview>(i, x => x.JobPostingId == request.JobPostingId && x.IsDeleted == false)
            .OrderBy<Interview>(i, x => x.ScheduledDate, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<InterviewListDto>(ct);

        // Get position names
        var positionIds = list.Where(x => x.PositionId != Guid.Empty).Select(x => x.PositionId).Distinct().ToList();
        var positionDict = new Dictionary<Guid, string>();
        if (positionIds.Any())
        {
            try
            {
                var positions = await _corHrmm.GetListPosition(ct);
                if (positions?.Res != null)
                {
                    positionDict = positions.Res.ToDictionary(p => Guid.Parse(p.Id), p => p.Name ?? "N/A");
                }
            }
            catch { /* Ignore */ }
        }

        foreach (var data in list)
        {
            data.InterviewTypeStr = MyEnumHelper.FormatEnum<InterviewType>(data.InterviewType);
            data.StatusStr = MyEnumHelper.FormatEnum<InterviewStatus>(data.Status);

            if (positionDict.TryGetValue(data.PositionId, out var positionName))
            {
                data.Position = positionName;
            }
            else
            {
                data.Position = "N/A";
            }

            // Get applicant name
            try
            {
                var jobApp = await _dapper.QueryFirstOrDefaultAsync<dynamic>(
                    "SELECT \"EmployeeId\", \"ApplicantId\" FROM \"JobApplication\" WHERE \"Id\" = @Id AND \"IsDeleted\" = false",
                    new { Id = data.ApplicantId }, ct);

                if (jobApp != null)
                {
                    if (jobApp.EmployeeId != null && jobApp.EmployeeId != Guid.Empty)
                    {
                        try
                        {
                            var emp = await _hrmProfile.GetEmp(jobApp.EmployeeId.ToString(), ct);
                            data.ApplicantName = emp?.Res?.Name ?? "N/A";
                        }
                        catch { data.ApplicantName = "N/A"; }
                    }
                    else if (jobApp.ApplicantId != null && jobApp.ApplicantId != Guid.Empty)
                    {
                        var externalSql = @"
                            SELECT
                                pp.""FirstName"" || ' ' || pp.""MiddleName"" || ' ' || pp.""LastName"" as Name
                            FROM ""Applicant"" a
                            JOIN ""ApplicantPerson"" pp ON a.""PersonId"" = pp.""Id""
                            WHERE a.""Id"" = @ApplicantId AND a.""IsDeleted"" = false
                        ";
                        var external = await _dapper.QueryFirstOrDefaultAsync<dynamic>(
                            externalSql, new { ApplicantId = jobApp.ApplicantId }, ct);
                        data.ApplicantName = external?.Name?.ToString() ?? "N/A";
                    }
                    else
                    {
                        data.ApplicantName = "N/A";
                    }
                }
                else
                {
                    data.ApplicantName = "N/A";
                }
            }
            catch
            {
                data.ApplicantName = "N/A";
            }

            // Get interviewer name
            if (data.InterviewerId.HasValue)
            {
                try
                {
                    var emp = await _hrmProfile.GetEmp(data.InterviewerId.Value.ToString(), ct);
                    data.InterviewerName = emp?.Res?.Name ?? "N/A";
                }
                catch
                {
                    data.InterviewerName = "N/A";
                }
            }
        }

        return list;
    }
}

// --------------------------------------------
// ✅ InterviewByIdHandler - Get single interview
// --------------------------------------------
public class InterviewByIdHandler : IRequestHandler<InterviewByIdQry, InterviewDetailDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;
    private readonly ICorModClient _corMod;
    private readonly ICorHrmmClient _corHrmm;
    private readonly IJobAppService _jobAppService;

    public InterviewByIdHandler(
        IDapperHelper dapper,
        IHrmProfileClient hrmProfile,
        ICorModClient corMod,
        ICorHrmmClient corHrmm,
        IJobAppService jobAppService)
    {
        _dapper = dapper;
        _hrmProfile = hrmProfile;
        _corMod = corMod;
        _corHrmm = corHrmm;
        _jobAppService = jobAppService;
    }

    public async Task<InterviewDetailDto?> Handle(InterviewByIdQry request, CancellationToken ct)
    {
        const string i = "i";
        const string ja = "ja";
        const string jp = "jp";
        const string jr = "jr";
        const string wfp = "wfp";
        const string jd = "jd";

        var qb = new QueryBuilder()
            .Select<Interview>(i,
                x => x.Id,
                x => x.ApplicantId,
                x => x.JobPostingId,
                x => x.InterviewType,
                x => x.ScheduledDate,
                x => x.Location!,
                x => x.MeetingLink!,
                x => x.Notes!,
                x => x.InterviewerId!,
                x => x.Status,
                x => x.DateAdd,
                x => x.DateMod!,
                x => x.xmin)
            .Select<JobApplication>(ja,
                x => x.ApplicantId!,
                x => x.EmployeeId!,
                x => x.JobPostingId,
                x => x.Status,
                x => x.PostType)
            .Select<JobPosting>(jp,
                x => x.PostNumber,
                x => x.JobReqId)
            .Select<JobRequisition>(jr,
                x => x.PositionId,
                x => x.JgStepId,
                x => x.JobDecId,
                x => x.WorkforcePlanId)
            .Select<WorkforcePlan>(wfp,
                x => x.DepartmentId)
            .Select<JobDec>(jd,
                x => x.Desc,
                x => x.KeyRespo,
                x => x.ReqQual,
                x => x.KeySkills,
                x => x.WorkLocation,
                x => x.EmpNature)
            .SelectAs<JobApplication, InterviewDetailDto>(ja, x => x.ApplicantId!, d => d.ApplicantId)
            .SelectAs<JobRequisition, InterviewDetailDto>(jr, x => x.PositionId, d => d.PositionId)
            .SelectAs<JobDec, InterviewDetailDto>(jd, x => x.Desc, d => d.JobPostingTitle)
            .SelectAs<JobPosting, InterviewDetailDto>(jp, x => x.PostNumber, d => d.JobPostingNumber)
            .From<Interview>(i)
            .Join<Interview, JobApplication>(i, ja, x => x.ApplicantId, x => x.Id)
            .Join<Interview, JobPosting>(i, jp, x => x.JobPostingId, x => x.Id)
            .Join<JobPosting, JobRequisition>(jp, jr, x => x.JobReqId, x => x.Id)
            .Join<JobRequisition, WorkforcePlan>(jr, wfp, x => x.WorkforcePlanId, x => x.Id)
            .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
            .Where<Interview>(i, x => x.Id == request.Id && x.IsDeleted == false)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<InterviewDetailDto>(sql, parameters, ct);

        if (data == null) return null;

        data.InterviewTypeStr = MyEnumHelper.FormatEnum<InterviewType>(data.InterviewType);
        data.StatusStr = MyEnumHelper.FormatEnum<InterviewStatus>(data.Status);

        // Get position name
        if (data.PositionId != Guid.Empty)
        {
            try
            {
                var pos = await _corHrmm.GetPosition(data.PositionId.ToString(), ct);
                data.Position = pos?.Res?.Name ?? "N/A";
            }
            catch
            {
                data.Position = "N/A";
            }
        }
        else
        {
            data.Position = "N/A";
        }

        // Get applicant name
        try
        {
            var jobApp = await _dapper.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT \"EmployeeId\", \"ApplicantId\" FROM \"JobApplication\" WHERE \"Id\" = @Id AND \"IsDeleted\" = false",
                new { Id = data.ApplicantId }, ct);

            if (jobApp != null)
            {
                if (jobApp.EmployeeId != null && jobApp.EmployeeId != Guid.Empty)
                {
                    try
                    {
                        var emp = await _hrmProfile.GetEmp(jobApp.EmployeeId.ToString(), ct);
                        data.ApplicantName = emp?.Res?.Name ?? "N/A";
                        data.ApplicantEmail = "N/A";
                        data.ApplicantPhone = "N/A";
                    }
                    catch
                    {
                        data.ApplicantName = "N/A";
                        data.ApplicantEmail = "N/A";
                        data.ApplicantPhone = "N/A";
                    }
                }
                else if (jobApp.ApplicantId != null && jobApp.ApplicantId != Guid.Empty)
                {
                    var externalSql = @"
                        SELECT
                            pp.""FirstName"" || ' ' || pp.""MiddleName"" || ' ' || pp.""LastName"" as Name
                        FROM ""Applicant"" a
                        JOIN ""ApplicantPerson"" pp ON a.""PersonId"" = pp.""Id""
                        WHERE a.""Id"" = @ApplicantId AND a.""IsDeleted"" = false
                    ";
                    var external = await _dapper.QueryFirstOrDefaultAsync<dynamic>(
                        externalSql, new { ApplicantId = jobApp.ApplicantId }, ct);
                    data.ApplicantName = external?.Name?.ToString() ?? "N/A";
                    data.ApplicantEmail = "N/A";
                    data.ApplicantPhone = "N/A";
                }
                else
                {
                    data.ApplicantName = "N/A";
                    data.ApplicantEmail = "N/A";
                    data.ApplicantPhone = "N/A";
                }
            }
            else
            {
                data.ApplicantName = "N/A";
                data.ApplicantEmail = "N/A";
                data.ApplicantPhone = "N/A";
            }
        }
        catch
        {
            data.ApplicantName = "N/A";
            data.ApplicantEmail = "N/A";
            data.ApplicantPhone = "N/A";
        }

        // Get interviewer name
        if (data.InterviewerId.HasValue)
        {
            try
            {
                var emp = await _hrmProfile.GetEmp(data.InterviewerId.Value.ToString(), ct);
                data.InterviewerName = emp?.Res?.Name ?? "N/A";
            }
            catch
            {
                data.InterviewerName = "N/A";
            }
        }

        data.Feedbacks = new List<InterviewFeedbackDto>();

        return data;
    }
}
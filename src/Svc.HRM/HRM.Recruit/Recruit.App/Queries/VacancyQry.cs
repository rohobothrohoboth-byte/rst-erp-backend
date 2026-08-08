using Common;
using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Recruit.App.Queries;

public class VacancyListQry : IRequest<List<VacancyListDto>> { }
public class VacancyDetailQry : IRequest<VacancyDetailDto?> { public Guid Id { get; set; } }
public class InternalVacancyListQry : IRequest<List<VacancyListDto>> { }
public class ExternalVacancyListQry : IRequest<List<VacancyListDto>> { }

public class VacancyListHandler : IRequestHandler<VacancyListQry, List<VacancyListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _hrmmClient;
    private readonly ICorModClient _modClient;
    private readonly ILogger<VacancyListHandler> _logger;

    public VacancyListHandler(
        IDapperHelper dapper,
        ICorHrmmClient hrmmClient,
        ICorModClient modClient,
        ILogger<VacancyListHandler> logger)
    {
        _dapper = dapper;
        _hrmmClient = hrmmClient;
        _modClient = modClient;
        _logger = logger;
    }

    public async Task<List<VacancyListDto>> Handle(VacancyListQry request, CancellationToken ct)
    {
        try
        {
            const string v = "v";
            const string jr = "jr";
            const string w = "w";
            const string jd = "jd";

            var stat = "Published";

            var qb = new QueryBuilder()
                .Select<JobPosting>(v,
                    x => x.Id,
                    x => x.PostNumber,
                    x => x.PublishedDate,
                    x => x.DeadlineDate,
                    x => x.JobReqId,
                    x => x.PostType)
                .Select<JobRequisition>(jr,
                    x => x.PositionId,
                    x => x.JgStepId,
                    x => x.WorkforcePlanId,
                    x => x.JobDecId,
                    x => x.ReqQuantity)
                .Select<JobDec>(jd,
                    x => x.EmpNature,
                    x => x.PreGender,
                    x => x.WorkLocation)
                .Select<WorkforcePlan>(w,
                    x => x.DepartmentId)
                .SelectAs<JobRequisition, VacancyListDto>(jr, x => x.ReqQuantity, d => d.NumOpen)
                .SelectAs<JobDec, VacancyListDto>(jd, x => x.WorkLocation, d => d.Location)
                .From<JobPosting>(v)
                .Join<JobPosting, JobRequisition>(v, jr, x => x.JobReqId, x => x.Id)
                .Join<JobRequisition, WorkforcePlan>(jr, w, x => x.WorkforcePlanId, x => x.Id)
                .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
                .Where<JobPosting>(v, x => x.Status == stat && x.IsDeleted == false)
                .OrderBy<JobPosting>(v, x => x.PublishedDate, desc: true);

            var (sql, parameters) = qb.Build();

            _logger?.LogInformation("Executing SQL: {Sql}", sql);

            await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
            var list = await reader.ToListAsync<VacancyListDto>(ct);

            _logger?.LogInformation("Found {Count} published vacancies", list.Count);

            // Get external data for enrichment
            var posTask = _hrmmClient.GetListPosition(ct);
            var jgsTask = _hrmmClient.GetListJgStep(ct);
            var deptTask = _modClient.GetListDept(ct);
            await Task.WhenAll(posTask, jgsTask, deptTask);

            var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));
            var jgsDict = jgsTask.Result.Res.ToDictionary(j => Guid.Parse(j.Id));
            var deptDict = deptTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));

            foreach (var data in list)
            {
                posDict.TryGetValue(data.PositionId, out var pos);
                jgsDict.TryGetValue(data.JgStepId, out var jgs);
                deptDict.TryGetValue(data.DepartmentId, out var dept);

                data.PreGenderStr = MyEnumHelper.FormatEnum<PositionGender>(data.PreGender ?? "");
                data.EmpNatureStr = MyEnumHelper.FormatEnum<EmpNature>(data.EmpNature ?? "");
                data.Position = pos?.Name ?? "Not Specified";
                data.JobGrade = jgs?.Name ?? "Not Specified";
                data.Department = dept?.Name ?? "Not Specified";

                // ? Parse PostType: "0" = Internal, "2" = Both
                data.IsInternal = data.PostType == "0" || data.PostType == "2";
                data.PostTypeStr = data.PostType switch
                {
                    "0" => "Internal",
                    "2" => "Internal & External",
                    "1" => "External",
                    _ => data.PostType
                };
            }

            return list;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in VacancyListHandler");
            throw;
        }
    }
}

public class InternalVacancyListHandler : IRequestHandler<InternalVacancyListQry, List<VacancyListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _hrmmClient;
    private readonly ICorModClient _modClient;
    private readonly ILogger<InternalVacancyListHandler> _logger;

    public InternalVacancyListHandler(
        IDapperHelper dapper,
        ICorHrmmClient hrmmClient,
        ICorModClient modClient,
        ILogger<InternalVacancyListHandler> logger)
    {
        _dapper = dapper;
        _hrmmClient = hrmmClient;
        _modClient = modClient;
        _logger = logger;
    }

    public async Task<List<VacancyListDto>> Handle(InternalVacancyListQry request, CancellationToken ct)
    {
        try
        {
            var stat = "Published";
            const string v = "v";
            const string jr = "jr";
            const string w = "w";
            const string jd = "jd";

            var qb = new QueryBuilder()
                .Select<JobPosting>(v,
                    x => x.Id,
                    x => x.PostNumber,
                    x => x.PublishedDate,
                    x => x.DeadlineDate,
                    x => x.JobReqId,
                    x => x.PostType)
                .Select<JobRequisition>(jr,
                    x => x.PositionId,
                    x => x.JgStepId,
                    x => x.WorkforcePlanId,
                    x => x.JobDecId,
                    x => x.ReqQuantity)
                .Select<JobDec>(jd,
                    x => x.EmpNature,
                    x => x.PreGender,
                    x => x.WorkLocation)
                .Select<WorkforcePlan>(w,
                    x => x.DepartmentId)
                .SelectAs<JobRequisition, VacancyListDto>(jr, x => x.ReqQuantity, d => d.NumOpen)
                .SelectAs<JobDec, VacancyListDto>(jd, x => x.WorkLocation, d => d.Location)
                .From<JobPosting>(v)
                .Join<JobPosting, JobRequisition>(v, jr, x => x.JobReqId, x => x.Id)
                .Join<JobRequisition, WorkforcePlan>(jr, w, x => x.WorkforcePlanId, x => x.Id)
                .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
                // ? "0" = Internal, "2" = Both
                .Where<JobPosting>(v, x => x.Status == stat && x.IsDeleted == false && (x.PostType == "0" || x.PostType == "2"))
                .OrderBy<JobPosting>(v, x => x.PublishedDate, desc: true);

            var (sql, parameters) = qb.Build();

            _logger?.LogInformation("Executing Internal SQL: {Sql}", sql);

            await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
            var list = await reader.ToListAsync<VacancyListDto>(ct);

            _logger?.LogInformation("Found {Count} internal vacancies", list.Count);

            // Enrich with external data
            var posTask = _hrmmClient.GetListPosition(ct);
            var jgsTask = _hrmmClient.GetListJgStep(ct);
            var deptTask = _modClient.GetListDept(ct);
            await Task.WhenAll(posTask, jgsTask, deptTask);

            var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));
            var jgsDict = jgsTask.Result.Res.ToDictionary(j => Guid.Parse(j.Id));
            var deptDict = deptTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));

            foreach (var data in list)
            {
                posDict.TryGetValue(data.PositionId, out var pos);
                jgsDict.TryGetValue(data.JgStepId, out var jgs);
                deptDict.TryGetValue(data.DepartmentId, out var dept);

                data.PreGenderStr = MyEnumHelper.FormatEnum<PositionGender>(data.PreGender ?? "");
                data.EmpNatureStr = MyEnumHelper.FormatEnum<EmpNature>(data.EmpNature ?? "");
                data.Position = pos?.Name ?? "Not Specified";
                data.JobGrade = jgs?.Name ?? "Not Specified";
                data.Department = dept?.Name ?? "Not Specified";
                data.IsInternal = true;
                data.PostTypeStr = "Internal";
            }

            return list;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in InternalVacancyListHandler");
            throw;
        }
    }
}

public class ExternalVacancyListHandler : IRequestHandler<ExternalVacancyListQry, List<VacancyListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _hrmmClient;
    private readonly ICorModClient _modClient;
    private readonly ILogger<ExternalVacancyListHandler> _logger;

    public ExternalVacancyListHandler(
        IDapperHelper dapper,
        ICorHrmmClient hrmmClient,
        ICorModClient modClient,
        ILogger<ExternalVacancyListHandler> logger)
    {
        _dapper = dapper;
        _hrmmClient = hrmmClient;
        _modClient = modClient;
        _logger = logger;
    }

    public async Task<List<VacancyListDto>> Handle(ExternalVacancyListQry request, CancellationToken ct)
    {
        try
        {
            var stat = "Published";
            const string v = "v";
            const string jr = "jr";
            const string w = "w";
            const string jd = "jd";

            var qb = new QueryBuilder()
                .Select<JobPosting>(v,
                    x => x.Id,
                    x => x.PostNumber,
                    x => x.PublishedDate,
                    x => x.DeadlineDate,
                    x => x.JobReqId,
                    x => x.PostType)
                .Select<JobRequisition>(jr,
                    x => x.PositionId,
                    x => x.JgStepId,
                    x => x.WorkforcePlanId,
                    x => x.JobDecId,
                    x => x.ReqQuantity)
                .Select<JobDec>(jd,
                    x => x.EmpNature,
                    x => x.PreGender,
                    x => x.WorkLocation)
                .Select<WorkforcePlan>(w,
                    x => x.DepartmentId)
                .SelectAs<JobRequisition, VacancyListDto>(jr, x => x.ReqQuantity, d => d.NumOpen)
                .SelectAs<JobDec, VacancyListDto>(jd, x => x.WorkLocation, d => d.Location)
                .From<JobPosting>(v)
                .Join<JobPosting, JobRequisition>(v, jr, x => x.JobReqId, x => x.Id)
                .Join<JobRequisition, WorkforcePlan>(jr, w, x => x.WorkforcePlanId, x => x.Id)
                .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
                // ? "1" = External, "2" = Both
                .Where<JobPosting>(v, x => x.Status == stat && x.IsDeleted == false && (x.PostType == "1" || x.PostType == "2"))
                .OrderBy<JobPosting>(v, x => x.PublishedDate, desc: true);

            var (sql, parameters) = qb.Build();

            _logger?.LogInformation("Executing External SQL: {Sql}", sql);

            await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
            var list = await reader.ToListAsync<VacancyListDto>(ct);

            _logger?.LogInformation("Found {Count} external vacancies", list.Count);

            // Enrich with external data
            var posTask = _hrmmClient.GetListPosition(ct);
            var jgsTask = _hrmmClient.GetListJgStep(ct);
            var deptTask = _modClient.GetListDept(ct);
            await Task.WhenAll(posTask, jgsTask, deptTask);

            var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));
            var jgsDict = jgsTask.Result.Res.ToDictionary(j => Guid.Parse(j.Id));
            var deptDict = deptTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));

            foreach (var data in list)
            {
                posDict.TryGetValue(data.PositionId, out var pos);
                jgsDict.TryGetValue(data.JgStepId, out var jgs);
                deptDict.TryGetValue(data.DepartmentId, out var dept);

                data.PreGenderStr = MyEnumHelper.FormatEnum<PositionGender>(data.PreGender ?? "");
                data.EmpNatureStr = MyEnumHelper.FormatEnum<EmpNature>(data.EmpNature ?? "");
                data.Position = pos?.Name ?? "Not Specified";
                data.JobGrade = jgs?.Name ?? "Not Specified";
                data.Department = dept?.Name ?? "Not Specified";
                data.IsInternal = false;
                data.PostTypeStr = "External";
            }

            return list;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in ExternalVacancyListHandler");
            throw;
        }
    }
}

public class VacancyDetailHandler : IRequestHandler<VacancyDetailQry, VacancyDetailDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _hrmmClient;
    private readonly ICorModClient _modClient;
    private readonly ILogger<VacancyDetailHandler> _logger;

    public VacancyDetailHandler(
        IDapperHelper dapper,
        ICorHrmmClient hrmmClient,
        ICorModClient modClient,
        ILogger<VacancyDetailHandler> logger)
    {
        _dapper = dapper;
        _hrmmClient = hrmmClient;
        _modClient = modClient;
        _logger = logger;
    }

    public async Task<VacancyDetailDto?> Handle(VacancyDetailQry request, CancellationToken ct)
    {
        try
        {
            var stat = "Published";
            const string v = "v";
            const string jr = "jr";
            const string w = "w";
            const string jd = "jd";

            // ? Select KeyRespo, ReqQual, KeySkills as strings
            var qb = new QueryBuilder()
                .Select<JobPosting>(v,
                    x => x.Id,
                    x => x.PostNumber,
                    x => x.PublishedDate,
                    x => x.DeadlineDate,
                    x => x.JobReqId,
                    x => x.PostType)
                .Select<JobRequisition>(jr,
                    x => x.PositionId,
                    x => x.JgStepId,
                    x => x.WorkforcePlanId,
                    x => x.JobDecId,
                    x => x.ReqQuantity)
                .Select<JobDec>(jd,
                    x => x.EmpNature,
                    x => x.PreGender,
                    x => x.WorkArr,
                    x => x.Desc,
                    x => x.KeyRespo,    // ? Store as string
                    x => x.ReqQual,     // ? Store as string
                    x => x.KeySkills,   // ? Store as string
                    x => x.WorkLocation)
                .Select<WorkforcePlan>(w,
                    x => x.DepartmentId)
                .SelectAs<JobDec, VacancyDetailDto>(jd, x => x.Desc, d => d.JobDesc)
                .SelectAs<JobRequisition, VacancyDetailDto>(jr, x => x.ReqQuantity, d => d.NumOpen)
                .SelectAs<JobDec, VacancyDetailDto>(jd, x => x.WorkLocation, d => d.Location)
                .SelectAs<JobDec, VacancyDetailDto>(jd, x => x.KeyRespo, d => d.KeyRespo!)   // ? String
                .SelectAs<JobDec, VacancyDetailDto>(jd, x => x.ReqQual, d => d.ReqQual!)     // ? String
                .SelectAs<JobDec, VacancyDetailDto>(jd, x => x.KeySkills, d => d.KeySkills!) // ? String
                .From<JobPosting>(v)
                .Join<JobPosting, JobRequisition>(v, jr, x => x.JobReqId, x => x.Id)
                .Join<JobRequisition, WorkforcePlan>(jr, w, x => x.WorkforcePlanId, x => x.Id)
                .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
                .Where<JobPosting>(v, x => x.Status == stat && x.Id == request.Id && x.IsDeleted == false)
                .Limit(1);

            var (sql, parameters) = qb.Build();

            _logger?.LogInformation("Executing Detail SQL: {Sql}", sql);

            var data = await _dapper.QueryFirstOrDefaultAsync<VacancyDetailDto>(sql, parameters, ct);
            if (data == null)
            {
                _logger?.LogWarning("Vacancy with ID {Id} not found or not published", request.Id);
                return null;
            }

            // Get external data
            var posTask = _hrmmClient.GetPosition(data.PositionId.ToString(), ct);
            var jgsTask = _hrmmClient.GetJgStep(data.JgStepId.ToString(), ct);
            var slyTask = _hrmmClient.GetJgStepSalary(data.JgStepId.ToString(), ct);
            var deptTask = _modClient.GetDept(data.DepartmentId.ToString(), ct);
            await Task.WhenAll(posTask, jgsTask, slyTask, deptTask);

            var pos = posTask.Result.Res;
            var jgs = jgsTask.Result.Res;
            var sly = slyTask.Result;
            var dept = deptTask.Result.Res;

            data.PreGenderStr = MyEnumHelper.FormatEnum<PositionGender>(data.PreGender ?? "");
            data.EmpNatureStr = MyEnumHelper.FormatEnum<EmpNature>(data.EmpNature ?? "");
            data.WorkArrStr = MyEnumHelper.FormatEnum<WorkArrangement>(data.WorkArr ?? "");
            data.Position = pos?.Name ?? "Not Specified";
            data.JobGrade = jgs?.Name ?? "Not Specified";
            data.Salary = sly?.Salary ?? "Not Specified";
            data.Department = dept?.Name ?? "Not Specified";

            // ? Parse PostType: "0" = Internal, "2" = Both
            data.IsInternal = data.PostType == "0" || data.PostType == "2";
            data.PostTypeStr = data.PostType switch
            {
                "0" => "Internal",
                "2" => "Internal & External",
                "1" => "External",
                _ => data.PostType
            };

            // ? Parse KeyRespo, ReqQual, KeySkills from raw strings
            data.KeyRespoList = ParseStringList(data.KeyRespo);
            data.ReqQualList = ParseStringList(data.ReqQual);
            data.KeySkillsList = ParseStringList(data.KeySkills);

            return data;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in VacancyDetailHandler for ID {Id}", request.Id);
            throw;
        }
    }

    private List<string> ParseStringList(string? input)
    {
        if (string.IsNullOrEmpty(input)) return new List<string>();

        // If it's JSON array
        if (input.Trim().StartsWith("["))
        {
            try
            {
                var result = System.Text.Json.JsonSerializer.Deserialize<List<string>>(input);
                return result ?? new List<string>();
            }
            catch
            {
                // If JSON parsing fails, split by comma
                return input.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            }
        }

        // If it's comma-separated
        if (input.Contains(','))
        {
            return input.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
        }

        // Single value
        return new List<string> { input.Trim() };
    }


}
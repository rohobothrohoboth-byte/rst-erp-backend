// Recruit.App/Queries/JobPostingQry.cs - FIXED VERSION

using Common;
using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;
using Microsoft.Extensions.Logging;
namespace Recruit.App.Queries;

public class JobPostingAllQry : IRequest<List<JobPostingListDto>> { }
public class JobPostingByIdQry : IRequest<JobPostingViewDto?>
{
    public Guid Id { get; set; }
}
public class JobPostingByJobReqIdQry : IRequest<List<JobPostingListDto>> { public Guid Id { get; set; } }
public class JobPostingByWfpIdQry : IRequest<List<JobPostingListDto>> { public Guid Id { get; set; } }
public class JobPostingByDeptIdQry : IRequest<List<JobPostingListDto>> { public Guid Id { get; set; } }
public class JobPostingViewQry : IRequest<JobPostingViewDto> { public Guid Id { get; set; } }



public class JobPostingAllHandler : IRequestHandler<JobPostingAllQry, List<JobPostingListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogger<JobPostingAllHandler> _logger;
    public JobPostingAllHandler(IDapperHelper dapper, ILogger<JobPostingAllHandler> logger) { _dapper = dapper; _logger = logger; }

    public async Task<List<JobPostingListDto>> Handle(JobPostingAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string rq = "rq";
        const string rr = "rr";
        var qb = new QueryBuilder()
            .Select<JobPosting>(v, x => x.Id, x => x.PostNumber, x => x.Status, x => x.PostType, x => x.PublishedDate, x => x.DeadlineDate, x => x.JobReqId, x => x.ClosedDate!, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .Select<JobReqReview>(rr, x => x.ReqQuantity, x => x.AppQuantity)
            .SelectAs<JobRequisition, JobPostingListDto>(rq, x => x.ReqNumber, d => d.ReqNumber)
            .From<JobPosting>(v)
            .Join<JobPosting, JobRequisition>(v, rq, x => x.JobReqId, x => x.Id)
            .LeftJoin<JobRequisition, JobReqReview>(rq, rr, x => x.Id, x => x.JobReqId)
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

public class JobPostingByJobReqIdHandler : IRequestHandler<JobPostingByJobReqIdQry, List<JobPostingListDto>>
{
    private readonly IDapperHelper _dapper;
    public JobPostingByJobReqIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<JobPostingListDto>> Handle(JobPostingByJobReqIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string rq = "rq";
        const string rr = "rr";
        var qb = new QueryBuilder()
            .Select<JobPosting>(v, x => x.Id, x => x.PostNumber, x => x.Status, x => x.PostType, x => x.PublishedDate, x => x.DeadlineDate, x => x.JobReqId, x => x.ClosedDate!, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .Select<JobReqReview>(rr, x => x.ReqQuantity, x => x.AppQuantity)
            .SelectAs<JobRequisition, JobPostingListDto>(rq, x => x.ReqNumber, d => d.ReqNumber)
            .From<JobPosting>(v)
            .Join<JobPosting, JobRequisition>(v, rq, x => x.JobReqId, x => x.Id)
            .LeftJoin<JobRequisition, JobReqReview>(rq, rr, x => x.Id, x => x.JobReqId)
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
        var stat = BoolToStr.EnumToString(ReqStatus.Approved);
        const string v = "v";
        const string rq = "rq";
        const string rr = "rr";
        var qb = new QueryBuilder()
            .Select<JobPosting>(v, x => x.Id, x => x.PostNumber, x => x.Status, x => x.PostType, x => x.PublishedDate, x => x.DeadlineDate, x => x.JobReqId, x => x.ClosedDate!, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .Select<JobReqReview>(rr, x => x.ReqQuantity, x => x.AppQuantity)
            .SelectAs<JobRequisition, JobPostingListDto>(rq, x => x.ReqNumber, d => d.ReqNumber)
            .From<JobPosting>(v)
            .Join<JobPosting, JobRequisition>(v, rq, x => x.JobReqId, x => x.Id)
            .LeftJoin<JobRequisition, JobReqReview>(rq, rr, x => x.Id, x => x.JobReqId)
            .Where<JobRequisition>(rq, x => x.WorkforcePlanId == request.Id && x.Status == stat)
            .OrderBy<JobPosting>(rq, x => x.DateAdd, desc: true);

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

public class JobPostingByDeptIdHandler : IRequestHandler<JobPostingByDeptIdQry, List<JobPostingListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;

    public JobPostingByDeptIdHandler(IDapperHelper dapper, IHrmProfileClient hrmProfile)
    {
        _dapper = dapper;
        _hrmProfile = hrmProfile;
    }

    private async Task<List<Guid>> GetWfp(Guid id, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<WorkforcePlan>(v, x => x.Id)
            .From<WorkforcePlan>(v)
            .Where<WorkforcePlan>(v, x => x.DepartmentId == id);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<IdListDto>(ct);
        var ids = list.Select(x => x.Id).ToList();
        return ids;
    }

    public async Task<List<JobPostingListDto>> Handle(JobPostingByDeptIdQry request, CancellationToken ct)
    {
        var dept = await _hrmProfile.GetEmpId(request.Id.ToString(), ct);
        if (dept.DeptId == null)
        {
            throw new DomainException("WORKFORCE PLANS for your department are NOT AVAILABLE.");
        }

        var deptId = new Guid(dept.DeptId);
        var dWfp = await GetWfp(deptId, ct);
        if (dWfp.Count <= 0) { return []; }

        const string v = "v";
        const string rq = "rq";
        const string rr = "rr";
        var qb = new QueryBuilder()
            .Select<JobPosting>(v, x => x.Id, x => x.PostNumber, x => x.Status, x => x.PostType, x => x.PublishedDate, x => x.DeadlineDate, x => x.JobReqId, x => x.ClosedDate!, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .Select<JobReqReview>(rr, x => x.ReqQuantity, x => x.AppQuantity)
            .SelectAs<JobRequisition, JobPostingListDto>(rq, x => x.ReqNumber, d => d.ReqNumber)
            .From<JobPosting>(v)
            .Join<JobPosting, JobRequisition>(v, rq, x => x.JobReqId, x => x.Id)
            .LeftJoin<JobRequisition, JobReqReview>(rq, rr, x => x.Id, x => x.JobReqId)
            .WhereIn<JobRequisition>(rq, x => x.WorkforcePlanId, dWfp);

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

// ? SINGLE HANDLER FOR JobPostingViewQry
public class JobPostingViewHandler : IRequestHandler<JobPostingViewQry, JobPostingViewDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHrmm;
    private readonly ICorModClient _corMod;
    private readonly IHrmProfileClient _hrmProfile;
  private readonly ILogger<JobPostingViewHandler> _logger;
    //public JobPostingByIdHandler(IDapperHelper dapper, ILogger<JobPostingAllHandler> logger) { _dapper = dapper; _logger = logger; }
    public JobPostingViewHandler(
        IDapperHelper dapper,
        ICorHrmmClient corHrmmClient,
         ILogger<JobPostingViewHandler> logger,
        ICorModClient corModClient,
        IHrmProfileClient hrmProfileClient)
    {
        _dapper = dapper;
        _corHrmm = corHrmmClient;
        _logger = logger;
        _corMod = corModClient;
        _hrmProfile = hrmProfileClient;
    }
// Recruit.App/Queries/JobPostingQry.cs - JobPostingViewHandler

public async Task<JobPostingViewDto> Handle(JobPostingViewQry request, CancellationToken ct)
{
    const string v = "v";
    const string jr = "jr";
    const string jrr = "jrr";
    const string jd = "jd";
    const string wfp = "wfp";

    var qb = new QueryBuilder()
        .Select<JobPosting>(v,
            x => x.Id,
            x => x.PostNumber,
            x => x.Status,
            x => x.PostType,
            x => x.PublishedDate,
            x => x.DeadlineDate,
            x => x.ClosedDate!,
            x => x.JobReqId,
            x => x.DateAdd,
            x => x.DateMod!,
            x => x.xmin)
        .Select<JobRequisition>(jr,
            x => x.ReqNumber,
            x => x.ReqReason,
            x => x.BudgetCode,
            x => x.JgStepId,
            x => x.PositionId,
            x => x.ReqQuantity)
        .Select<JobReqReview>(jrr,
            x => x.ReqQuantity,
            x => x.AppQuantity)
        .Select<JobDec>(jd,
            x => x.KeyRespo,
            x => x.Desc,
            x => x.ReqQual,
            x => x.KeySkills,
            x => x.WorkLocation,
            x => x.PreGender,
            x => x.EmpNature,
            x => x.WorkArr)
        .Select<WorkforcePlan>(wfp,
            x => x.DepartmentId,
            x => x.RequistionById,
            x => x.PeriodId!)
        .From<JobPosting>(v)
        .Join<JobPosting, JobRequisition>(v, jr, x => x.JobReqId, x => x.Id)
        .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
        .LeftJoin<JobRequisition, JobReqReview>(jr, jrr, x => x.Id, x => x.JobReqId)
        .Join<JobRequisition, WorkforcePlan>(jr, wfp, x => x.WorkforcePlanId, x => x.Id)
        .Where<JobPosting>(v, x => x.Id == request.Id && x.IsDeleted == false)
        .Limit(1);

    var (sql, parameters) = qb.Build();
    var data = await _dapper.QueryFirstOrDefaultAsync<JobPostingViewDto>(sql, parameters, ct);

    if (data == null) return null!;

    // ? Get additional data from external services with proper null/empty checking
    string positionName = "";
    string jgStepName = "";
    string departmentName = "";
    string periodName = "";
    string requistionByName = "";

    // ? Get Position with error handling
    try
    {
        if (data.PositionId != Guid.Empty)
        {
            var pos = await _corHrmm.GetPosition(data.PositionId.ToString(), ct);
            positionName = pos?.Res?.Name ?? "";
        }
    }
    catch (Exception ex)
    {
        _logger?.LogWarning(ex, "Failed to get Position for ID: {PositionId}", data.PositionId);
        positionName = "N/A";
    }

    // ? Get JgStep with error handling
    try
    {
        if (data.JgStepId != Guid.Empty)
        {
            var jgs = await _corHrmm.GetJgStep(data.JgStepId.ToString(), ct);
            jgStepName = jgs?.Res?.Name ?? "";
        }
    }
    catch (Exception ex)
    {
        _logger?.LogWarning(ex, "Failed to get JgStep for ID: {JgStepId}", data.JgStepId);
        jgStepName = "N/A";
    }

    // ? Get Department with error handling
    try
    {
        if (data.DepartmentId != Guid.Empty)
        {
            var dept = await _corMod.GetDept(data.DepartmentId.ToString(), ct);
            departmentName = dept?.Res?.Name ?? "";
        }
    }
    catch (Exception ex)
    {
        _logger?.LogWarning(ex, "Failed to get Department for ID: {DepartmentId}", data.DepartmentId);
        departmentName = "N/A";
    }

    // ? FIXED: Get Period with proper null/empty checking
    try
    {
        // ? Check if PeriodId has a valid value
        if (data.PeriodId.HasValue && data.PeriodId.Value != Guid.Empty)
        {
            var periodIdStr = data.PeriodId.Value.ToString();
            if (!string.IsNullOrEmpty(periodIdStr) && periodIdStr != "00000000-0000-0000-0000-000000000000")
            {
                var per = await _corMod.GetPeriod(periodIdStr, ct);
                periodName = per?.Name ?? "N/A";
            }
            else
            {
                periodName = "N/A";
            }
        }
        else
        {
            periodName = "N/A";
        }
    }
    catch (Exception ex)
    {
        _logger?.LogWarning(ex, "Failed to get Period for ID: {PeriodId}", data.PeriodId);
        periodName = "N/A";
    }

    // ? Get Employee with error handling
    try
    {
        if (data.RequistionById != Guid.Empty)
        {
            var emp = await _hrmProfile.GetEmp(data.RequistionById.ToString(), ct);
            requistionByName = emp?.Res?.Name ?? "";
        }
    }
    catch (Exception ex)
    {
        _logger?.LogWarning(ex, "Failed to get Employee for ID: {RequistionById}", data.RequistionById);
        requistionByName = "N/A";
    }

    // Map all data to ViewDto
    var result = new JobPostingViewDto
    {
        Id = data.Id,
        PublishedDate = data.PublishedDate,
        DeadlineDate = data.DeadlineDate,
        ClosedDate = data.ClosedDate,
        PostNumber = data.PostNumber,
        ReqNumber = data.ReqNumber,
        StatusStr = MyEnumHelper.FormatEnum<PostingStatus>(data.Status),
        PostTypeStr = MyEnumHelper.FormatEnum<JobPostingType>(data.PostType),
        ReqReason = data.ReqReason ?? "",
        ReqQuantity = data.ReqQuantity ?? 0,
        AppQuantity = data.AppQuantity ?? 0,
        BudgetCode = data.BudgetCode ?? "",
        Position = positionName,
        JgStep = jgStepName,
        Department = departmentName,
        Period = periodName,
        RequistionBy = requistionByName,
        Title = "",
        KeyRespo = data.KeyRespo ?? "",
        WorkArr = data.WorkArr ?? "",
        Desc = data.Desc ?? "",
        KeySkills = data.KeySkills ?? "",
        WorkLocation = data.WorkLocation ?? "",
        PreGenderStr = MyEnumHelper.FormatEnum<Gender>(data.PreGender),
        IsDeleted = data.IsDeleted,
        DateAdd = data.DateAdd,
        DateMod = data.DateMod,
        RowVersion = data.xmin.ToString()
    };

    return result;
}
}


// Recruit.App/Queries/JobPostingQry.cs - Add logging to debug
// Recruit.App/Queries/JobPostingQry.cs - Updated JobPostingByIdHandler

public class JobPostingByIdHandler : IRequestHandler<JobPostingByIdQry, JobPostingViewDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHrmm;
    private readonly ICorModClient _corMod;
    private readonly IHrmProfileClient _hrmProfile;
     private readonly ILogger<JobPostingByIdHandler> _logger;
    //public JobPostingByIdHandler(IDapperHelper dapper, ILogger<JobPostingAllHandler> logger) { _dapper = dapper; _logger = logger; }

    public JobPostingByIdHandler(
        IDapperHelper dapper,
        ICorHrmmClient corHrmmClient,
         ILogger<JobPostingByIdHandler> logger,
        ICorModClient corModClient,
        IHrmProfileClient hrmProfileClient)
    {
        _dapper = dapper;
        _logger = logger;
        _corHrmm = corHrmmClient;
        _corMod = corModClient;
        _hrmProfile = hrmProfileClient;
    }

    public async Task<JobPostingViewDto?> Handle(JobPostingByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string jr = "jr";
        const string jrr = "jrr";
        const string jd = "jd";
        const string wfp = "wfp";

        var qb = new QueryBuilder()
            .Select<JobPosting>(v,
                x => x.Id,
                x => x.PostNumber,
                x => x.Status,
                x => x.PostType,
                x => x.PublishedDate,
                x => x.DeadlineDate,
                x => x.ClosedDate!,
                x => x.JobReqId,
                x => x.DateAdd,
                x => x.DateMod!,
                x => x.xmin)
            .Select<JobRequisition>(jr,
                x => x.ReqNumber,
                x => x.ReqReason,
                x => x.BudgetCode,
                x => x.JgStepId,
                x => x.PositionId,
                x => x.ReqQuantity)
            .Select<JobReqReview>(jrr,
                x => x.ReqQuantity,
                x => x.AppQuantity)
            .Select<JobDec>(jd,
                x => x.KeyRespo,
                x => x.Desc,
                x => x.ReqQual,
                x => x.KeySkills,
                x => x.WorkLocation,
                x => x.PreGender,
                x => x.EmpNature,
                x => x.WorkArr)
            .Select<WorkforcePlan>(wfp,
                x => x.DepartmentId,
                x => x.RequistionById,
                x => x.PeriodId!)
            .From<JobPosting>(v)
            .Join<JobPosting, JobRequisition>(v, jr, x => x.JobReqId, x => x.Id)
            .Join<JobRequisition, JobDec>(jr, jd, x => x.JobDecId, x => x.Id)
            .LeftJoin<JobRequisition, JobReqReview>(jr, jrr, x => x.Id, x => x.JobReqId)
            .Join<JobRequisition, WorkforcePlan>(jr, wfp, x => x.WorkforcePlanId, x => x.Id)
            .Where<JobPosting>(v, x => x.Id == request.Id && x.IsDeleted == false)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<JobPostingViewDto>(sql, parameters, ct);

        if (data == null) return null;

        // ? Initialize with default values
        string positionName = "";
        string jgStepName = "";
        string departmentName = "";
        string periodName = "";
        string requistionByName = "";
          periodName = "N/A";
    try
    {
        if (data.PeriodId.HasValue && data.PeriodId.Value != Guid.Empty)
        {
            var periodIdStr = data.PeriodId.Value.ToString();
            if (!string.IsNullOrEmpty(periodIdStr) && periodIdStr != "00000000-0000-0000-0000-000000000000")
            {
                var per = await _corMod.GetPeriod(periodIdStr, ct);
                periodName = per?.Name ?? "N/A";
            }
        }
    }
    catch (Exception ex)
    {
        _logger?.LogWarning(ex, "Failed to get Period for ID: {PeriodId}", data.PeriodId);
        periodName = "N/A";
    }
        // ? Safely get Position
        try
        {
            if (data.PositionId != Guid.Empty)
            {
                var posTask = _corHrmm.GetPosition(data.PositionId.ToString(), ct);
                var pos = await posTask;
                positionName = pos.Res?.Name ?? "";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting Position: {ex.Message}");
        }

        // ? Safely get JgStep
        try
        {
            if (data.JgStepId != Guid.Empty)
            {
                var jStepTask = _corHrmm.GetJgStep(data.JgStepId.ToString(), ct);
                var jgs = await jStepTask;
                jgStepName = jgs.Res?.Name ?? "";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting JgStep: {ex.Message}");
        }

        // ? Safely get Department
        try
        {
            if (data.DepartmentId != Guid.Empty)
            {
                var deptTask = _corMod.GetDept(data.DepartmentId.ToString(), ct);
                var dept = await deptTask;
                departmentName = dept.Res?.Name ?? "";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting Department: {ex.Message}");
        }

        // ? Safely get Period (handle null PeriodId)
        try
        {
            if (data.PeriodId.HasValue && data.PeriodId.Value != Guid.Empty)
            {
                var periodTask = _corMod.GetPeriod(data.PeriodId.Value.ToString(), ct);
                var per = await periodTask;
                periodName = per?.Name ?? "";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting Period: {ex.Message}");
            // ? Don't throw, just continue with empty period name
        }

        // ? Safely get Employee
        try
        {
            if (data.RequistionById != Guid.Empty)
            {
                var empTask = _hrmProfile.GetEmp(data.RequistionById.ToString(), ct);
                var emp = await empTask;
                requistionByName = emp.Res?.Name ?? "";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting Employee: {ex.Message}");
        }

        // Map all data to ViewDto
        var result = new JobPostingViewDto
        {
            Id = data.Id,
            PublishedDate = data.PublishedDate,
            DeadlineDate = data.DeadlineDate,
            ClosedDate = data.ClosedDate,
            PostNumber = data.PostNumber,
            ReqNumber = data.ReqNumber,
            StatusStr = MyEnumHelper.FormatEnum<PostingStatus>(data.Status),
            PostTypeStr = MyEnumHelper.FormatEnum<JobPostingType>(data.PostType),
            ReqReason = data.ReqReason ?? "",
            ReqQuantity = data.ReqQuantity ?? 0,
            AppQuantity = data.AppQuantity ?? 0,
            BudgetCode = data.BudgetCode ?? "",
            Position = positionName,
            JgStep = jgStepName,
            Department = departmentName,
            Period = periodName,
            RequistionBy = requistionByName,
            Title = "",
            KeyRespo = data.KeyRespo ?? "",
            WorkArr = data.WorkArr ?? "",
            Desc = data.Desc ?? "",

            KeySkills = data.KeySkills ?? "",
            WorkLocation = data.WorkLocation ?? "",
            PreGenderStr = MyEnumHelper.FormatEnum<Gender>(data.PreGender),

            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };

        return result;
    }
}
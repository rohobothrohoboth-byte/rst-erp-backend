using Common;
using Helpers;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Services;

public interface IJobAppService
{
    Task<JobAppIdDto> GetJobAppId(Guid Id);
    Task<JobAppInfoDto> GetJobAppInfo(Guid Id, CancellationToken ctx);

}

public class JobAppService : IJobAppService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmProfileClient;
    private readonly ICorModClient _corModClient;
    private readonly ICorHrmmClient _corHrmmClient;

    public JobAppService(IUnitOfWork unitOfWork, IHrmProfileClient hrmProfileClient, ICorModClient corModClient, ICorHrmmClient corHrmmClient)
    {
        _unitOfWork = unitOfWork;
        _hrmProfileClient = hrmProfileClient;
        _corModClient = corModClient;
        _corHrmmClient = corHrmmClient;
    }

    public async Task<JobAppIdDto> GetJobAppId(Guid Id)
    {
        var idVm = new JobAppIdDto();
        var jobApp = await _unitOfWork.Repository<JobApplication>().GetById(Id);
        if (jobApp != null)
        {
            var jPost = await _unitOfWork.Repository<JobPosting>().GetById(jobApp.JobPostingId);
            var jReq = await _unitOfWork.Repository<JobRequisition>().GetById(jPost!.JobReqId);
            var wfPlan = await _unitOfWork.Repository<WorkforcePlan>().GetById(jReq!.WorkforcePlanId);

            idVm.ApplicantId = jobApp.ApplicantId;
            idVm.EmployeeId = jobApp.EmployeeId;
            idVm.JobPostingId = jobApp.JobPostingId;
            idVm.JobReqId = jPost!.JobReqId;
            idVm.PositionId = jReq!.PositionId;
            idVm.JgStepId = jReq.JgStepId;
            idVm.WorkforcePlanId = jReq.WorkforcePlanId;
            idVm.JobDecId = jReq.JobDecId;
            idVm.DepartmentId = wfPlan!.DepartmentId;
            idVm.PeriodId = wfPlan.PeriodId;
        }

        return idVm;
    }

    public async Task<JobAppInfoDto> GetJobAppInfo(Guid Id, CancellationToken ctx)
    {
        var jobApp = await _unitOfWork.Repository<JobApplication>().GetById(Id);
        var pType = BoolToStr.EnumToString(JobPostingType.External);
        var app = "";
        if (jobApp!.PostType == pType)
        {
            var appV = await _unitOfWork.Repository<Applicant>().GetById((Guid)jobApp.ApplicantId!);
            var per = await _unitOfWork.Repository<ApplicantPerson>().GetById(appV!.PersonId);
            app = $"{per!.FirstName} {per.MiddleName} {per.LastName}";
        }
        else
        {
            var empV = await _hrmProfileClient.GetEmp(jobApp.EmployeeId.ToString()!, ctx);
            app = empV != null && empV.Res.Name != null ? empV.Res.Name : "NOT AVAILABLE";
        }

        var jPost = await _unitOfWork.Repository<JobPosting>().GetById(jobApp.JobPostingId);
        var jReq = await _unitOfWork.Repository<JobRequisition>().GetById(jPost!.JobReqId);
        var jd = await _unitOfWork.Repository<JobDec>().GetById(jReq!.JobDecId);
        var wfPlan = await _unitOfWork.Repository<WorkforcePlan>().GetById(jReq!.WorkforcePlanId);
        var pos = await _corHrmmClient.GetPosition(jReq.PositionId.ToString(), ctx);
        var jStep = await _corHrmmClient.GetJgStep(jReq.JgStepId.ToString(), ctx);
        var dept = await _corModClient.GetDept(wfPlan!.DepartmentId.ToString(), ctx);
        var perd = await _corHrmmClient.GetJgStep(wfPlan.PeriodId.ToString()!, ctx);

        var infoV = new JobAppInfoDto
        {
            Applicant = app,
            PostNumber = jPost.PostNumber,
            ReqNumber = jReq!.ReqNumber,
            Position = pos != null && pos.Res.Name != null ? pos.Res.Name : "NOT AVAILABLE",
            JgStep = jStep != null && jStep.Res.Name != null ? jStep.Res.Name : "NOT AVAILABLE",
            PlanCode = wfPlan!.PlanCode,
            Title = jd!.Title,
            Desc = jd!.Desc,
            Qualification = jd!.Qualification,
            KeySkills = jd!.KeySkills,
            WorkLocation = jd!.WorkLocation,
            PreGender = ((Gender)Enum.Parse(typeof(Gender), jd!.PreGender)).ToDisplayName(),
            ContractType = ((EmpNature)Enum.Parse(typeof(EmpNature), jd!.ContractType)).ToDisplayName(),
            Department = dept != null && dept.Res.Name != null ? dept.Res.Name : "NOT AVAILABLE",
            Period = perd != null && perd.Res.Name != null ? perd.Res.Name : "NOT AVAILABLE"
        };

        return infoV;
    }

}
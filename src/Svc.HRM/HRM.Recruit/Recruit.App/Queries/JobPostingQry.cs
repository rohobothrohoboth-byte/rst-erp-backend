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
    private readonly IUnitOfWork _unitOfWork;
    public JobPostingAllHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<JobPostingListDto>> Handle(JobPostingAllQry request, CancellationToken cancellationToken)
    {
        var dbData = (await _unitOfWork.Repository<JobPosting>().GetAll()).ToList();
        var dataL = new List<JobPostingListDto>();
        if (dbData.Count <= 0) { return dataL; }
        var jReqRvwL = (await _unitOfWork.Repository<JobReqReview>().GetAll()).ToList();

        foreach (var data in dbData)
        {
            var jReq = await _unitOfWork.Repository<JobRequisition>().GetById(data.JobReqId);
            var jReqRvw = jReqRvwL.FirstOrDefault(x => x.JobReqId == data.JobReqId);
            if (jReq != null)
            {
                var c = new JobPostingListDto
                {
                    Id = data.Id,
                    PostNumber = data.PostNumber,
                    ReqNumber = jReq.ReqNumber,
                    Status = data.Status,
                    PostType = data.PostType,
                    PublishedDate = data.PublishedDate,
                    DeadlineDate = data.DeadlineDate,
                    ClosedDate = data.ClosedDate,
                    StatusStr = ((PostingStatus)Enum.Parse(typeof(PostingStatus), data.Status)).ToDisplayName(),
                    PostTypeStr = ((PostingStatus)Enum.Parse(typeof(PostingStatus), data.PostType)).ToDisplayName(),
                    ReqAppQuan = jReqRvw != null ? $"{jReqRvw.ReqQuantity} | {jReqRvw.AppQuantity}" : "0 | 0",
                    IsDeleted = data.IsDeleted,
                    DateAdd = data.DateAdd,
                    DateMod = data.DateMod,
                    RowVersion = Convert.ToBase64String(data.RowVersion)
                };
                dataL.Add(c);
            }
        }

        return dataL;
    }
}

public class JobPostingByIdHandler : IRequestHandler<JobPostingByIdQry, JobPostingListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobPostingByIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<JobPostingListDto?> Handle(JobPostingByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<JobPosting>().GetById(request.Id);
        if (data == null) { return null; }
        var jReq = await _unitOfWork.Repository<JobRequisition>().GetById(data.JobReqId);
        if (jReq == null) { return null; }
        var jReqRvw = await _unitOfWork.Repository<JobReqReview>().GetFoD(x => x.JobReqId == data.JobReqId);

        var c = new JobPostingListDto
        {
            Id = data.Id,
            PostNumber = data.PostNumber,
            ReqNumber = jReq.ReqNumber,
            Status = data.Status,
            PostType = data.PostType,
            PublishedDate = data.PublishedDate,
            DeadlineDate = data.DeadlineDate,
            ClosedDate = data.ClosedDate,
            StatusStr = ((PostingStatus)Enum.Parse(typeof(PostingStatus), data.Status)).ToDisplayName(),
            PostTypeStr = ((PostingStatus)Enum.Parse(typeof(PostingStatus), data.PostType)).ToDisplayName(),
            ReqAppQuan = jReqRvw != null ? $"{jReqRvw.ReqQuantity} | {jReqRvw.AppQuantity}" : "0 | 0",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

public class JobPostingByJobReqIdHandler : IRequestHandler<JobPostingByJobReqIdQry, List<JobPostingListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobPostingByJobReqIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<JobPostingListDto>> Handle(JobPostingByJobReqIdQry request, CancellationToken cancellationToken)
    {
        var dataL = new List<JobPostingListDto>();
        var dbData = (await _unitOfWork.Repository<JobPosting>().Find(p => p.JobReqId == request.Id)).ToList();
        if (dbData.Count <= 0) { return dataL; }
        var jReq = await _unitOfWork.Repository<JobRequisition>().GetById(request.Id);
        var jReqRvw = await _unitOfWork.Repository<JobReqReview>().GetFoD(x => x.JobReqId == request.Id);

        foreach (var data in dbData)
        {
            var c = new JobPostingListDto
            {
                Id = data.Id,
                PostNumber = data.PostNumber,
                ReqNumber = jReq!.ReqNumber,
                Status = data.Status,
                PostType = data.PostType,
                PublishedDate = data.PublishedDate,
                DeadlineDate = data.DeadlineDate,
                ClosedDate = data.ClosedDate,
                StatusStr = ((PostingStatus)Enum.Parse(typeof(PostingStatus), data.Status)).ToDisplayName(),
                PostTypeStr = ((PostingStatus)Enum.Parse(typeof(PostingStatus), data.PostType)).ToDisplayName(),
                ReqAppQuan = jReqRvw != null ? $"{jReqRvw.ReqQuantity} | {jReqRvw.AppQuantity}" : "0 | 0",
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class JobPostingByWfpIdHandler : IRequestHandler<JobPostingByWfpIdQry, List<JobPostingListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobPostingByWfpIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    private async Task<List<JobPostingListDto>> GetPostings(JobRequisition req)
    {
        var dataL = new List<JobPostingListDto>();
        var dbData = (await _unitOfWork.Repository<JobPosting>().Find(p => p.JobReqId == req.Id)).ToList();
        if (dbData.Count <= 0) { return dataL; }
        var jReqRvw = await _unitOfWork.Repository<JobReqReview>().GetFoD(x => x.JobReqId == req.Id);
        foreach (var data in dbData)
        {
            var c = new JobPostingListDto
            {
                Id = data.Id,
                PostNumber = data.PostNumber,
                ReqNumber = req.ReqNumber,
                Status = data.Status,
                PostType = data.PostType,
                PublishedDate = data.PublishedDate,
                DeadlineDate = data.DeadlineDate,
                ClosedDate = data.ClosedDate,
                StatusStr = ((PostingStatus)Enum.Parse(typeof(PostingStatus), data.Status)).ToDisplayName(),
                PostTypeStr = ((PostingStatus)Enum.Parse(typeof(PostingStatus), data.PostType)).ToDisplayName(),
                ReqAppQuan = jReqRvw != null ? $"{jReqRvw.ReqQuantity} | {jReqRvw.AppQuantity}" : "0 | 0",
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };
            dataL.Add(c);
        }

        return dataL;
    }

    public async Task<List<JobPostingListDto>> Handle(JobPostingByWfpIdQry request, CancellationToken cancellationToken)
    {
        var dataL = new List<JobPostingListDto>();
        var stat = BoolToStr.EnumToString(ReqStatus.Approved);
        var jReqL = (await _unitOfWork.Repository<JobRequisition>().Find(r => r.WorkforcePlanId == request.Id && r.Status == stat)).ToList();
        if (jReqL.Count <= 0) { return dataL; }

        foreach (var jReq in jReqL)
        {
            var dbData = await GetPostings(jReq);
            if (dbData.Count <= 0) { continue; }

            dataL.AddRange(dbData);
        }

        return dataL;
    }
}

public class JobPostingViewHandler : IRequestHandler<JobPostingViewQry, JobPostingViewDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHrmmClient _corHrmmClient;
    private readonly ICorModClient _corModClient;
    private readonly IHrmProfileClient _hrmProfileClient;
    public JobPostingViewHandler(IUnitOfWork unitOfWork, ICorHrmmClient corHrmmClient, ICorModClient corModClient, IHrmProfileClient hrmProfileClient)
    {
        _unitOfWork = unitOfWork;
        _corHrmmClient = corHrmmClient;
        _corModClient = corModClient;
        _hrmProfileClient = hrmProfileClient;
    }

    public async Task<JobPostingViewDto> Handle(JobPostingViewQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<JobPosting>().GetById(request.Id);
        if (data == null) { return new JobPostingViewDto(); }
        var jReq = await _unitOfWork.Repository<JobRequisition>().GetById(data.JobReqId);
        if (jReq == null) { return new JobPostingViewDto(); }
        var jd = await _unitOfWork.Repository<JobDec>().GetById(jReq.JobDecId);
        var jReqRvw = await _unitOfWork.Repository<JobReqReview>().GetFoD(x => x.JobReqId == data.JobReqId);
        var wfp = await _unitOfWork.Repository<WorkforcePlan>().GetById(jReq.WorkforcePlanId);
        var dept = await _corModClient.GetDept(wfp!.DepartmentId.ToString(), cancellationToken);
        var emp = await _hrmProfileClient.GetEmp(wfp.RequistionById.ToString(), cancellationToken);
        var jgs = await _corHrmmClient.GetJgStep(jReq.JgStepId.ToString(), cancellationToken);
        var pos = await _corHrmmClient.GetPosition(jReq.PositionId.ToString(), cancellationToken);
        var per = "NOT AVAILABLE";
        if (wfp.PeriodId != null)
        {
            var peRes = await _corModClient.GetPeriod(wfp.PeriodId.ToString()!, cancellationToken);
            if (peRes.Id != null) { per = peRes.Name; }
        }

        var c = new JobPostingViewDto
        {
            Id = data.Id,
            PublishedDate = data.PublishedDate,
            DeadlineDate = data.DeadlineDate,
            ClosedDate = data.ClosedDate,
            PostNumber = data.PostNumber,
            ReqNumber = jReq.ReqNumber,
            Status = ((PostingStatus)Enum.Parse(typeof(PostingStatus), data.Status)).ToDisplayName(),
            PostType = ((PostingStatus)Enum.Parse(typeof(PostingStatus), data.PostType)).ToDisplayName(),
            ReqReason = jReq.ReqReason,
            ReqQuantity = jReqRvw != null ? jReqRvw.ReqQuantity : 0,
            AppQuantity = jReqRvw != null ? jReqRvw.AppQuantity : 0,
            BudgetCode = jReq.BudgetCode,
            Position = pos.Res.Name ?? "NOT AVAILABLE",
            JgStep = jgs.Res.Name ?? "NOT AVAILABLE",
            Department = dept.Res.Name ?? "NOT AVAILABLE",
            Period = per,
            RequistionBy = emp.Res.Name ?? "NOT AVAILABLE",
            Title = jd != null ? jd.Title : "NOT AVAILABLE",
            Desc = jd != null ? jd.Desc : "NOT AVAILABLE",
            Qualification = jd != null ? jd.Qualification : "NOT AVAILABLE",
            KeySkills = jd != null ? jd.KeySkills : "NOT AVAILABLE",
            WorkLocation = jd != null ? jd.WorkLocation : "NOT AVAILABLE",
            PreGender = jd != null ? ((Gender)Enum.Parse(typeof(Gender), jd.PreGender)).ToDisplayName() : "NOT AVAILABLE",
            ContractType = jd != null ? ((EmpNature)Enum.Parse(typeof(EmpNature), jd.ContractType)).ToDisplayName() : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}
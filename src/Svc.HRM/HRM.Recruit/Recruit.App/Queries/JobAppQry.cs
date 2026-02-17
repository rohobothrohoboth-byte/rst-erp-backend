using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.App.Services;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class JobAppAllQry : IRequest<List<JobAppListDto>> { }
public class JobAppByJobPostQry : IRequest<List<JobAppListDto>> { public Guid Id { get; set; } }
public class JobAppByIdQry : IRequest<JobAppListDto?> { public Guid Id { get; set; } }


public class JobAppAllHandler : IRequestHandler<JobAppAllQry, List<JobAppListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJobAppService _jobAppService;

    public JobAppAllHandler(IUnitOfWork unitOfWork, IJobAppService jobAppService)
    {
        _unitOfWork = unitOfWork;
        _jobAppService = jobAppService;
    }

    public async Task<List<JobAppListDto>> Handle(JobAppAllQry request, CancellationToken cancellationToken)
    {
        var dbData = (await _unitOfWork.Repository<JobApplication>().GetAll()).ToList();
        var dataL = new List<JobAppListDto>();
        if (dbData.Count <= 0) { return dataL; }

        foreach (var data in dbData)
        {
            var jAppInfo = await _jobAppService.GetJobAppInfo(data.Id, cancellationToken);
            var c = new JobAppListDto
            {
                Id = data.Id,
                AppliedDate = data.AppliedDate,
                Status = ((ApplicationStatus)Enum.Parse(typeof(ApplicationStatus), data.Status)).ToDisplayName(),
                Applicant = jAppInfo.Applicant,
                JobPostingNum = jAppInfo.PostNumber,
                Position = jAppInfo.Position,
                Department = jAppInfo.Department,
                Period = jAppInfo.Period,
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

public class JobAppByIdHandler : IRequestHandler<JobAppByIdQry, JobAppListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJobAppService _jobAppService;

    public JobAppByIdHandler(IUnitOfWork unitOfWork, IJobAppService jobAppService)
    {
        _unitOfWork = unitOfWork;
        _jobAppService = jobAppService;
    }

    public async Task<JobAppListDto?> Handle(JobAppByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<JobApplication>().GetById(request.Id);
        if (data == null) { return null; }

        var jAppInfo = await _jobAppService.GetJobAppInfo(data.Id, cancellationToken);
        var c = new JobAppListDto
        {
            Id = data.Id,
            AppliedDate = data.AppliedDate,
            Status = ((ApplicationStatus)Enum.Parse(typeof(ApplicationStatus), data.Status)).ToDisplayName(),
            Applicant = jAppInfo.Applicant,
            JobPostingNum = jAppInfo.PostNumber,
            Position = jAppInfo.Position,
            Department = jAppInfo.Department,
            Period = jAppInfo.Period,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

public class JobAppByJobPostHandler : IRequestHandler<JobAppByJobPostQry, List<JobAppListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJobAppService _jobAppService;

    public JobAppByJobPostHandler(IUnitOfWork unitOfWork, IJobAppService jobAppService)
    {
        _unitOfWork = unitOfWork;
        _jobAppService = jobAppService;
    }

    public async Task<List<JobAppListDto>> Handle(JobAppByJobPostQry request, CancellationToken cancellationToken)
    {
        var dbData = (await _unitOfWork.Repository<JobApplication>().Find(a => a.JobPostingId == request.Id)).ToList();
        var dataL = new List<JobAppListDto>();
        if (dbData.Count <= 0) { return dataL; }

        foreach (var data in dbData)
        {
            var jAppInfo = await _jobAppService.GetJobAppInfo(data.Id, cancellationToken);
            var c = new JobAppListDto
            {
                Id = data.Id,
                AppliedDate = data.AppliedDate,
                Status = ((ApplicationStatus)Enum.Parse(typeof(ApplicationStatus), data.Status)).ToDisplayName(),
                Applicant = jAppInfo.Applicant,
                JobPostingNum = jAppInfo.PostNumber,
                Position = jAppInfo.Position,
                Department = jAppInfo.Department,
                Period = jAppInfo.Period,
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
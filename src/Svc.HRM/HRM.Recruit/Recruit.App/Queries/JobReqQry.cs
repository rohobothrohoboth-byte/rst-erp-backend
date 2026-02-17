using Common;
using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class JobReqAllQry : IRequest<List<JobReqListDto>> { public Guid Id { get; set; } }
public class JobReqByIdQry : IRequest<JobReqListDto?> { public Guid Id { get; set; } }

public class JobReqAllHandler : IRequestHandler<JobReqAllQry, List<JobReqListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHrmmClient _corHrmmClient;
    public JobReqAllHandler(IUnitOfWork unitOfWork, ICorHrmmClient corHrmmClient)
    {
        _unitOfWork = unitOfWork;
        _corHrmmClient = corHrmmClient;
    }
    public async Task<List<JobReqListDto>> Handle(JobReqAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<JobRequisition>().Find(e => e.WorkforcePlanId == request.Id);
        var dataL = new List<JobReqListDto>();
        var posL = await _corHrmmClient.GetListPosition(cancellationToken);
        var jgsL = await _corHrmmClient.GetListJgStep(cancellationToken);

        foreach (var data in dbData)
        {
            var jgsV = posL.Res.FirstOrDefault(r => r.Id == data.JgStepId.ToString());
            var jgs = "NOT AVAILABLE";
            if (jgsV != null) { jgs = jgsV.Name; }

            var posV = posL.Res.FirstOrDefault(r => r.Id == data.PositionId.ToString());
            var pos = "NOT AVAILABLE";
            if (posV != null) { pos = posV.Name; }
            var c = new JobReqListDto
            {
                Id = data.Id,
                JobDecId = data.JobDecId,
                Status = data.Status,
                StartDate = data.StartDate,
                ReqNumber = data.ReqNumber,
                ReqReason = data.ReqReason,
                ReqPositions = data.ReqQuantity,
                BudgetCode = data.BudgetCode,
                StatusStr = ((ReqStatus)Enum.Parse(typeof(ReqStatus), data.Status)).ToDisplayName(),
                Position = pos,
                JgStep = jgs,
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

public class JobReqByIdHandler : IRequestHandler<JobReqByIdQry, JobReqListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHrmmClient _corHrmmClient;
    public JobReqByIdHandler(IUnitOfWork unitOfWork, ICorHrmmClient corHrmmClient)
    {
        _unitOfWork = unitOfWork;
        _corHrmmClient = corHrmmClient;
    }
    public async Task<JobReqListDto?> Handle(JobReqByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<JobRequisition>().GetById(request.Id);
        if (data == null) { return null; }
        var pos = await _corHrmmClient.GetPosition(data.PositionId.ToString(), cancellationToken);
        var jgs = await _corHrmmClient.GetJgStep(data.JgStepId.ToString(), cancellationToken);

        var c = new JobReqListDto
        {
            Id = data.Id,
            JobDecId = data.JobDecId,
            Status = data.Status,
            StartDate = data.StartDate,
            ReqNumber = data.ReqNumber,
            ReqReason = data.ReqReason,
            ReqPositions = data.ReqQuantity,
            BudgetCode = data.BudgetCode,
            StatusStr = ((ReqStatus)Enum.Parse(typeof(ReqStatus), data.Status)).ToDisplayName(),
            Position = pos.Res.Name ?? "NOT AVAILABLE",
            JgStep = jgs.Res.Name ?? "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}
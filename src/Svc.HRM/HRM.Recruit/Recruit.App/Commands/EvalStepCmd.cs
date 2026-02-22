using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class EvalStepAddCmd : IRequest<EvalStepListDto> { public EvalStepAddDto AddDto { get; set; } = default!; }
public class EvalStepModCmd : IRequest<EvalStepListDto> { public EvalStepModDto ModDto { get; set; } = default!; }
public class EvalStepDelCmd : IRequest { public Guid Id { get; set; } }



public class EvalStepAddHandler : IRequestHandler<EvalStepAddCmd, EvalStepListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EvalStepAddHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EvalStepListDto> Handle(EvalStepAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new EvaluationStep
            {
                StepName = request.AddDto.StepName,
                StepOrder = request.AddDto.StepOrder,
                IsFinal = request.AddDto.IsFinal,
                EvalTypeId = request.AddDto.EvalTypeId,
                EvaluationFlowId = request.AddDto.EvaluationFlowId
            };
            await _unitOfWork.Repository<EvaluationStep>().Add(data);
            await _unitOfWork.Commit();

            var res = new EvalStepListDto();
            var response = await _med.Send(new EvalStepByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class EvalStepModHandler : IRequestHandler<EvalStepModCmd, EvalStepListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EvalStepModHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EvalStepListDto> Handle(EvalStepModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EvaluationStep>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"EVALUATION STEP with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.StepName = request.ModDto.StepName;
            oldData.StepOrder = request.ModDto.StepOrder;
            oldData.IsFinal = request.ModDto.IsFinal;
            oldData.EvalTypeId = request.ModDto.EvalTypeId;
            var data = await _unitOfWork.Repository<EvaluationStep>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EvalStepListDto();
            var response = await _med.Send(new EvalStepByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class EvalStepDelHandler : IRequestHandler<EvalStepDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EvalStepDelHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EvalStepDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<EvaluationStep>().GetById(request.Id);
            if (data == null) { throw new DomainException($"EVALUATION STEP with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<EvaluationStep>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EvalStepAddHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<EvalStepListDto> Handle(EvalStepAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new EvaluationStep
            {
                StepName = request.AddDto.StepName,
                StepOrder = request.AddDto.StepOrder,
                MaxScore = request.AddDto.MaxScore,
                MinScore = request.AddDto.MinScore,
                IsFinal = request.AddDto.IsFinal,
                EvalTypeId = request.AddDto.EvalTypeId,
                EvaluationFlowId = request.AddDto.EvaluationFlowId
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new EvalStepListDto();
            var response = await _med.Send(new EvalStepByIdQry { Id = data.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EvalStepModHandler : IRequestHandler<EvalStepModCmd, EvalStepListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EvalStepModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<EvalStepListDto> Handle(EvalStepModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<EvaluationStep>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"EVALUATION STEP with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.StepName = request.ModDto.StepName;
            oldData.StepOrder = request.ModDto.StepOrder;
            oldData.MaxScore = request.ModDto.MaxScore;
            oldData.MinScore = request.ModDto.MinScore;
            oldData.IsFinal = request.ModDto.IsFinal;
            oldData.EvalTypeId = request.ModDto.EvalTypeId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new EvalStepListDto();
            var response = await _med.Send(new EvalStepByIdQry { Id = request.ModDto.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EvalStepDelHandler : IRequestHandler<EvalStepDelCmd>
{
    private readonly IUnitOfWork _uow;
    public EvalStepDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(EvalStepDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<EvaluationStep>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"EVALUATION STEP with id [{request.Id}] NOT FOUND."); }
            await _uow.Delete(data);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}
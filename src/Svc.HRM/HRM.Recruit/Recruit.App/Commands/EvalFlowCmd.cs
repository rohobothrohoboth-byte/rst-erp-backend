using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class EvalFlowAddCmd : IRequest<EvalFlowListDto> { public EvalFlowAddDto AddDto { get; set; } = default!; }
public class EvalFlowModCmd : IRequest<EvalFlowListDto> { public EvalFlowModDto ModDto { get; set; } = default!; }
public class EvalFlowStatCmd : IRequest<EvalFlowListDto> { public StatChangeDto StatDto { get; set; } = default!; }
public class EvalFlowDelCmd : IRequest { public Guid Id { get; set; } }



public class EvalFlowAddHandler : IRequestHandler<EvalFlowAddCmd, EvalFlowListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EvalFlowAddHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EvalFlowListDto> Handle(EvalFlowAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new EvaluationFlow
            {
                Name = request.AddDto.Name,
                IsGlobal = request.AddDto.IsGlobal,
                IsActive = true
            };
            await _unitOfWork.Repository<EvaluationFlow>().Add(data);
            await _unitOfWork.Commit();

            var res = new EvalFlowListDto();
            var response = await _med.Send(new EvalFlowByIdQry { Id = data.Id }, cancellationToken);
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

public class EvalFlowModHandler : IRequestHandler<EvalFlowModCmd, EvalFlowListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EvalFlowModHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EvalFlowListDto> Handle(EvalFlowModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EvaluationFlow>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"EVALUATION FLOW with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.Name = request.ModDto.Name;
            oldData.IsGlobal = request.ModDto.IsGlobal;
            oldData.IsActive = request.ModDto.IsActive;
            var data = await _unitOfWork.Repository<EvaluationFlow>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EvalFlowListDto();
            var response = await _med.Send(new EvalFlowByIdQry { Id = data.Id }, cancellationToken);
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

public class EvalFlowStatHandler : IRequestHandler<EvalFlowStatCmd, EvalFlowListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EvalFlowStatHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EvalFlowListDto> Handle(EvalFlowStatCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EvaluationFlow>().GetById(request.StatDto.Id);
        if (oldData == null) { throw new DomainException($"EVALUATION FLOW with Id {request.StatDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.IsActive = request.StatDto.Stat;
            var data = await _unitOfWork.Repository<EvaluationFlow>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EvalFlowListDto();
            var response = await _med.Send(new EvalFlowByIdQry { Id = data.Id }, cancellationToken);
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

public class EvalFlowDelHandler : IRequestHandler<EvalFlowDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EvalFlowDelHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EvalFlowDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<EvaluationFlow>().GetById(request.Id);
            if (data == null) { throw new DomainException($"EVALUATION FLOW with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<EvaluationFlow>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
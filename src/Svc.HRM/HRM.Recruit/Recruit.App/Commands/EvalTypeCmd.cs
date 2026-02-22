using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class EvalTypeAddCmd : IRequest<EvalTypeListDto> { public EvalTypeAddDto AddDto { get; set; } = default!; }
public class EvalTypeModCmd : IRequest<EvalTypeListDto> { public EvalTypeModDto ModDto { get; set; } = default!; }
public class EvalTypeStatCmd : IRequest<EvalTypeListDto> { public StatChangeDto StatDto { get; set; } = default!; }
public class EvalTypeDelCmd : IRequest { public Guid Id { get; set; } }



public class EvalTypeAddHandler : IRequestHandler<EvalTypeAddCmd, EvalTypeListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EvalTypeAddHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EvalTypeListDto> Handle(EvalTypeAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new EvaluationType
            {
                Name = request.AddDto.Name,
                MaxScore = request.AddDto.MaxScore,
                IsActive = true
            };
            await _unitOfWork.Repository<EvaluationType>().Add(data);
            await _unitOfWork.Commit();

            var res = new EvalTypeListDto();
            var response = await _med.Send(new EvalTypeByIdQry { Id = data.Id }, cancellationToken);
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

public class EvalTypeModHandler : IRequestHandler<EvalTypeModCmd, EvalTypeListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EvalTypeModHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EvalTypeListDto> Handle(EvalTypeModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EvaluationType>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"EVALUATION TYPE with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.Name = request.ModDto.Name;
            oldData.MaxScore = request.ModDto.MaxScore;
            oldData.IsActive = request.ModDto.IsActive;
            var data = await _unitOfWork.Repository<EvaluationType>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EvalTypeListDto();
            var response = await _med.Send(new EvalTypeByIdQry { Id = data.Id }, cancellationToken);
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

public class EvalTypeStatHandler : IRequestHandler<EvalTypeStatCmd, EvalTypeListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EvalTypeStatHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EvalTypeListDto> Handle(EvalTypeStatCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EvaluationType>().GetById(request.StatDto.Id);
        if (oldData == null) { throw new DomainException($"EVALUATION TYPE with Id {request.StatDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.IsActive = request.StatDto.Stat;
            var data = await _unitOfWork.Repository<EvaluationType>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EvalTypeListDto();
            var response = await _med.Send(new EvalTypeByIdQry { Id = data.Id }, cancellationToken);
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

public class EvalTypeDelHandler : IRequestHandler<EvalTypeDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EvalTypeDelHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EvalTypeDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<EvaluationType>().GetById(request.Id);
            if (data == null) { throw new DomainException($"EVALUATION TYPE with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<EvaluationType>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
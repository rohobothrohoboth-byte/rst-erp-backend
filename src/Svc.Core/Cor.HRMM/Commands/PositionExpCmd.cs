using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;

namespace Cor.HRMM.Commands;

public class PositionExpAddCmd : IRequest<PositionExpListDto> { public PositionExpAddDto AddDto { get; set; } = default!; }
public class PositionExpModCmd : IRequest<PositionExpListDto> { public PositionExpModDto ModDto { get; set; } = default!; }
public class PositionExpDelCmd : IRequest { public Guid Id { get; set; } }

public class PositionExpAddCmdHandler : IRequestHandler<PositionExpAddCmd, PositionExpListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PositionExpAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PositionExpListDto> Handle(PositionExpAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new PositionExp
            {
                SamePosExp = request.AddDto.SamePosExp,
                OtherPosExp = request.AddDto.OtherPosExp,
                MinAge = request.AddDto.MinAge,
                MaxAge = request.AddDto.MaxAge,
                PositionId = request.AddDto.PositionId
            };
            await _unitOfWork.Repository<PositionExp>().Add(data);
            await _unitOfWork.Commit();

            var res = new PositionExpListDto();
            var response = await _med.Send(new PositionExpByIdQry { Id = data.Id }, cancellationToken);
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

public class PositionExpModCmdHandler : IRequestHandler<PositionExpModCmd, PositionExpListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PositionExpModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PositionExpListDto> Handle(PositionExpModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<PositionExp>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"POSITION EXPERIENCE with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.SamePosExp = request.ModDto.SamePosExp;
            oldData.OtherPosExp = request.ModDto.OtherPosExp;
            oldData.MinAge = request.ModDto.MinAge;
            oldData.MaxAge = request.ModDto.MaxAge;
            oldData.PositionId = request.ModDto.PositionId;
            var data = await _unitOfWork.Repository<PositionExp>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new PositionExpListDto();
            var response = await _med.Send(new PositionExpByIdQry { Id = data.Id }, cancellationToken);
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

public class PositionExpDelCmdHandler : IRequestHandler<PositionExpDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionExpDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(PositionExpDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<PositionExp>().GetById(request.Id);
            if (data == null) { throw new DomainException($"POSITION EXPERIENCE with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<PositionExp>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
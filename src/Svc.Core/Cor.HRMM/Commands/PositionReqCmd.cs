using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using MediatR;

namespace Cor.HRMM.Commands;

public class PositionReqAddCmd : IRequest<PositionReqListDto> { public PositionReqAddDto AddDto { get; set; } = default!; }

public class PositionReqModCmd : IRequest<PositionReqListDto> { public PositionReqModDto ModDto { get; set; } = default!; }

public class PositionReqDelCmd : IRequest { public Guid Id { get; set; } }

public class PositionReqAddCmdHandler : IRequestHandler<PositionReqAddCmd, PositionReqListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PositionReqAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PositionReqListDto> Handle(PositionReqAddCmd request, CancellationToken cancellationToken)
    {
        var data = new PositionReq
        {
            Gender = request.AddDto.Gender,
            SaturdayWorkOption = request.AddDto.SaturdayWorkOption,
            SundayWorkOption = request.AddDto.SundayWorkOption,
            WorkingHours = request.AddDto.WorkingHours,
            ProfessionTypeId = request.AddDto.ProfessionTypeId,
            PositionId = request.AddDto.PositionId
        };
        await _unitOfWork.Begin();

        try
        {
            await _unitOfWork.Repository<PositionReq>().Add(data);
            await _unitOfWork.Commit();

            var res = new PositionReqListDto();
            var response = await _med.Send(new PositionReqByIdQry { Id = data.Id }, cancellationToken);
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

public class PositionReqModCmdHandler : IRequestHandler<PositionReqModCmd, PositionReqListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PositionReqModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PositionReqListDto> Handle(PositionReqModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<PositionReq>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new KeyNotFoundException($"PositionReq with Id {request.ModDto.Id} NOT FOUND."); }

        oldData.Gender = request.ModDto.Gender;
        oldData.SaturdayWorkOption = request.ModDto.SaturdayWorkOption;
        oldData.SundayWorkOption = request.ModDto.SundayWorkOption;
        oldData.WorkingHours = request.ModDto.WorkingHours;
        oldData.ProfessionTypeId = request.ModDto.ProfessionTypeId;
        oldData.PositionId = request.ModDto.PositionId;
        await _unitOfWork.Begin();

        try
        {
            var data = await _unitOfWork.Repository<PositionReq>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new PositionReqListDto();
            var response = await _med.Send(new PositionReqByIdQry { Id = data.Id }, cancellationToken);
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

public class PositionReqDelCmdHandler : IRequestHandler<PositionReqDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionReqDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(PositionReqDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<PositionReq>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
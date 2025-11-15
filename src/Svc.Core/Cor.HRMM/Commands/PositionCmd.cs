using Cor.HRMM.Helpers;
using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using MediatR;

namespace Cor.HRMM.Commands;

public class PositionAddCmd : IRequest<PositionListDto> { public PositionAddDto AddDto { get; set; } = default!; }
public class PositionModCmd : IRequest<PositionListDto> { public PositionModDto ModDto { get; set; } = default!; }
public class PositionDelCmd : IRequest { public Guid Id { get; set; } }

public class PositionAddCmdHandler : IRequestHandler<PositionAddCmd, PositionListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PositionAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PositionListDto> Handle(PositionAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new Position
            {
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                NoOfPosition = request.AddDto.NoOfPosition,
                IsVacant = request.AddDto.IsVacant,
                DepartmentId = request.AddDto.DepartmentId
            };
            await _unitOfWork.Repository<Position>().Add(data);
            await _unitOfWork.Commit();

            var res = new PositionListDto();
            var response = await _med.Send(new PositionByIdQry { Id = data.Id }, cancellationToken);
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

public class PositionModCmdHandler : IRequestHandler<PositionModCmd, PositionListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PositionModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PositionListDto> Handle(PositionModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<Position>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"POSITION with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.Name = request.ModDto.Name;
            oldData.NameAm = request.ModDto.NameAm;
            oldData.NoOfPosition = request.ModDto.NoOfPosition;
            oldData.IsVacant = request.ModDto.IsVacant;
            oldData.DepartmentId = request.ModDto.DepartmentId;
            var data = await _unitOfWork.Repository<Position>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new PositionListDto();
            var response = await _med.Send(new PositionByIdQry { Id = data.Id }, cancellationToken);
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

public class PositionDelCmdHandler : IRequestHandler<PositionDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(PositionDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<Position>().GetById(request.Id);
            if (data == null) { throw new DomainException($"POSITION with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<Position>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
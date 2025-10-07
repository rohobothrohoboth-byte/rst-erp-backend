using MediatR;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpStateAddCmd : IRequest<EmpStateListDto> { public EmpStateAddDto AddDto { get; set; } = default!; }
public class EmpStateModCmd : IRequest<EmpStateListDto> { public EmpStateModDto ModDto { get; set; } = default!; }
public class EmpStateDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpStateAddCmdHandler : IRequestHandler<EmpStateAddCmd, EmpStateListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpStateAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpStateListDto> Handle(EmpStateAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new EmpState
            {
                IsTerminated = request.AddDto.IsTerminated,
                IsApproved = request.AddDto.IsApproved,
                IsStandBy = request.AddDto.IsStandBy,
                IsRetired = request.AddDto.IsRetired,
                IsUnderProbation = request.AddDto.IsUnderProbation,
                EmployeeId = request.AddDto.EmployeeId
            };
            await _unitOfWork.Repository<EmpState>().Add(data);
            await _unitOfWork.Commit();

            var res = new EmpStateListDto();
            var response = await _med.Send(new EmpStateByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpStateModCmdHandler : IRequestHandler<EmpStateModCmd, EmpStateListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpStateModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpStateListDto> Handle(EmpStateModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EmpState>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new KeyNotFoundException($"EMPLOYEE STATE with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();

        try
        {
            oldData.IsTerminated = request.ModDto.IsTerminated;
            oldData.IsApproved = request.ModDto.IsApproved;
            oldData.IsStandBy = request.ModDto.IsStandBy;
            oldData.IsRetired = request.ModDto.IsRetired;
            oldData.IsUnderProbation = request.ModDto.IsUnderProbation;
            oldData.EmployeeId = request.ModDto.EmployeeId;
            var data = await _unitOfWork.Repository<EmpState>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EmpStateListDto();
            var response = await _med.Send(new EmpStateByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpStateDelCmdHandler : IRequestHandler<EmpStateDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpStateDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EmpStateDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<EmpState>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
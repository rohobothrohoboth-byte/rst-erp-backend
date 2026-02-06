using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class LeaveTypeAddCmd : IRequest<LeaveTypeListDto> { public LeaveTypeAddDto AddDto { get; set; } = default!; }
public class LeaveTypeModCmd : IRequest<LeaveTypeListDto> { public LeaveTypeModDto ModDto { get; set; } = default!; }
public class LeaveTypeStatCmd : IRequest<LeaveTypeListDto> { public StatChangeDto StatDto { get; set; } = default!; }
public class LeaveTypeDelCmd : IRequest { public Guid Id { get; set; } }

public class LeaveTypeAddHandler : IRequestHandler<LeaveTypeAddCmd, LeaveTypeListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveTypeAddHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveTypeListDto> Handle(LeaveTypeAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new LeaveType
            {
                Name = request.AddDto.Name,
                LeaveCategory = request.AddDto.LeaveCategory,
                RequiresApproval = request.AddDto.RequiresApproval,
                AllowHalfDay = request.AddDto.AllowHalfDay,
                HolidaysAsLeave = request.AddDto.HolidaysAsLeave,
                IsActive = true
            };
            await _unitOfWork.Repository<LeaveType>().Add(data);
            await _unitOfWork.Commit();

            var res = new LeaveTypeListDto();
            var response = await _med.Send(new LeaveTypeByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveTypeModHandler : IRequestHandler<LeaveTypeModCmd, LeaveTypeListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveTypeModHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveTypeListDto> Handle(LeaveTypeModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<LeaveType>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"LEAVE TYPE with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.Name = request.ModDto.Name;
            oldData.LeaveCategory = request.ModDto.LeaveCategory;
            oldData.RequiresApproval = request.ModDto.RequiresApproval;
            oldData.AllowHalfDay = request.ModDto.AllowHalfDay;
            oldData.HolidaysAsLeave = request.ModDto.HolidaysAsLeave;
            oldData.IsActive = request.ModDto.IsActive;
            var data = await _unitOfWork.Repository<LeaveType>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new LeaveTypeListDto();
            var response = await _med.Send(new LeaveTypeByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveTypeStatHandler : IRequestHandler<LeaveTypeStatCmd, LeaveTypeListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveTypeStatHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveTypeListDto> Handle(LeaveTypeStatCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<LeaveType>().GetById(request.StatDto.Id);
        if (oldData == null) { throw new DomainException($"LEAVE TYPE with Id {request.StatDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.IsActive = request.StatDto.Stat;
            var data = await _unitOfWork.Repository<LeaveType>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new LeaveTypeListDto();
            var response = await _med.Send(new LeaveTypeByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveTypeDelHandler : IRequestHandler<LeaveTypeDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveTypeDelHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(LeaveTypeDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<LeaveType>().GetById(request.Id);
            if (data == null) { throw new DomainException($"LEAVE TYPE with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<LeaveType>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
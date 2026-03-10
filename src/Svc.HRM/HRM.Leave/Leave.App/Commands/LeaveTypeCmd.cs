using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Commands;

public class LeaveTypeAddCmd : IRequest<LeaveTypeListDto> { public LeaveTypeAddDto AddDto { get; set; } = default!; }
public class LeaveTypeModCmd : IRequest<LeaveTypeListDto> { public LeaveTypeModDto ModDto { get; set; } = default!; }
public class LeaveTypeStatCmd : IRequest<LeaveTypeListDto> { public StatChangeDto StatDto { get; set; } = default!; }
public class LeaveTypeDelCmd : IRequest { public Guid Id { get; set; } }



public class LeaveTypeAddHandler : IRequestHandler<LeaveTypeAddCmd, LeaveTypeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveTypeAddHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<LeaveTypeListDto> Handle(LeaveTypeAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
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
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new LeaveTypeListDto();
            var response = await _med.Send(new LeaveTypeByIdQry { Id = data.Id }, ct);
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

public class LeaveTypeModHandler : IRequestHandler<LeaveTypeModCmd, LeaveTypeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveTypeModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<LeaveTypeListDto> Handle(LeaveTypeModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<LeaveType>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"LEAVE TYPE with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.LeaveCategory = request.ModDto.LeaveCategory;
            oldData.RequiresApproval = request.ModDto.RequiresApproval;
            oldData.AllowHalfDay = request.ModDto.AllowHalfDay;
            oldData.HolidaysAsLeave = request.ModDto.HolidaysAsLeave;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new LeaveTypeListDto();
            var response = await _med.Send(new LeaveTypeByIdQry { Id = request.ModDto.Id }, ct);
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

public class LeaveTypeStatHandler : IRequestHandler<LeaveTypeStatCmd, LeaveTypeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveTypeStatHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<LeaveTypeListDto> Handle(LeaveTypeStatCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<LeaveType>().FirstOrDefaultAsync(x => x.Id == request.StatDto.Id, ct);
            if (oldData == null) { throw new DomainException($"LEAVE TYPE with Id {request.StatDto.Id} NOT FOUND."); }

            oldData.IsActive = request.StatDto.Stat;
            oldData.SetRowVersion(uint.Parse(request.StatDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new LeaveTypeListDto();
            var response = await _med.Send(new LeaveTypeByIdQry { Id = request.StatDto.Id }, ct);
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

public class LeaveTypeDelHandler : IRequestHandler<LeaveTypeDelCmd>
{
    private readonly IUnitOfWork _uow;
    public LeaveTypeDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(LeaveTypeDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<LeaveType>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"LEAVE TYPE with id [{request.Id}] NOT FOUND."); }
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
using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Commands;

public class LeavePolicyAddCmd : IRequest<LeavePolicyListDto> { public LeavePolicyAddDto AddDto { get; set; } = default!; }
public class LeavePolicyModCmd : IRequest<LeavePolicyListDto> { public LeavePolicyModDto ModDto { get; set; } = default!; }
public class LeavePolicyDelCmd : IRequest { public Guid Id { get; set; } }



public class LeavePolicyAddCmdHandler : IRequestHandler<LeavePolicyAddCmd, LeavePolicyListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeavePolicyAddCmdHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<LeavePolicyListDto> Handle(LeavePolicyAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new LeavePolicy
            {
                Code = request.AddDto.Code,
                Name = request.AddDto.Name,
                AllowEncashment = request.AddDto.AllowEncashment,
                RequiresAttachment = request.AddDto.RequiresAttachment,
                Status = BoolToStr.EnumToString(PolicyStatus.Active),
                LeaveTypeId = request.AddDto.LeaveTypeId
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new LeavePolicyListDto();
            var response = await _med.Send(new LeavePolicyByIdQry { Id = data.Id }, ct);
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

public class LeavePolicyModCmdHandler : IRequestHandler<LeavePolicyModCmd, LeavePolicyListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeavePolicyModCmdHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<LeavePolicyListDto> Handle(LeavePolicyModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<LeavePolicy>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id);
            if (oldData == null) { throw new DomainException($"LEAVE POLICY with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Code = request.ModDto.Code;
            oldData.Name = request.ModDto.Name;
            oldData.AllowEncashment = request.ModDto.AllowEncashment;
            oldData.RequiresAttachment = request.ModDto.RequiresAttachment;
            oldData.Status = request.ModDto.Status;
            oldData.LeaveTypeId = request.ModDto.LeaveTypeId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new LeavePolicyListDto();
            var response = await _med.Send(new LeavePolicyByIdQry { Id = request.ModDto.Id }, ct);
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

public class LeavePolicyDelCmdHandler : IRequestHandler<LeavePolicyDelCmd>
{
    private readonly IUnitOfWork _uow;
    public LeavePolicyDelCmdHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(LeavePolicyDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<LeavePolicy>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"LEAVE POLICY with id [{request.Id}] NOT FOUND."); }
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
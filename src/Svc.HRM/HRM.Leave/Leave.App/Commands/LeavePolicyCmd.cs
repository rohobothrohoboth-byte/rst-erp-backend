using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class LeavePolicyAddCmd : IRequest<LeavePolicyListDto> { public LeavePolicyAddDto AddDto { get; set; } = default!; }
public class LeavePolicyModCmd : IRequest<LeavePolicyListDto> { public LeavePolicyModDto ModDto { get; set; } = default!; }
public class LeavePolicyDelCmd : IRequest { public Guid Id { get; set; } }

public class LeavePolicyAddCmdHandler : IRequestHandler<LeavePolicyAddCmd, LeavePolicyListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeavePolicyAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeavePolicyListDto> Handle(LeavePolicyAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new LeavePolicy
            {
                Name = request.AddDto.Name,
                RequiresAttachment = request.AddDto.RequiresAttachment,
                //MinDurPerReq = request.AddDto.MinDurPerReq,
                //MaxDurPerReq = request.AddDto.MaxDurPerReq,
                //HolidaysAsLeave = request.AddDto.HolidaysAsLeave,
                LeaveTypeId = request.AddDto.LeaveTypeId
            };
            await _unitOfWork.Repository<LeavePolicy>().Add(data);
            await _unitOfWork.Commit();

            var res = new LeavePolicyListDto();
            var response = await _med.Send(new LeavePolicyByIdQry { Id = data.Id }, cancellationToken);
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

public class LeavePolicyModCmdHandler : IRequestHandler<LeavePolicyModCmd, LeavePolicyListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeavePolicyModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeavePolicyListDto> Handle(LeavePolicyModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<LeavePolicy>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"LEAVE POLICY with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.Name = request.ModDto.Name;
            oldData.RequiresAttachment = request.ModDto.RequiresAttachment;
            //oldData.MinDurPerReq = request.ModDto.MinDurPerReq;
            //oldData.MaxDurPerReq = request.ModDto.MaxDurPerReq;
            //oldData.HolidaysAsLeave = request.ModDto.HolidaysAsLeave;
            oldData.LeaveTypeId = request.ModDto.LeaveTypeId;
            var data = await _unitOfWork.Repository<LeavePolicy>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new LeavePolicyListDto();
            var response = await _med.Send(new LeavePolicyByIdQry { Id = data.Id }, cancellationToken);
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

public class LeavePolicyDelCmdHandler : IRequestHandler<LeavePolicyDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeavePolicyDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(LeavePolicyDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<LeavePolicy>().GetById(request.Id);
            if (data == null) { throw new DomainException($"LEAVE POLICY with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<LeavePolicy>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
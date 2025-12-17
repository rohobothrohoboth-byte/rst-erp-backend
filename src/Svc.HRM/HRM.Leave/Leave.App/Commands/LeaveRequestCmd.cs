using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class LeaveRequestAddCmd : IRequest<LeaveRequestListDto> { public LeaveRequestAddDto AddDto { get; set; } = default!; public Guid EmpId { get; set; } = default!;}
public class LeaveRequestModCmd : IRequest<LeaveRequestListDto> { public LeaveRequestModDto ModDto { get; set; } = default!; }
public class LeaveRequestDelCmd : IRequest { public Guid Id { get; set; } }

public class LeaveRequestAddCmdHandler : IRequestHandler<LeaveRequestAddCmd, LeaveRequestListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveRequestAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveRequestListDto> Handle(LeaveRequestAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var dRequested = request.AddDto.EndDate.Subtract(request.AddDto.StartDate);
            var data = new LeaveRequest
            {
                EmployeeId = request.EmpId,
                LeaveTypeId = request.AddDto.LeaveTypeId,
                StartDate = request.AddDto.StartDate,
                EndDate = request.AddDto.EndDate,
                DaysRequested = dRequested.TotalDays,
                IsHalfDay = request.AddDto.IsHalfDay,
                Status = "0",
                Comments = request.AddDto.Comments
            };
            await _unitOfWork.Repository<LeaveRequest>().Add(data);
            await _unitOfWork.Commit();

            var res = new LeaveRequestListDto();
            var response = await _med.Send(new LeaveRequestByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveRequestModCmdHandler : IRequestHandler<LeaveRequestModCmd, LeaveRequestListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveRequestModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveRequestListDto> Handle(LeaveRequestModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<LeaveRequest>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"LEAVE REQUEST with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();

        try
        {
            var dRequested = request.ModDto.EndDate.Subtract(request.ModDto.StartDate);
            oldData.LeaveTypeId = request.ModDto.LeaveTypeId;
            oldData.StartDate = request.ModDto.StartDate;
            oldData.EndDate = request.ModDto.EndDate;
            oldData.DaysRequested = dRequested.TotalDays;
            oldData.IsHalfDay = request.ModDto.IsHalfDay;
            oldData.Status = "0";
            oldData.Comments = request.ModDto.Comments;
            var data = await _unitOfWork.Repository<LeaveRequest>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new LeaveRequestListDto();
            var response = await _med.Send(new LeaveRequestByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveRequestDelCmdHandler : IRequestHandler<LeaveRequestDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveRequestDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(LeaveRequestDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<LeaveRequest>().GetById(request.Id);
            if (data == null) { throw new DomainException($"LEAVE REQUEST with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<LeaveRequest>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
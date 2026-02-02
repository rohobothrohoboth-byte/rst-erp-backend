using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.App.Services;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Enums;
using MediatR;

namespace Leave.App.Commands;

public class LeaveRequestAddCmd : IRequest<LeaveRequestListDto> { public LeaveRequestAddDto AddDto { get; set; } = default!; public Guid EmpId { get; set; } = default!; }
public class LeaveRequestModCmd : IRequest<LeaveRequestListDto> { public LeaveRequestModDto ModDto { get; set; } = default!; }
public class LeaveRequestDelCmd : IRequest { public Guid Id { get; set; } }



public class LeaveRequestAddCmdHandler : IRequestHandler<LeaveRequestAddCmd, LeaveRequestListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;
    private readonly HolidayService _hdService;
    private readonly LeaveValService _lvService;

    public LeaveRequestAddCmdHandler(IUnitOfWork unitOfWork, IMediator med, HolidayService hdService, LeaveValService lvService)
    {
        _unitOfWork = unitOfWork;
        _med = med;
        _hdService = hdService;
        _lvService = lvService;
    }

    public async Task<LeaveRequestListDto> Handle(LeaveRequestAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var workingDays = await _hdService.CalculateLeaveWorkingDays(request.AddDto.StartDate, request.AddDto.EndDate, request.AddDto.IsHalfDay);
            var valReq = await _lvService.ValLeaveRequest(request.EmpId, request.AddDto.LeaveTypeId, request.AddDto.StartDate, request.AddDto.EndDate, request.AddDto.IsHalfDay);
            if (!valReq.IsValid)
            {
                throw new DomainException($"Leave request VALIDATION FAILED: {string.Join(", ", valReq.Errors)}");
            }

            var dRequested = request.AddDto.EndDate.Subtract(request.AddDto.StartDate);

            var stat = BoolToStr.EnumToString(Status.Pending);
            var data = new LeaveRequest
            {
                EmployeeId = request.EmpId,
                LeaveTypeId = request.AddDto.LeaveTypeId,
                StartDate = request.AddDto.StartDate,
                EndDate = request.AddDto.EndDate,
                DaysRequested = dRequested.TotalDays,
                IsHalfDay = request.AddDto.IsHalfDay,
                Status = stat,
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
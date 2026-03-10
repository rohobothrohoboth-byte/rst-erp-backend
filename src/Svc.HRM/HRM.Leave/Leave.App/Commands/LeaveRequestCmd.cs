using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.App.Services;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Commands;

public class LeaveRequestAddCmd : IRequest<LeaveRequestListDto> { public LeaveRequestAddDto AddDto { get; set; } = default!; public Guid EmpId { get; set; } = default!; }
public class LeaveRequestModCmd : IRequest<LeaveRequestListDto> { public LeaveRequestModDto ModDto { get; set; } = default!; }
public class LeaveRequestDelCmd : IRequest { public Guid Id { get; set; } }



public class LeaveRequestAddCmdHandler : IRequestHandler<LeaveRequestAddCmd, LeaveRequestListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IHolidayService _hdService;
    private readonly ILeaveValService _lvService;
    private readonly IApprovalEngine _approvalEngine;

    public LeaveRequestAddCmdHandler(IUnitOfWork uow, IMediator med, IHolidayService hdService, ILeaveValService lvService, IApprovalEngine approvalEngine)
    {
        _uow = uow;
        _med = med;
        _hdService = hdService;
        _lvService = lvService;
        _approvalEngine = approvalEngine;
    }

    public async Task<LeaveRequestListDto> Handle(LeaveRequestAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var empId = request.EmpId;
            var sDate = request.AddDto.StartDate;
            var eDate = request.AddDto.EndDate;
            var iHalf = request.AddDto.IsHalfDay;

            var workingDays = await _hdService.CalEmpLeaveWorkingDays(empId, sDate, eDate, iHalf);
            var valReq = await _lvService.ValLeaveRequest(empId, request.AddDto.LeaveTypeId, sDate, eDate, iHalf, ct);

            if (!valReq.IsValid) { throw new DomainException($"Leave request VALIDATION FAILED: {string.Join(", ", valReq.Errors)}"); }

            var data = new LeaveRequest
            {
                EmployeeId = empId,
                LeaveTypeId = request.AddDto.LeaveTypeId,
                StartDate = sDate,
                EndDate = eDate,
                DaysRequested = workingDays,
                IsHalfDay = iHalf,
                Status = BoolToStr.EnumToString(Status.Pending),
                Comments = request.AddDto.Comments
            };
            await _uow.Add(data, ct);

            await _approvalEngine.InitApproval(data.Id, ct);
            await _uow.Commit(ct);

            var res = new LeaveRequestListDto();
            var response = await _med.Send(new LeaveRequestByIdQry { Id = data.Id }, ct);
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

public class LeaveRequestModCmdHandler : IRequestHandler<LeaveRequestModCmd, LeaveRequestListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveRequestModCmdHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<LeaveRequestListDto> Handle(LeaveRequestModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"LEAVE REQUEST with Id {request.ModDto.Id} NOT FOUND."); }

            var dRequested = request.ModDto.EndDate.Subtract(request.ModDto.StartDate);
            oldData.LeaveTypeId = request.ModDto.LeaveTypeId;
            oldData.StartDate = request.ModDto.StartDate;
            oldData.EndDate = request.ModDto.EndDate;
            oldData.DaysRequested = dRequested.TotalDays;
            oldData.IsHalfDay = request.ModDto.IsHalfDay;
            oldData.Status = BoolToStr.EnumToString(Status.Pending);
            oldData.Comments = request.ModDto.Comments;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new LeaveRequestListDto();
            var response = await _med.Send(new LeaveRequestByIdQry { Id = request.ModDto.Id }, ct);
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

public class LeaveRequestDelCmdHandler : IRequestHandler<LeaveRequestDelCmd>
{
    private readonly IUnitOfWork _uow;
    public LeaveRequestDelCmdHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(LeaveRequestDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"LEAVE REQUEST with id [{request.Id}] NOT FOUND."); }
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